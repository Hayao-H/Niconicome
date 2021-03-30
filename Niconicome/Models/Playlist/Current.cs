using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Domain.Network;
using Niconicome.Models.Network;

namespace Niconicome.Models.Playlist
{

    public interface ICurrent
    {
        void Update(int playlistId);
        void Update(int playlistId, IVideoListInfo video);
        void Uncheck(int playlistID, int videoID);
        ITreePlaylistInfo? CurrentSelectedPlaylist { get; set; }
        event EventHandler? SelectedItemChanged;
        event EventHandler VideosChanged;
        ObservableCollection<IVideoListInfo> Videos { get; }
    }

    class Current : ICurrent
    {
        public Current(ICacheHandler cacheHandler, IVideoHandler videoHandler, IVideoThumnailUtility videoThumnailUtility, IPlaylistStoreHandler playlistStoreHandler)
        {
            this.cacheHandler = cacheHandler;
            this.videoHandler = videoHandler;
            this.videoThumnailUtility = videoThumnailUtility;
            this.playlistStoreHandler = playlistStoreHandler;
            this.Videos = new ObservableCollection<IVideoListInfo>();
            BindingOperations.EnableCollectionSynchronization(this.Videos, new object());
            this.VideosChanged += this.OnVideoschanged;

        }

        ~Current()
        {
            this.SelectedItemChanged -= this.OnSelectedItemChanged;
            this.VideosChanged -= this.OnVideoschanged;
        }

        private readonly IVideoThumnailUtility videoThumnailUtility;

        private readonly IVideoHandler videoHandler;

        private readonly ICacheHandler cacheHandler;

        private readonly IPlaylistStoreHandler playlistStoreHandler;

        private readonly object lockObj = new();

        public ObservableCollection<IVideoListInfo> Videos { get; init; }

        /// <summary>
        /// 選択中のプレイリスト
        /// </summary>
        private ITreePlaylistInfo? currentSelectedPlaylistField;

        /// <summary>
        /// ひとつまえに選択中だったプレイリスト
        /// </summary>
        private ITreePlaylistInfo? prevSelectedPlaylist;

        /// <summary>
        /// 公開プロパティー
        /// </summary>
        public ITreePlaylistInfo? CurrentSelectedPlaylist
        {
            get { return this.currentSelectedPlaylistField; }
            set
            {
                this.prevSelectedPlaylist = this.CurrentSelectedPlaylist;
                this.prevSelectedPlaylist?.Videos.Clear();
                this.prevSelectedPlaylist?.Videos.AddRange(this.Videos);
                this.SavePrevPlaylistVideos();

                this.currentSelectedPlaylistField = value;
                this.Update(this.CurrentSelectedPlaylist?.Id ?? -1);
            }
        }

        /// <summary>
        /// チェックを外す
        /// </summary>
        /// <param name="playlistID"></param>
        /// <param name="videoID"></param>
       　public void Uncheck(int playlistID, int videoID)
        {
            if (this.CurrentSelectedPlaylist is null) return;

            if (playlistID == this.CurrentSelectedPlaylist.Id)
            {
                var video = this.Videos.FirstOrDefault(v => v.Id == videoID);
                if (video is not null)
                {
                    video.IsSelected = false;
                }
            }
            else
            {
                var messageGuid = string.Empty;
                if (LightVideoListinfoHandler.Contains(new LightVideoListInfo(messageGuid, playlistID, videoID, false)))
                {
                    messageGuid = LightVideoListinfoHandler.GetLightVideoListInfo(videoID, playlistID)!.MessageGuid;
                }

                var video = new LightVideoListInfo(messageGuid, playlistID, videoID, false);
                LightVideoListinfoHandler.AddVideo(video);
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
        /// 動画のコレクション変更時に発火するイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnVideoschanged(object? sender, EventArgs e)
        {

        }

        /// <summary>
        /// イベントを発火させる
        /// </summary>
        private void RaiseSelectedItemChanged()
        {
            this.SelectedItemChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 動画変更イベントを発火させる
        /// </summary>
        private void RaiseVideoschanged()
        {
            this.VideosChanged.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 選択解除されたプレイリストを保持する
        /// </summary>
        private void SavePrevPlaylistVideos()
        {
            if (this.prevSelectedPlaylist is null) return;

            foreach (var video in this.prevSelectedPlaylist.Videos)
            {
                var info = new LightVideoListInfo(video.MessageGuid, this.prevSelectedPlaylist.Id, video.Id, video.IsSelected);
                LightVideoListinfoHandler.AddVideo(info);
            }
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
                    var videos = this.playlistStoreHandler.GetPlaylist(playlistId)?.Videos.Copy();

                    if (videos is not null)
                    {
                        foreach (var oldVideo in videos)
                        {
                            if (!this.videoHandler.Exist(oldVideo.Id)) continue;
                            if (playlistId != (this.CurrentSelectedPlaylist?.Id ?? -1)) return;

                            var video = this.videoHandler.GetVideo(oldVideo.Id);

                            //保持されている動画情報があれば引き継ぐ
                            var lightVideo = LightVideoListinfoHandler.GetLightVideoListInfo(oldVideo.Id, playlistId);

                            if (lightVideo is not null)
                            {
                                video.MessageGuid = lightVideo.MessageGuid;
                                video.IsSelected = lightVideo.IsSelected;
                                video.Message = VideoMessanger.GetMessage(lightVideo.MessageGuid);
                            }

                            //サムネイル
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
                            }
                        }

                        this.RaiseVideoschanged();
                    }

                }
            });
            this.RaiseSelectedItemChanged();
        }

        /// <summary>
        /// 特定の動画のみ更新する
        /// </summary>
        /// <param name="playlistId"></param>
        /// <param name="video"></param>
        public void Update(int playlistId, IVideoListInfo video)
        {

            if (this.Videos.Count == 0) return;

            lock (this.lockObj)
            {
                if (!this.Videos.Any(v => (v?.Id ?? 0) == video.Id))
                {
                    this.Videos.Add(video);
                }
                else
                {
                    var currentVideo = this.Videos.FirstOrDefault(v => v.Id == video.Id);
                    if (currentVideo is null) return;
                    BindableVIdeoListInfo.SetData(currentVideo, video);
                }
            }


        }

        public event EventHandler? SelectedItemChanged;

        public event EventHandler VideosChanged;
    }
}
