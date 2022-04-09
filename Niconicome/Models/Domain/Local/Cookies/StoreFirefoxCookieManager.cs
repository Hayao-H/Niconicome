using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.External.Software.Mozilla.Firefox;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Local.SQLite;

namespace Niconicome.Models.Domain.Local.Cookies
{
    public interface IStoreFirefoxCookieManager : IFirefoxCookieManager { }

    public class StoreFirefoxCookieManager : FirefoxCookieManager, IStoreFirefoxCookieManager
    {
        public StoreFirefoxCookieManager(IStoreFirefoxProfileManager profileManager, INicoFileIO fileIO, ISqliteCookieLoader loader) : base(profileManager, fileIO, loader) { }
    }
}
