using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Helper.Event.Generic;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Playlist.VideoList
{

    public interface IVideoListContainer
    {
        IEnumerable<IListVideoInfo> GetVideos();
        IAttemptResult Remove(IListVideoInfo video, int? playlistID = null, bool commit = true);
        IAttemptResult RemoveRange(IEnumerable<IListVideoInfo> videos, int? playlistID = null, bool commit = true);
        IAttemptResult Add(IListVideoInfo video, int? playlistID = null, bool commit = true);
        IAttemptResult AddRange(IEnumerable<IListVideoInfo> videos, int? playlistID = null, bool commit = true);
        IAttemptResult Update(IListVideoInfo video, bool commit = true);
        IAttemptResult UpdateRange(IEnumerable<IListVideoInfo> videos, bool commit = true);
        IAttemptResult Refresh();
        IAttemptResult Clear();
        IAttemptResult Uncheck(int videoID, int playlistID);
        int Count { get; }
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
            this.videos = new List<IListVideoInfo>();
        }

        #region DIされるクラス

        private readonly IPlaylistStoreHandler playlistStoreHandler;

        private readonly IVideoHandler videoHandler;

        private readonly IVideoListRefresher refresher;

        private readonly ICurrent current;

        #endregion

        #region プライベートフィールド

        private readonly List<IListVideoInfo> videos;

        #endregion

        #region プライベートメソッド

        /// <summary>
        /// 選択解除されたプレイリストを保持する
        /// </summary>
        private void SavePrevPlaylistVideos()
        {
            if (this.current.SelectedPlaylist is null) return;

            foreach (var video in this.videos)
            {
                var info = new LightVideoListInfo(video.MessageGuid, video.FileName, this.current.SelectedPlaylist.Id, video.Id, video.IsSelected);
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
        public int Count { get => this.videos.Count; }

        /// <summary>
        /// 動画を取得する
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IListVideoInfo> GetVideos()
        {
            return this.videos;
        }

        /// <summary>
        /// 動画を削除する
        /// </summary>
        /// <param name="video"></param>
        /// <param name="commit"></param>
        /// <returns></returns>
        public IAttemptResult Remove(IListVideoInfo video, int? playlistID = null, bool commit = true)
        {
            var id = playlistID ?? this.current.SelectedPlaylist?.Id ?? -1;

            if (!this.videos.Any(v => v.NiconicoId == video.NiconicoId))
            {
                return new AttemptResult()
                {
                    Message = $"{video.NiconicoId}は現在のプレイリストに存在しません。",
                };
            }

            try
            {
                this.videos.RemoveAll(v => v.NiconicoId == video.NiconicoId);
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
                if (id == (this.current.SelectedPlaylist?.Id ?? -1)) this.RaiseListChanged(video, ChangeType.Remove);
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
                this.playlistStoreHandler.RemoveVideo(video.Id, id);
            }
            catch (Exception e)
            {
                return new AttemptResult()
                {
                    Message = $"データベース上のプレイリストからの削除に失敗しました。({video.NiconicoId})",
                    Exception = e,
                };
            }

            if (id == (this.current.SelectedPlaylist?.Id ?? -1)) this.RaiseListChanged(video, ChangeType.Remove);
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

            var id = playlistID ?? this.current.SelectedPlaylist?.Id ?? -1;

            if (this.videos.Any(v => v.NiconicoId == video.NiconicoId))
            {
                return new AttemptResult()
                {
                    Message = $"{video.NiconicoId}は既に現在のプレイリストに存在します。",
                };
            }

            try
            {
                this.videos.Add(video);
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
                if (id == (this.current.SelectedPlaylist?.Id ?? -1)) this.RaiseListChanged(video, ChangeType.Add);
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

            if (id == (this.current.SelectedPlaylist?.Id ?? -1)) this.RaiseListChanged(video, ChangeType.Add);
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
            if (!this.videos.Any(v => v.NiconicoId == video.NiconicoId))
            {
                return new AttemptResult()
                {
                    Message = $"{video.NiconicoId}は現在のプレイリストに存在しません。",
                };
            }

            try
            {
                var targetVIdeo = this.videos.First(v => v.NiconicoId == video.NiconicoId);
                BindableListVIdeoInfo.SetData(targetVIdeo, video);
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
            if (this.videos.Any(v => v.Id == videoID))
            {
                var target = this.videos.First(v => v.Id == videoID);
                target.IsSelected = false;
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
            var result = this.refresher.Refresh(this.videos);

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
                this.videos.Clear();
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
        /// 動画リスト変更イベント
        /// </summary>
        public event EventHandler<ListChangedEventArgs<IListVideoInfo>>? ListChanged;

    }
}
