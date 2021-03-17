using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Local;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Setting.Pages
{
    class ExternalSoftwareSettingsViewModel:SettingaBase
    {

        public ExternalSoftwareSettingsViewModel()
        {
            this.playerAPathField = WS::SettingPage.SettingHandler.GetStringSetting(Settings.PlayerAPath)??string.Empty;
            this.playerBPathField = WS::SettingPage.SettingHandler.GetStringSetting(Settings.PlayerBPath)??string.Empty;
            this.appIdPathField = WS::SettingPage.SettingHandler.GetStringSetting(Settings.AppIdPath)??string.Empty;
            this.appUrlPathField = WS::SettingPage.SettingHandler.GetStringSetting(Settings.AppUrlPath)??string.Empty;
            this.appIdParamField = WS::SettingPage.SettingHandler.GetStringSetting(Settings.AppIdParam)??string.Empty;
            this.appUrlParamField = WS::SettingPage.SettingHandler.GetStringSetting(Settings.AppUrlParam)??string.Empty;
            this.ffmpegPathField = WS::SettingPage.SettingHandler.GetStringSetting(Settings.FfmpegPath) ?? string.Empty;
            this.useShellWhenLaunchingFFmpegFIeld = WS::SettingPage.SettingHandler.GetBoolSetting(Settings.FFmpegShell);
        }

        private string playerAPathField;

        private string playerBPathField;

        private string appIdPathField;

        private string appUrlPathField;

        private string appIdParamField;

        private string appUrlParamField;

        private string ffmpegPathField;

        private bool useShellWhenLaunchingFFmpegFIeld;

        /// <summary>
        /// プレイヤーAのパス
        /// </summary>
        public string PlayerAPath { get => this.playerAPathField; set => this.Savesetting(ref this.playerAPathField, value, Settings.PlayerAPath); }

        /// <summary>
        /// プレイヤーBのパス
        /// </summary>
        public string PlayerBPath { get => this.playerBPathField; set => this.Savesetting(ref this.playerBPathField, value, Settings.PlayerBPath); }

        /// <summary>
        /// Id
        /// </summary>
        public string AppIdPath { get => this.appIdPathField; set => this.Savesetting(ref this.appIdPathField, value, Settings.AppIdPath); }

        /// <summary>
        /// Url
        /// </summary>
        public string AppUrlPath { get => this.appUrlPathField; set => this.Savesetting(ref this.appUrlPathField, value, Settings.AppUrlPath); }

        /// <summary>
        /// Id
        /// </summary>
        public string AppIdParam { get => this.appIdParamField; set => this.Savesetting(ref this.appIdParamField, value, Settings.AppIdParam); }

        /// <summary>
        /// Url
        /// </summary>
        public string AppUrlParam { get => this.appUrlParamField; set => this.Savesetting(ref this.appUrlParamField, value, Settings.AppUrlParam); }

        /// <summary>
        /// ffmpegのパス
        /// </summary>
        public string FfmpegPath { get => this.ffmpegPathField; set => this.Savesetting(ref this.ffmpegPathField, value, Settings.FfmpegPath); }

        /// <summary>
        /// シェルを利用する
        /// </summary>
        public bool UseShellWhenLaunchingFFmpeg { get => this.useShellWhenLaunchingFFmpegFIeld; set => this.Savesetting(ref this.useShellWhenLaunchingFFmpegFIeld, value,Settings.FFmpegShell); }
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
        /// Id
        /// </summary>
        public string AppIdPath { get; set; } = "設定値";

        /// <summary>
        /// Url
        /// </summary>
        public string AppUrlPath { get; set; } = "設定値";

        /// <summary>
        /// Id
        /// </summary>
        public string AppIdParam { get; set; } = "設定値";

        /// <summary>
        /// Url
        /// </summary>
        public string AppUrlParam { get; set; } = "設定値";

        /// <summary>
        /// ffmpegのパス
        /// </summary>
        public string FfmpegPath { get; set; } = "設定値";

        public bool UseShellWhenLaunchingFFmpeg { get; set; } = true;
    }
}
