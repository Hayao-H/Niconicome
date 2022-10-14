using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Update;

namespace Niconicome.ViewModels.Mainpage.Subwindows.AddonManager.Shared
{
    public class UpdateCheckInfomationViewModel : AddonInfomationViewModel
    {
        public UpdateCheckInfomationViewModel(UpdateCheckInfomation infomation) : base(infomation.Infomation)
        {
            this.NewVersion = infomation.Version.ToString();
            this.ChangeLogURL = infomation.ChangeLog;
        }

        /// <summary>
        /// 新しいバージョン
        /// </summary>
        public string NewVersion { get; init; }

        /// <summary>
        /// 更新情報のURL
        /// </summary>
        public string ChangeLogURL { get; init; }
    }
}
