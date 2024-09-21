using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Utils.Reactive;
using Niconicome.ViewModels.Setting.Utils;
using WS = Niconicome.Workspaces.SettingPageV2;
using Const = Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.Settings;

namespace Niconicome.ViewModels.Setting.V2.Page
{
    public class ExternalSoftwareViewModel
    {
        public ExternalSoftwareViewModel()
        {
            this.UseShellWhenLaunchingFFmpeg = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.UseShellWhenLaunchingFFmpeg, false), false).AddTo(this.Bindables);

            this.ReAllocateCommands = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.ReAllocateIfVideoisNotSaved, false), false).AddTo(this.Bindables);

            this.PlayerAPath = new BindableSettingInfo<string>(WS.SettingsContainer.GetSetting(SettingNames.PlayerAPath, string.Empty), string.Empty).AddTo(this.Bindables);

            this.PlayerBPath = new BindableSettingInfo<string>(WS.SettingsContainer.GetSetting(SettingNames.PlayerBPath, string.Empty), string.Empty).AddTo(this.Bindables);

            this.AppAPath = new BindableSettingInfo<string>(WS.SettingsContainer.GetSetting(SettingNames.AppIdPath, string.Empty), string.Empty).AddTo(this.Bindables);

            this.AppBPath = new BindableSettingInfo<string>(WS.SettingsContainer.GetSetting(SettingNames.AppUrlPath, string.Empty), string.Empty).AddTo(this.Bindables);

            this.AppAParam = new BindableSettingInfo<string>(WS.SettingsContainer.GetSetting(SettingNames.AppIdParam, string.Empty), string.Empty).AddTo(this.Bindables);

            this.AppBParam = new BindableSettingInfo<string>(WS.SettingsContainer.GetSetting(SettingNames.AppUrlParam, string.Empty), string.Empty).AddTo(this.Bindables);

            this.FfmpegPath = new BindableSettingInfo<string>(WS.SettingsContainer.GetSetting(SettingNames.FFmpegPath, Const::Format.FFmpegPath), Const::Format.FFmpegPath).AddTo(this.Bindables);

            this.FFprovePath = new BindableSettingInfo<string>(WS.SettingsContainer.GetSetting(SettingNames.FFprobePath, Const::Format.FFprobePath), Const::Format.FFprobePath).AddTo(this.Bindables);

            this.FFmpegFormat = new BindableSettingInfo<string>(WS.SettingsContainer.GetSetting(SettingNames.FFmpegFormat, Const::Format.DefaultFFmpegFormat), Const::Format.DefaultFFmpegFormat).AddTo(this.Bindables);

            this.PrioritizeMp4 = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.PrioritizeMp4, true), true).AddTo(this.Bindables);
        }

        /// <summary>
        /// 変更監視オブジェクト
        /// </summary>
        public Bindables Bindables { get; init; } = new();

        /// <summary>
        /// プレイヤーAのパス
        /// </summary>
        public IBindableSettingInfo<string> PlayerAPath { get; init; }

        /// <summary>
        /// プレイヤーBのパス
        /// </summary>
        public IBindableSettingInfo<string> PlayerBPath { get; init; }

        /// <summary>
        /// A
        /// </summary>
        public IBindableSettingInfo<string> AppAPath { get; init; }

        /// <summary>
        /// B
        /// </summary>
        public IBindableSettingInfo<string> AppBPath { get; init; }

        /// <summary>
        /// A
        /// </summary>
        public IBindableSettingInfo<string> AppAParam { get; init; }

        /// <summary>
        /// B
        /// </summary>
        public IBindableSettingInfo<string> AppBParam { get; init; }

        /// <summary>
        /// ffmpegのパス
        /// </summary>
        public IBindableSettingInfo<string> FfmpegPath { get; init; }

        /// <summary>
        /// ffmpegのフォーマット
        /// </summary>
        public IBindableSettingInfo<string> FFmpegFormat { get; init; }

        /// <summary>
        /// ffprobeのパス
        /// </summary>
        public IBindableSettingInfo<string> FFprovePath { get; init; }

        /// <summary>
        /// シェルを利用する
        /// </summary>
        public IBindableSettingInfo<bool> UseShellWhenLaunchingFFmpeg { get; init; }

        /// <summary>
        /// 保存されていない動画の場合再割り当てを行う
        /// </summary>
        public IBindableSettingInfo<bool> ReAllocateCommands { get; init; }

        /// <summary>
        /// 動画を開く場合、MP4がある場合はそちらを開く
        /// </summary>
        public IBindableSettingInfo<bool> PrioritizeMp4 { get; init; }
    }
}
