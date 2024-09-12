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
        void RequestBlazorToNavigate(string url);

        /// <summary>
        /// NavigationManagerを登録
        /// </summary>
        /// <param name="window"></param>
        /// <param name="navigationManager"></param>
        void RegisterNavigationManager(NavigationManager navigationManager);

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

        private NavigationManager? _navigation;

        #endregion

        #region Method

        public void RequestBlazorToNavigate(string url)
        {
            this._navigation?.NavigateTo(url);
            this.CurrentRequestedPage = url;
        }


        public void RegisterNavigationManager(NavigationManager navigation)
        {
            this._navigation = navigation;
        }


        public void ReloadRequested(string currentURL)
        {
            this.CurrentRequestedPage = currentURL;
        }


        #endregion

        #region Props

        public string CurrentRequestedPage { get; private set; } = "/videos";

        #endregion
    }

    public enum BlazorWindows
    {
        MainPage,
        Addon,
        Settings,
        DLTask,
    }
}
