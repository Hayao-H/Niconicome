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
            var openInPlayerA = new SelectBoxItem<VideoClickSettings>("アプリで開く(A)", VideoClickSettings.OpenInPlayerA);
            var openInPlayerB = new SelectBoxItem<VideoClickSettings>("アプリで開く(B)", VideoClickSettings.OpenInPlayerB);
            var sendToAppA = new SelectBoxItem<VideoClickSettings>("アプリに送る(A)", VideoClickSettings.SendToAppA);
            var sendToAppB = new SelectBoxItem<VideoClickSettings>("アプリに送る(B)", VideoClickSettings.SendToAppB);
            var download = new SelectBoxItem<VideoClickSettings>("ダウンロードする", VideoClickSettings.Download);
            var none = new SelectBoxItem<VideoClickSettings>("何もしない", VideoClickSettings.NotConfigured);

            this.SelectableVideodbClickAction = new() { openInPlayerA, openInPlayerB, sendToAppA, sendToAppB, download, none };
            this.VideodbClickAction = new BindableSettingInfo<VideoClickSettings>(WS.SettingsContainer.GetSetting(SettingNames.VideoListItemdbClickAction, VideoClickSettings.NotConfigured), VideoClickSettings.NotConfigured).AddTo(this.Bindables);

            this.SelectableVideoMiddleClickAction = new() { openInPlayerA, openInPlayerB, sendToAppA, sendToAppB, download, none };
            this.VideoMiddleClickAction = new BindableSettingInfo<VideoClickSettings>(WS.SettingsContainer.GetSetting(SettingNames.VideoListItemMiddleClickAction, VideoClickSettings.NotConfigured), VideoClickSettings.NotConfigured).AddTo(this.Bindables);

            this.IsRestoreingColumnWidthDisabled = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.IsRestoringColumnWidthDisabled, false), false).AddTo(this.Bindables);

            this.IsPlaybackHistoryDisabled = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.DisablePlaybackHistory, false), false).AddTo(this.Bindables);

            this.IsDownloadSucceededHistoryDisabled = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.DisableDownloadSucceededHistory, false), false).AddTo(this.Bindables);

            this.IsDownloadFailedHistoryDisabled = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.DisableDownloadFailedHistory, false), false).AddTo(this.Bindables);

            this.IsAutoDownloadEnable = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.AutomaticalyStartDownloadOnVideoAdded, false), false).AddTo(this.Bindables);

            this.IsVideoClickSelectEnable = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.IsVideoClickSelectEnable, false), false).AddTo(this.Bindables);

            this.IsDeletionConfirmDisabled = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.IsDeletionConfirmDisabled, false), false).AddTo(this.Bindables);

            var noneD = new SelectBoxItem<VideoDelKeySettings>("何もしない", VideoDelKeySettings.NotConfigured);
            var deleteVideo = new SelectBoxItem<VideoDelKeySettings>("選択された動画をリストから削除", VideoDelKeySettings.DeleteVideo);
            var deleteFile = new SelectBoxItem<VideoDelKeySettings>("選択された動画の実体を削除", VideoDelKeySettings.DeleteFile);

            this.SelectableVideoDelKeyAction = new List<SelectBoxItem<VideoDelKeySettings>>() { noneD, deleteVideo, deleteFile };
            this.VideoDelKeyAction = new BindableSettingInfo<VideoDelKeySettings>(WS.SettingsContainer.GetSetting(SettingNames.VideoListItemDelKeyAction, VideoDelKeySettings.NotConfigured), VideoDelKeySettings.NotConfigured);
        }

        public Bindables Bindables { get; init; } = new();

        /// <summary>
        /// 選択可能なダブルクリックアクション
        /// </summary>
        public List<SelectBoxItem<VideoClickSettings>> SelectableVideodbClickAction { get; init; }

        /// <summary>
        /// 選択可能な中クリックアクション
        /// </summary>
        public List<SelectBoxItem<VideoClickSettings>> SelectableVideoMiddleClickAction { get; init; }

        /// <summary>
        /// 選択可能なDelキーアクション
        /// </summary>
        public List<SelectBoxItem<VideoDelKeySettings>> SelectableVideoDelKeyAction { get; init; }

        /// <summary>
        /// ダブルクリックアクション
        /// </summary>
        public IBindableSettingInfo<VideoClickSettings> VideodbClickAction { get; init; }

        /// <summary>
        /// 中クリックアクション
        /// </summary>
        public IBindableSettingInfo<VideoClickSettings> VideoMiddleClickAction { get; init; }

        /// <summary>
        /// Delキーアクション
        /// </summary>
        public IBindableSettingInfo<VideoDelKeySettings> VideoDelKeyAction { get; init; }

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

        /// <summary>
        /// 追加時の自動ダウンロード
        /// </summary>
        public IBindableSettingInfo<bool> IsAutoDownloadEnable { get; init; }


        /// <summary>
        /// 動画クリックで選択
        /// </summary>
        public IBindableSettingInfo<bool> IsVideoClickSelectEnable { get; set; }

        /// <summary>
        /// 削除時の確認を無効にする
        /// </summary>
        public IBindableSettingInfo<bool> IsDeletionConfirmDisabled { get; set; }
    }
}
