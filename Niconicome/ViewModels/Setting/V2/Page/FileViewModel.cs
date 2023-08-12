using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Utils.Reactive;
using Niconicome.ViewModels.Setting.Utils;
using WS = Niconicome.Workspaces.SettingPageV2;
using Const = Niconicome.Models.Const;

namespace Niconicome.ViewModels.Setting.V2.Page
{
    public class FileViewModel
    {
        public FileViewModel()
        {
            this.DefaultFolder = new BindableSettingInfo<string>(WS.SettingsContainer.GetSetting(SettingNames.DefaultFolder, Const::FileFolder.DefaultDownloadDir), Const::FileFolder.DefaultDownloadDir).AddTo(this.Bindables);
            this.FileFormat = new BindableSettingInfo<string>(WS.SettingsContainer.GetSetting(SettingNames.FileNameFormat, string.Empty), string.Empty).AddTo(this.Bindables);
            this.IsReplaceSBToSBEnable = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.ReplaceSingleByteToMultiByte, false), false).AddTo(this.Bindables);
            this.HtmlFileExt = new BindableSettingInfo<string>(WS.SettingsContainer.GetSetting(SettingNames.HtmlFileExtension, Const::FileFolder.DefaultHtmlFileExt), Const::FileFolder.DefaultHtmlFileExt).AddTo(this.Bindables);
            this.JpegFileExt = new BindableSettingInfo<string>(WS.SettingsContainer.GetSetting(SettingNames.JpegFileExtension, Const::FileFolder.DefaultJpegFileExt), Const::FileFolder.DefaultJpegFileExt).AddTo(this.Bindables);
            this.VideoInfoSuffix = new BindableSettingInfo<string>(WS.SettingsContainer.GetSetting(SettingNames.VideoInfoSuffix, Const::Format.DefaultFileNameFormat), Const::Format.DefaultFileNameFormat).AddTo(this.Bindables);
            this.ThumbnailSuffix = new BindableSettingInfo<string>(WS.SettingsContainer.GetSetting(SettingNames.ThumbnailSuffix, Const::Format.DefaultThumbnailSuffix), Const::Format.DefaultThumbnailSuffix).AddTo(this.Bindables);
            this.OwnerCommentSuffix = new BindableSettingInfo<string>(WS.SettingsContainer.GetSetting(SettingNames.OwnerCommentSuffix, Const::Format.DefaultOwnerCommentSuffix), Const::Format.DefaultOwnerCommentSuffix).AddTo(this.Bindables);
            this.IchibaSuffix = new BindableSettingInfo<string>(WS.SettingsContainer.GetSetting(SettingNames.IchibaSuffix, Const::Format.DefaultIchibaSuffix), Const::Format.DefaultIchibaSuffix).AddTo(this.Bindables);
            this.EconomySuffix = new BindableSettingInfo<string>(WS.SettingsContainer.GetSetting(SettingNames.DeleteExistingEconomyFile, Const::Format.DefaultEconomyVideoSuffix), Const::Format.DefaultEconomyVideoSuffix).AddTo(this.Bindables);
            this.IsSearchingVideosExactEnable = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.SearchVideosExact, false), false).AddTo(this.Bindables);
        }

        public Bindables Bindables { get; init; } = new();

        /// <summary>
        /// デフォルトのDLパス
        /// </summary>
        public IBindableSettingInfo<string> DefaultFolder { get; init; }

        /// <summary>
        /// ファイル名のフォーマット
        /// </summary>
        public IBindableSettingInfo<string> FileFormat { get; init; }

        /// <summary>
        /// 禁則文字を2バイト文字に置き換える
        /// </summary>
        public IBindableSettingInfo<bool> IsReplaceSBToSBEnable { get; init; }

        /// <summary>
        /// htmlファイル
        /// </summary>
        public IBindableSettingInfo<string> HtmlFileExt { get; init; }

        /// <summary>
        /// jpegファイル
        /// </summary>
        public IBindableSettingInfo<string> JpegFileExt { get; init; }

        /// <summary>
        /// 動画情報の接尾辞
        /// </summary>
        public IBindableSettingInfo<string> VideoInfoSuffix { get; init; }

        /// <summary>
        /// 市場情報の接尾辞
        /// </summary>
        public IBindableSettingInfo<string> IchibaSuffix { get; init; }

        /// <summary>
        /// 動画をIDで探索する
        /// </summary>
        public IBindableSettingInfo<bool> IsSearchingVideosExactEnable { get; init; }

        /// <summary>
        /// サムネイルの接尾辞
        /// </summary>
        public IBindableSettingInfo<string> ThumbnailSuffix { get; init; }

        /// <summary>
        /// 投コメの接尾辞
        /// </summary>
        public IBindableSettingInfo<string> OwnerCommentSuffix { get; init; }


        /// <summary>
        /// エコノミー動画の接尾辞
        /// </summary>
        public IBindableSettingInfo<string> EconomySuffix { get; init; }
    }
}
