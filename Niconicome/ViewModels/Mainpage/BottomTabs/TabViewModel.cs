using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Local.Addon.API.Local.Tab;
using Reactive.Bindings;

namespace Niconicome.ViewModels.Mainpage.BottomTabs
{
    public class TabViewModel
    {
        public TabViewModel(ITabItem tabItem)
        {
            this._tabItem = tabItem;
        }

        public string Title => this._tabItem.Title;

        #region field

        private readonly ITabItem _tabItem;

        #endregion
    }
}
