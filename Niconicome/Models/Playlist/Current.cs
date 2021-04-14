using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using Niconicome.Extensions.System;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Domain.Network;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Local;
using Niconicome.Models.Network;

namespace Niconicome.Models.Playlist
{

    public interface ICurrent
    {
        void Update(int playlistId, bool savePrev = true);
        void Update(int playlistId, IListVideoInfo video);
        void Uncheck(int playlistID, int videoID);
        ITreePlaylistInfo? CurrentSelectedPlaylist { get; set; }
        event EventHandler? SelectedItemChanged;
        event EventHandler VideosChanged;
        ObservableCollection<IListVideoInfo> Videos { get; }
    }

    class Current : ICurrent
    {
        public Current(ICacheHandler cacheHandler, IVideoHandler videoHandler, IVideoThumnailUtility videoThumnailUtility, IPlaylistStoreHandler playlistStoreHandler, ILocalSettingHandler settingHandler, ILocalVideoUtils localVideoUtils)
        {
            this.cacheHandler = cacheHandler;
            this.videoHandler = videoHandler;
            this.videoThumnailUtility = videoThumnailUtility;
            this.playlistStoreHandler = playlistStoreHandler;
            this.settingHandler = settingHandler;
            this.localVideoUtils = localVideoUtils;
            this.Videos = new ObservableCollection<IListVideoInfo>();
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

        private readonly ILocalSettingHandler settingHandler;

        private readonly ILocalVideoUtils localVideoUtils;

        private readonly object lockObj = new();

        public ObservableCollection<IListVideoInfo> Videos { get; init; }

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
                this.SavePrevPlaylistVideos();

                if (value is not null && value.Folderpath.IsNullOrEmpty())
                {
                    value.Folderpath = this.settingHandler.GetStringSetting(Settings.DefaultFolder) ?? "download";
                }

                this.currentSelectedPlaylistField = value;
                this.Update(this.CurrentSelectedPlaylist?.Id ?? -1, false);
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
                var fileName = string.Empty;
                if (LightVideoListinfoHandler.Contains(videoID, playlistID))
                {
                    var light = LightVideoListinfoHandler.GetLightVideoListInfo(videoID, playlistID);
                    messageGuid = light!.MessageGuid;
                    fileName = light!.FileName;
                }

                var video = new LightVideoListInfo(messageGuid, fileName, playlistID, videoID, false);
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
            if (this.CurrentSelectedPlaylist is null) return;

            foreach (var video in this.Videos)
            {
                var info = new LightVideoListInfo(video.MessageGuid, video.FileName, this.CurrentSelectedPlaylist.Id, video.Id, video.IsSelected);
                LightVideoListinfoHandler.AddVideo(info);
            }
        }

        /// <summary>
        /// 動画リストを更新する
        /// </summary>
        public void Update(int playlistId, bool savePrev = true)
        {
            if (playlistId != (this.CurrentSelectedPlaylist?.Id ?? -1)) return;

            if (savePrev)
            {
                this.SavePrevPlaylistVideos();
            }

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
                    var format = this.settingHandler.GetStringSetting(Settings.FileNameFormat) ?? "[<id>]<title>";
                    var replaceStricted = this.settingHandler.GetBoolSetting(Settings.ReplaceSBToMB);
                    var folderPath = this.CurrentSelectedPlaylist.Folderpath;

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
                                video.FileName = lightVideo.FileName;
                            }
                            else
                            {
                                video.FileName = string.Empty;
                            }

                            if (video.FileName.IsNullOrEmpty())
                            {
                                video.FileName = this.localVideoUtils.GetFilePath(video, folderPath, format, replaceStricted);
                            }
                            video.IsDownloaded = !video.FileName.IsNullOrEmpty();

                            //サムネイル
                            bool hasCache = this.videoThumnailUtility.HasThumbnailCache(video);
                            bool isValid = this.videoThumnailUtility.IsValidThumbnail(video);

                            if (isValid && !hasCache)
                            {
                                this.videoThumnailUtility.GetThumbAsync(video);
                                video.ThumbPath = this.videoThumnailUtility.GetThumbFilePath(video.NiconicoId);
                                this.videoHandler.Update(video);
                                this.Videos.Add(video);
                            }
                            else if (!hasCache)
                            {
                                video.ThumbPath = this.videoThumnailUtility.GetThumbFilePath("0");
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
        public void Update(int playlistId, IListVideoInfo video)
        {

            if (this.Videos.Count == 0) return;

            if (this.CurrentSelectedPlaylist is null) return;

            lock (this.lockObj)
            {
                if (this.Videos.Any(v => (v?.Id ?? 0) == video.Id))
                {
                    var format = this.settingHandler.GetStringSetting(Settings.FileNameFormat) ?? "[<id>]<title>";
                    var replaceStricted = this.settingHandler.GetBoolSetting(Settings.ReplaceSBToMB);
                    var currentVideo = this.Videos.FirstOrDefault(v => v.Id == video.Id);
                    if (currentVideo is null) return;

                    video.FileName = this.localVideoUtils.GetFilePath(video, this.CurrentSelectedPlaylist.Folderpath, format, replaceStricted);
                    video.IsDownloaded = !video.FileName.IsNullOrEmpty();

                    BindableListVIdeoInfo.SetData(currentVideo, video);
                    this.Videos.Add(video);
                }
            }


        }

        public event EventHandler? SelectedItemChanged;

        public event EventHandler VideosChanged;
    }
}
