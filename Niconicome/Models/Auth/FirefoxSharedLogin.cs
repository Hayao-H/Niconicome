using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Cookies;
using Niconicome.Models.Domain.Local.External.Software.Mozilla.Firefox;
using Niconicome.Models.Domain.Niconico;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Auth
{
    public interface IFirefoxSharedLogin
    {
        bool CanLogin(string profileName);
        Task<bool> TryLogin(string profileName);
        IEnumerable<IFirefoxProfileInfo> GetFirefoxProfiles();
    }

    public class FirefoxSharedLogin : SharedLoginBase, IFirefoxSharedLogin
    {
        public FirefoxSharedLogin(IFirefoxCookieManager firefoxCookieManager, ILogger logger,  INiconicoContext context, IFirefoxProfileManager firefoxProfileManager) : base( context)
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

        public async Task<bool> TryLogin(string profileName)
        {
            if (!this.firefoxCookieManager.IsInitialized)
            {
                IAttemptResult iResult = this.firefoxCookieManager.Initialize();
                if (!iResult.IsSucceeded)
                {
                    return false;
                }
            }

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

            if (cookie.UserSession is null || cookie.UserSessionSecure is null) return false;

            IAttemptResult result = await this.LoginAndSaveCookieAsync(cookie.UserSession, cookie.UserSessionSecure);

            return result.IsSucceeded;
        }

        /// <summary>
        /// ログイン可能であるかどうかを返す
        /// </summary>
        /// <returns></returns>
        public bool CanLogin(string profileName)
        {
            if (!this.firefoxCookieManager.IsInitialized)
            {
                IAttemptResult result = this.firefoxCookieManager.Initialize();
                if (!result.IsSucceeded)
                {
                    return false;
                }
            }
            return this.firefoxCookieManager.CanLoginWithFirefox(profileName);
        }

        /// <summary>
        /// FFのプロファイルを取得する
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IFirefoxProfileInfo> GetFirefoxProfiles()
        {
            if (!this.firefoxProfileManager.IsInitialized)
            {
                IAttemptResult result = this.firefoxProfileManager.Initialize();
                if (!result.IsSucceeded)
                {
                    return Enumerable.Empty<IFirefoxProfileInfo>();
                }
            }
            return this.firefoxProfileManager.GetAllProfiles();
        }



    }
}
