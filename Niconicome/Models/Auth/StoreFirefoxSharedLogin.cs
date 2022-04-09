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
    public interface IStoreFirefoxSharedLogin : IFirefoxSharedLogin { }

    public class StoreFirefoxSharedLogin : FirefoxSharedLogin, IStoreFirefoxSharedLogin
    {
        public StoreFirefoxSharedLogin(IStoreFirefoxCookieManager firefoxCookie, ILogger logger, INicoHttp http, INiconicoContext context, ICookieManager cookieManager, IStoreFirefoxProfileManager profileManager) : base(firefoxCookie, logger, http, context, cookieManager, profileManager) { }
    }
}
