using System.Windows;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Setting.Pages
{
    class DebugSettingsPageViewModel:SettingaBase
    {

        public DebugSettingsPageViewModel()
        {
            this.isDebugModeField = WS::SettingPage.State.IsDebugMode;
            this.LogFilePath = WS::SettingPage.LocalInfo.LogFileName;
            this.CopyLogFIlePathCommand = new CommandBase<object>(_ => true, _ =>
            {
                try
                {
                    Clipboard.SetText(this.LogFilePath);
                }
                catch { return; }
                WS::SettingPage.SnackbarMessageQueue.Enqueue("コピーしました。");
            });
        }


        private bool isDebugModeField;

        public CommandBase<object> CopyLogFIlePathCommand { get; init; }

        /// <summary>
        /// デバッグフラグ
        /// </summary>
        public bool IsDebugMode
        {
            get => this.isDebugModeField;
            set  {
                WS::SettingPage.State.IsDebugMode = value;
                string message = value ? "有効" : "無効";
                WS::SettingPage.SnackbarMessageQueue.Enqueue($"デバッグモード: {message}");
                this.SetProperty(ref this.isDebugModeField, value);
            }
        }

        /// <summary>
        /// ログファイルパス
        /// </summary>
        public string LogFilePath { get; init; }
    }
}
