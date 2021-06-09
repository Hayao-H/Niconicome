using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Watch;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Local.Settings.EnumSettingsValue;
using Niconicome.ViewModels.Mainpage.Utils;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Setting.Pages
{
    class VideoListSettingsPageViewModel : SettingaBase
    {
        public VideoListSettingsPageViewModel()
        {
            #region ダブルクリック
            var openInPlayerA = new ComboboxItem<VideodbClickSettings>(VideodbClickSettings.OpenInPlayerA, "アプリで開く(A)");
            var openInPlayerB = new ComboboxItem<VideodbClickSettings>(VideodbClickSettings.OpenInPlayerB, "アプリで開く(B)");
            var sendToAppA = new ComboboxItem<VideodbClickSettings>(VideodbClickSettings.SendToAppA, "アプリに送る(A)");
            var sendToAppB = new ComboboxItem<VideodbClickSettings>(VideodbClickSettings.SendToAppB, "アプリに送る(B)");
            var download = new ComboboxItem<VideodbClickSettings>(VideodbClickSettings.Download, "ダウンロードする");
            var none = new ComboboxItem<VideodbClickSettings>(VideodbClickSettings.NotConfigured, "何もしない");

            this.SelectableVideodbClickAction = new List<ComboboxItem<VideodbClickSettings>>() { none, openInPlayerA, openInPlayerB, sendToAppA, sendToAppB, download };
            var settingdbClickvalue = WS::SettingPage.EnumSettingsHandler.GetSetting<VideodbClickSettings>();
            this.videodbClickActionField = settingdbClickvalue switch
            {
                VideodbClickSettings.OpenInPlayerA => openInPlayerA,
                VideodbClickSettings.OpenInPlayerB => openInPlayerB,
                VideodbClickSettings.SendToAppA => sendToAppA,
                VideodbClickSettings.SendToAppB => sendToAppB,
                VideodbClickSettings.Download => download,
                _ => none,
            };

            this.IsRestoreingColumnWidthDisabled = new ReactiveProperty<bool>(WS::SettingPage.SettingHandler.GetBoolSetting(SettingsEnum.NoRestoreClumnWIdth));
            this.IsRestoreingColumnWidthDisabled.Subscribe(value =>
            {
                this.SaveSetting(value, SettingsEnum.NoRestoreClumnWIdth);
            });

            this.IsDownloadSucceededHistoryDisabled = new ReactiveProperty<bool>(WS::SettingPage.SettingHandler.GetBoolSetting(SettingsEnum.DisableDLSucceededHistory));
            this.IsDownloadSucceededHistoryDisabled.Subscribe(value => WS::SettingPage.SettingHandler.SaveSetting(value, SettingsEnum.DisableDLSucceededHistory)).AddTo(this.disposables);
            this.IsDownloadFailedHistoryDisabled = new ReactiveProperty<bool>(WS::SettingPage.SettingHandler.GetBoolSetting(SettingsEnum.DisableDLFailedHistory));
            this.IsDownloadFailedHistoryDisabled.Subscribe(value => WS::SettingPage.SettingHandler.SaveSetting(value, SettingsEnum.DisableDLFailedHistory)).AddTo(this.disposables);

            #endregion
        }

        #region フィールド
        private ComboboxItem<VideodbClickSettings> videodbClickActionField;
        #endregion

        /// <summary>
        /// 選択可能なダブルクリックアクション
        /// </summary>
        public List<ComboboxItem<VideodbClickSettings>> SelectableVideodbClickAction { get; init; }

        /// <summary>
        /// ダブルクリックアクション
        /// </summary>
        public ComboboxItem<VideodbClickSettings> VideodbClickAction { get => this.videodbClickActionField; set => this.SaveEnumSetting(ref this.videodbClickActionField, value); }

        /// <summary>
        /// 幅を継承しない
        /// </summary>
        public ReactiveProperty<bool> IsRestoreingColumnWidthDisabled { get; init; }

        /// <summary>
        /// DL成功履歴を無効にする
        /// </summary>
        public ReactiveProperty<bool> IsDownloadSucceededHistoryDisabled { get; init; }

        /// <summary>
        /// DL失敗履歴を無効にする
        /// </summary>
        public ReactiveProperty<bool> IsDownloadFailedHistoryDisabled { get; init; } 

    }

    class VideoListSettingsPageViewModelD
    {
        public VideoListSettingsPageViewModelD()
        {
            #region ダブルクリック
            var openInPlayerA = new ComboboxItem<VideodbClickSettings>(VideodbClickSettings.OpenInPlayerA, "アプリで開く(A)");
            var openInPlayerB = new ComboboxItem<VideodbClickSettings>(VideodbClickSettings.OpenInPlayerB, "アプリで開く(B)");
            var sendToAppA = new ComboboxItem<VideodbClickSettings>(VideodbClickSettings.SendToAppA, "アプリに送る(A)");
            var sendToAppB = new ComboboxItem<VideodbClickSettings>(VideodbClickSettings.SendToAppB, "アプリに送る(B)");
            var download = new ComboboxItem<VideodbClickSettings>(VideodbClickSettings.Download, "ダウンロードする");
            var none = new ComboboxItem<VideodbClickSettings>(VideodbClickSettings.NotConfigured, "何もしない");

            this.SelectableVideodbClickAction = new List<ComboboxItem<VideodbClickSettings>>() { none, openInPlayerA, openInPlayerB, sendToAppA, sendToAppB, download };
            this.VideodbClickAction = none;
            #endregion
        }

        public List<ComboboxItem<VideodbClickSettings>> SelectableVideodbClickAction { get; init; }

        public ComboboxItem<VideodbClickSettings> VideodbClickAction { get; set; }

        public ReactiveProperty<bool> IsRestoreingColumnWidthDisabled { get; init; } = new(false);

        public ReactiveProperty<bool> IsDownloadSucceededHistoryDisabled { get; init; } = new(true);

        public ReactiveProperty<bool> IsDownloadFailedHistoryDisabled { get; init; } = new(true);

    }
}
