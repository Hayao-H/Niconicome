using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Local.Settings;
using Reactive.Bindings;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Setting.Pages
{
    class ExternalSoftwareSettingsViewModel : SettingaBase
    {

        public ExternalSoftwareSettingsViewModel()
        {
            this.PlayerAPath = WS::SettingPage.SettingsContainer.GetReactiveStringSetting(SettingsEnum.PlayerAPath);
            this.PlayerBPath = WS::SettingPage.SettingsContainer.GetReactiveStringSetting(SettingsEnum.PlayerBPath);
            this.AppAPath = WS::SettingPage.SettingsContainer.GetReactiveStringSetting(SettingsEnum.AppAPath);
            this.AppBPath = WS::SettingPage.SettingsContainer.GetReactiveStringSetting(SettingsEnum.AppBPath);
            this.AppAParam = WS::SettingPage.SettingsContainer.GetReactiveStringSetting(SettingsEnum.AppAParam);
            this.AppBParam = WS::SettingPage.SettingsContainer.GetReactiveStringSetting(SettingsEnum.AppBParam);
            this.FfmpegPath = WS::SettingPage.SettingsContainer.GetReactiveStringSetting(SettingsEnum.FfmpegPath);
            this.useShellWhenLaunchingFFmpegFIeld = WS::SettingPage.SettingHandler.GetBoolSetting(SettingsEnum.FFmpegShell);
            this.reAllocateCommandsField = WS::SettingPage.SettingHandler.GetBoolSetting(SettingsEnum.ReAllocateCommands);
            this.FFmpegFormat = WS::SettingPage.SettingsContainer.GetReactiveStringSetting(SettingsEnum.FFmpegFormat);
        }

        private bool useShellWhenLaunchingFFmpegFIeld;

        private bool reAllocateCommandsField;

        /// <summary>
        /// プレイヤーAのパス
        /// </summary>
        public ReactiveProperty<string> PlayerAPath { get; init; }

        /// <summary>
        /// プレイヤーBのパス
        /// </summary>
        public ReactiveProperty<string> PlayerBPath { get; init; }

        /// <summary>
        /// A
        /// </summary>
        public ReactiveProperty<string> AppAPath { get; init; }

        /// <summary>
        /// B
        /// </summary>
        public ReactiveProperty<string> AppBPath { get; init; }

        /// <summary>
        /// A
        /// </summary>
        public ReactiveProperty<string> AppAParam { get; init; }

        /// <summary>
        /// B
        /// </summary>
        public ReactiveProperty<string> AppBParam { get; init; }

        /// <summary>
        /// ffmpegのパス
        /// </summary>
        public ReactiveProperty<string> FfmpegPath { get; init; }

        /// <summary>
        /// ffmpegのフォーマット
        /// </summary>
        public ReactiveProperty<string> FFmpegFormat { get; init; }

        /// <summary>
        /// シェルを利用する
        /// </summary>
        public bool UseShellWhenLaunchingFFmpeg { get => this.useShellWhenLaunchingFFmpegFIeld; set => this.Savesetting(ref this.useShellWhenLaunchingFFmpegFIeld, value, SettingsEnum.FFmpegShell); }

        /// <summary>
        /// 保存されていない動画の場合再割り当てを行う
        /// </summary>
        public bool ReAllocateCommands { get => this.reAllocateCommandsField; set => this.Savesetting(ref this.reAllocateCommandsField, value, SettingsEnum.ReAllocateCommands); }

    }

    class ExternalSoftwareSettingsViewModelD
    {
        public ReactiveProperty<string> PlayerAPath { get; init; } = new("設定値");
        public ReactiveProperty<string> PlayerBPath { get; init; } = new("設定値");
        public ReactiveProperty<string> AppAPath { get; init; } = new("設定値");
        public ReactiveProperty<string> AppBPath { get; init; } = new("設定値");
        public ReactiveProperty<string> AppAParam { get; init; } = new("設定値");
        public ReactiveProperty<string> AppBParam { get; init; } = new("設定値");
        public ReactiveProperty<string> FfmpegPath { get; init; } = new("設定値");
        public ReactiveProperty<string> FFmpegFormat { get; init; } = new("設定値");

        public bool UseShellWhenLaunchingFFmpeg { get; set; } = true;

        public bool ReAllocateCommands { get; set; } = true;
    }
}
