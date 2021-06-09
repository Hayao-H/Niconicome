using Niconicome.Models.Local.Settings;

namespace Niconicome.Models.Playlist.Playlist
{
    public interface IPlaylistSettingsHandler
    {
        bool IsDownloadFailedHistoryDisabled { get; }
        bool IsDownloadSucceededHistoryDisabled { get; }
    }

    class PlaylistSettingsHandler : IPlaylistSettingsHandler
    {
        public PlaylistSettingsHandler(ILocalSettingHandler settingHandler)
        {
            this.settingHandler = settingHandler;
        }

        #region field

        private readonly ILocalSettingHandler settingHandler;

        #endregion

        /// <summary>
        /// DL失敗履歴
        /// </summary>
        public bool IsDownloadFailedHistoryDisabled => this.settingHandler.GetBoolSetting(SettingsEnum.DisableDLFailedHistory);

        /// <summary>
        /// DL成功履歴
        /// </summary>
        public bool IsDownloadSucceededHistoryDisabled => this.settingHandler.GetBoolSetting(SettingsEnum.DisableDLSucceededHistory);

    }
}
