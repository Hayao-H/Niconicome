using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }

    public enum EventType
    {
        DoubleClick,
        MiddleClick,
    }

    public class PlaylistEventManager : IPlaylistEventManager
    {
        public PlaylistEventManager(IExternalAppUtilsV2 externalAppUtils, ISettingsContainer settings, IDownloadManager downloadManager)
        {
            this._externalAppUtils = externalAppUtils;
            this._settings = settings;
            this._downloadManaer = downloadManager;
        }

        #region field

        private readonly IExternalAppUtilsV2 _externalAppUtils;

        private readonly ISettingsContainer _settings;

        private readonly IDownloadManager _downloadManaer;

        #endregion

        #region Method

        public void OnVideoClick(IVideoInfo video, EventType eventType)
        {
            VideoClickSettings setting;

            if (eventType == EventType.DoubleClick)
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


        #endregion

    }
}
