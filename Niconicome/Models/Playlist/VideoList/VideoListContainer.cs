using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using Niconicome.Extensions.System;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Domain.Local.Store.Types;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Event.Generic;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Playlist.Playlist;
using Reactive.Bindings;

namespace Niconicome.Models.Playlist.VideoList
{

    public interface IVideoListContainer
    {
        /// <summary>
        /// 動画を削除する
        /// </summary>
        /// <param name="video"></param>
        /// <param name="playlistID"></param>
        /// <param name="commit"></param>
        /// <returns></returns>
        IAttemptResult Remove(IListVideoInfo video, int? playlistID = null, bool commit = true);

        /// <summary>
        /// 複数の動画を削除する
        /// </summary>
        /// <param name="videos"></param>
        /// <param name="playlistID"></param>
        /// <param name="commit"></param>
        /// <returns></returns>
        IAttemptResult RemoveRange(IEnumerable<IListVideoInfo> videos, int? playlistID = null, bool commit = true);

        /// <summary>
        /// 動画を追加する
        /// </summary>
        /// <param name="video"></param>
        /// <param name="playlistID"></param>
        /// <param name="commit"></param>
        /// <returns></returns>
        IAttemptResult Add(IListVideoInfo video, int? playlistID = null, bool commit = true);

        /// <summary>
        /// 複数の動画を追加する
        /// </summary>
        /// <param name="videos"></param>
        /// <param name="playlistID"></param>
        /// <param name="commit"></param>
        /// <returns></returns>
        IAttemptResult AddRange(IEnumerable<IListVideoInfo> videos, int? playlistID = null, bool commit = true);

        /// <summary>
        /// 動画を更新する
        /// </summary>
        /// <param name="video"></param>
        /// <param name="playlistID"></param>
        /// <param name="commit"></param>
        /// <returns></returns>
        IAttemptResult Update(IListVideoInfo video, int? playlistID = null, bool commit = true);

        /// <summary>
        /// 複数の動画を更新する
        /// </summary>
        /// <param name="videos"></param>
        /// <param name="playlistID"></param>
        /// <param name="commit"></param>
        /// <returns></returns>
        IAttemptResult UpdateRange(IEnumerable<IListVideoInfo> videos, int? playlistID = null, bool commit = true);

        /// <summary>
        /// プレイリストを更新
        /// </summary>
        /// <returns></returns>
        IAttemptResult Refresh();

        /// <summary>
        /// プレイリストをクリア
        /// </summary>
        /// <returns></returns>
        IAttemptResult Clear();

        /// <summary>
        /// forEach処理
        /// </summary>
        /// <param name="foreachFunc"></param>
        /// <returns></returns>
        IAttemptResult ForEach(Action<IListVideoInfo> foreachFunc);

        /// <summary>
        /// ソートする
        /// </summary>
        /// <param name="sortType"></param>
        /// <param name="isDescending"></param>
        /// <param name="customSortSequence"></param>
        /// <returns></returns>
        IAttemptResult Sort(VideoSortType sortType, bool isDescending, List<int>? customSortSequence = null);

        /// <summary>
        /// 選択している動画を前に移動
        /// </summary>
        /// <param name="videoIndex"></param>
        /// <param name="playlistID"></param>
        /// <param name="commit"></param>
        /// <returns></returns>
        IAttemptResult MovevideotoPrev(int videoIndex, int? playlistID = null, bool commit = true);

        /// <summary>
        /// 選択している動画を後ろに移動
        /// </summary>
        /// <param name="videoIndex"></param>
        /// <param name="playlistID"></param>
        /// <param name="commit"></param>
        /// <returns></returns>
        IAttemptResult MovevideotoForward(int videoIndex, int? playlistID = null, bool commit = true);

        /// <summary>
        /// 動画数
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 動画
        /// </summary>
        ObservableCollection<IListVideoInfo> Videos { get; }

        /// <summary>
        /// リスト変更イベント
        /// </summary>
        event EventHandler<ListChangedEventArgs<IListVideoInfo>>? ListChanged;
    }

    public class VideoListContainer : IVideoListContainer
    {
        public VideoListContainer(IPlaylistHandler playlistHandler, IVideoHandler videoHandler, IVideoListRefresher refresher, ICurrent current, ILogger logger)
        {
            this.videoHandler = videoHandler;
            this.refresher = refresher;
            this.current = current;
            this.logger = logger;
            this.Videos = new ObservableCollection<IListVideoInfo>();
            this._playlistHandler = playlistHandler;

            this.current.IsTemporaryPlaylist.Subscribe(value =>
            {
                if (value)
                {
                    this.Clear();
                }
            });
        }

        #region field

        private readonly IPlaylistHandler _playlistHandler;

        private readonly IVideoHandler videoHandler;

        private readonly IVideoListRefresher refresher;

        private readonly ICurrent current;

        private readonly ILogger logger;

        #endregion

        #region private

        /// <summary>
        /// 変更イベントを通知する
        /// </summary>
        /// <param name="video"></param>
        /// <param name="changeType"></param>
        private void RaiseListChanged(IListVideoInfo? video, ChangeType changeType)
        {
            this.ListChanged?.Invoke(this, new ListChangedEventArgs<IListVideoInfo>(video, changeType));
        }

        #endregion

        /// <summary>
        /// 動画数
        /// </summary>
        public int Count { get => this.Videos.Count; }

        /// <summary>
        /// 動画を取得する
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IListVideoInfo> GetVideos()
        {
            return this.Videos;
        }

        /// <summary>
        /// 動画を削除する
        /// </summary>
        /// <param name="video"></param>
        /// <param name="commit"></param>
        /// <returns></returns>
        public IAttemptResult Remove(IListVideoInfo video, int? playlistID = null, bool commit = true)
        {
            var id = playlistID ?? this.current.SelectedPlaylist.Value?.Id ?? -1;

            if (!this.Videos.Any(v => v.NiconicoId == video.NiconicoId))
            {
                return AttemptResult.Fail("{video.NiconicoId.Value}は現在のプレイリストに存在しません。");
            }

            try
            {
                this.Videos.RemoveAll(v => v.NiconicoId == video.NiconicoId);
            }
            catch (Exception e)
            {
                return AttemptResult.Fail($"メモリ上のプレイリストからの削除に失敗しました。({video.NiconicoId.Value})", e);
            }

            if (video.IsSelected.Value)
            {
                this.current.SelectedVideos.Value--;
            }


            if (!commit)
            {
                if (id == (this.current.SelectedPlaylist.Value?.Id ?? -1)) this.RaiseListChanged(video, ChangeType.Remove);
                return AttemptResult.Succeeded();
            }


            if (id == -1)
            {
                return AttemptResult.Fail("プレイストが選択されていません。");
            }

            IAttemptResult pResult = this._playlistHandler.UnWireVideoToPlaylist(video.Id.Value, id);
            if (!pResult.IsSucceeded)
            {
                return AttemptResult.Fail($"データベース上のプレイリストからの削除に失敗しました。({video.NiconicoId.Value})");
            }

            if (id == (this.current.SelectedPlaylist.Value?.Id ?? -1)) this.RaiseListChanged(video, ChangeType.Remove);
            return AttemptResult.Succeeded();
        }

        /// <summary>
        /// 複数の動画を削除する
        /// </summary>
        /// <param name="videos"></param>
        /// <param name="commit"></param>
        public IAttemptResult RemoveRange(IEnumerable<IListVideoInfo> videos, int? playlistID = null, bool commit = true)
        {
            foreach (var video in videos.Copy())
            {
                var result = this.Remove(video, playlistID, commit);
                if (!result.IsSucceeded)
                {
                    return result;
                }
            }

            return AttemptResult.Succeeded();
        }

        /// <summary>
        /// 動画を追加する
        /// </summary>
        /// <param name="video"></param>
        /// <param name="commit"></param>
        /// <returns></returns>
        public IAttemptResult Add(IListVideoInfo video, int? playlistID = null, bool commit = true)
        {

            int id = playlistID ?? this.current.SelectedPlaylist.Value?.Id ?? -1;
            bool isSame = id == (this.current.SelectedPlaylist.Value?.Id ?? -1);


            if (isSame)
            {
                if (this.Videos.Any(v => v.NiconicoId.Value == video.NiconicoId.Value))
                {
                    return AttemptResult.Fail($"{video.NiconicoId.Value}は既に現在のプレイリストに存在します。");
                }

                try
                {
                    this.Videos.Add(video);
                }
                catch (Exception e)
                {
                    return AttemptResult.Fail($"メモリ上のプレイリストへの追加に失敗しました。({video.NiconicoId.Value})", e);
                }
            }

            if (video.IsSelected.Value)
            {
                this.current.SelectedVideos.Value++;
            }

            if (!commit)
            {
                if (id == (this.current.SelectedPlaylist.Value?.Id ?? -1)) this.RaiseListChanged(video, ChangeType.Add);
                return AttemptResult.Succeeded();
            }


            if (id == -1)
            {
                return AttemptResult.Fail("プレイストが選択されていません。");
            }


            IAttemptResult<int> vResult = this.videoHandler.AddVideo(video);

            if (!vResult.IsSucceeded)
            {
                return AttemptResult.Fail($"動画のデータベースへの保存に失敗しました。({video.NiconicoId.Value})");
            }
            else
            {
                video.Id.Value = vResult.Data;
            }

            IAttemptResult pResult = this._playlistHandler.WireVideoToPlaylist(vResult.Data, id);

            if (!pResult.IsSucceeded)
            {
                return AttemptResult.Fail($"データベース上のプレイリストへの追加に失敗しました。({video.NiconicoId.Value})");
            }
            else
            {
                video.Id.Value = vResult.Data;
            }

            if (id == (this.current.SelectedPlaylist.Value?.Id ?? -1)) this.RaiseListChanged(video, ChangeType.Add);
            return AttemptResult.Succeeded();
        }

        /// <summary>
        /// 複数の動画を追加する
        /// </summary>
        /// <param name="videos"></param>
        /// <param name="commit"></param>
        /// <returns></returns>
        public IAttemptResult AddRange(IEnumerable<IListVideoInfo> videos, int? playlistID = null, bool commit = true)
        {
            foreach (var video in videos.Copy())
            {
                var result = this.Add(video, playlistID, commit);
                if (!result.IsSucceeded)
                {
                    return result;
                }
            }

            return AttemptResult.Succeeded();
        }

        /// <summary>
        /// 動画情報を更新する
        /// </summary>
        /// <param name="video"></param>
        /// <param name="playlistID"></param>
        /// <returns></returns>
        /// <param name="commit"></param>
        public IAttemptResult Update(IListVideoInfo video, int? playlistID = null, bool commit = true)
        {
            int id = playlistID ?? this.current.SelectedPlaylist.Value?.Id ?? -1;
            bool isSame = id == (this.current.SelectedPlaylist.Value?.Id ?? -1);

            if (isSame)
            {
                if (!this.Videos.Any(v => v.NiconicoId.Value == video.NiconicoId.Value))
                {
                    return AttemptResult.Fail($"{video.NiconicoId.Value}は現在のプレイリストに存在しません。");
                }

                try
                {
                    var targetVIdeo = this.Videos.First(v => v.NiconicoId.Value == video.NiconicoId.Value);
                    targetVIdeo.SetNewData(video);
                }
                catch (Exception e)
                {
                    AttemptResult.Fail($"メモリ上の動画情報の更新に失敗しました。({video.NiconicoId.Value})", e);
                }
            }

            if (!commit)
            {
                return new AttemptResult()
                {
                    IsSucceeded = true,
                };
            }

            IAttemptResult vResult = this.videoHandler.Update(video);
            if (!vResult.IsSucceeded)
            {
                return AttemptResult.Fail($"データベース上の動画情報の更新に失敗しました。({video.NiconicoId.Value})");

            }

            return AttemptResult.Succeeded();
        }

        /// <summary>
        /// 複数の動画情報を更新する
        /// </summary>
        /// <param name="videos"></param>
        /// <param name="playlistID"></param>
        /// <returns></returns>
        /// <param name="commit"></param>
        public IAttemptResult UpdateRange(IEnumerable<IListVideoInfo> videos, int? playlistID = null, bool commit = true)
        {
            foreach (var video in videos.Copy())
            {
                var result = this.Update(video, playlistID, commit);
                if (!result.IsSucceeded)
                {
                    return result;
                }
            }

            return AttemptResult.Succeeded();
        }


        /// <summary>
        /// 動画リストを再読み込みする
        /// </summary>
        /// <returns></returns>
        public IAttemptResult Refresh()
        {
            IAttemptResult result;

            this.current.SelectedVideos.Value = 0;

            if (this.current.IsTemporaryPlaylist.Value)
            {
                var videos = this.Videos.Copy();
                this.Clear();
                result = this.refresher.Refresh(videos, v =>
                {
                    this.Videos.Add(v);
                    if (v.IsSelected.Value) this.current.SelectedVideos.Value++;
                }, true);
            }
            else if (this.current.SelectedPlaylist.Value is not null)
            {
                this.Clear();
                result = this.refresher.Refresh(this.current.SelectedPlaylist.Value.Videos, v =>
                {
                    this.Videos.Add(v);
                    if (v.IsSelected.Value) this.current.SelectedVideos.Value++;
                });
            }
            else
            {
                return AttemptResult.Fail();
            }

            this.Sort(this.current.SelectedPlaylist.Value!.VideoSortType, this.current.SelectedPlaylist.Value!.IsVideoDescending, this.current.SelectedPlaylist.Value!.CustomSortSequence);

            if (result.IsSucceeded)
            {
                this.RaiseListChanged(null, ChangeType.Overall);
            }

            return result;
        }

        /// <summary>
        /// すべての動画をクリアする
        /// </summary>
        /// <returns></returns>
        public IAttemptResult Clear()
        {
            try
            {
                this.Videos.Clear();
            }
            catch (Exception e)
            {
                return new AttemptResult()
                {
                    Message = "動画のクリアに失敗しました",
                    Exception = e,
                };
            }

            this.RaiseListChanged(null, ChangeType.Clear);
            return new AttemptResult()
            {
                IsSucceeded = true,
            };
        }

        /// <summary>
        /// 動画に対するforeach処理
        /// </summary>
        /// <param name="foreachFunc"></param>
        /// <returns></returns>
        public IAttemptResult ForEach(Action<IListVideoInfo> foreachFunc)
        {
            foreach (var video in this.Videos)
            {
                try
                {
                    foreachFunc(video);
                }
                catch (Exception e)
                {
                    return new AttemptResult<IListVideoInfo>() { Exception = e, Data = video, Message = $"{video.NiconicoId.Value}への処理中にエラーが発生しました。" };
                }
            }

            return new AttemptResult<IListVideoInfo>() { IsSucceeded = true };
        }

        /// <summary>
        /// 並び替える
        /// </summary>
        /// <param name="sortType"></param>
        /// <param name="customSortSequence"></param>
        /// <returns></returns>
        public IAttemptResult Sort(VideoSortType sortType, bool isDescending, List<int>? customSortSequence = null)
        {
            if (sortType == VideoSortType.Custom && customSortSequence is null) return new AttemptResult() { Message = "並び替えの設定がカスタムになっていますが、Listがnullです。" };

            List<IListVideoInfo> tmp = this.Videos.ToList();
            this.Clear();
            if (!isDescending)
            {
                IEnumerable<IListVideoInfo> sorted = sortType switch
                {
                    VideoSortType.Register => tmp.OrderBy(v => v.Id.Value),
                    VideoSortType.Title => tmp.OrderBy(v => v.Title.Value),
                    VideoSortType.NiconicoID => tmp.OrderBy(v => long.Parse(Regex.Replace(v.NiconicoId.Value, @"[^\d]", "", RegexOptions.Compiled))),
                    VideoSortType.UploadedDT => tmp.OrderBy(v => v.UploadedOn.Value),
                    VideoSortType.ViewCount => tmp.OrderBy(v => v.ViewCount.Value),
                    VideoSortType.DownloadedFlag => tmp.OrderBy(v => v.IsDownloaded.Value ? 1 : 0),
                    VideoSortType.Economy => tmp.OrderBy(v => v.IsEconomy.Value ? 1 : 0),
                    VideoSortType.State => tmp.OrderBy(v => v.Message.Value),
                    _ => Enumerable.Reverse(tmp),
                };
                this.Videos.Addrange(sorted);
            }
            else
            {

                IEnumerable<IListVideoInfo> sorted = sortType switch
                {
                    VideoSortType.Register => tmp.OrderByDescending(v => v.Id.Value),
                    VideoSortType.Title => tmp.OrderByDescending(v => v.Title.Value),
                    VideoSortType.NiconicoID => tmp.OrderByDescending(v => long.Parse(Regex.Replace(v.NiconicoId.Value, @"[^\d]", "", RegexOptions.Compiled))),
                    VideoSortType.UploadedDT => tmp.OrderByDescending(v => v.UploadedOn.Value),
                    VideoSortType.ViewCount => tmp.OrderByDescending(v => v.ViewCount.Value),
                    VideoSortType.DownloadedFlag => tmp.OrderByDescending(v => v.IsDownloaded.Value ? 1 : 0),
                    VideoSortType.Economy => tmp.OrderByDescending(v => v.IsEconomy.Value ? 1 : 0),
                    VideoSortType.State => tmp.OrderByDescending(v => v.Message.Value),
                    _ => Enumerable.Reverse(tmp),
                };
                this.Videos.Addrange(sorted);
            }

            if (this.current.SelectedPlaylist.Value is not null)
            {
                ITreePlaylistInfo playlist = this.current.SelectedPlaylist.Value;
                if (!playlist.IsTemporary)
                {
                    playlist.Videos.Clear();
                    playlist.Videos.AddRange(this.Videos);
                }
            }

            this.RaiseListChanged(null, ChangeType.Overall);
            return AttemptResult.Succeeded();
        }

        /// <summary>
        /// 動画を一つ前に挿入する
        /// </summary>
        /// <param name="videoIndex"></param>
        /// <param name="playlistID"></param>
        /// <param name="commit"></param>
        /// <returns></returns>
        public IAttemptResult MovevideotoPrev(int videoIndex, int? playlistID = null, bool commit = true)
        {
            var determinedPlaylistID = playlistID ?? this.current.SelectedPlaylist.Value?.Id ?? -1;

            if (determinedPlaylistID == -1)
            {
                return AttemptResult.Fail("プレイリストが選択されていません");
            }

            if (determinedPlaylistID == this.current.SelectedPlaylist.Value!.Id)
            {
                if (videoIndex == 0) return AttemptResult.Fail("選択している動画は既に先頭にあるため、これ以上前に移動できません。");
                try
                {
                    this.Videos.InsertIntoPrev(videoIndex);
                }
                catch (Exception e)
                {
                    this.logger.Error("メモリ上のプレイリストにおける動画の並び替えに失敗しました。", e);
                    return AttemptResult.Fail("メモリ上のプレイリストにおける動画の並び替えに失敗しました。", e);
                }
            }

            if (commit)
            {
                var result = this._playlistHandler.MoveVideoToPrev(videoIndex, determinedPlaylistID);
                if (!result.IsSucceeded)
                {

                    return AttemptResult.Fail("プレイリストにおける動画の並び替えに失敗しました。", result.Exception);
                }
            }

            return AttemptResult.Succeeded();

        }

        /// <summary>
        /// 動画を一つ後ろに挿入する
        /// </summary>
        /// <param name="videoIndex"></param>
        /// <param name="playlistID"></param>
        /// <param name="commit"></param>
        /// <returns></returns>
        public IAttemptResult MovevideotoForward(int videoIndex, int? playlistID = null, bool commit = true)
        {
            var determinedPlaylistID = playlistID ?? this.current.SelectedPlaylist.Value?.Id ?? -1;

            if (determinedPlaylistID == -1)
            {
                return AttemptResult.Fail("プレイリストが選択されていません");
            }

            if (determinedPlaylistID == this.current.SelectedPlaylist.Value!.Id)
            {
                if (videoIndex + 1 == this.Videos.Count) return AttemptResult.Fail("選択している動画は既に最後尾にあるため、これ以上後ろに移動できません。");
                try
                {
                    this.Videos.InsertIntoForward(videoIndex);
                }
                catch (Exception e)
                {
                    this.logger.Error("メモリ上のプレイリストにおける動画の並び替えに失敗しました。", e);
                    return AttemptResult.Fail("メモリ上のプレイリストにおける動画の並び替えに失敗しました。", e);
                }
            }

            if (commit)
            {
                IAttemptResult result = this._playlistHandler.MoveVideoToForward(videoIndex, determinedPlaylistID);
                if (!result.IsSucceeded)
                {
                    return AttemptResult.Fail("プレイリストにおける動画の並び替えに失敗しました。");
                }
            }

            return AttemptResult.Succeeded();
        }

        /// <summary>
        /// 動画一覧
        /// </summary>
        public ObservableCollection<IListVideoInfo> Videos { get; init; }

        /// <summary>
        /// 動画リスト変更イベント
        /// </summary>
        public event EventHandler<ListChangedEventArgs<IListVideoInfo>>? ListChanged;

    }
}
