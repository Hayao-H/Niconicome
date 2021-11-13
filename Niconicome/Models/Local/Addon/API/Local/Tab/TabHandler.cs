using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Local.Addon.API.Local.Tab
{
    public interface ITabHandler
    {

        Task<bool> close();

        Task navigateToString(string htmlContent);

        bool isClosed { get; }
    }

    public class TabHandler : ITabHandler
    {
        public TabHandler(ITabItem tabItem)
        {
            this._tabItem = tabItem;
        }

        #region field

        private readonly ITabItem _tabItem;

        #endregion

        #region Props

        public bool isClosed => this._tabItem.IsClosed;

        #endregion


        #region Methods

        public async Task<bool> close()
        {
            await Task.Delay(0);
            return this._tabItem.Close();
        }

        public async Task navigateToString(string htmlContent)
        {
            await Task.Delay(0);
            this._tabItem.NavigateString(htmlContent);
        }

        #endregion
    }
}
