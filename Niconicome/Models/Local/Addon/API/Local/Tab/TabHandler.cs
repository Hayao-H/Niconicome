using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ClearScript;

namespace Niconicome.Models.Local.Addon.API.Local.Tab
{
    public interface ITabHandler
    {

        Task<bool> close();

        Task navigateToString(string htmlContent);

        void postMessage(string message);

        void addMessageHandler(ScriptObject handler);

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

        public void postMessage(string message)
        {
            this._tabItem.PostMessage(message);
        }

        public void addMessageHandler(ScriptObject handler)
        {
            this._tabItem.AddMessageHandler(message =>
            {
                try
                {
                    handler.Invoke(false, message);
                }
                catch { }
            });
        }

        #endregion
    }
}
