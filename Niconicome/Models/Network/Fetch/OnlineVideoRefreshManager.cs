using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Playlist;
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
        public OnlineVideoRefreshManager(ICurrent current, IVideoListContainer container, INetworkVideoHandler netVideoHandler)
        {
            this._current = current;
            this._container = container;
            this._netVideoHandler = netVideoHandler;
        }

        #region field

        private readonly ICurrent _current;

        private readonly IVideoListContainer _container;

        private readonly INetworkVideoHandler _netVideoHandler;

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
            this.IsFetching.Value = true;
            this._cts = new CancellationTokenSource();
            int? playlistID = this._current.SelectedPlaylist.Value?.Id;

            //情報取得
            IEnumerable<IListVideoInfo> retrieved = await this._netVideoHandler.GetVideoListInfosAsync(videos, playlistID: playlistID, ct: this._cts.Token);

            if (this._cts.IsCancellationRequested)
            {
                this.FinishFetching();
                return AttemptResult<int>.Fail("処理がキャンセルされました。");
            }

            int failedCount = videos.Count - retrieved.Count();

            //保存
            IAttemptResult updateResult = this._container.UpdateRange(videos, playlistID: playlistID);

            if (!updateResult.IsSucceeded)
            {
                this.FinishFetching();
                return AttemptResult<int>.Fail($"動画情報の保存に失敗しました。（詳細：{updateResult.Message}）");
            }

            //後始末
            this.FinishFetching();

            return new AttemptResult<int>() { IsSucceeded = true, Data = failedCount };

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
        private void FinishFetching()
        {
            this._cts = null;
            this.IsFetching.Value = false;
        }

        #endregion
    }
}
