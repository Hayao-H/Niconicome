using System;
using System.Windows;
using Niconicome.Models.Local.Settings;
using Reactive.Bindings;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Setting.Pages
{
    class DebugSettingsPageViewModel : SettingaBase
    {

        public DebugSettingsPageViewModel()
        {
            this.IsDebugMode = new ReactiveProperty<bool>();
            this.IsDebugMode.Subscribe(value =>
            {
                WS::SettingPage.State.IsDebugMode = value;
                string message = value ? "有効" : "無効";
                WS::SettingPage.SnackbarMessageQueue.Enqueue($"デバッグモード: {message}");
            });

            this.IsDevMode = WS::SettingPage.SettingsContainer.GetReactiveBoolSetting(SettingsEnum.IsDevMode);

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
        public ReactiveProperty<bool> IsDebugMode { get; init; }

        /// <summary>
        /// 開発者モード
        /// </summary>
        public ReactiveProperty<bool> IsDevMode { get; init; }

        /// <summary>
        /// ログファイルパス
        /// </summary>
        public string LogFilePath { get; init; }

        #endregion
    }

    public class DebugSettingsPageViewModelD
    {
        public ReactiveProperty<bool> IsDebugMode { get; init; } = new(true);

        public string LogFilePath { get; init; } = @"path\to\log";

        public ReactiveProperty<bool> IsDevMode { get; init; } = new(true);
    }
}
