using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Local.Settings.EnumSettingsValue;
using Niconicome.Models.Utils.Reactive;
using Niconicome.ViewModels.Setting.V2.Utils;
using WS = Niconicome.Workspaces.SettingPageV2;

namespace Niconicome.ViewModels.Setting.V2.Page
{
    public class VideoListViewModel
    {

        public VideoListViewModel()
        {
            var openInPlayerA = new SelectBoxItem<VideodbClickSettings>( "アプリで開く(A)", VideodbClickSettings.OpenInPlayerA);
            var openInPlayerB = new SelectBoxItem<VideodbClickSettings>("アプリで開く(B)", VideodbClickSettings.OpenInPlayerB);
            var sendToAppA = new SelectBoxItem<VideodbClickSettings>("アプリに送る(A)", VideodbClickSettings.SendToAppA);
            var sendToAppB = new SelectBoxItem<VideodbClickSettings>("アプリに送る(B)", VideodbClickSettings.SendToAppB);
            var download = new SelectBoxItem<VideodbClickSettings>("ダウンロードする", VideodbClickSettings.Download);
            var none = new SelectBoxItem<VideodbClickSettings>("何もしない", VideodbClickSettings.NotConfigured);

            this.SelectableVideodbClickAction = new() { openInPlayerA,openInPlayerB,sendToAppA,sendToAppB ,download,none};
            this.VideodbClickAction = new BindableSettingInfo<VideodbClickSettings>(WS.SettingsContainer.GetSetting(SettingNames.VideoListItemdbClickAction, VideodbClickSettings.NotConfigured), VideodbClickSettings.NotConfigured).AddTo(this.Bindables);

            this.IsRestoreingColumnWidthDisabled = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.IsRestoringColumnWidthDisabled, false), false).AddTo(this.Bindables);

            this.IsPlaybackHistoryDisabled = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.DisablePlaybackHistory, false), false).AddTo(this.Bindables);

            this.IsDownloadSucceededHistoryDisabled = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.DisableDownloadSucceededHistory, false), false).AddTo(this.Bindables);

            this.IsDownloadFailedHistoryDisabled = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.DisableDownloadFailedHistory, false), false).AddTo(this.Bindables);
        }

        public Bindables Bindables { get; init; } = new();

        /// <summary>
        /// 選択可能なダブルクリックアクション
        /// </summary>
        public List<SelectBoxItem<VideodbClickSettings>> SelectableVideodbClickAction { get; init; }

        /// <summary>
        /// ダブルクリックアクション
        /// </summary>
        public IBindableSettingInfo<VideodbClickSettings> VideodbClickAction { get; init; }

        /// <summary>
        /// 幅を継承しない
        /// </summary>
        public IBindableSettingInfo<bool> IsRestoreingColumnWidthDisabled { get; init; }

        /// <summary>
        /// DL成功履歴を無効にする
        /// </summary>
        public IBindableSettingInfo<bool> IsDownloadSucceededHistoryDisabled { get; init; }

        /// <summary>
        /// DL失敗履歴を無効にする
        /// </summary>
        public IBindableSettingInfo<bool> IsDownloadFailedHistoryDisabled { get; init; }

        /// <summary>
        /// 再生履歴を無効にする
        /// </summary>
        public IBindableSettingInfo<bool> IsPlaybackHistoryDisabled { get; init; }
    }
}
