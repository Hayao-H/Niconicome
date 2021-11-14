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
        event EventHandler<TabItem>? Add;

        /// <summary>
        /// タブ削除イベント
        /// </summary>
        event EventHandler<string>? Remove;
    }

    public class TabsContainer : ITabsContainer
    {
        #region field

        private List<TabItem> tabs = new();

        #endregion


        #region event

        public event EventHandler<string>? Remove;

        public event EventHandler<TabItem>? Add;

        #endregion

        #region Method

        public TabItem AddTab(ITabInfomation infomation)
        {

            var tab = new TabItem(infomation, item =>
            {
                if (!this.tabs.Any(t => t.ID == item.ID)) return false;
                this.Remove?.Invoke(this, item.ID);
                this.tabs.RemoveAll(tab => tab.ID == item.ID);
                return true;
            });

            this.tabs.Add(tab);
            this.Add?.Invoke(this, tab);

            return tab;
        }

        #endregion
    }
}
