using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS = Niconicome.Workspaces;
using Local = Niconicome.Models.Local;
using System.ComponentModel;
using Niconicome.Models.Local;

namespace Niconicome.ViewModels.Setting.Pages
{
    class DownloadSettingPageViewModel : SettingaBase
    {
        public DownloadSettingPageViewModel()
        {
            var cOffset = WS::SettingPage.SettingHandler.GetIntSetting(Local::Settings.CommentOffset);
            if (cOffset == -1)
            {
                this.commentOffsetField = 40;
            }
            else
            {
                this.commentOffsetField = cOffset;
            }
            this.isAutoSwitchOffsetEnableField = WS::SettingPage.SettingHandler.GetBoolSetting(Settings.SwitchOffset);

            var maxP = WS::SettingPage.SettingHandler.GetIntSetting(Settings.MaxParallelDL);
            var maxSP = WS::SettingPage.SettingHandler.GetIntSetting(Settings.MaxParallelSegDl);

            if (maxP <= 0)
            {
                maxP = 3;
            }
            if (maxSP <= 0)
            {
                maxSP = 1;
            }

            this.maxParallelDownloadCountFIeld = maxP;
            this.maxParallelSegmentDownloadCountField = maxSP;
            this.isDownloadFromQueueEnableField = WS::SettingPage.SettingHandler.GetBoolSetting(Settings.DLAllFromQueue);
            this.isDupeOnStageAllowedField = WS::SettingPage.SettingHandler.GetBoolSetting(Settings.AllowDupeOnStage);
            this.isOverrideVideoFileDTToUploadedDTField = WS::SettingPage.SettingHandler.GetBoolSetting(Settings.OverrideVideoFileDTToUploadedDT);
        }

        private int commentOffsetField;

        private bool isAutoSwitchOffsetEnableField;

        private int maxParallelDownloadCountFIeld;

        private int maxParallelSegmentDownloadCountField;

        private bool isDownloadFromQueueEnableField;

        private bool isDupeOnStageAllowedField;

        private bool isOverrideVideoFileDTToUploadedDTField;

        /// <summary>
        /// コメントのオフセット
        /// </summary>
        public int CommentOffsetFIeld
        {
            get => this.commentOffsetField;
            set
            {
                if (!int.TryParse(value.ToString(), out int _)) return;
                this.Savesetting(ref this.commentOffsetField, value, Local::Settings.CommentOffset);
            }
        }

        /// <summary>
        /// オフセット調節
        /// </summary>
        public bool IsAutoSwitchOffsetEnable { get => this.isAutoSwitchOffsetEnableField; set => this.Savesetting(ref this.isAutoSwitchOffsetEnableField, value, Settings.SwitchOffset); }

        /// <summary>
        /// 最大並列DL数
        /// </summary>
        public int MaxParallelDownloadCount { get => this.maxParallelDownloadCountFIeld; set => this.Savesetting(ref this.maxParallelDownloadCountFIeld, value, Settings.MaxParallelDL); }

        /// <summary>
        /// 最大セグメント並列DL数
        /// </summary>
        public int MaxParallelSegmentDownloadCount
        {
            get => this.maxParallelSegmentDownloadCountField;
            set
            {
                if (value > 5)
                {
                    WS::SettingPage.SnackbarMessageQueue.Enqueue("セグメントの最大並列ダウンロード数は5です");
                    return;
                }
                this.Savesetting(ref this.maxParallelSegmentDownloadCountField, value, Settings.MaxParallelSegDl);
            }
        }

        /// <summary>
        /// キューからもDLする
        /// </summary>
        public bool IsDownloadFromQueueEnable { get => this.isDownloadFromQueueEnableField; set => this.Savesetting(ref this.isDownloadFromQueueEnableField, value, Settings.DLAllFromQueue); }

        /// <summary>
        /// ステージングの際の重複を許可する
        /// </summary>
        public bool IsDupeOnStageAllowed { get => this.isDupeOnStageAllowedField; set => this.Savesetting(ref this.isDupeOnStageAllowedField, value, Settings.AllowDupeOnStage); }

        /// <summary>
        /// 動画ファイルの更新日時を投稿日時にする
        /// </summary>
        public bool IsOverrideVideoFileDTToUploadedDT { get => this.isOverrideVideoFileDTToUploadedDTField; set => this.Savesetting(ref this.isOverrideVideoFileDTToUploadedDTField, value, Settings.OverrideVideoFileDTToUploadedDT); }


    }

    [Obsolete("Desinger", true)]
    class DownloadSettingPageViewModelD
    {
        public int CommentOffsetFIeld { get; set; } = 40;

        public bool IsAutoSwitchOffsetEnable { get; set; } = true;

        public int MaxParallelDownloadCount { get; set; } = 3;

        public int MaxParallelSegmentDownloadCount { get; set; } = 1;

        public bool IsDownloadFromQueueEnable { get; set; } = true;

        public bool IsDupeOnStageAllowed { get; set; } = true;

        public bool IsOverrideVideoFileDTToUploadedDT { get; set; } = true;
    }
}
