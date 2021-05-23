using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Win32;
using Niconicome.Extensions.System;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Domain.Local.Store.Types;
using Niconicome.Models.Helper.Event.Generic;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Helper.Result.Generic;

namespace Niconicome.Models.Playlist.VideoList
{

    public interface IVideoListContainer
    {
        IAttemptResult Remove(IListVideoInfo video, int? playlistID = null, bool commit = true);
        IAttemptResult RemoveRange(IEnumerable<IListVideoInfo> videos, int? playlistID = null, bool commit = true);
        IAttemptResult Add(IListVideoInfo video, int? playlistID = null, bool commit = true);
        IAttemptResult AddRange(IEnumerable<IListVideoInfo> videos, int? playlistID = null, bool commit = true);
        IAttemptResult Update(IListVideoInfo video, bool commit = true);
        IAttemptResult UpdateRange(IEnumerable<IListVideoInfo> videos, bool commit = true);
        IAttemptResult Refresh();
        IAttemptResult Clear();
        IAttemptResult Uncheck(int videoID, int playlistID);
        IAttemptResult ForEach(Action<IListVideoInfo> foreachFunc);
        IAttemptResult Sort(VideoSortType sortType, bool isDescending, List<int>? customSortSequence = null);
        int Count { get; }
        ObservableCollection<IListVideoInfo> Videos { get; }
        event EventHandler<ListChangedEventArgs<IListVideoInfo>>? ListChanged;
    }

    public class VideoListContainer : IVideoListContainer
    {
        public VideoListContainer(IPlaylistStoreHandler playlistStoreHandler, IVideoHandler videoHandler, IVideoListRefresher refresher, ICurrent current)
        {
            this.playlistStoreHandler = playlistStoreHandler;
            this.videoHandler = videoHandler;
            this.refresher = refresher;
            this.current = current;
            this.Videos = new ObservableCollection<IListVideoInfo>();
        }

        #region DIされるクラス

        private readonly IPlaylistStoreHandler playlistStoreHandler;

        private readonly IVideoHandler videoHandler;

        private readonly IVideoListRefresher refresher;

        private readonly ICurrent current;

        #endregion

        #region プライベートフィールド


        #endregion

        #region プライベートメソッド

        /// <summary>
        /// 選択解除されたプレイリストを保持する
        /// </summary>
        private void SavePrevPlaylistVideos()
        {
            if (this.current.SelectedPlaylist is null) return;

            foreach (var video in this.Videos)
            {
                var info = new LightVideoListInfo(video.MessageGuid, video.FileName.Value, this.current.PrevSelectedPlaylistID, video.Id.Value, video.IsSelected.Value);
                LightVideoListinfoHandler.AddVideo(info);
            }
        }

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
                return new AttemptResult()
                {
                    Message = $"{video.NiconicoId}は現在のプレイリストに存在しません。",
                };
            }

            try
            {
                this.Videos.RemoveAll(v => v.NiconicoId == video.NiconicoId);
            }
            catch (Exception e)
            {
                return new AttemptResult()
                {
                    Message = $"メモリ上のプレイリストからの削除に失敗しました。({video.NiconicoId})",
                    Exception = e,
                };
            }


            if (!commit)
            {
                if (id == (this.current.SelectedPlaylist.Value?.Id ?? -1)) this.RaiseListChanged(video, ChangeType.Remove);
                return new AttemptResult()
                {
                    IsSucceeded = true,
                };
            }


            if (id == -1)
            {
                return new AttemptResult()
                {
                    Message = $"プレイストが選択されていません。",
                };
            }

            try
            {
                this.playlistStoreHandler.RemoveVideo(video.Id.Value, id);
            }
            catch (Exception e)
            {
                return new AttemptResult()
                {
                    Message = $"データベース上のプレイリストからの削除に失敗しました。({video.NiconicoId})",
                    Exception = e,
                };
            }

            if (id == (this.current.SelectedPlaylist.Value?.Id ?? -1)) this.RaiseListChanged(video, ChangeType.Remove);
            return new AttemptResult()
            {
                IsSucceeded = true,
            };
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

            return new AttemptResult()
            {
                IsSucceeded = true,
            };
        }

        /// <summary>
        /// 動画を追加する
        /// </summary>
        /// <param name="video"></param>
        /// <param name="commit"></param>
        /// <returns></returns>
        public IAttemptResult Add(IListVideoInfo video, int? playlistID = null, bool commit = true)
        {

            var id = playlistID ?? this.current.SelectedPlaylist.Value?.Id ?? -1;

            if (this.Videos.Any(v => v.NiconicoId.Value == video.NiconicoId.Value))
            {
                return new AttemptResult()
                {
                    Message = $"{video.NiconicoId}は既に現在のプレイリストに存在します。",
                };
            }

            try
            {
                this.Videos.Add(video);
            }
            catch (Exception e)
            {
                return new AttemptResult()
                {
                    Message = $"メモリ上のプレイリストへの追加に失敗しました。({video.NiconicoId})",
                    Exception = e,
                };
            }


            if (!commit)
            {
                if (id == (this.current.SelectedPlaylist.Value?.Id ?? -1)) this.RaiseListChanged(video, ChangeType.Add);
                return new AttemptResult()
                {
                    IsSucceeded = true,
                };
            }


            if (id == -1)
            {
                return new AttemptResult()
                {
                    Message = $"プレイストが選択されていません。",
                };
            }

            try
            {
                this.playlistStoreHandler.AddVideo(video, id);
            }
            catch (Exception e)
            {
                return new AttemptResult()
                {
                    Message = $"データベース上のプレイリストへの追加に失敗しました。({video.NiconicoId})",
                    Exception = e,
                };
            }

            if (id == (this.current.SelectedPlaylist.Value?.Id ?? -1)) this.RaiseListChanged(video, ChangeType.Add);
            return new AttemptResult()
            {
                IsSucceeded = true,
            };
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

            return new AttemptResult()
            {
                IsSucceeded = true,
            };
        }

        /// <summary>
        /// 動画情報を更新する
        /// </summary>
        /// <param name="video"></param>
        /// <param name="commit"></param>
        /// <returns></returns>
        public IAttemptResult Update(IListVideoInfo video, bool commit = true)
        {
            if (!this.Videos.Any(v => v.NiconicoId == video.NiconicoId))
            {
                return new AttemptResult()
                {
                    Message = $"{video.NiconicoId}は現在のプレイリストに存在しません。",
                };
            }

            try
            {
                var targetVIdeo = this.Videos.First(v => v.NiconicoId == video.NiconicoId);
                NonBindableListVideoInfo.SetData(targetVIdeo, video);
            }
            catch (Exception e)
            {
                return new AttemptResult()
                {
                    Message = $"メモリ上の動画情報の更新に失敗しました。({video.NiconicoId})",
                    Exception = e,
                };
            }


            if (!commit)
            {
                return new AttemptResult()
                {
                    IsSucceeded = true,
                };
            }

            try
            {
                this.videoHandler.Update(video);
            }
            catch (Exception e)
            {
                return new AttemptResult()
                {
                    Message = $"データベース上の動画情報の更新に失敗しました。({video.NiconicoId})",
                    Exception = e,
                };
            }

            return new AttemptResult()
            {
                IsSucceeded = true,
            };
        }

        /// <summary>
        /// 複数の動画情報を更新する
        /// </summary>
        /// <param name="videos"></param>
        /// <param name="commit"></param>
        /// <returns></returns>
        public IAttemptResult UpdateRange(IEnumerable<IListVideoInfo> videos, bool commit = true)
        {
            foreach (var video in videos.Copy())
            {
                var result = this.Update(video, commit);
                if (!result.IsSucceeded)
                {
                    return result;
                }
            }

            return new AttemptResult()
            {
                IsSucceeded = true,
            };
        }

        /// <summary>
        /// チャックを外す
        /// </summary>
        /// <param name="video"></param>
        /// <param name="playlistID"></param>
        /// <returns></returns>
        public IAttemptResult Uncheck(int videoID, int playlistID)
        {
            if (this.Videos.Any(v => v.Id.Value == videoID))
            {
                var target = this.Videos.First(v => v.Id.Value == videoID);
                target.IsSelected.Value = false;
            }
            else if (LightVideoListinfoHandler.Contains(videoID, playlistID))
            {
                var target = LightVideoListinfoHandler.GetLightVideoListInfo(videoID, playlistID)!;
                var light = new LightVideoListInfo(target.MessageGuid, target.FileName, target.PlaylistId, target.VideoId, false);
                LightVideoListinfoHandler.AddVideo(light);
            }
            else
            {
                return new AttemptResult()
                {
                    Message = $"指定された動画は存在しません。(videoID:{videoID}, playlistID:{playlistID})",
                };
            }

            return new AttemptResult()
            {
                IsSucceeded = true,
            };
        }


        /// <summary>
        /// 動画リストを再読み込みする
        /// </summary>
        /// <returns></returns>
        public IAttemptResult Refresh()
        {
            this.SavePrevPlaylistVideos();
            this.Clear();
            var result = this.refresher.Refresh(this.Videos);

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

            IEnumerable<IListVideoInfo> SortWithCustom(List<IListVideoInfo> source, List<int>? seq)
            {
                if (seq is null) throw new InvalidOperationException();
                return seq.Select(id => source.FirstOrDefault(v => v.Id.Value == id) ?? new NonBindableListVideoInfo()).Where(v => !v.Title.Value.IsNullOrEmpty());
            }

            var tmp = this.Videos.ToList();
            this.Clear();
            if (!isDescending)
            {
                var sorted = sortType switch
                {
                    VideoSortType.Register => tmp.OrderBy(v => v.Id.Value),
                    VideoSortType.Title => tmp.OrderBy(v => v.Title.Value),
                    VideoSortType.NiconicoID => tmp.OrderBy(v => v.NiconicoId.Value),
                    VideoSortType.UploadedDT => tmp.OrderBy(v => v.UploadedOn.Value),
                    VideoSortType.ViewCount => tmp.OrderBy(v => v.ViewCount.Value),
                    VideoSortType.DownloadedFlag => tmp.OrderBy(v => v.IsDownloaded.Value ? 1 : 0),
                    _ => SortWithCustom(tmp, customSortSequence),
                };
                this.AddRange(sorted);
            }
            else
            {

                var sorted = sortType switch
                {
                    VideoSortType.Register => tmp.OrderByDescending(v => v.Id.Value),
                    VideoSortType.Title => tmp.OrderByDescending(v => v.Title.Value),
                    VideoSortType.NiconicoID => tmp.OrderByDescending(v => v.NiconicoId.Value),
                    VideoSortType.UploadedDT => tmp.OrderByDescending(v => v.UploadedOn.Value),
                    VideoSortType.ViewCount => tmp.OrderByDescending(v => v.ViewCount.Value),
                    VideoSortType.DownloadedFlag => tmp.OrderByDescending(v => v.IsDownloaded.Value ? 1 : 0),
                    _ => SortWithCustom(tmp, customSortSequence),
                };
                this.AddRange(sorted);
            }

            return new AttemptResult() { IsSucceeded = true };
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
