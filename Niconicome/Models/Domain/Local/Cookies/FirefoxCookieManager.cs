using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.External.Software.Mozilla.Firefox;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Local.SQLite;

namespace Niconicome.Models.Domain.Local.Cookies
{
    public interface IFirefoxCookieManager
    {
        bool CanLoginWithFirefox(string profileName);
        IUserCookieInfo GetCookieInfo(string profileName);
    }

    class FirefoxCookieManager: IFirefoxCookieManager
    {
        public FirefoxCookieManager(IFirefoxProfileManager profileManager,INicoFileIO fileIO,ISqliteCookieLoader cookieLoader)
        {
            this.profileManager = profileManager;
            this.fileIO = fileIO;
            this.cookieLoader = cookieLoader;
        }

        #region DIされるクラス
        private readonly IFirefoxProfileManager profileManager;

        private readonly INicoFileIO fileIO;

        private readonly ISqliteCookieLoader cookieLoader;
        #endregion

        /// <summary>
        /// ログイン可能であるかどうかをチェックする
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns></returns>
        public bool CanLoginWithFirefox(string profileName)
        {
            var exists = this.profileManager.HasProfile(profileName);

            //そもそもプロファイルが存在しない
            if (!exists) return false;

            var profile = this.profileManager.GetProfile(profileName);

            //Cookieファイルが存在するかどうか
            return this.fileIO.Exists(profile.CookiePath);
        }

        /// <summary>
        /// Cookieを取得する
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns></returns>
        public IUserCookieInfo GetCookieInfo(string profileName)
        {
            var profile = this.profileManager.GetProfile(profileName);
            var info = this.cookieLoader.GetCookies(profile.CookiePath, CookieType.Firefox);

            if (info.UserSession is null || info.Nicosid is null || info.UserSessionSecure is null) throw new IOException("Cookieの値がnullです。");

            return new UserCookieInfo()
            {
                UserSession = Encoding.UTF8.GetString(info.UserSession),
                Nicosid = Encoding.UTF8.GetString(info.Nicosid),
                UserSessionSecure = Encoding.UTF8.GetString(info.UserSessionSecure),
            };
        }


    }
}
