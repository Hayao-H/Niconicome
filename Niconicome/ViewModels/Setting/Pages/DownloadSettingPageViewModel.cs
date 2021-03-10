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
            } else
            {
                this.commentOffsetField = cOffset;
            }
            this.isAutoSwitchOffsetEnableField = WS::SettingPage.SettingHandler.GetBoolSetting(Settings.SwitchOffset);
        }

        private int commentOffsetField;

        private bool isAutoSwitchOffsetEnableField;

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
        public bool IsAutoSwitchOffsetEnable { get => this.isAutoSwitchOffsetEnableField; set => this.Savesetting(ref this.isAutoSwitchOffsetEnableField, value,Settings.SwitchOffset); }

    }

    [Obsolete("Desinger", true)]
    class DownloadSettingPageViewModelD
    {
        public int CommentOffsetFIeld { get; set; } = 40;

        public bool IsAutoSwitchOffsetEnable { get; set; } = true;
    }
}
