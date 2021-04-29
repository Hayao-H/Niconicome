using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS = Niconicome.Workspaces;
using System.ComponentModel;
using Niconicome.ViewModels.Mainpage.Utils;
using Niconicome.Models.Local.Settings;

namespace Niconicome.ViewModels.Setting.Pages
{
    class DownloadSettingPageViewModel : SettingaBase
    {
        public DownloadSettingPageViewModel()
        {
            var cOffset = WS::SettingPage.SettingHandler.GetIntSetting(SettingsEnum.CommentOffset);
            if (cOffset == -1)
            {
                this.commentOffsetField = 40;
            }
            else
            {
                this.commentOffsetField = cOffset;
            }
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

            this.isDownloadVideoInfoInJsonEnableField = WS::SettingPage.SettingHandler.GetBoolSetting(SettingsEnum.VideoInfoInJson);
            this.isDownloadFromQueueEnableField = WS::SettingPage.SettingHandler.GetBoolSetting(SettingsEnum.DLAllFromQueue);
            this.isDupeOnStageAllowedField = WS::SettingPage.SettingHandler.GetBoolSetting(SettingsEnum.AllowDupeOnStage);
            this.isOverrideVideoFileDTToUploadedDTField = WS::SettingPage.SettingHandler.GetBoolSetting(SettingsEnum.OverrideVideoFileDTToUploadedDT);
            this.isUnsafeCommentHandleEnableField = WS::SettingPage.SettingHandler.GetBoolSetting(SettingsEnum.UnsafeCommentHandle);

            var n1 = new ComboboxItem<int>(1, "1");
            var n2 = new ComboboxItem<int>(2, "2");
            var n3 = new ComboboxItem<int>(3, "3");
            var n4 = new ComboboxItem<int>(4, "4");

            this.SelectableMaxParallelDownloadCount = new List<ComboboxItem<int>>() { n1, n2, n3, n4 };
            this.maxParallelDownloadCountFIeld = maxP switch
            {
                1 => n1,
                2 => n2,
                3 => n3,
                4 => n4,
                _ => n4,
            };
            this.maxParallelSegmentDownloadCountField = maxSP switch
            {
                1 => n1,
                2 => n2,
                3 => n3,
                4 => n4,
                _ => n4,
            };

            this.isDownloadResumingEnableField = WS::SettingPage.SettingHandler.GetBoolSetting(SettingsEnum.EnableResume);
            var maxTmp = WS::SettingPage.SettingHandler.GetIntSetting(SettingsEnum.MaxTmpDirCount);
            this.maxTmpDirCountField = maxTmp < 0 ? 20 : maxTmp;

        }

        #region 設定値のフィールド
        private int commentOffsetField;

        private bool isAutoSwitchOffsetEnableField;

        private ComboboxItem<int> maxParallelDownloadCountFIeld;

        private ComboboxItem<int> maxParallelSegmentDownloadCountField;

        private bool isDownloadFromQueueEnableField;

        private bool isDupeOnStageAllowedField;

        private bool isOverrideVideoFileDTToUploadedDTField;

        private bool isDownloadVideoInfoInJsonEnableField;

        private bool isDownloadResumingEnableField;

        private bool isUnsafeCommentHandleEnableField;

        private int maxTmpDirCountField;
        #endregion

        public List<ComboboxItem<int>> SelectableMaxParallelDownloadCount { get; init; }

        /// <summary>
        /// コメントのオフセット
        /// </summary>
        public int CommentOffsetFIeld
        {
            get => this.commentOffsetField;
            set
            {
                if (!int.TryParse(value.ToString(), out int _)) return;
                this.Savesetting(ref this.commentOffsetField, value, SettingsEnum.CommentOffset);
            }
        }

        /// <summary>
        /// オフセット調節
        /// </summary>
        public bool IsAutoSwitchOffsetEnable { get => this.isAutoSwitchOffsetEnableField; set => this.Savesetting(ref this.isAutoSwitchOffsetEnableField, value, SettingsEnum.SwitchOffset); }

        /// <summary>
        /// 最大並列DL数
        /// </summary>
        public ComboboxItem<int> MaxParallelDownloadCount { get => this.maxParallelDownloadCountFIeld; set => this.Savesetting(ref this.maxParallelDownloadCountFIeld, value, SettingsEnum.MaxParallelDL); }

        /// <summary>
        /// 最大セグメント並列DL数
        /// </summary>
        public ComboboxItem<int> MaxParallelSegmentDownloadCount
        {
            get => this.maxParallelSegmentDownloadCountField;
            set => this.Savesetting(ref this.maxParallelSegmentDownloadCountField, value, SettingsEnum.MaxParallelSegDl);
        }

        /// <summary>
        /// キューからもDLする
        /// </summary>
        public bool IsDownloadFromQueueEnable { get => this.isDownloadFromQueueEnableField; set => this.Savesetting(ref this.isDownloadFromQueueEnableField, value, SettingsEnum.DLAllFromQueue); }

        /// <summary>
        /// ステージングの際の重複を許可する
        /// </summary>
        public bool IsDupeOnStageAllowed { get => this.isDupeOnStageAllowedField; set => this.Savesetting(ref this.isDupeOnStageAllowedField, value, SettingsEnum.AllowDupeOnStage); }

        /// <summary>
        /// 動画ファイルの更新日時を投稿日時にする
        /// </summary>
        public bool IsOverrideVideoFileDTToUploadedDT { get => this.isOverrideVideoFileDTToUploadedDTField; set => this.Savesetting(ref this.isOverrideVideoFileDTToUploadedDTField, value, SettingsEnum.OverrideVideoFileDTToUploadedDT); }

        /// <summary>
        /// JSONでDLする
        /// </summary>
        public bool IsDownloadVideoInfoInJsonEnable { get => this.isDownloadVideoInfoInJsonEnableField; set => this.Savesetting(ref this.isDownloadVideoInfoInJsonEnableField, value, SettingsEnum.VideoInfoInJson); }

        /// <summary>
        /// レジュームを有効にする
        /// </summary>
        public bool IsDownloadResumingEnable { get => this.isDownloadResumingEnableField; set => this.Savesetting(ref this.isDownloadResumingEnableField, value, SettingsEnum.EnableResume); }

        /// <summary>
        /// 安全でないコメントハンドルを有効にする
        /// </summary>
        public bool IsUnsafeCommentHandleEnable { get => this.isUnsafeCommentHandleEnableField; set => this.Savesetting(ref this.isUnsafeCommentHandleEnableField, value, SettingsEnum.UnsafeCommentHandle); }


        /// <summary>
        /// 一時フォルダーの最大保持数
        /// </summary>
        public int MaxTmpDirCount { get => this.maxTmpDirCountField; set => this.Savesetting(ref this.maxTmpDirCountField, value, SettingsEnum.MaxTmpDirCount); }


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
            this.MaxParallelDownloadCount = n3;
            this.MaxParallelSegmentDownloadCount = n4;
        }

        public int CommentOffsetFIeld { get; set; } = 40;

        public bool IsAutoSwitchOffsetEnable { get; set; } = true;

        public List<ComboboxItem<int>> SelectableMaxParallelDownloadCount { get; init; }

        public ComboboxItem<int> MaxParallelDownloadCount { get; set; }

        public ComboboxItem<int> MaxParallelSegmentDownloadCount { get; set; }

        public bool IsDownloadFromQueueEnable { get; set; } = true;

        public bool IsDupeOnStageAllowed { get; set; } = true;

        public bool IsOverrideVideoFileDTToUploadedDT { get; set; } = true;

        public bool IsDownloadVideoInfoInJsonEnable { get; set; } = true;

        public bool IsDownloadResumingEnable { get; set; } = true;

        public bool IsUnsafeCommentHandleEnable { get; set; } = false;

        public int MaxTmpDirCount { get; set; } = 20;
    }
}
