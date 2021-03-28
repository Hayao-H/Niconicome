using System.IO;
using Niconicome.Models.Domain.Local.LocalFile;
using Niconicome.Models.Domain.Local.SQLite;

namespace Niconicome.Models.Domain.Local.Cookies
{
    public interface IUserCookieInfo
    {
        bool IsUserSessionExpired { get; }
        bool IsUserSessionSecureExpired { get; }
        bool IsNicosidExpired { get; }
        string? UserSession { get; }
        string? UserSessionSecure { get; }
        string? Nicosid { get; }
    }

    interface IWebview2LocalCookieManager
    {
        bool CanLoginWithWebview2();
        IUserCookieInfo GetCookieInfo();
    }

    class Webview2LocalCookieManager : IWebview2LocalCookieManager
    {
        public Webview2LocalCookieManager(ICookieJsonLoader cookieJsonLoader, ISqliteCookieLoader sqliteCookieLoader, IChromeCookieDecryptor chromeCookieDecryptor)
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
        public bool CanLoginWithWebview2()
        {
            var cookiePath = this.sqliteCookieLoader.GetCookiePath(CookieType.Webview2);
            var jsonPath = this.cookieJsonLoader.GetJsonPath(CookieType.Webview2);

            return File.Exists(cookiePath) && File.Exists(jsonPath);
        }

        /// <summary>
        /// Cookieを取得する
        /// </summary>
        /// <returns></returns>
        public IUserCookieInfo GetCookieInfo()
        {
            var cookiePath = this.sqliteCookieLoader.GetCookiePath(CookieType.Webview2);
            var jsonPath = this.cookieJsonLoader.GetJsonPath(CookieType.Webview2);

            var cookies = this.sqliteCookieLoader.GetCookies(cookiePath);
            var rawKey = this.cookieJsonLoader.GetEncryptedKey(jsonPath);
            var key = this.chromeCookieDecryptor.GetEncryptionKey(rawKey);

            if (cookies.UserSession is null || cookies.UserSessionSecure is null || cookies.Nicosid is null)
            {
                throw new IOException("Cookieの値がnullです。");
            }

            var usersessionEncrypted = this.chromeCookieDecryptor.DecryptCookie(cookies.UserSession, key);
            var usersessionSecureEncrypted = this.chromeCookieDecryptor.DecryptCookie(cookies.UserSessionSecure, key);
            var nicosidEncrypted = this.chromeCookieDecryptor.DecryptCookie(cookies.Nicosid, key);

            var info = new UserCookieInfo()
            {
                IsUserSessionExpired = cookies.IsUserSessionExpires,
                IsUserSessionSecureExpired = cookies.IsUserSessionSecureExpires,
                IsNicosidExpired = cookies.IsNnicosidExpires,
                UserSession = this.chromeCookieDecryptor.ToUtf8String(usersessionEncrypted),
                UserSessionSecure = this.chromeCookieDecryptor.ToUtf8String(usersessionSecureEncrypted),
                Nicosid = this.chromeCookieDecryptor.ToUtf8String(nicosidEncrypted)
            };

            return info;

        }
    }

    class UserCookieInfo:IUserCookieInfo
    {
        public bool IsUserSessionExpired { get; set; }

        public bool IsUserSessionSecureExpired { get; set; }

        public bool IsNicosidExpired { get; set; }

        public string? UserSession { get; set; }

        public string? UserSessionSecure { get; set; }

        public string? Nicosid { get; set; }
    }
}
