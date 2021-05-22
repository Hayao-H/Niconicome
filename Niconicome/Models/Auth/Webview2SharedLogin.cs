using System;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Cookies;
using Niconicome.Models.Domain.Niconico;
using Niconicome.Models.Domain.Utils;

namespace Niconicome.Models.Auth
{
    interface IWebview2SharedLogin
    {
        bool CanLogin();
        Task<bool> TryLogin();
    }

    class Webview2SharedLogin : IWebview2SharedLogin
    {
        public Webview2SharedLogin(IWebview2LocalCookieManager webview2LocalCookieManager, ILogger logger, INicoHttp http, ICookieManager cookieManager,INiconicoContext context)
        {
            this.webview2LocalCookieManager = webview2LocalCookieManager;
            this.logger = logger;
            this.http = http;
            this.cookieManager = cookieManager;
            this.context = context;

        }

        private readonly IWebview2LocalCookieManager webview2LocalCookieManager;

        private readonly ILogger logger;

        private readonly INicoHttp http;

        private readonly ICookieManager cookieManager;

        private readonly INiconicoContext context;

        /// <summary>
        /// ログインを試行する
        /// </summary>
        /// <returns></returns>
        public async Task<bool> TryLogin()
        {
            IUserCookieInfo cookie;

            try
            {
                cookie = this.webview2LocalCookieManager.GetCookieInfo();
            }
            catch (Exception e)
            {
                this.logger.Error("Webview2からのクッキーの取得に失敗しました。", e);
                return false;
            }

            if (cookie.UserSession is null || cookie.UserSessionSecure is null || cookie.Nicosid is null) return false;

            this.cookieManager.AddCookie("user_session", cookie.UserSession);
            this.cookieManager.AddCookie("user_session_secure", cookie.UserSessionSecure);
            this.cookieManager.AddCookie("nicosid", cookie.Nicosid);

            var result = await this.CheckIfLoginSucceeded();

            if (result)
            {
                await this.context.RefreshUser();
            }

            return result;
        }

        /// <summary>
        /// ログインに成功したかどうかを確かめる
        /// </summary>
        /// <returns></returns>
        private async Task<bool> CheckIfLoginSucceeded()
        {
            var result = await this.http.GetAsync(new Uri(@"https://www.nicovideo.jp/my"));

            if (result.Headers.Contains("Location"))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// ログイン可能であるかどうかを返す
        /// </summary>
        /// <returns></returns>
        public bool CanLogin()
        {
            return this.webview2LocalCookieManager.CanLoginWithWebview2();
        }
    }
}
