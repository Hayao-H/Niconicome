using System;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using Niconicome.Models.Domain.Local.Cookies;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Niconico;
using Niconicome.Models.Domain.Utils;

namespace Niconicome.Models.Auth
{
    interface IWebview2SharedLogin
    {
        bool CanLogin();
        Task<bool> TryLogin();
    }

    class Webview2SharedLogin : SharedLoginBase, IWebview2SharedLogin
    {
        public Webview2SharedLogin(INiconicoContext context,IWebview2LocalCookieManager webview2LocalCookieManager) : base(context)
        {
            this._webview2LocalCookieManager = webview2LocalCookieManager;
        }

        private readonly IWebview2LocalCookieManager _webview2LocalCookieManager;


        /// <summary>
        /// ログインを試行する
        /// </summary>
        /// <returns></returns>
        public async Task<bool> TryLogin()
        {
            if (!this.CanLogin())
            {
                return false;
            }

            IUserCookieInfo cookieInfo = this._webview2LocalCookieManager.GetCookieInfo();

            if (cookieInfo.UserSession is null||cookieInfo.UserSessionSecure is null)
            {
                return false;
            }

            var result = await this.context.LoginAndSaveCookieAsync(cookieInfo.UserSession, cookieInfo.UserSessionSecure);

            return result.IsSucceeded;
        }

        /// <summary>
        /// ログイン可能であるかどうかを返す
        /// </summary>
        /// <returns></returns>
        public bool CanLogin()
        {
            return this._webview2LocalCookieManager.CanLoginWithWebview2();
        }
    }
}
