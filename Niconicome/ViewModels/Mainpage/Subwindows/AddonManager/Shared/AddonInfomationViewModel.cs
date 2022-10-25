using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.Infomation;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Permisson;
using Const = Niconicome.Models.Const;

namespace Niconicome.ViewModels.Mainpage.Subwindows.AddonManager.Shared
{
    public class AddonInfomationViewModel
    {
        public AddonInfomationViewModel(IAddonInfomation infomation)
        {
            this.ID = infomation.ID;
            this.Name = infomation.Name;
            this.Author = infomation.Author;
            this.Description = infomation.Description;
            this.HomePage = string.IsNullOrEmpty(infomation.HomePage) ? null : infomation.HomePage;
            this.Permissions = infomation.Permissions;

            string iconPath = Path.Combine(AppContext.BaseDirectory, Const::FileFolder.AddonsFolder, infomation.DirectoryName, infomation.IconPath);
            var stream = File.ReadAllBytes(iconPath);
            var base64 = Convert.ToBase64String(stream);
            this.IconData = $"data:image/png;base64,{base64}";
            this.Version = infomation.Version.ToString();
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
        /// 作者名
        /// </summary>
        public string Author { get; init; }

        /// <summary>
        /// 詳細情報
        /// </summary>
        public string Description { get; init; }

        /// <summary>
        /// アイコンのパス
        /// </summary>
        public string IconData { get; init; }

        /// <summary>
        /// バージョン
        /// </summary>
        public string Version { get; init; }

        /// <summary>
        /// ホームページ
        /// </summary>
        public string? HomePage { get; init; }

        /// <summary>
        /// 権限の一覧
        /// </summary>
        public IReadOnlyList<Permission> Permissions { get; init; }
    }
}
