using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Cookies;
using Niconicome.Models.Domain.Niconico;
using Niconicome.Models.Domain.Utils.Error;
using Err = Niconicome.Models.Auth.Error.ChromeSharedLoginError;

namespace Niconicome.Models.Auth
{
    public interface IChromeSharedLogin
    {
        /// <summary>
        /// ログインの可否
        /// </summary>
        /// <returns></returns>
        bool CanLogin();

        /// <summary>
        /// ログインを試行する
        /// </summary>
        /// <returns></returns>
        Task<bool> TryLogin();
    }

    internal class ChromeSharedLogin : SharedLoginBase, IChromeSharedLogin
    {
        public ChromeSharedLogin(IChromeCookieManager chromeCookieManager, IErrorHandler errorHandler, INicoHttp http, ICookieManager cookieManager, INiconicoContext context) : base(http, cookieManager, context)
        {
            this._chcomrCookieManager = chromeCookieManager;
            this._errorHandler = errorHandler;

        }

        private readonly IChromeCookieManager _chcomrCookieManager;

        private readonly IErrorHandler _errorHandler;


        /// <summary>
        /// ログインを試行する
        /// </summary>
        /// <returns></returns>
        public async Task<bool> TryLogin()
        {
            IUserCookieInfo cookie;

            try
            {
                cookie = this._chcomrCookieManager.GetCookieInfo();
            }
            catch (Exception e)
            {
                this._errorHandler.HandleError(Err.FailedToGetCookie, e);
                return false;
            }

            if (cookie.UserSession is null || cookie.UserSessionSecure is null) return false;

            this.cookieManager.AddCookie("user_session", cookie.UserSession);
            this.cookieManager.AddCookie("user_session_secure", cookie.UserSessionSecure);

            var result = await this.CheckIfLoginSucceeded();

            if (result)
            {
                await this.context.RefreshUser();
            }

            return result;
        }

        /// <summary>
        /// ログイン可能であるかどうかを返す
        /// </summary>
        /// <returns></returns>
        public bool CanLogin()
        {
            return this._chcomrCookieManager.CanLoginWithChrome();
        }
    }
}
