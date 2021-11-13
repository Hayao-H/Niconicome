using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Models.Domain.Local.Addons.API.Tab;
using Niconicome.Models.Domain.Utils;

namespace Niconicome.Models.Local.Addon.API.Local.Tab
{
    public interface ITabsManager : IAPIBase
    {

        Task<ITabHandler> add(string title);

    }


    public class TabsManager : APIBase, ITabsManager
    {
        public TabsManager(ITabsContainer container)
        {
            this._container = container;
        }

        #region field

        private readonly ITabsContainer _container;

        #endregion

        #region Method

        public async Task<ITabHandler> add(string title)
        {
            if (this._addonInfomation is null) throw new InvalidOperationException("Not Initialized!");

            var tabInfo = DIFactory.Provider.GetRequiredService<ITabInfomation>();
            tabInfo.Initialize(this._addonInfomation, title);

            ITabItem item = this._container.AddTab(tabInfo);
            var handler = new TabHandler(item);

            await item.WaitUntilInitialize();

            return handler;
        }

        #endregion
    }
}
