using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Local.Addon.API.Local.Tab
{
    public interface ITabsContainerHandler
    {
        /// <summary>
        /// タブ追加イベントハンドラ
        /// </summary>
        /// <param name="handler"></param>
        void RegisterAddHandler(Action<AddTabEventArgs> handler);

        /// <summary>
        /// タブ削除イベントハンドラ
        /// </summary>
        /// <param name="handler"></param>
        void RegisterRemoveHandler(Action<RemoveTabEventArgs> handler);
    }

    public class TabsContainerHandler : ITabsContainerHandler
    {
        public TabsContainerHandler(ITabsContainer container)
        {
            container.Add += (_, e) => this.addHandler?.Invoke(e);
            container.Remove += (_, e) => this.removeHandler?.Invoke(e);
        }

        #region field

        private Action<AddTabEventArgs>? addHandler;

        private Action<RemoveTabEventArgs>? removeHandler;

        #endregion

        #region Method

        public void RegisterAddHandler(Action<AddTabEventArgs> handler)
        {
            this.addHandler += handler;
        }

        public void RegisterRemoveHandler(Action<RemoveTabEventArgs> handler)
        {
            this.removeHandler += handler;
        }
        #endregion
    }
}
