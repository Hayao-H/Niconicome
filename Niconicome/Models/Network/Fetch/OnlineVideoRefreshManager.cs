using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.State;
using Niconicome.Models.Playlist;
using Niconicome.Models.Playlist.Playlist;
using Niconicome.Models.Playlist.VideoList;
using Reactive.Bindings;

namespace Niconicome.Models.Network.Fetch
{
    public interface IOnlineVideoRefreshManager
    {
        /// <summary>
        /// 動画情報を更新して保存する
        /// </summary>
        /// <param name="videos">失敗した動画数</param>
        /// <returns></returns>
        Task<IAttemptResult<int>> RefreshAndSaveAsync(List<IListVideoInfo> videos);

        /// <summary>
        /// リモートプレイリストを更新して保存する
        /// </summary>
        /// <param name="playlistID"></param>
        /// <returns>通知事項</returns>
        Task<IAttemptResult<string>> RefreshRemoteAndSaveAsync();

        /// <summary>
        /// 指定したプレイリストがリモートプレイリストとして更新可能かどうかをチェックする
        /// </summary>
        /// <param name="playlistInfo"></param>
        /// <returns></returns>
        bool CheckIfRemotePlaylistCanBeFetched(ITreePlaylistInfo playlistInfo);

        /// <summary>
        /// 取得作業をキャンセルする
        /// </summary>
        void Cancel();

        /// <summary>
        /// 取得中フラグ
        /// </summary>
        ReactivePropertySlim<bool> IsFetching { get; }
    }

    public class OnlineVideoRefreshManager : IOnlineVideoRefreshManager
    {
        public OnlineVideoRefreshManager(ICurrent current, IVideoListContainer container, INetworkVideoHandler netVideoHandler, IRemotePlaylistHandler remotePlaylistHandler, IMessageHandler message,IVideosUnchecker unchecker)
        {
            this._current = current;
            this._container = container;
            this._netVideoHandler = netVideoHandler;
            this._remotePlaylistHandler = remotePlaylistHandler;
            this._message = message;
            this._unchcecker = unchecker;
        }

        #region field

        private readonly ICurrent _current;

        private readonly IVideoListContainer _container;

        private readonly INetworkVideoHandler _netVideoHandler;

        private readonly IRemotePlaylistHandler _remotePlaylistHandler;

        private readonly IMessageHandler _message;

        private readonly IVideosUnchecker _unchcecker;

        private CancellationTokenSource? _cts = null;

        #endregion

        #region Props

        public ReactivePropertySlim<bool> IsFetching { get; init; } = new();

        #endregion

        #region Methods

        public async Task<IAttemptResult<int>> RefreshAndSaveAsync(List<IListVideoInfo> videos)
        {
            if (this.IsFetching.Value)
            {
                return AttemptResult<int>.Fail("現在動画を取得中です。");
            }

            //準備
            this.PreFetching();
            int? playlistID = this._current.SelectedPlaylist.Value?.Id;

            //情報取得
            IAttemptResult<IEnumerable<IListVideoInfo>> retrieved = await this._netVideoHandler.GetVideoListInfosAsync(videos.Select(v => v.NiconicoId.Value), id => this._unchcecker.Uncheck(id, playlistID ?? -1), this._cts!.Token);

            if (this._cts!.IsCancellationRequested)
            {
                this.PostFetching();
                return AttemptResult<int>.Fail("処理がキャンセルされました。");
            }

            int failedCount = videos.Count - retrieved.Data!.Count();

            //保存
            IAttemptResult updateResult = this._container.UpdateRange(retrieved.Data!, playlistID: playlistID);

            if (!updateResult.IsSucceeded)
            {
                this.PostFetching();
                return AttemptResult<int>.Fail($"動画情報の保存に失敗しました。（詳細：{updateResult.Message}）");
            }

            //後始末
            this.PostFetching();

            return new AttemptResult<int>() { IsSucceeded = true, Data = failedCount };

        }

        public async Task<IAttemptResult<string>> RefreshRemoteAndSaveAsync()
        {
            if (this.IsFetching.Value)
            {
                return AttemptResult<string>.Fail("現在動画を取得中です。");
            }
            else if (this._current.SelectedPlaylist.Value is null)
            {
                return AttemptResult<string>.Fail("プレイリストが選択されていません。");
            }

            //プレイリストを取得
            ITreePlaylistInfo playlistInfo = this._current.SelectedPlaylist.Value;

            if (!this.CheckIfRemotePlaylistCanBeFetched(playlistInfo))
            {
                return AttemptResult<string>.Fail("指定されたプレイリストはリモートプレイリストではないか、IDが指定されていません。");
            }

            //変数定義
            RemoteType remoteType = playlistInfo.RemoteType;
            string remoteID = playlistInfo.RemoteId;
            var videos = new List<IListVideoInfo>();
            List<string> registeredVIdeos = this._container.Videos.Select(v => v.NiconicoId.Value).ToList();

            //準備
            this.PreFetching();

            //同期処理（Fetch）
            IAttemptResult<string> result = await this._remotePlaylistHandler.TryGetRemotePlaylistAsync(remoteID, videos, remoteType, m => this._message.AppendMessage(m));

            if (!result.IsSucceeded)
            {
                this.PostFetching();
                return AttemptResult<string>.Fail(result.Data ?? "詳細不明");
            }

            //動画を分類
            List<IListVideoInfo> toBeAddedVideos = videos.Where(v => !registeredVIdeos.Contains(v.NiconicoId.Value)).ToList();
            List<IListVideoInfo> toBeUpdatedVideos = videos.Where(v => registeredVIdeos.Contains(v.NiconicoId.Value)).ToList();

            //アップデート
            IAttemptResult updateResult = this._container.UpdateRange(toBeUpdatedVideos, playlistInfo.Id);

            //追加
            IAttemptResult addResult = this._container.AddRange(toBeAddedVideos, playlistInfo.Id);

            if (!updateResult.IsSucceeded)
            {
                this.PostFetching();
                return AttemptResult<string>.Fail($"動画情報の保存に失敗しました。（詳細：{updateResult.Message}）");
            }
            else if (!addResult.IsSucceeded)
            {
                this.PostFetching();
                return AttemptResult<string>.Fail($"動画の追加に失敗しました。（詳細：{updateResult.Message}）");
            }

            this.PostFetching();
            return AttemptResult<string>.Succeeded(result.Data, $"追加:{toBeAddedVideos.Count},更新:{toBeUpdatedVideos.Count}");
        }

        public bool CheckIfRemotePlaylistCanBeFetched(ITreePlaylistInfo playlistInfo)
        {
            if (!playlistInfo.IsRemotePlaylist || playlistInfo.RemoteType == RemoteType.None)
            {
                return false;
            }
            else if (playlistInfo.RemoteId.IsNullOrEmpty() && playlistInfo.RemoteType != RemoteType.WatchLater)
            {
                return false;
            }

            return true;
        }


        public void Cancel()
        {
            this._cts?.Cancel();
        }



        #endregion

        #region private

        /// <summary>
        /// 終了処理
        /// </summary>
        private void PostFetching()
        {
            this._cts = null;
            this.IsFetching.Value = false;
        }

        /// <summary>
        /// 取得準備処理
        /// </summary>
        private void PreFetching()
        {
            this.IsFetching.Value = true;
            this._cts = new CancellationTokenSource();
        }

        #endregion
    }
}
