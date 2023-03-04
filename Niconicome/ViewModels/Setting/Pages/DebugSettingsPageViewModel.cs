using System;
using System.Windows;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Local.Settings;
using Niconicome.ViewModels.Setting.Utils;
using Reactive.Bindings;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Setting.Pages
{
    class DebugSettingsPageViewModel 
    {

        public DebugSettingsPageViewModel()
        {
            this.IsDebugMode = new SettingInfoViewModel<bool>(WS::SettingPage.SettingsConainer.GetSetting(SettingNames.IsDebugMode, false), false);
            this.IsDebugMode.RegisterPropChangeHandler(value =>
            {
                WS::SettingPage.State.IsDebugMode = value;
                string message = value ? "有効" : "無効";
                WS::SettingPage.SnackbarMessageQueue.Enqueue($"デバッグモード: {message}");
            });

            this.IsDevMode = new SettingInfoViewModel<bool>(WS::SettingPage.SettingsConainer.GetSetting(SettingNames.IsDeveloppersMode, false), false);

            this.LogFilePath = WS::SettingPage.LocalInfo.LogFileName;
            this.CopyLogFIlePathCommand = new ReactiveCommand()
                .WithSubscribe(() =>
            {
                try
                {
                    Clipboard.SetText(this.LogFilePath);
                }
                catch { return; }
                WS::SettingPage.SnackbarMessageQueue.Enqueue("コピーしました。");
            });
        }


        public ReactiveCommand CopyLogFIlePathCommand { get; init; }

        #region Props

        /// <summary>
        /// デバッグフラグ
        /// </summary>
        public SettingInfoViewModel<bool> IsDebugMode { get; init; }

        /// <summary>
        /// 開発者モード
        /// </summary>
        public SettingInfoViewModel<bool> IsDevMode { get; init; }

        /// <summary>
        /// ログファイルパス
        /// </summary>
        public string LogFilePath { get; init; }

        #endregion
    }

    public class DebugSettingsPageViewModelD
    {
        public SettingInfoViewModelD<bool> IsDebugMode { get; init; } = new(true);

        public string LogFilePath { get; init; } = @"path\to\log";

        public SettingInfoViewModelD<bool> IsDevMode { get; init; } = new(true);
    }
}
