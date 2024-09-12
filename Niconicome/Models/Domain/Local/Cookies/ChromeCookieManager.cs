using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.LocalFile;
using Niconicome.Models.Domain.Local.SQLite;

namespace Niconicome.Models.Domain.Local.Cookies
{

    public interface IChromeCookieManager
    {

        /// <summary>
        /// Cookieが存在するかどうか
        /// </summary>
        /// <returns></returns>
        bool CanLoginWithChrome();

        /// <summary>
        /// Cookieを取得する
        /// </summary>
        /// <returns></returns>
        IUserCookieInfo GetCookieInfo();
    }


    public class ChromeCookieManager : IChromeCookieManager
    {
        public ChromeCookieManager(ICookieJsonLoader cookieJsonLoader, ISqliteCookieLoader sqliteCookieLoader, IChromeCookieDecryptor chromeCookieDecryptor)
        {
            this.cookieJsonLoader = cookieJsonLoader;
            this.sqliteCookieLoader = sqliteCookieLoader;
            this.chromeCookieDecryptor = chromeCookieDecryptor;
        }

        private readonly ICookieJsonLoader cookieJsonLoader;

        private readonly ISqliteCookieLoader sqliteCookieLoader;

        private readonly IChromeCookieDecryptor chromeCookieDecryptor;

        /// <summary>
        /// Webview2のcookieを共有可能であるかどうか
        /// </summary>
        /// <returns></returns>
        public bool CanLoginWithChrome()
        {
            var cookiePath = this.sqliteCookieLoader.GetCookiePath(CookieType.Chrome);
            var jsonPath = this.cookieJsonLoader.GetJsonPath(CookieType.Chrome);

            return File.Exists(cookiePath) && File.Exists(jsonPath);
        }

        /// <summary>
        /// Cookieを取得する
        /// </summary>
        /// <returns></returns>
        public IUserCookieInfo GetCookieInfo()
        {
            var cookiePath = this.sqliteCookieLoader.GetCookiePath(CookieType.Chrome);
            var jsonPath = this.cookieJsonLoader.GetJsonPath(CookieType.Chrome);

            var cookies = this.sqliteCookieLoader.GetCookies(cookiePath);
            var rawKey = this.cookieJsonLoader.GetEncryptedKey(jsonPath);
            var key = this.chromeCookieDecryptor.GetEncryptionKey(rawKey);

            if (cookies.UserSession is null || cookies.UserSessionSecure is null)
            {
                throw new IOException("Cookieの値がnullです。");
            }

            var usersessionEncrypted = this.chromeCookieDecryptor.DecryptCookie(cookies.UserSession, key);
            var usersessionSecureEncrypted = this.chromeCookieDecryptor.DecryptCookie(cookies.UserSessionSecure, key);

            var info = new UserCookieInfo()
            {
                IsUserSessionExpired = cookies.IsUserSessionExpires,
                IsUserSessionSecureExpired = cookies.IsUserSessionSecureExpires,
                IsNicosidExpired = cookies.IsNnicosidExpires,
                UserSession = this.chromeCookieDecryptor.ToUtf8String(usersessionEncrypted),
                UserSessionSecure = this.chromeCookieDecryptor.ToUtf8String(usersessionSecureEncrypted),
            };

            return info;

        }
    }
}
