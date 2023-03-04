using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Local.Settings;
using Niconicome.ViewModels.Setting.Utils;
using Reactive.Bindings;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Setting.Pages
{
    class ExternalSoftwareSettingsViewModel 
    {

        public ExternalSoftwareSettingsViewModel()
        {
            this.PlayerAPath = new SettingInfoViewModel<string>(WS::SettingPage.SettingsConainer.GetSetting(SettingNames.PlayerAPath, ""), "");
            this.PlayerBPath = new SettingInfoViewModel<string>(WS::SettingPage.SettingsConainer.GetSetting(SettingNames.PlayerBPath, ""), "");
            this.AppAPath = new SettingInfoViewModel<string>(WS::SettingPage.SettingsConainer.GetSetting(SettingNames.AppIdPath, ""), "");
            this.AppBPath = new SettingInfoViewModel<string>(WS::SettingPage.SettingsConainer.GetSetting(SettingNames.AppUrlPath, ""), "");
            this.AppAParam = new SettingInfoViewModel<string>(WS::SettingPage.SettingsConainer.GetSetting(SettingNames.AppIdParam, ""), "");
            this.AppBParam = new SettingInfoViewModel<string>(WS::SettingPage.SettingsConainer.GetSetting(SettingNames.AppUrlParam, ""), "");
            this.FfmpegPath = new SettingInfoViewModel<string>(WS::SettingPage.SettingsConainer.GetSetting(SettingNames.FFmpegPath, Format.FFmpegPath), Format.FFmpegPath);
            this.UseShellWhenLaunchingFFmpeg = new SettingInfoViewModel<bool>(WS::SettingPage.SettingsConainer.GetSetting(SettingNames.UseShellWhenLaunchingFFmpeg, false), false);
            this.ReAllocateCommands = new SettingInfoViewModel<bool>(WS::SettingPage.SettingsConainer.GetSetting(SettingNames.ReAllocateIfVideoisNotSaved, false), false);
            this.FFmpegFormat = new SettingInfoViewModel<string>(WS::SettingPage.SettingsConainer.GetSetting(SettingNames.FFmpegFormat, Format.DefaultFFmpegFormat), Format.DefaultFFmpegFormat);
        }

        /// <summary>
        /// プレイヤーAのパス
        /// </summary>
        public SettingInfoViewModel<string> PlayerAPath { get; init; }

        /// <summary>
        /// プレイヤーBのパス
        /// </summary>
        public SettingInfoViewModel<string> PlayerBPath { get; init; }

        /// <summary>
        /// A
        /// </summary>
        public SettingInfoViewModel<string> AppAPath { get; init; }

        /// <summary>
        /// B
        /// </summary>
        public SettingInfoViewModel<string> AppBPath { get; init; }

        /// <summary>
        /// A
        /// </summary>
        public SettingInfoViewModel<string> AppAParam { get; init; }

        /// <summary>
        /// B
        /// </summary>
        public SettingInfoViewModel<string> AppBParam { get; init; }

        /// <summary>
        /// ffmpegのパス
        /// </summary>
        public SettingInfoViewModel<string> FfmpegPath { get; init; }

        /// <summary>
        /// ffmpegのフォーマット
        /// </summary>
        public SettingInfoViewModel<string> FFmpegFormat { get; init; }

        /// <summary>
        /// シェルを利用する
        /// </summary>
        public SettingInfoViewModel<bool> UseShellWhenLaunchingFFmpeg { get; init; }

        /// <summary>
        /// 保存されていない動画の場合再割り当てを行う
        /// </summary>
        public SettingInfoViewModel<bool> ReAllocateCommands { get; init; }

    }

    class ExternalSoftwareSettingsViewModelD
    {
        public SettingInfoViewModelD<string> PlayerAPath { get; init; } = new("設定値");
        public SettingInfoViewModelD<string> PlayerBPath { get; init; } = new("設定値");
        public SettingInfoViewModelD<string> AppAPath { get; init; } = new("設定値");
        public SettingInfoViewModelD<string> AppBPath { get; init; } = new("設定値");
        public SettingInfoViewModelD<string> AppAParam { get; init; } = new("設定値");
        public SettingInfoViewModelD<string> AppBParam { get; init; } = new("設定値");
        public SettingInfoViewModelD<string> FfmpegPath { get; init; } = new("設定値");
        public SettingInfoViewModelD<string> FFmpegFormat { get; init; } = new("設定値");

        public SettingInfoViewModelD<bool> UseShellWhenLaunchingFFmpeg { get; set; } = new(true);

        public SettingInfoViewModelD<bool> ReAllocateCommands { get; set; } = new(true);
    }
}
