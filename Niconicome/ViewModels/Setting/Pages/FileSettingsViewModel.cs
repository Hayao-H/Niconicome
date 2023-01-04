using System.ComponentModel.DataAnnotations;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.ViewModels.Setting.Utils;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Setting.Pages
{
    class FileSettingsViewModel
    {

        public FileSettingsViewModel()
        {
            this.FileFormat = new SettingInfoViewModel<string>(WS::SettingPage.SettingsConainer.GetSetting(SettingNames.FileNameFormat, ""), "");
            this.DefaultFolder = new SettingInfoViewModel<string>(WS::SettingPage.SettingsConainer.GetSetting(SettingNames.DefaultFolder, FileFolder.DefaultDownloadDir), FileFolder.DefaultDownloadDir);
            this.IsReplaceSBToSBEnable = new SettingInfoViewModel<bool>(WS::SettingPage.SettingsConainer.GetSetting(SettingNames.ReplaceSingleByteToMultiByte, false), false);
            this.HtmlFileExt = new SettingInfoViewModel<string>(WS::SettingPage.SettingsConainer.GetSetting(SettingNames.HtmlFileExtension, FileFolder.DefaultHtmlFileExt), FileFolder.DefaultHtmlFileExt);
            this.JpegFileExt = new SettingInfoViewModel<string>(WS::SettingPage.SettingsConainer.GetSetting(SettingNames.JpegFileExtension, FileFolder.DefaultJpegFileExt), FileFolder.DefaultJpegFileExt);
            this.VideoInfoSuffix = new SettingInfoViewModel<string>(WS::SettingPage.SettingsConainer.GetSetting(SettingNames.VideoInfoSuffix, Format.DefaultVideoInfoSuffix), Format.DefaultVideoInfoSuffix);
            this.IchibaSuffix = new SettingInfoViewModel<string>(WS::SettingPage.SettingsConainer.GetSetting(SettingNames.IchibaInfoType, Format.DefaultIchibaSuffix), Format.DefaultIchibaSuffix);

            this.IsSearchingVideosExactEnable = new SettingInfoViewModel<bool>(WS::SettingPage.SettingsConainer.GetSetting(SettingNames.SearchVideosExact, false), false);
            this.ThumbnailSuffix = new SettingInfoViewModel<string>(WS::SettingPage.SettingsConainer.GetSetting(SettingNames.ThumbnailSuffix, Format.DefaultThumbnailSuffix), Format.DefaultThumbnailSuffix);
            this.OwnerCommentSuffix = new SettingInfoViewModel<string>(WS::SettingPage.SettingsConainer.GetSetting(SettingNames.OwnerCommentSuffix, Format.DefaultOwnerCommentSuffix), Format.DefaultOwnerCommentSuffix);
            this.EconomySuffix = new SettingInfoViewModel<string>(WS::SettingPage.SettingsConainer.GetSetting(SettingNames.EnonomyQualitySuffix, Format.DefaultEconomyVideoSuffix), Format.DefaultEconomyVideoSuffix);
        }

        /// <summary>
        /// デフォルトのDLパス
        /// </summary>
        public SettingInfoViewModel<string> DefaultFolder { get; init; }

        /// <summary>
        /// ファイル名のフォーマット
        /// </summary>
        public SettingInfoViewModel<string> FileFormat { get; init; }

        /// <summary>
        /// 禁則文字を2バイト文字に置き換える
        /// </summary>
        public SettingInfoViewModel<bool> IsReplaceSBToSBEnable { get; init; }

        /// <summary>
        /// htmlファイル
        /// </summary>
        [RegularExpression(Format.FileExtRegExp, ErrorMessage = "正しい形式で指定してください。")]
        public SettingInfoViewModel<string> HtmlFileExt { get; init; }

        /// <summary>
        /// jpegファイル
        /// </summary>
        [RegularExpression(Format.FileExtRegExp, ErrorMessage = "正しい形式で指定してください。")]
        public SettingInfoViewModel<string> JpegFileExt { get; init; }

        /// <summary>
        /// 動画情報の接尾辞
        /// </summary>
        public SettingInfoViewModel<string> VideoInfoSuffix { get; init; }

        /// <summary>
        /// 市場情報の接尾辞
        /// </summary>
        public SettingInfoViewModel<string> IchibaSuffix { get; init; }

        /// <summary>
        /// 動画をIDで探索する
        /// </summary>
        public SettingInfoViewModel<bool> IsSearchingVideosExactEnable { get; init; }

        /// <summary>
        /// サムネイルの接尾辞
        /// </summary>
        public SettingInfoViewModel<string> ThumbnailSuffix { get; init; }

        /// <summary>
        /// 投コメの接尾辞
        /// </summary>
        public SettingInfoViewModel<string> OwnerCommentSuffix { get; init; }


        /// <summary>
        /// エコノミー動画の接尾辞
        /// </summary>
        public SettingInfoViewModel<string> EconomySuffix { get; init; }
    }

    class FileSettingsViewModelD
    {


        public SettingInfoViewModelD<string> DefaultFolder { get; init; } = new("download");


        public SettingInfoViewModelD<string> FileFormat { get; init; } = new("[<id>]<title>");

        public SettingInfoViewModelD<bool> IsReplaceSBToSBEnable { get; init; } = new(true);

        public SettingInfoViewModelD<string> HtmlFileExt { get; init; } = new(".html");

        public SettingInfoViewModelD<string> JpegFileExt { get; init; } = new(".jpg");

        public SettingInfoViewModelD<string> VideoInfoSuffix { get; init; } = new(Format.DefaultVideoInfoSuffix);

        public SettingInfoViewModelD<string> IchibaSuffix { get; init; } = new(Format.DefaultIchibaSuffix);

        public SettingInfoViewModelD<bool> IsSearchingVideosExactEnable { get; init; } = new(true);

        public SettingInfoViewModelD<string> ThumbnailSuffix { get; init; } = new(Format.DefaultThumbnailSuffix);

        public SettingInfoViewModelD<string> OwnerCommentSuffix { get; init; } = new(Format.DefaultOwnerCommentSuffix);

        public SettingInfoViewModelD<string> EconomySuffix { get; init; } = new(Format.DefaultEconomyVideoSuffix);
    }
}
