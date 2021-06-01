using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Attributes;
using Niconicome.Models.Const;
using Niconicome.Models.Local.Settings;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Setting.Pages
{
    class FileSettingsViewModel : SettingaBase
    {

        public FileSettingsViewModel()
        {
            this.FileFormat = new ReactiveProperty<string>(WS::SettingPage.SettingHandler.GetStringSetting(SettingsEnum.FileNameFormat) ?? string.Empty);
            this.DefaultFolder = new ReactiveProperty<string>(WS::SettingPage.SettingHandler.GetStringSetting(SettingsEnum.DefaultFolder) ?? FileFolder.DefaultDownloadDir);
            this.IsReplaceSBToSBEnable = new ReactiveProperty<bool>(WS::SettingPage.SettingHandler.GetBoolSetting(SettingsEnum.ReplaceSBToMB));
            this.HtmlFileExt = new ReactiveProperty<string>(WS::SettingPage.SettingHandler.GetStringSetting(SettingsEnum.HtmlExt) ?? FileFolder.DefaultHtmlFileExt).SetValidateAttribute(() => this.HtmlFileExt);
            this.JpegFileExt = new ReactiveProperty<string>(WS::SettingPage.SettingHandler.GetStringSetting(SettingsEnum.JpegExt) ?? FileFolder.DefaultJpegFileExt).SetValidateAttribute(() => this.JpegFileExt);
            this.VideoInfoSuffix = new ReactiveProperty<string>(WS::SettingPage.SettingHandler.GetStringSetting(SettingsEnum.VideoinfoSuffix) ?? Format.DefaultVideoInfoSuffix);
            this.IchibaSuffix = new ReactiveProperty<string>(WS::SettingPage.SettingHandler.GetStringSetting(SettingsEnum.IchibaInfoSuffix) ?? Format.DefaultIchibaSuffix);

            this.FileFormat.Subscribe(value => this.SaveSetting(value, SettingsEnum.FileNameFormat)).AddTo(this.disposables);
            this.DefaultFolder.Subscribe(value => this.SaveSetting(value, SettingsEnum.DefaultFolder)).AddTo(this.disposables);
            this.IsReplaceSBToSBEnable.Subscribe(value => this.SaveSetting(value, SettingsEnum.ReplaceSBToMB)).AddTo(this.disposables);
            this.HtmlFileExt.Subscribe(value =>
            {
                if (this.HtmlFileExt.HasErrors) return;
                this.SaveSetting(value, SettingsEnum.HtmlExt);
            }).AddTo(this.disposables);
            this.JpegFileExt.Subscribe(value =>
            {
                if (this.JpegFileExt.HasErrors) return;
                this.SaveSetting(value, SettingsEnum.JpegExt);
            }).AddTo(this.disposables);
            this.VideoInfoSuffix.Subscribe(value => this.SaveSetting(value, SettingsEnum.VideoinfoSuffix)).AddTo(this.disposables);
            this.IchibaSuffix.Subscribe(value => this.SaveSetting(value, SettingsEnum.IchibaInfoSuffix)).AddTo(this.disposables);
        }

        /// <summary>
        /// デフォルトのDLパス
        /// </summary>
        public ReactiveProperty<string> DefaultFolder { get; init; }

        /// <summary>
        /// ファイル名のフォーマット
        /// </summary>
        public ReactiveProperty<string> FileFormat { get; init; }

        /// <summary>
        /// 禁則文字を2バイト文字に置き換える
        /// </summary>
        public ReactiveProperty<bool> IsReplaceSBToSBEnable { get; init; }

        /// <summary>
        /// htmlファイル
        /// </summary>
        [RegularExpression(Format.FileExtRegExp, ErrorMessage = "正しい形式で指定してください。")]
        public ReactiveProperty<string> HtmlFileExt { get; init; }

        /// <summary>
        /// jpegファイル
        /// </summary>
        [RegularExpression(Format.FileExtRegExp, ErrorMessage = "正しい形式で指定してください。")]
        public ReactiveProperty<string> JpegFileExt { get; init; }

        /// <summary>
        /// 動画情報の接尾子
        /// </summary>
        public ReactiveProperty<string> VideoInfoSuffix { get; init; }

        /// <summary>
        /// 市場情報の接尾子
        /// </summary>
        public ReactiveProperty<string> IchibaSuffix { get; init; } 

    }

    class FileSettingsViewModelD : SettingaBase
    {


        public ReactiveProperty<string> DefaultFolder { get; init; } = new("download");


        public ReactiveProperty<string> FileFormat { get; init; } = new("[<id>]<title>");

        public ReactiveProperty<bool> IsReplaceSBToSBEnable { get; init; } = new(true);

        public ReactiveProperty<string> HtmlFileExt { get; init; } = new(".html");

        public ReactiveProperty<string> JpegFileExt { get; init; } = new(".jpg");

        public ReactiveProperty<string> VideoInfoSuffix { get; init; } = new(Format.DefaultVideoInfoSuffix);

        public ReactiveProperty<string> IchibaSuffix { get; init; } = new(Format.DefaultIchibaSuffix);


    }
}
