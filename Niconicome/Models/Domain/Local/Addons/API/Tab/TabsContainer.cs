using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions.System.List;

namespace Niconicome.Models.Domain.Local.Addons.API.Tab
{
    internal interface ITabsContainer
    {
        /// <summary>
        /// タブの一覧
        /// </summary>
        ObservableCollection<TabItem> Tabs { get; init; }

        /// <summary>
        /// タブを追加する
        /// </summary>
        /// <param name="infomation"></param>
        /// <returns></returns>
        TabItem AddTab(ITabInfomation infomation);
    }

    internal class TabsContainer : ITabsContainer
    {
        public ObservableCollection<TabItem> Tabs { get; init; } = new();

        #region Method

        public TabItem AddTab(ITabInfomation infomation)
        {
            string id = Guid.NewGuid().ToString("D");

            var tab = new TabItem(infomation, () =>
            {
                if (!this.Tabs.Any(t => t.ID == id)) return false;
                this.Tabs.RemoveAll(tab => tab.ID == id);
                return true;
            }, id);

            this.Tabs.Add(tab);

            return tab;
        }

        #endregion
    }
}
