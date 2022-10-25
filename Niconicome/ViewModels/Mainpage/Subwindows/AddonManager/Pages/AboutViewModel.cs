using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Text;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.Infomation;
using Niconicome.ViewModels.Mainpage.Subwindows.AddonManager.Shared;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage.Subwindows.AddonManager.Pages
{
    public class AboutViewModel
    {
        #region Props

        /// <summary>
        /// アドオン情報
        /// </summary>
        public AddonInfomationViewModel? Infomation { get; private set; }

        #endregion

        #region Method

        /// <summary>
        /// ページを初期化する
        /// </summary>
        /// <param name="ID"></param>
        public void Initialize(string ID)
        {
            IAddonInfomation? info = WS::AddonPage.AddonStatusContainer.LoadedAddons.FirstOrDefault(i => i.ID == ID);
            if (info is null) return;
            this.Infomation = new AddonInfomationViewModel(info);
        }

        #endregion
    }
}
