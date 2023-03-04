using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Niconicome.Models.Local.State
{
    public interface IBlazorPageManager
    {
        /// <summary>
        /// Blazorにルーティングを要求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="window"></param>
        void RequestBlazorToNavigate(string url, BlazorWindows window);

        /// <summary>
        /// NavigationManagerを登録
        /// </summary>
        /// <param name="window"></param>
        /// <param name="navigationManager"></param>
        void RegisterNavigationManager(BlazorWindows window, NavigationManager navigationManager);

        /// <summary>
        /// 遷移すべきページを取得
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        string GetPageToNavigate(BlazorWindows window);

        /// <summary>
        /// リロードが要求された場合
        /// </summary>
        /// <param name="currentURL"></param>
        void ReloadRequested(string currentURL);

        /// <summary>
        /// 直近にリクエストされたページ
        /// </summary>
        string CurrentRequestedPage { get; }
    }

    public class BlazorPageManager : IBlazorPageManager
    {
        #region field

        private readonly Dictionary<BlazorWindows, string> _pages = new();

        private readonly Dictionary<BlazorWindows, NavigationManager> _navigations = new();

        #endregion

        #region Method

        public void RequestBlazorToNavigate(string url, BlazorWindows window)
        {
            if (this._pages.ContainsKey(window))
            {
                this._pages[window] = url;
            }
            else
            {
                this._pages.Add(window, url);
            }

            if (this._navigations.ContainsKey(window))
            {
                this._navigations[window].NavigateTo(url);
            }

            this.CurrentRequestedPage = url;
        }

        public string GetPageToNavigate(BlazorWindows window)
        {
            if (this._pages.TryGetValue(window, out string? page))
            {
                return page;
            }
            else
            {
                this._pages.Add(window, "/");
                return "/";
            }
        }

        public void RegisterNavigationManager(BlazorWindows window, NavigationManager navigation)
        {
            if (this._navigations.ContainsKey(window))
            {
                this._navigations[window] = navigation;
                return;
            }

            this._navigations.Add(window, navigation);
        }

        public void ReloadRequested(string currentURL)
        {
            this.CurrentRequestedPage = currentURL;
        }


        #endregion

        #region Props

        public string CurrentRequestedPage { get; private set; } = "/";

        #endregion
    }

    public enum BlazorWindows
    {
        MainPage,
        Addon,
    }
}
