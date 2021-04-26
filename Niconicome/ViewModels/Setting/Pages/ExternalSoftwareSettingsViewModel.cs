using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Local;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Setting.Pages
{
    class ExternalSoftwareSettingsViewModel : SettingaBase
    {

        public ExternalSoftwareSettingsViewModel()
        {
            this.playerAPathField = WS::SettingPage.SettingHandler.GetStringSetting(Settings.PlayerAPath) ?? string.Empty;
            this.playerBPathField = WS::SettingPage.SettingHandler.GetStringSetting(Settings.PlayerBPath) ?? string.Empty;
            this.appAPathField = WS::SettingPage.SettingHandler.GetStringSetting(Settings.AppAPath) ?? string.Empty;
            this.appBPathField = WS::SettingPage.SettingHandler.GetStringSetting(Settings.AppBPath) ?? string.Empty;
            this.appAParamField = WS::SettingPage.SettingHandler.GetStringSetting(Settings.AppAParam) ?? string.Empty;
            this.appBParamField = WS::SettingPage.SettingHandler.GetStringSetting(Settings.AppBParam) ?? string.Empty;
            this.ffmpegPathField = WS::SettingPage.SettingHandler.GetStringSetting(Settings.FfmpegPath) ?? string.Empty;
            this.useShellWhenLaunchingFFmpegFIeld = WS::SettingPage.SettingHandler.GetBoolSetting(Settings.FFmpegShell);
            this.reAllocateCommandsField = WS::SettingPage.SettingHandler.GetBoolSetting(Settings.ReAllocateCommands);
        }

        private string playerAPathField;

        private string playerBPathField;

        private string appAPathField;

        private string appBPathField;

        private string appAParamField;

        private string appBParamField;

        private string ffmpegPathField;

        private bool useShellWhenLaunchingFFmpegFIeld;

        private bool reAllocateCommandsField;

        /// <summary>
        /// プレイヤーAのパス
        /// </summary>
        public string PlayerAPath { get => this.playerAPathField; set => this.Savesetting(ref this.playerAPathField, value, Settings.PlayerAPath); }

        /// <summary>
        /// プレイヤーBのパス
        /// </summary>
        public string PlayerBPath { get => this.playerBPathField; set => this.Savesetting(ref this.playerBPathField, value, Settings.PlayerBPath); }

        /// <summary>
        /// A
        /// </summary>
        public string AppAPath { get => this.appAPathField; set => this.Savesetting(ref this.appAPathField, value, Settings.AppAPath); }

        /// <summary>
        /// B
        /// </summary>
        public string AppBPath { get => this.appBPathField; set => this.Savesetting(ref this.appBPathField, value, Settings.AppBPath); }

        /// <summary>
        /// A
        /// </summary>
        public string AppAParam { get => this.appAParamField; set => this.Savesetting(ref this.appAParamField, value, Settings.AppAParam); }

        /// <summary>
        /// B
        /// </summary>
        public string AppBParam { get => this.appBParamField; set => this.Savesetting(ref this.appBParamField, value, Settings.AppBParam); }

        /// <summary>
        /// ffmpegのパス
        /// </summary>
        public string FfmpegPath { get => this.ffmpegPathField; set => this.Savesetting(ref this.ffmpegPathField, value, Settings.FfmpegPath); }

        /// <summary>
        /// シェルを利用する
        /// </summary>
        public bool UseShellWhenLaunchingFFmpeg { get => this.useShellWhenLaunchingFFmpegFIeld; set => this.Savesetting(ref this.useShellWhenLaunchingFFmpegFIeld, value, Settings.FFmpegShell); }

        /// <summary>
        /// 保存されていない動画の場合再割り当てを行う
        /// </summary>
        public bool ReAllocateCommands { get => this.reAllocateCommandsField; set => this.Savesetting(ref this.reAllocateCommandsField, value, Settings.ReAllocateCommands); }

    }

    class ExternalSoftwareSettingsViewModelD
    {
        /// <summary>
        /// プレイヤーAのパス
        /// </summary>
        public string PlayerAPath { get; set; } = "設定値";

        /// <summary>
        /// プレイヤーBのパス
        /// </summary>
        public string PlayerBPath { get; set; } = "設定値";

        /// <summary>
        /// A
        /// </summary>
        public string AppAPath { get; set; } = "設定値";

        /// <summary>
        /// B
        /// </summary>
        public string AppBPath { get; set; } = "設定値";

        /// <summary>
        /// A
        /// </summary>
        public string AppAParam { get; set; } = "設定値";

        /// <summary>
        /// B
        /// </summary>
        public string AppBParam { get; set; } = "設定値";

        /// <summary>
        /// ffmpegのパス
        /// </summary>
        public string FfmpegPath { get; set; } = "設定値";

        public bool UseShellWhenLaunchingFFmpeg { get; set; } = true;

        public bool ReAllocateCommands { get; set; } = true;
    }
}
