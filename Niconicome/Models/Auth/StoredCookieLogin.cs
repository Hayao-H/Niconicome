using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Cookies;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Niconico;
using Niconicome.Models.Domain.Niconico.UserAuth;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Auth
{
    public interface IStoredCookieLogin
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

    public class StoredCookieLogin : SharedLoginBase, IStoredCookieLogin
    {
        public StoredCookieLogin(INiconicoContext context,ICookieStore store) : base(context)
        {
            this._store = store;
        }

        private readonly ICookieStore _store;

        public bool CanLogin()
        {
            return this._store.Exists();
        }

        public async Task<bool> TryLogin()
        {
            IAttemptResult<ICookieInfo> cookieResult = this._store.GetCookieInfo();
            if (!cookieResult.IsSucceeded||cookieResult.Data is null)
            {
                return false;
            }

            ICookieInfo cookie = cookieResult.Data;

            IAttemptResult result = await this.LoginAndSaveCookieAsync(cookie.UserSession, cookie.UserSessionSecure);
            if (!result.IsSucceeded)
            {
                this._store.DeleteCookieInfo();
            }

            return result.IsSucceeded;
        }

    }
}
