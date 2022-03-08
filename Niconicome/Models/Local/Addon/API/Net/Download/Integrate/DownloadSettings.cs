using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ClearScript;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Network.Download;
using Niconicome.Models.Playlist.VideoList;
using Info = Niconicome.Models.Domain.Niconico.Video.Infomations;

namespace Niconicome.Models.Local.Addon.API.Net.Download.Integrate
{
    public interface IDownloadSettings : IEventDispatcher
    {
        bool canDownload { get; }

        bool downloadVideo { get; set; }

        bool downloadComment { get; set; }

        bool downloadLog { get; set; }

        bool downloadOwner { get; set; }

        bool downloadEasy { get; set; }

        bool downloadThumbnail { get; set; }

        bool downloadDescription { get; set; }

        bool downloadIchiba { get; set; }

        bool limitComment { get; set; }

        int commentLimitCount { get; set; }

        string videoResolution { get; set; }

        string thumbnailSize { get; set; }

        bool overwriteIfExist { get; set; }

        bool skipIfExist { get; set; }

        bool copyIfExist { get; set; }

        bool disableEncode { get; set; }

        Task startDownload(ScriptObject onMessage, ScriptObject onMessageVerbose);

        void cancelDownload();

        void stageSelectedVideos();
    }

    public class DownloadSettings : IDownloadSettings
    {
        public DownloadSettings(IContentDownloader downloader, IDownloadSettingsHandler downloadSettingsHandler, ILocalSettingsContainer settingsContainer,IDownloadTasksHandler tasksHandler,IVideoListContainer container,ICurrent current)
        {
            this._downloader = downloader;
            this._downloadSettings = downloadSettingsHandler;
            this._settingContainer = settingsContainer;
            this._tasksHandler = tasksHandler;
            this._container = container;
            this._current = current;


        }

        #region field

        private readonly IContentDownloader _downloader;

        private readonly IDownloadSettingsHandler _downloadSettings;

        private readonly ILocalSettingsContainer _settingContainer;

        private readonly IDownloadTasksHandler _tasksHandler;

        private readonly IVideoListContainer _container;

        private readonly ICurrent _current;

        private static string canDownloadChangeEventName = "canDownloadChange";

        #endregion

        #region Props

        public bool canDownload => this._downloader.CanDownload.Value;

        public bool downloadVideo
        {
            get => this._downloadSettings.IsDownloadingVideoEnable.Value;
            set => this._downloadSettings.IsDownloadingVideoEnable.Value = value;
        }

        public bool downloadComment
        {
            get => this._downloadSettings.IsDownloadingCommentEnable.Value;
            set => this._downloadSettings.IsDownloadingCommentEnable.Value = value;
        }

        public bool downloadLog
        {
            get => this._downloadSettings.IsDownloadingCommentLogEnable.Value;
            set => this._downloadSettings.IsDownloadingCommentLogEnable.Value = value;
        }

        public bool downloadOwner
        {
            get => this._downloadSettings.IsDownloadingOwnerComment.Value;
            set => this._downloadSettings.IsDownloadingOwnerComment.Value = value;
        }

        public bool downloadEasy
        {
            get => this._downloadSettings.IsDownloadingEasyComment.Value;
            set => this._downloadSettings.IsDownloadingEasyComment.Value = value;
        }

        public bool downloadThumbnail
        {
            get => this._downloadSettings.IsDownloadingThumbEnable.Value;
            set => this._downloadSettings.IsDownloadingThumbEnable.Value = value;
        }

        public bool downloadDescription
        {
            get => this._downloadSettings.IsDownloadingVideoInfoEnable.Value;
            set => this._downloadSettings.IsDownloadingVideoInfoEnable.Value = value;
        }

        public bool downloadIchiba
        {
            get => this._downloadSettings.IsDownloadingIchibaInfoEnable.Value;
            set => this._downloadSettings.IsDownloadingIchibaInfoEnable.Value = value;
        }

        public bool limitComment
        {
            get => this._downloadSettings.IsLimittingCommentCountEnable.Value;
            set => this._downloadSettings.IsLimittingCommentCountEnable.Value = value;
        }

        public int commentLimitCount
        {
            get => this._downloadSettings.MaxCommentsCount.Value;
            set => this._downloadSettings.MaxCommentsCount.Value = value;
        }

        public string videoResolution
        {
            get => $"{this._downloadSettings.Resolution.Value.Vertical}x${this._downloadSettings.Resolution.Value.Horizontal}";
            set => this._downloadSettings.Resolution.Value = new Info::Resolution(value);
        }

        public string thumbnailSize
        {
            get => this._downloadSettings.ThumbnailSize.Value switch
            {
                Info::ThumbSize.Large => "large",
                Info::ThumbSize.Middle => "middle",
                Info::ThumbSize.Normal => "normal",
                _ => "player"
            };
            set => this._downloadSettings.ThumbnailSize.Value = value switch
            {
                "large" => Info::ThumbSize.Large,
                "middle" => Info::ThumbSize.Middle,
                "normal" => Info::ThumbSize.Normal,
                _ => Info::ThumbSize.Player
            };
        }

        public bool overwriteIfExist
        {
            get => this._downloadSettings.IsOverwriteEnable.Value;
            set => this._downloadSettings.IsOverwriteEnable.Value = value;
        }

        public bool skipIfExist
        {
            get => this._downloadSettings.IsSkippingEnable.Value;
            set => this._downloadSettings.IsSkippingEnable.Value = value;
        }

        public bool copyIfExist
        {
            get => this._downloadSettings.IsCopyFromAnotherFolderEnable.Value;
            set => this._downloadSettings.IsCopyFromAnotherFolderEnable.Value = value;
        }

        public bool disableEncode
        {
            get => this._downloadSettings.IsNoEncodeEnable.Value;
            set => this._downloadSettings.IsNoEncodeEnable.Value = value;
        }

        #endregion

        #region Method

        public void addEventListner(string eventName, ScriptObject listner)
        {
            if (eventName == canDownloadChangeEventName)
            {
                this._downloader.CanDownload.Subscribe(_ =>
                {
                    try
                    {
                        listner.Invoke(false);
                    }
                    catch { }
                });
            }
        }

        public async Task startDownload(ScriptObject onMessage, ScriptObject onMessageVerbose)
        {
            if (this._current.SelectedPlaylist.Value is null) return;

            bool dlFromQueue = this._settingContainer.GetReactiveBoolSetting(SettingsEnum.DLAllFromQueue).Value;
            int currentPlaylistID = this._current.SelectedPlaylist.Value.Id;

            if (dlFromQueue)
            {
                this._tasksHandler.MoveStagedToQueue();
            }
            else
            {
                this._tasksHandler.MoveStagedToQueue(t => t.PlaylistID == currentPlaylistID);
            }

            await this._downloader.DownloadVideosFriendlyAsync(m =>
             {
                 try
                 {
                     onMessageVerbose.Invoke(false, m);
                 }
                 catch { };
             }, m =>
             {
                 try
                 {
                     onMessage.Invoke(false, m);
                 }
                 catch { };
             });
        }

        public void cancelDownload()
        {
            this._downloader.Cancel();
        }

        public void stageSelectedVideos()
        {
            bool allowDupe = this._settingContainer.GetReactiveBoolSetting(SettingsEnum.AllowDupeOnStage).Value;

            this._tasksHandler.StageVIdeos(this._container.Videos.Where(v => v.IsSelected.Value), this._downloadSettings.CreateDownloadSettings(), allowDupe);
        }

        #endregion
    }
}
