using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.Addons.API.Tab;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Permisson;
using Niconicome.Models.Domain.Utils;

namespace Niconicome.Models.Local.Addon.API.Local.Tab
{
    public interface ITabsManager : IAPIBase
    {

        Task<ITabHandler> add(string title, string position = LocalConstant.TabPositionBottom);

        void RegisterHandler(Action<ITabItem> addHandler, Action<ITabItem> closeHandler);

    }


    public class TabsManager : APIBase, ITabsManager
    {
        public TabsManager(ITabsContainer container)
        {
            this._container = container;
        }

        #region field

        private readonly ITabsContainer _container;

        private event Action<ITabItem>? _addHandler;

        private event Action<ITabItem>? _closeHandler;

        #endregion

        #region Method

        public async Task<ITabHandler> add(string title, string position = LocalConstant.TabPositionBottom)
        {
            if (this._addonInfomation is null || this._closeHandler is null || this._addHandler is null) throw new InvalidOperationException("Not Initialized!");

            var tabInfo = DIFactory.Provider.GetRequiredService<ITabInfomation>();
            TabType type = position switch
            {
                LocalConstant.TabPositionTop => TabType.Top,
                _ => TabType.Bottom
            };

            tabInfo.Initialize(this._addonInfomation, title, type);

            ITabItem item = this._container.AddTab(tabInfo);
            item.AddCloseHandler(this._closeHandler!);
            var handler = new TabHandler(item);

            await item.WaitUntilInitialize();

            if (this._addonInfomation.HasPermission(Permissions.Resource))
            {
                var path = Path.Combine(AppContext.BaseDirectory, FileFolder.AddonsFolder, this._addonInfomation.DirectoryName, AddonConstant.ResourceDirectoryName);
                if (Directory.Exists(path))
                {
                    item.SetVirtualHostName(AddonConstant.ResourceHost, path);
                }
            }

            this.OnAdd(item);

            return handler;
        }

        public void RegisterHandler(Action<ITabItem> addHandler, Action<ITabItem> closeHandler)
        {
            this._addHandler += addHandler;
            this._closeHandler += closeHandler;
        }


        #endregion

        #region private

        /// <summary>
        /// 追加イベントを発火
        /// </summary>
        private void OnAdd(ITabItem item)
        {
            try
            {
                this._addHandler?.Invoke(item);
            }
            catch { }
        }

        #endregion
    }
}
