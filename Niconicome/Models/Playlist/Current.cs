using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Domain.Network;
using Niconicome.Models.Network;

namespace Niconicome.Models.Playlist
{

    public interface ICurrent
    {
        void Update(int playlistId);
        void Update(int playlistId, ITreeVideoInfo video);
        ITreePlaylistInfo? CurrentSelectedPlaylist { get; set; }
        event EventHandler SelectedItemChanged;
        ObservableCollection<ITreeVideoInfo> Videos { get; }
    }

    class Current : ICurrent
    {
        public Current(IPlaylistTreeHandler handler,ICacheHandler cacheHandler, IVideoHandler videoHandler,IVideoThumnailUtility videoThumnailUtility)
        {
            this.handler = handler;
            this.cacheHandler = cacheHandler;
            this.videoHandler = videoHandler;
            this.videoThumnailUtility = videoThumnailUtility;
            this.Videos = new ObservableCollection<ITreeVideoInfo>();
            BindingOperations.EnableCollectionSynchronization(this.Videos, new object());
            this.SelectedItemChanged += this.OnSelectedItemChanged;
        }

        ~Current()
        {
            this.SelectedItemChanged -= this.OnSelectedItemChanged;
        }

        private readonly IPlaylistTreeHandler handler;

        private readonly IVideoThumnailUtility videoThumnailUtility;

        private readonly IVideoHandler videoHandler;

        private readonly ICacheHandler cacheHandler;

        public ObservableCollection<ITreeVideoInfo> Videos { get; init; }

        /// <summary>
        /// 選択中のプレイリスト
        /// </summary>
        private ITreePlaylistInfo? currentSelectedPlaylistField;

        /// <summary>
        /// 公開プロパティー
        /// </summary>
        public ITreePlaylistInfo? CurrentSelectedPlaylist
        {
            get { return this.currentSelectedPlaylistField; }
            set
            {
                this.currentSelectedPlaylistField = value;
                this.RaiseSelectedItemChanged();
            }
        }

        /// <summary>
        /// プレイリスト変更時に発火するイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSelectedItemChanged(object? sender, EventArgs e)
        {
            this.Update(this.CurrentSelectedPlaylist!.Id);
        }

        /// <summary>
        /// イベントを発火させる
        /// </summary>
        private void RaiseSelectedItemChanged()
        {
            this.SelectedItemChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 動画リストを更新する
        /// </summary>
        public void Update(int playlistId)
        {
            if (playlistId != (this.CurrentSelectedPlaylist?.Id ?? -1)) return;

            this.Videos.Clear();
            Task.Run(async () =>
            {
                if (!this.cacheHandler.HasCache("0", CacheType.Thumbnail))
                {
                    await this.cacheHandler.GetOrCreateCacheAsync("0", CacheType.Thumbnail, "https://nicovideo.cdn.nimg.jp/web/img/common/video_deleted.jpg");
                }

                if (this.CurrentSelectedPlaylist is not null)
                {
                    int id = this.CurrentSelectedPlaylist.Id;
                    ITreePlaylistInfo? playlist = this.handler.GetPlaylist(id);
                    IEnumerable<ITreeVideoInfo>? videos = playlist?.Videos.Copy();

                    if (videos is not null && videos.Any())
                    {
                        foreach (var oldVideo in videos)
                        {
                            var video = this.videoHandler.GetVideo(oldVideo.Id);
                            video.MessageGuid = oldVideo.MessageGuid;
                            bool isValid = this.videoThumnailUtility.IsValidThumbnail(video);
                            bool hasCache = this.videoThumnailUtility.HasThumbnailCache(video);
                            if (!isValid || !hasCache)
                            {
                                if (!isValid)
                                {
                                    await this.videoThumnailUtility.GetAndSetThumbFilePathAsync(video, true);
                                }
                                else
                                {
                                    await this.videoThumnailUtility.SetThumbPathAsync(video);
                                }
                                this.videoHandler.Update(video);
                                this.Videos.Add(video);
                            }
                            else
                            {
                                this.Videos.Add(video);
                                continue;
                            }
                        }
                    }

                }
            });

        }

        /// <summary>
        /// 特定の動画のみ更新する
        /// </summary>
        /// <param name="playlistId"></param>
        /// <param name="video"></param>
        public void Update(int playlistId, ITreeVideoInfo video)
        {

            if (this.Videos.Count == 0) return;
            if (!this.Videos.Any(v => v.Id == video.Id))
            {
                this.Videos.Add(video);
            } else
            {
                int index = this.Videos.FindIndex(v => v.Id == video.Id);
                if (index == -1) return;
                this.Videos.RemoveAt(index);
                this.Videos.Insert(index, video);
            }


        }


        public event EventHandler SelectedItemChanged;
    }
}
