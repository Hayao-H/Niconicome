using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Cookies;
using Niconicome.Models.Domain.Local.External.Software.Mozilla.Firefox;
using Niconicome.Models.Domain.Niconico;
using Niconicome.Models.Domain.Utils;

namespace Niconicome.Models.Auth
{
    interface IFirefoxSharedLogin
    {
        bool CanLogin(string profileName);
        Task<bool> TryLogin(string profileName);
        IEnumerable<IFirefoxProfileInfo> GetFirefoxProfiles();
    }

    class FirefoxSharedLogin : SharedLoginBase, IFirefoxSharedLogin
    {
        public FirefoxSharedLogin(IFirefoxCookieManager firefoxCookieManager, ILogger logger, INicoHttp http, INiconicoContext context, ICookieManager cookieManager, IFirefoxProfileManager firefoxProfileManager) : base(http, cookieManager, context)
        {
            this.firefoxCookieManager = firefoxCookieManager;
            this.logger = logger;
            this.firefoxProfileManager = firefoxProfileManager;
        }

        #region DIされるコード
        private readonly IFirefoxCookieManager firefoxCookieManager;

        private readonly ILogger logger;

        private readonly IFirefoxProfileManager firefoxProfileManager;
        #endregion

        /// <summary>
        /// ログインを試行する
        /// </summary>
        /// <returns></returns>
        public async Task<bool> TryLogin(string profileName)
        {
            IUserCookieInfo cookie;

            try
            {
                cookie = this.firefoxCookieManager.GetCookieInfo(profileName);
            }
            catch (Exception e)
            {
                this.logger.Error("Firefoxからのクッキーの取得に失敗しました。", e);
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
        /// ログイン可能であるかどうかを返す
        /// </summary>
        /// <returns></returns>
        public bool CanLogin(string profileName)
        {
            return this.firefoxCookieManager.CanLoginWithFirefox(profileName);
        }

        /// <summary>
        /// FFのプロファイルを取得する
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IFirefoxProfileInfo> GetFirefoxProfiles()
        {
            return this.firefoxProfileManager.GetAllProfiles();
        }



    }
}
