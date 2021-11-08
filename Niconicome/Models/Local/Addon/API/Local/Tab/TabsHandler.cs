using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Local.Addon.API.Local.Tab
{
    public interface ITabHandler
    {
        /// <summary>
        /// タブ追加イベントハンドラ
        /// </summary>
        /// <param name="handler"></param>
        void RegisterAddHandler(Action<TabItem> handler);

        /// <summary>
        /// タブ削除イベントハンドラ
        /// </summary>
        /// <param name="handler"></param>
        void RegisterRemoveHandler(Action<string> handler);
    }

    public class TabsHandler : ITabHandler
    {
        public TabsHandler(ITabsContainer container)
        {
            container.Add += (_, tab) => this.addHandler?.Invoke(tab);
            container.Remove += (_, id) => this.removeHandler?.Invoke(id);
        }

        #region field

        private Action<TabItem>? addHandler;

        private Action<string>? removeHandler;

        #endregion

        #region Method

        public void RegisterAddHandler(Action<TabItem> handler)
        {
            this.addHandler += handler;
        }

        public void RegisterRemoveHandler(Action<string> handler)
        {
            this.removeHandler += handler;
        }
        #endregion
    }
}
