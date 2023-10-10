using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Niconico.Watch;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Local.Settings.EnumSettingsValue;
using Niconicome.ViewModels.Mainpage.Utils;
using Niconicome.ViewModels.Setting.Utils;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Setting.Pages
{
    class VideoListSettingsPageViewModel
    {
        public VideoListSettingsPageViewModel()
        {
            #region ダブルクリック
            var openInPlayerA = new ComboboxItem<VideoClickSettings>(VideoClickSettings.OpenInPlayerA, "アプリで開く(A)");
            var openInPlayerB = new ComboboxItem<VideoClickSettings>(VideoClickSettings.OpenInPlayerB, "アプリで開く(B)");
            var sendToAppA = new ComboboxItem<VideoClickSettings>(VideoClickSettings.SendToAppA, "アプリに送る(A)");
            var sendToAppB = new ComboboxItem<VideoClickSettings>(VideoClickSettings.SendToAppB, "アプリに送る(B)");
            var download = new ComboboxItem<VideoClickSettings>(VideoClickSettings.Download, "ダウンロードする");
            var none = new ComboboxItem<VideoClickSettings>(VideoClickSettings.NotConfigured, "何もしない");

            this.SelectableVideodbClickAction = new List<ComboboxItem<VideoClickSettings>>() { none, openInPlayerA, openInPlayerB, sendToAppA, sendToAppB, download };
            this.VideodbClickAction = new ConvertableSettingInfoViewModel<VideoClickSettings, ComboboxItem<VideoClickSettings>>(WS::SettingPage.SettingsConainer.GetSetting(SettingNames.VideoListItemdbClickAction,VideoClickSettings.NotConfigured), none, x => x switch
            {
                VideoClickSettings.OpenInPlayerA => openInPlayerA,
                VideoClickSettings.OpenInPlayerB => openInPlayerB,
                VideoClickSettings.SendToAppA => sendToAppA,
                VideoClickSettings.SendToAppB => sendToAppB,
                VideoClickSettings.Download => download,
                _ => none,
            }, x => x.Value);

            this.IsRestoreingColumnWidthDisabled = new SettingInfoViewModel<bool>(WS::SettingPage.SettingsConainer.GetSetting(SettingNames.IsRestoringColumnWidthDisabled,false), false);

            this.IsDownloadSucceededHistoryDisabled = new SettingInfoViewModel<bool>(WS::SettingPage.SettingsConainer.GetSetting<bool>(SettingNames.DisableDownloadSucceededHistory,false), false);
            this.IsDownloadFailedHistoryDisabled = new SettingInfoViewModel<bool>(WS::SettingPage.SettingsConainer.GetSetting<bool>(SettingNames.DisableDownloadFailedHistory,false), false);
            this.IsRestoringScrollPosDisabled = new SettingInfoViewModel<bool>(WS::SettingPage.SettingsConainer.GetSetting<bool>(SettingNames.DisableScrollRestore, false), false);
            this.IsPlaybackHistoryDisabled = new SettingInfoViewModel<bool>(WS::SettingPage.SettingsConainer.GetSetting(SettingNames.DisablePlaybackHistory, false), false);

            #endregion
        }

        #region フィールド
        #endregion

        /// <summary>
        /// 選択可能なダブルクリックアクション
        /// </summary>
        public List<ComboboxItem<VideoClickSettings>> SelectableVideodbClickAction { get; init; }

        /// <summary>
        /// ダブルクリックアクション
        /// </summary>
        public ConvertableSettingInfoViewModel<VideoClickSettings, ComboboxItem<VideoClickSettings>> VideodbClickAction { get; init; }

        /// <summary>
        /// 幅を継承しない
        /// </summary>
        public SettingInfoViewModel<bool> IsRestoreingColumnWidthDisabled { get; init; }

        /// <summary>
        /// DL成功履歴を無効にする
        /// </summary>
        public SettingInfoViewModel<bool> IsDownloadSucceededHistoryDisabled { get; init; }

        /// <summary>
        /// DL失敗履歴を無効にする
        /// </summary>
        public SettingInfoViewModel<bool> IsDownloadFailedHistoryDisabled { get; init; }

        /// <summary>
        /// 再生履歴を無効にする
        /// </summary>
        public SettingInfoViewModel<bool> IsPlaybackHistoryDisabled { get; init; }

        /// <summary>
        /// スクロール一復元を無効にする
        /// </summary>
        public SettingInfoViewModel<bool> IsRestoringScrollPosDisabled { get; init; }

    }

    class VideoListSettingsPageViewModelD
    {
        public VideoListSettingsPageViewModelD()
        {
            #region ダブルクリック
            var openInPlayerA = new ComboboxItem<VideoClickSettings>(VideoClickSettings.OpenInPlayerA, "アプリで開く(A)");
            var openInPlayerB = new ComboboxItem<VideoClickSettings>(VideoClickSettings.OpenInPlayerB, "アプリで開く(B)");
            var sendToAppA = new ComboboxItem<VideoClickSettings>(VideoClickSettings.SendToAppA, "アプリに送る(A)");
            var sendToAppB = new ComboboxItem<VideoClickSettings>(VideoClickSettings.SendToAppB, "アプリに送る(B)");
            var download = new ComboboxItem<VideoClickSettings>(VideoClickSettings.Download, "ダウンロードする");
            var none = new ComboboxItem<VideoClickSettings>(VideoClickSettings.NotConfigured, "何もしない");

            this.SelectableVideodbClickAction = new List<ComboboxItem<VideoClickSettings>>() { none, openInPlayerA, openInPlayerB, sendToAppA, sendToAppB, download };
            this.VideodbClickAction = none;
            #endregion
        }

        public List<ComboboxItem<VideoClickSettings>> SelectableVideodbClickAction { get; init; }

        public ComboboxItem<VideoClickSettings> VideodbClickAction { get; set; }

        public SettingInfoViewModelD<bool> IsRestoreingColumnWidthDisabled { get; init; } = new(false);

        public SettingInfoViewModelD<bool> IsDownloadSucceededHistoryDisabled { get; init; } = new(true);

        public SettingInfoViewModelD<bool> IsDownloadFailedHistoryDisabled { get; init; } = new(true);

        public SettingInfoViewModelD<bool> IsRestoringScrollPosDisabled { get; init; } = new(true);

        public SettingInfoViewModelD<bool> IsPlaybackHistoryDisabled { get; init; } = new(true);

    }
}
