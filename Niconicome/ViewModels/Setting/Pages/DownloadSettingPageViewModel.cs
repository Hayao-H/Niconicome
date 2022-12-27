using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS = Niconicome.Workspaces;
using System.ComponentModel;
using Niconicome.ViewModels.Mainpage.Utils;
using Niconicome.Models.Local.Settings;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Niconicome.Models.Local.Settings.EnumSettingsValue;
using System.ComponentModel.DataAnnotations;
using Niconicome.Models.Const;
using Niconicome.ViewModels.Setting.Utils;
using Niconicome.Models.Domain.Local.Settings;

namespace Niconicome.ViewModels.Setting.Pages
{
    class DownloadSettingPageViewModel
    {
        public DownloadSettingPageViewModel()
        {
            this.CommentOffset = new SettingInfoViewModel<int>(WS::SettingPage.SettingsConainer.GetSetting<int>(SettingNames.CommentOffset, 40), 40);
            if (this.CommentOffset.Value < 0)
            {
                this.CommentOffset.Value = 40;
            }

            this.IsAutoSwitchOffsetEnable = new SettingInfoViewModel<bool>(WS::SettingPage.SettingsConainer.GetSetting<bool>(SettingNames.IsAutoSwitchOffsetEnable, false), false);

            this.IsDownloadVideoInfoInJsonEnable = new SettingInfoViewModel<bool>(WS::SettingPage.SettingsConainer.GetSetting<bool>(SettingNames.IsDownloadingVideoInfoInJsonEnable, false), false);

            this.IsDownloadFromQueueEnable = new SettingInfoViewModel<bool>(WS::SettingPage.SettingsConainer.GetSetting<bool>(SettingNames.DownloadAllWhenPushDLButton, false), false);

            this.IsDupeOnStageAllowed = new SettingInfoViewModel<bool>(WS::SettingPage.SettingsConainer.GetSetting<bool>(SettingNames.AllowDupeOnStage, false), false);

            this.IsOverrideVideoFileDTToUploadedDT = new SettingInfoViewModel<bool>(WS::SettingPage.SettingsConainer.GetSetting<bool>(SettingNames.OverideVideoFileDTToUploadedDT, false), false);

            this.IsUnsafeCommentHandleEnable = new SettingInfoViewModel<bool>(WS::SettingPage.SettingsConainer.GetSetting<bool>(SettingNames.EnableUnsafeCommentHandle, false), false);

            this.IsDownloadResumingEnable = new SettingInfoViewModel<bool>(WS::SettingPage.SettingsConainer.GetSetting<bool>(SettingNames.IsResumeEnable, false), false);

            this.IsOmitXmlDeclarationEnable = new SettingInfoViewModel<bool>(WS::SettingPage.SettingsConainer.GetSetting<bool>(SettingNames.IsOmittingXmlDeclarationIsEnable, false), false);

            var maxPResult = WS::SettingPage.SettingsConainer.GetSetting(SettingNames.MaxParallelDownloadCount, NetConstant.DefaultMaxParallelDownloadCount);
            var maxSPResult = WS::SettingPage.SettingsConainer.GetSetting(SettingNames.MaxParallelSegmentDownloadCount, NetConstant.DefaultMaxParallelDownloadCount);


            var n1 = new ComboboxItem<int>(1, "1");
            var n2 = new ComboboxItem<int>(2, "2");
            var n3 = new ComboboxItem<int>(3, "3");
            var n4 = new ComboboxItem<int>(4, "4");

            this.SelectableMaxParallelDownloadCount = new List<ComboboxItem<int>>() { n1, n2, n3, n4 };
            this.MaxParallelDownloadCount = new ConvertableSettingInfoViewModel<int, ComboboxItem<int>>(WS::SettingPage.SettingsConainer.GetSetting(SettingNames.MaxParallelDownloadCount, NetConstant.DefaultMaxParallelDownloadCount), n2, x =>
            {
                if (this.SelectableMaxParallelDownloadCount.Any(box => box.Value == x))
                {
                    return this.SelectableMaxParallelDownloadCount.First(box => box.Value == x);
                }
                else
                {
                    var nx = new ComboboxItem<int>(x, x.ToString());
                    this.SelectableMaxParallelDownloadCount.Add(nx);
                    return nx;
                }
            }, x => x.Value);


            this.MaxParallelSegmentDownloadCount = new ConvertableSettingInfoViewModel<int, ComboboxItem<int>>(WS::SettingPage.SettingsConainer.GetSetting(SettingNames.MaxParallelSegmentDownloadCount, NetConstant.DefaultMaxParallelSegmentDownloadCount), n3, x =>
            {
                if (this.SelectableMaxParallelDownloadCount.Any(box => box.Value == x))
                {
                    return this.SelectableMaxParallelDownloadCount.First(box => box.Value == x);
                }
                else
                {
                    var nx = new ComboboxItem<int>(x, x.ToString());
                    this.SelectableMaxParallelDownloadCount.Add(nx);
                    return nx;
                }
            }, x => x.Value);

            this.MaxTmpDirCount = new SettingInfoViewModel<int>(WS::SettingPage.SettingsConainer.GetSetting(SettingNames.MaxParallelSegmentDownloadCount, 20), 20);

            var t1 = new ComboboxItem<VideoInfoTypeSettings>(VideoInfoTypeSettings.Text, "テキスト(Xeno互換)");
            var t2 = new ComboboxItem<VideoInfoTypeSettings>(VideoInfoTypeSettings.Json, "JSON");
            var t3 = new ComboboxItem<VideoInfoTypeSettings>(VideoInfoTypeSettings.Xml, "XML");

            this.SelectableVideoInfoType = new List<ComboboxItem<VideoInfoTypeSettings>>() { t1, t2, t3 };
            this.VideoInfoType = new ConvertableSettingInfoViewModel<VideoInfoTypeSettings, ComboboxItem<VideoInfoTypeSettings>>(WS::SettingPage.SettingsConainer.GetSetting(SettingNames.VideoInfoType, VideoInfoTypeSettings.Text), t1, x => x switch
            {
                VideoInfoTypeSettings.Json => t2,
                VideoInfoTypeSettings.Xml => t3,
                _ => t1
            }, x => x.Value);

            var i1 = new ComboboxItem<IchibaInfoTypeSettings>(IchibaInfoTypeSettings.Html, "html");
            var i2 = new ComboboxItem<IchibaInfoTypeSettings>(IchibaInfoTypeSettings.Json, "json");
            var i3 = new ComboboxItem<IchibaInfoTypeSettings>(IchibaInfoTypeSettings.Xml, "xml");
            this.SelectableIchibaInfoType = new List<ComboboxItem<IchibaInfoTypeSettings>>() { i1, i2, i3 };
            this.IchibaInfoType = new ConvertableSettingInfoViewModel<IchibaInfoTypeSettings, ComboboxItem<IchibaInfoTypeSettings>>(WS::SettingPage.SettingsConainer.GetSetting(SettingNames.IchibaInfoType, IchibaInfoTypeSettings.Html), i1, x => x switch
            {
                IchibaInfoTypeSettings.Json => i2,
                IchibaInfoTypeSettings.Xml => i3,
                _ => i1
            }, x => x.Value);

            this.IsExperimentalCommentSafetySystemEnable = new SettingInfoViewModel<bool>(WS::SettingPage.SettingsConainer.GetSetting(SettingNames.IsExperimentalCommentSafetySystemEnable, false), false);
            this.CommentFetchWaitSpan = new ConvertableSettingInfoViewModel<int, int>(WS::SettingPage.SettingsConainer.GetSetting(SettingNames.CommentFetchWaitSpan, LocalConstant.DefaultCommetFetchWaitSpan), LocalConstant.DefaultCommetFetchWaitSpan, x => x < 0 ? LocalConstant.DefaultCommetFetchWaitSpan : x, x => x < 0 ? LocalConstant.DefaultCommetFetchWaitSpan : x);
            this.IsDeletingExistingEconomyFileEnable = new SettingInfoViewModel<bool>(WS::SettingPage.SettingsConainer.GetSetting(SettingNames.DeleteExistingEconomyFile, false), false);

            this.CommentCountPerBlock = new SettingInfoViewModel<int>(WS::SettingPage.SettingsConainer.GetSetting(SettingNames.CommentCountPerBlock, NetConstant.DefaultCommentCountPerBlock), NetConstant.DefaultCommentCountPerBlock);

        }

        /// <summary>
        /// 選択可能な並列ダウンロード数
        /// </summary>
        public List<ComboboxItem<int>> SelectableMaxParallelDownloadCount { get; init; }

        /// <summary>
        /// 選択可能な動画情報ファイルの保存形式
        /// </summary>
        public List<ComboboxItem<VideoInfoTypeSettings>> SelectableVideoInfoType { get; init; }

        /// <summary>
        /// 選択可能な市場情報ファイルの保存形式
        /// </summary>
        public List<ComboboxItem<IchibaInfoTypeSettings>> SelectableIchibaInfoType { get; init; }


        /// <summary>
        /// コメントのオフセット
        /// </summary>
        public SettingInfoViewModel<int> CommentOffset { get; init; }

        /// <summary>
        /// オフセット調節
        /// </summary>
        public SettingInfoViewModel<bool> IsAutoSwitchOffsetEnable { get; init; }

        /// <summary>
        /// 最大並列DL数
        /// </summary>
        public ConvertableSettingInfoViewModel<int, ComboboxItem<int>> MaxParallelDownloadCount { get; init; }

        /// <summary>
        /// 最大セグメント並列DL数
        /// </summary>
        public ConvertableSettingInfoViewModel<int, ComboboxItem<int>> MaxParallelSegmentDownloadCount { get; init; }

        /// <summary>
        /// 動画情報ファイルの保存形式
        /// </summary>
        public ConvertableSettingInfoViewModel<VideoInfoTypeSettings, ComboboxItem<VideoInfoTypeSettings>> VideoInfoType { get; init; }

        /// <summary>
        /// 市場情報の保存形式
        /// </summary>
        public ConvertableSettingInfoViewModel<IchibaInfoTypeSettings, ComboboxItem<IchibaInfoTypeSettings>> IchibaInfoType { get; init; }

        /// <summary>
        /// キューからもDLする
        /// </summary>
        public SettingInfoViewModel<bool> IsDownloadFromQueueEnable { get; init; }

        /// <summary>
        /// ステージングの際の重複を許可する
        /// </summary>
        public SettingInfoViewModel<bool> IsDupeOnStageAllowed { get; init; }

        /// <summary>
        /// 動画ファイルの更新日時を投稿日時にする
        /// </summary>
        public SettingInfoViewModel<bool> IsOverrideVideoFileDTToUploadedDT { get; init; }

        /// <summary>
        /// JSONでDLする
        /// </summary>
        public SettingInfoViewModel<bool> IsDownloadVideoInfoInJsonEnable { get; init; }

        /// <summary>
        /// レジュームを有効にする
        /// </summary>
        public SettingInfoViewModel<bool> IsDownloadResumingEnable { get; init; }

        /// <summary>
        /// 安全でないコメントハンドルを有効にする
        /// </summary>
        public SettingInfoViewModel<bool> IsUnsafeCommentHandleEnable { get; init; }

        /// <summary>
        /// /試験的な安全システムを有効にする
        /// </summary>
        public SettingInfoViewModel<bool> IsExperimentalCommentSafetySystemEnable { get; set; }

        /// <summary>
        /// Xml宣言を出力
        /// </summary>
        public SettingInfoViewModel<bool> IsOmitXmlDeclarationEnable { get; init; }

        /// <summary>
        /// 1ブロックあたりのコメント数
        /// </summary>
        public SettingInfoViewModel<int> CommentCountPerBlock { get; init; }

        /// <summary>
        /// エコノミーファイル削除
        /// </summary>
        public SettingInfoViewModel<bool> IsDeletingExistingEconomyFileEnable { get; init; }

        /// <summary>
        /// 一時フォルダーの最大保持数
        /// </summary>
        public SettingInfoViewModel<int> MaxTmpDirCount { get; init; }

        /// <summary>
        /// コメント取得時の待機時間
        /// </summary>
        [RegularExpression(@"^\d$", ErrorMessage = "整数値を入力してください。")]
        public ConvertableSettingInfoViewModel<int, int> CommentFetchWaitSpan { get; init; }


    }

    [Obsolete("Desinger", true)]
    class DownloadSettingPageViewModelD
    {
        public DownloadSettingPageViewModelD()
        {
            var n1 = new ComboboxItem<int>(1, "1");
            var n2 = new ComboboxItem<int>(2, "2");
            var n3 = new ComboboxItem<int>(3, "3");
            var n4 = new ComboboxItem<int>(4, "4");

            this.SelectableMaxParallelDownloadCount = new List<ComboboxItem<int>>() { n1, n2, n3, n4 };
            this.MaxParallelDownloadCount = new ConvertableSettingInfoViewModelD<ComboboxItem<int>>(n3);
            this.MaxParallelSegmentDownloadCount = new ConvertableSettingInfoViewModelD<ComboboxItem<int>>(n4);

            var t1 = new ComboboxItem<VideoInfoTypeSettings>(VideoInfoTypeSettings.Text, "テキスト(Xeno互換)");
            var t2 = new ComboboxItem<VideoInfoTypeSettings>(VideoInfoTypeSettings.Json, "JSON");
            var t3 = new ComboboxItem<VideoInfoTypeSettings>(VideoInfoTypeSettings.Xml, "XML");

            this.SelectableVideoInfoType = new List<ComboboxItem<VideoInfoTypeSettings>>() { t1, t2, t3 };
            this.VideoInfoType = new ConvertableSettingInfoViewModelD<ComboboxItem<VideoInfoTypeSettings>>(t1);

            var i1 = new ComboboxItem<IchibaInfoTypeSettings>(IchibaInfoTypeSettings.Html, "html");
            var i2 = new ComboboxItem<IchibaInfoTypeSettings>(IchibaInfoTypeSettings.Json, "json");
            var i3 = new ComboboxItem<IchibaInfoTypeSettings>(IchibaInfoTypeSettings.Xml, "xml");
            this.SelectableIchibaInfoType = new List<ComboboxItem<IchibaInfoTypeSettings>>() { i1, i2, i3 };
            this.IchibaInfoType = new ConvertableSettingInfoViewModelD<ComboboxItem<IchibaInfoTypeSettings>>(i1);
        }


        public List<ComboboxItem<int>> SelectableMaxParallelDownloadCount { get; init; }

        public List<ComboboxItem<VideoInfoTypeSettings>> SelectableVideoInfoType { get; init; }

        public List<ComboboxItem<IchibaInfoTypeSettings>> SelectableIchibaInfoType { get; init; }

        public ConvertableSettingInfoViewModelD<ComboboxItem<int>> MaxParallelDownloadCount { get; init; }

        public ConvertableSettingInfoViewModelD<ComboboxItem<int>> MaxParallelSegmentDownloadCount { get; init; }

        public ConvertableSettingInfoViewModelD<ComboboxItem<VideoInfoTypeSettings>> VideoInfoType { get; init; }

        public ConvertableSettingInfoViewModelD<ComboboxItem<IchibaInfoTypeSettings>> IchibaInfoType { get; init; }

        public SettingInfoViewModelD<int> CommentOffset { get; set; } = new(200);

        public SettingInfoViewModelD<bool> IsAutoSwitchOffsetEnable { get; set; } = new(true);

        public SettingInfoViewModelD<bool> IsDownloadFromQueueEnable { get; set; } = new(false);

        public SettingInfoViewModelD<bool> IsDupeOnStageAllowed { get; set; } = new(true);

        public SettingInfoViewModelD<bool> IsOverrideVideoFileDTToUploadedDT { get; set; } = new(true);

        public SettingInfoViewModelD<bool> IsDownloadVideoInfoInJsonEnable { get; set; } = new(true);

        public SettingInfoViewModelD<bool> IsDownloadResumingEnable { get; set; } = new(true);

        public SettingInfoViewModelD<bool> IsUnsafeCommentHandleEnable { get; set; } = new(true);

        public SettingInfoViewModelD<bool> IsExperimentalCommentSafetySystemEnable { get; set; } = new(true);

        public SettingInfoViewModelD<bool> IsDeletingExistingEconomyFileEnable { get; init; } = new(true);

        public SettingInfoViewModelD<bool> IsOmitXmlDeclarationEnable { get; init; } = new(true);

        public SettingInfoViewModelD<int> CommentCountPerBlock { get; init; } = new(NetConstant.DefaultCommentCountPerBlock);

        public SettingInfoViewModelD<int> MaxTmpDirCount { get; set; } = new(20);

        public SettingInfoViewModelD<int> CommentFetchWaitSpan { get; init; } = new(0);
    }
}
