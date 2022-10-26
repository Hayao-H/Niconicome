using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.AI.MachineLearning;

namespace Niconicome.Models.Const
{
    public static class FileFolder
    {
       public const string DefaultDownloadDir = "downloaded";

        public const string FirefoxCookieFileName= "cookies.sqlite";

        public const string Mp4FileExt = ".mp4";

        public const string TsFileExt = ".ts";

        public const string DefaultHtmlFileExt = ".html";

        public const string DefaultJpegFileExt = ".jpg";

        public const string DefaultAddonExtension = ".zip";

        public const string UserChromePath = @"chrome\userChrome.json";

        public const string UserChromeFileName = @"userChrome.json";

        public const string AddonsFolder = @"addons";

        public const string ManifestFileName = "manifest.json";

        public const string AddonsTmpDirectory = @"tmp\addons";
    }
}
