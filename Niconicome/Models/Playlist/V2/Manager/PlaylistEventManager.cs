using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.LocalFile;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.External;
using Niconicome.Models.Local.Settings.EnumSettingsValue;
using Niconicome.Models.Network.Download.DLTask;

namespace Niconicome.Models.Playlist.V2.Manager
{
    public interface IPlaylistEventManager
    {
        /// <summary>
        /// 動画クリック時
        /// </summary>
        /// <param name="video"></param>
        /// <param name="eventType"></param>
        void OnVideoClick(IVideoInfo video, EventType eventType);

        /// <summary>
        /// Delキー押下時
        /// </summary>
        /// <param name="video"></param>
        /// <param name="deleteVideoAction"></param>
        void OnDelKeyDown(Action deleteVideoAction);
    }

    public enum EventType
    {
        Click,
        DoubleClick,
        MiddleClick,
    }

    public class PlaylistEventManager : IPlaylistEventManager
    {
        public PlaylistEventManager(IExternalAppUtilsV2 externalAppUtils, ISettingsContainer settings, IDownloadManager downloadManager,IVideoListManager videoListManager,IPlaylistVideoContainer container)
        {
            this._externalAppUtils = externalAppUtils;
            this._settings = settings;
            this._downloadManaer = downloadManager;
            this._videoListManager = videoListManager;
            this._container = container;
        }

        #region field

        private readonly IExternalAppUtilsV2 _externalAppUtils;

        private readonly ISettingsContainer _settings;

        private readonly IDownloadManager _downloadManaer;

        private readonly IVideoListManager _videoListManager;

        private readonly IPlaylistVideoContainer _container;

        #endregion

        #region Method

        public void OnVideoClick(IVideoInfo video, EventType eventType)
        {
            VideoClickSettings setting;
            if (eventType == EventType.Click)
            {
                IAttemptResult<bool> result = this._settings.GetOnlyValue(SettingNames.IsVideoClickSelectEnable, false);
                if (!result.IsSucceeded || !result.Data) return;

                video.IsSelected.Value = !video.IsSelected.Value;
                return;

            }
            else if (eventType == EventType.DoubleClick)
            {
                IAttemptResult<VideoClickSettings> result = this._settings.GetOnlyValue(SettingNames.VideoListItemdbClickAction, VideoClickSettings.NotConfigured);
                if (!result.IsSucceeded || result.Data == VideoClickSettings.NotConfigured) return;

                setting = result.Data;
            }
            else
            {

                IAttemptResult<VideoClickSettings> result = this._settings.GetOnlyValue(SettingNames.VideoListItemMiddleClickAction, VideoClickSettings.NotConfigured);
                if (!result.IsSucceeded || result.Data == VideoClickSettings.NotConfigured) return;

                setting = result.Data;
            }

            if (setting == VideoClickSettings.OpenInPlayerA)
            {
                this._externalAppUtils.OpenInPlayerA(video);
            }
            else if (setting == VideoClickSettings.OpenInPlayerB)
            {
                this._externalAppUtils.OpenInPlayerB(video);
            }
            else if (setting == VideoClickSettings.SendToAppA)
            {
                this._externalAppUtils.SendToAppA(video);
            }
            else if (setting == VideoClickSettings.SendToAppB)
            {
                this._externalAppUtils.SendToAppB(video);
            }
            else
            {
                this._downloadManaer.StageVIdeo(video);
                this._downloadManaer.StartDownloadAsync();
            }
        }

       public void OnDelKeyDown(Action deleteVideoAction)
        {
            IAttemptResult<VideoDelKeySettings> result = this._settings.GetOnlyValue(SettingNames.VideoListItemDelKeyAction, VideoDelKeySettings.NotConfigured);
            if (!result.IsSucceeded || result.Data == VideoDelKeySettings.NotConfigured) return;

            if (result.Data == VideoDelKeySettings.DeleteVideo)
            {
                deleteVideoAction();
            } else
            {
                var videos = this._container.Videos.Where(v => v.IsSelected.Value).ToArray();
                this._videoListManager.DeleteVideoFilesFromCurrentPlaylistAsync(videos);
            }
        }



        #endregion

    }
}
