using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Local.Handlers;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Niconico;
using Niconicome.Models.Domain.Niconico.UserAuth;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Auth.Cookie
{
    interface INiconicoCookieManager
    {
        /// <summary>
        /// Webviewをハンドリングする
        /// </summary>
        /// <param name="handler"></param>
        void Wire(ICoreWebview2Handler handler);

        /// <summary>
        /// WebViewのハンドリングを解除する
        /// </summary>
        void UnWire();

        /// <summary>
        /// URL遷移をハンドリングする
        /// </summary>
        Task<bool> HandleNavigate();

        /// <summary>
        /// ログイン状態を確認する
        /// </summary>
        /// <returns></returns>
        Task<bool> IsLoggedIn();

    }

    public class NiconicoCookieManager : INiconicoCookieManager
    {
        #region field

        private ICoreWebview2Handler? handler;

        private readonly INiconicoContext _context;

        private readonly IStoredCookieLogin _storedCookieLogin;

        #endregion

        public NiconicoCookieManager(INiconicoContext context,IStoredCookieLogin storedCookieLogin)
        {
            this._context = context;
            this._storedCookieLogin = storedCookieLogin;
        }

        #region Method

        public void Wire(ICoreWebview2Handler handler)
        {
            this.handler = handler;
        }

        public void UnWire()
        {
            this.handler = null;
        }

        public async Task<bool> HandleNavigate()
        {
            if (this.handler is null) return false;

            var cookies = await this.handler.GetCookiesAsync(@"https://nicovideo.jp");

            string userSession = string.Empty;
            string userSessionSecure = string.Empty;

            foreach (var cookie in cookies)
            {
                if (cookie.Name == "user_session")
                {
                    userSession = cookie.Value;
                }
                else if (cookie.Name == "user_session_secure")
                {
                    userSessionSecure = cookie.Value;
                }
            }

            if (userSession.IsNullOrEmpty() || userSessionSecure.IsNullOrEmpty()) return false;

            var result = await this._context.LoginAndSaveCookieAsync(userSession, userSessionSecure);

            return result.IsSucceeded;
        }

        public async Task<bool> IsLoggedIn()
        {
            return this._storedCookieLogin.CanLogin() && await this._storedCookieLogin.TryLogin();
        }

        #endregion
    }
}
