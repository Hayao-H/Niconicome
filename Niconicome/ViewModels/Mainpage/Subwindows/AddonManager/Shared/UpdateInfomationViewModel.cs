using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Permisson;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Update;

namespace Niconicome.ViewModels.Mainpage.Subwindows.AddonManager.Shared
{
    public class UpdateInfomationViewModel
    {
        public UpdateInfomationViewModel(UpdateInfomation infomation)
        {
            this.ID = infomation.AddonInfomation.ID;
            this.Name = infomation.AddonInfomation.Name;
            this.NewPermissions = infomation.NewPermissions.AsReadOnly();
            this.ArchivePath = infomation.archivePath;
        }

        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; init; }

        /// <summary>
        /// アドオン名
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// インストールファイルのパス
        /// </summary>
        public string ArchivePath { get; init; }

        /// <summary>
        /// 新しい権限の一覧
        /// </summary>
        public IReadOnlyList<Permission> NewPermissions { get; init; }
    }
}
