using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS = Niconicome.Workspaces;
using Local = Niconicome.Models.Local;
using System.ComponentModel;

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
        }

        private int commentOffsetField;

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

    }

    [Obsolete("Desinger", true)]
    class DownloadSettingPageViewModelD
    {
        public int CommentOffsetFIeld { get; set; } = 40;
    }
}
