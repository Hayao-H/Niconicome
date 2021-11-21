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

namespace Niconicome.ViewModels.Setting.Pages
{
    class DownloadSettingPageViewModel : SettingaBase, IDisposable
    {
        public DownloadSettingPageViewModel()
        {
            var cOffset = WS::SettingPage.SettingHandler.GetIntSetting(SettingsEnum.CommentOffset);
            if (cOffset == -1)
            {
                this.CommentOffset = new ReactiveProperty<int>(40);
            }
            else
            {
                this.CommentOffset = new ReactiveProperty<int>(cOffset);
            }
            this.CommentOffset.Subscribe(value => this.SaveSetting(value, SettingsEnum.CommentOffset));

            this.isAutoSwitchOffsetEnableField = WS::SettingPage.SettingHandler.GetBoolSetting(SettingsEnum.SwitchOffset);

            var maxP = WS::SettingPage.SettingHandler.GetIntSetting(SettingsEnum.MaxParallelDL);
            var maxSP = WS::SettingPage.SettingHandler.GetIntSetting(SettingsEnum.MaxParallelSegDl);

            if (maxP <= 0)
            {
                maxP = 3;
            }
            if (maxSP <= 0)
            {
                maxSP = 1;
            }

            this.IsDownloadVideoInfoInJsonEnable = new ReactivePropertySlim<bool>(WS::SettingPage.SettingHandler.GetBoolSetting(SettingsEnum.VideoInfoInJson)).AddTo(this.disposables);
            this.IsDownloadVideoInfoInJsonEnable.Subscribe(value => this.SaveSetting(value, SettingsEnum.VideoInfoInJson));

            this.IsDownloadFromQueueEnable = new ReactivePropertySlim<bool>(WS::SettingPage.SettingHandler.GetBoolSetting(SettingsEnum.DLAllFromQueue)).AddTo(this.disposables);
            this.IsDownloadFromQueueEnable.Subscribe(value => this.SaveSetting(value, SettingsEnum.DLAllFromQueue));

            this.IsDupeOnStageAllowed = new ReactivePropertySlim<bool>(WS::SettingPage.SettingHandler.GetBoolSetting(SettingsEnum.AllowDupeOnStage)).AddTo(this.disposables);
            this.IsDupeOnStageAllowed.Subscribe(value => this.SaveSetting(value, SettingsEnum.AllowDupeOnStage));

            this.IsOverrideVideoFileDTToUploadedDT = new ReactivePropertySlim<bool>(WS::SettingPage.SettingHandler.GetBoolSetting(SettingsEnum.OverrideVideoFileDTToUploadedDT)).AddTo(this.disposables);
            this.IsOverrideVideoFileDTToUploadedDT.Subscribe(value => this.SaveSetting(value, SettingsEnum.OverrideVideoFileDTToUploadedDT));

            this.IsUnsafeCommentHandleEnable = new ReactivePropertySlim<bool>(WS::SettingPage.SettingHandler.GetBoolSetting(SettingsEnum.UnsafeCommentHandle)).AddTo(this.disposables);
            this.IsUnsafeCommentHandleEnable.Subscribe(value => this.SaveSetting(value, SettingsEnum.UnsafeCommentHandle));

            this.IsDownloadResumingEnable = new ReactivePropertySlim<bool>(WS::SettingPage.SettingHandler.GetBoolSetting(SettingsEnum.EnableResume)).AddTo(this.disposables);
            this.IsDownloadResumingEnable.Subscribe(value => this.SaveSetting(value, SettingsEnum.EnableResume));

            var n1 = new ComboboxItem<int>(1, "1");
            var n2 = new ComboboxItem<int>(2, "2");
            var n3 = new ComboboxItem<int>(3, "3");
            var n4 = new ComboboxItem<int>(4, "4");
            var nq = new ComboboxItem<int>(maxSP, maxSP.ToString());

            this.SelectableMaxParallelDownloadCount = new List<ComboboxItem<int>>() { n1, n2, n3, n4 };
            if (maxSP > 4)
            {
                this.SelectableMaxParallelDownloadCount.Add(nq);
            }

            this.MaxParallelDownloadCount = maxP switch
            {
                1 => new ReactivePropertySlim<ComboboxItem<int>>(n1),
                2 => new ReactivePropertySlim<ComboboxItem<int>>(n2),
                3 => new ReactivePropertySlim<ComboboxItem<int>>(n3),
                4 => new ReactivePropertySlim<ComboboxItem<int>>(n4),
                _ => new ReactivePropertySlim<ComboboxItem<int>>(n4),
            };
            this.MaxParallelDownloadCount.Subscribe(value => this.SaveSetting(value, SettingsEnum.MaxParallelDL)).AddTo(this.disposables);

            this.MaxParallelSegmentDownloadCount = WS::SettingPage.SettingsContainer.GetReactiveIntSetting(SettingsEnum.MaxParallelSegDl)
                .ToReactivePropertySlimAsSynchronized(value => value.Value, value =>
                    value switch
                    {
                        1 => n1,
                        2 => n2,
                        3 => n3,
                        4 => n4,
                        _ => nq,

                    }, value => value.Value);

            var maxTmp = WS::SettingPage.SettingHandler.GetIntSetting(SettingsEnum.MaxTmpDirCount);
            this.MaxTmpDirCount = new ReactiveProperty<int>(maxTmp < 0 ? 20 : maxTmp).AddTo(this.disposables);
            this.MaxTmpDirCount.Subscribe(value => this.SaveSetting(value, SettingsEnum.MaxTmpDirCount));

            var t1 = new ComboboxItem<VideoInfoTypeSettings>(VideoInfoTypeSettings.Text, "テキスト(Xeno互換)");
            var t2 = new ComboboxItem<VideoInfoTypeSettings>(VideoInfoTypeSettings.Json, "JSON");
            var t3 = new ComboboxItem<VideoInfoTypeSettings>(VideoInfoTypeSettings.Xml, "XML");

            this.SelectableVideoInfoType = new List<ComboboxItem<VideoInfoTypeSettings>>() { t1, t2, t3 };
            this.VideoInfoType = WS::SettingPage.EnumSettingsHandler.GetSetting<VideoInfoTypeSettings>() switch
            {
                VideoInfoTypeSettings.Json => new ReactivePropertySlim<ComboboxItem<VideoInfoTypeSettings>>(t2),
                VideoInfoTypeSettings.Xml => new ReactivePropertySlim<ComboboxItem<VideoInfoTypeSettings>>(t3),
                _ => new ReactivePropertySlim<ComboboxItem<VideoInfoTypeSettings>>(t1),
            };
            this.VideoInfoType.Subscribe(value => this.SaveEnumSetting(value)).AddTo(this.disposables);


            var i1 = new ComboboxItem<IchibaInfoTypeSettings>(IchibaInfoTypeSettings.Html, "html");
            var i2 = new ComboboxItem<IchibaInfoTypeSettings>(IchibaInfoTypeSettings.Json, "json");
            var i3 = new ComboboxItem<IchibaInfoTypeSettings>(IchibaInfoTypeSettings.Xml, "xml");
            this.SelectableIchibaInfoType = new List<ComboboxItem<IchibaInfoTypeSettings>>() { i1, i2, i3 };
            this.IchibaInfoType = WS::SettingPage.EnumSettingsHandler.GetSetting<IchibaInfoTypeSettings>() switch
            {
                IchibaInfoTypeSettings.Xml => new ReactiveProperty<ComboboxItem<IchibaInfoTypeSettings>>(i3),
                IchibaInfoTypeSettings.Json => new ReactiveProperty<ComboboxItem<IchibaInfoTypeSettings>>(i2),
                _ => new ReactiveProperty<ComboboxItem<IchibaInfoTypeSettings>>(i1),
            };
            this.IchibaInfoType.Subscribe(value => this.SaveEnumSetting(value)).AddTo(this.disposables);

            this.IsExperimentalCommentSafetySystemEnable = WS::SettingPage.SettingsContainer.GetReactiveBoolSetting(SettingsEnum.ExperimentalSafety);
            this.CommentFetchWaitSpan = WS::SettingPage.SettingsContainer.GetReactiveIntSetting(SettingsEnum.CommentWaitSpan, null, value => value < 0 ? LocalConstant.DefaultCommetFetchWaitSpan : value);
            this.IsDeletingExistingEconomyFileEnable = WS::SettingPage.SettingsContainer.GetReactiveBoolSetting(SettingsEnum.DeleteEcoFile);

        }

        #region 設定値のフィールド
        private bool isAutoSwitchOffsetEnableField;
        #endregion

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
        public ReactiveProperty<int> CommentOffset { get; init; }

        /// <summary>
        /// オフセット調節
        /// </summary>
        public bool IsAutoSwitchOffsetEnable { get => this.isAutoSwitchOffsetEnableField; set => this.Savesetting(ref this.isAutoSwitchOffsetEnableField, value, SettingsEnum.SwitchOffset); }

        /// <summary>
        /// 最大並列DL数
        /// </summary>
        public ReactivePropertySlim<ComboboxItem<int>> MaxParallelDownloadCount { get; init; }

        /// <summary>
        /// 最大セグメント並列DL数
        /// </summary>
        public ReactivePropertySlim<ComboboxItem<int>> MaxParallelSegmentDownloadCount { get; init; }

        /// <summary>
        /// 動画情報ファイルの保存形式
        /// </summary>
        public ReactivePropertySlim<ComboboxItem<VideoInfoTypeSettings>> VideoInfoType { get; init; }

        /// <summary>
        /// 市場情報の保存形式
        /// </summary>
        public ReactiveProperty<ComboboxItem<IchibaInfoTypeSettings>> IchibaInfoType { get; init; }

        /// <summary>
        /// キューからもDLする
        /// </summary>
        public ReactivePropertySlim<bool> IsDownloadFromQueueEnable { get; init; }

        /// <summary>
        /// ステージングの際の重複を許可する
        /// </summary>
        public ReactivePropertySlim<bool> IsDupeOnStageAllowed { get; init; }

        /// <summary>
        /// 動画ファイルの更新日時を投稿日時にする
        /// </summary>
        public ReactivePropertySlim<bool> IsOverrideVideoFileDTToUploadedDT { get; init; }

        /// <summary>
        /// JSONでDLする
        /// </summary>
        public ReactivePropertySlim<bool> IsDownloadVideoInfoInJsonEnable { get; init; }

        /// <summary>
        /// レジュームを有効にする
        /// </summary>
        public ReactivePropertySlim<bool> IsDownloadResumingEnable { get; init; }

        /// <summary>
        /// 安全でないコメントハンドルを有効にする
        /// </summary>
        public ReactivePropertySlim<bool> IsUnsafeCommentHandleEnable { get; init; }

        /// <summary>
        /// /試験的な安全システムを有効にする
        /// </summary>
        public ReactiveProperty<bool> IsExperimentalCommentSafetySystemEnable { get; set; }

        /// <summary>
        /// エコノミーファイル削除
        /// </summary>
        public ReactiveProperty<bool> IsDeletingExistingEconomyFileEnable { get; init; }

        /// <summary>
        /// 一時フォルダーの最大保持数
        /// </summary>
        public ReactiveProperty<int> MaxTmpDirCount { get; init; }

        /// <summary>
        /// コメント取得時の待機時間
        /// </summary>
        [RegularExpression(@"^\d$", ErrorMessage = "整数値を入力してください。")]
        public ReactiveProperty<int> CommentFetchWaitSpan { get; init; }


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
            this.MaxParallelDownloadCount = new ReactivePropertySlim<ComboboxItem<int>>(n3);
            this.MaxParallelSegmentDownloadCount = new ReactivePropertySlim<ComboboxItem<int>>(n4);

            var t1 = new ComboboxItem<VideoInfoTypeSettings>(VideoInfoTypeSettings.Text, "テキスト(Xeno互換)");
            var t2 = new ComboboxItem<VideoInfoTypeSettings>(VideoInfoTypeSettings.Json, "JSON");
            var t3 = new ComboboxItem<VideoInfoTypeSettings>(VideoInfoTypeSettings.Xml, "XML");

            this.SelectableVideoInfoType = new List<ComboboxItem<VideoInfoTypeSettings>>() { t1, t2, t3 };
            this.VideoInfoType = new ReactivePropertySlim<ComboboxItem<VideoInfoTypeSettings>>(t2);

            var i1 = new ComboboxItem<IchibaInfoTypeSettings>(IchibaInfoTypeSettings.Html, "html");
            var i2 = new ComboboxItem<IchibaInfoTypeSettings>(IchibaInfoTypeSettings.Json, "json");
            var i3 = new ComboboxItem<IchibaInfoTypeSettings>(IchibaInfoTypeSettings.Xml, "xml");
            this.SelectableIchibaInfoType = new List<ComboboxItem<IchibaInfoTypeSettings>>() { i1, i2, i3 };
            this.IchibaInfoType = new ReactiveProperty<ComboboxItem<IchibaInfoTypeSettings>>(i1);
        }


        public List<ComboboxItem<int>> SelectableMaxParallelDownloadCount { get; init; }

        public List<ComboboxItem<VideoInfoTypeSettings>> SelectableVideoInfoType { get; init; }

        public List<ComboboxItem<IchibaInfoTypeSettings>> SelectableIchibaInfoType { get; init; }

        public ReactivePropertySlim<ComboboxItem<int>> MaxParallelDownloadCount { get; init; }

        public ReactivePropertySlim<ComboboxItem<int>> MaxParallelSegmentDownloadCount { get; init; }

        public ReactivePropertySlim<ComboboxItem<VideoInfoTypeSettings>> VideoInfoType { get; init; }

        public ReactiveProperty<ComboboxItem<IchibaInfoTypeSettings>> IchibaInfoType { get; init; }

        public ReactivePropertySlim<int> CommentOffset { get; set; } = new(200);

        public ReactivePropertySlim<bool> IsAutoSwitchOffsetEnable { get; set; } = new(true);

        public ReactivePropertySlim<bool> IsDownloadFromQueueEnable { get; set; } = new();

        public ReactivePropertySlim<bool> IsDupeOnStageAllowed { get; set; } = new(true);

        public ReactivePropertySlim<bool> IsOverrideVideoFileDTToUploadedDT { get; set; } = new(true);

        public ReactivePropertySlim<bool> IsDownloadVideoInfoInJsonEnable { get; set; } = new(true);

        public ReactivePropertySlim<bool> IsDownloadResumingEnable { get; set; } = new(true);

        public ReactivePropertySlim<bool> IsUnsafeCommentHandleEnable { get; set; } = new(true);

        public ReactivePropertySlim<bool> IsExperimentalCommentSafetySystemEnable { get; set; } = new(true);

        public ReactivePropertySlim<bool> IsDeletingExistingEconomyFileEnable { get; init; } = new(true);

        public ReactiveProperty<int> MaxTmpDirCount { get; set; } = new(20);

        public ReactiveProperty<int> CommentFetchWaitSpan { get; init; } = new(0);
    }
}
