using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;
using Niconicome.Models.Local.Addon.API.Local.Tab;

namespace Niconicome.ViewModels.Mainpage.Tabs
{
    public class TabViewModel : TabViewModelBase
    {
        public TabViewModel(ITabItem item) : base(item.Title, item.ID)
        {
            this._tabItem = item;
        }

        #region field

        ITabItem _tabItem;

        #endregion

        #region Method

        public void Initialize(CoreWebView2 wv2)
        {
            this._tabItem.Initialize(wv2);
        }

        #endregion
    }
}
