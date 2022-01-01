using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Domain.Local.Addons.API.Tab;

namespace Niconicome.Models.Local.Addon.API.Local.Tab
{
    public interface ITabsContainer
    {

        /// <summary>
        /// タブを追加する
        /// </summary>
        /// <param name="infomation"></param>
        /// <returns></returns>
        TabItem AddTab(ITabInfomation infomation);

        /// <summary>
        /// タブ追加イベント
        /// </summary>
        event EventHandler<AddTabEventArgs>? Add;

        /// <summary>
        /// タブ削除イベント
        /// </summary>
        event EventHandler<RemoveTabEventArgs>? Remove;
    }

    public class TabsContainer : ITabsContainer
    {
        #region field

        private List<TabItem> tabs = new();

        #endregion


        #region event

        public event EventHandler<RemoveTabEventArgs>? Remove;

        public event EventHandler<AddTabEventArgs>? Add;

        #endregion

        #region Method

        public TabItem AddTab(ITabInfomation infomation)
        {

            var tab = new TabItem(infomation, item =>
            {
                if (!this.tabs.Any(t => t.ID == item.ID)) return false;
                this.Remove?.Invoke(this, new RemoveTabEventArgs(item.TabType, item.ID));
                this.tabs.RemoveAll(tab => tab.ID == item.ID);
                return true;
            });

            this.tabs.Add(tab);
            this.Add?.Invoke(this, new AddTabEventArgs(infomation.TabType, tab));

            return tab;
        }

        #endregion
    }
    public class AddTabEventArgs : EventArgs
    {
        public AddTabEventArgs(TabType tabType, ITabItem tabItem) : base()
        {
            this.TabType = tabType;
            this.TabItem = tabItem;
        }

        public TabType TabType { get; init; }

        public ITabItem TabItem { get; init; }
    }

    public class RemoveTabEventArgs : EventArgs
    {
        public RemoveTabEventArgs(TabType tabType, string tabID) : base()
        {
            this.TabType = tabType;
            this.TabID = tabID;
        }

        public TabType TabType { get; init; }

        public string TabID { get; init; }
    }


}
