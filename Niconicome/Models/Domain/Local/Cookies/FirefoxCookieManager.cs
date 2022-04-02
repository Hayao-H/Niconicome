using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.External.Software.Mozilla.Firefox;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Local.SQLite;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.Cookies
{
    public interface IFirefoxCookieManager
    {
        /// <summary>
        /// ログイン可能かどうかをチェック
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns></returns>
        bool CanLoginWithFirefox(string profileName);

        /// <summary>
        /// Cookieを取得
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns></returns>
        IUserCookieInfo GetCookieInfo(string profileName);

        /// <summary>
        /// 初期化
        /// </summary>
        /// <returns></returns>
        IAttemptResult Initialize();

        /// <summary>
        /// 初期化フラグ
        /// </summary>
        bool IsInitialized { get; }
    }

    public class FirefoxCookieManager : IFirefoxCookieManager
    {
        public FirefoxCookieManager(IFirefoxProfileManager profileManager, INicoFileIO fileIO, ISqliteCookieLoader cookieLoader)
        {
            this.profileManager = profileManager;
            this.fileIO = fileIO;
            this.cookieLoader = cookieLoader;
        }

        #region field

        private readonly IFirefoxProfileManager profileManager;

        private readonly INicoFileIO fileIO;

        private readonly ISqliteCookieLoader cookieLoader;

        private bool isInitialized;

        #endregion

        public bool IsInitialized => this.isInitialized;

        public bool CanLoginWithFirefox(string profileName)
        {
            var exists = this.profileManager.HasProfile(profileName);

            //そもそもプロファイルが存在しない
            if (!exists) return false;

            var profile = this.profileManager.GetProfile(profileName);

            //Cookieファイルが存在するかどうか
            return this.fileIO.Exists(profile.CookiePath);
        }

        public IUserCookieInfo GetCookieInfo(string profileName)
        {
            var profile = this.profileManager.GetProfile(profileName);
            var info = this.cookieLoader.GetCookies(profile.CookiePath, CookieType.Firefox);

            if (info.UserSession is null || info.UserSessionSecure is null) throw new IOException("Cookieの値がnullです。");

            return new UserCookieInfo()
            {
                UserSession = Encoding.UTF8.GetString(info.UserSession),
                UserSessionSecure = Encoding.UTF8.GetString(info.UserSessionSecure),
            };
        }

        public IAttemptResult Initialize()
        {
            if (this.isInitialized) return AttemptResult.Succeeded();

            IAttemptResult result = this.profileManager.Initialize();
            if (result.IsSucceeded)
            {
                this.isInitialized = true;
            }

            return result;
        }


    }
}
