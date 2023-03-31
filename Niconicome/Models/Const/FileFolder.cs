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
        public static string DefaultDownloadDir = "downloaded";

        public static string FirefoxCookieFileName = "cookies.sqlite";

        public static string Mp4FileExt = ".mp4";

        public static string TsFileExt = ".ts";

        public static string DefaultHtmlFileExt = ".html";

        public static string DefaultJpegFileExt = ".jpg";

        public static string DefaultAddonExtension = ".zip";

        public static string UserChromePath = @"chrome\userChrome.css";

        public static string UserChromeFileName = @"userChrome.css";

        public static string AddonsFolder = @"addons";

        public static string ManifestFileName = "manifest.json";

        public static string AddonsTmpDirectory = @"tmp\addons";

        public static string LogFolderName = "log";

        public static string SettingJSONPath = @"data\settings.json";

        public static string ExportFolderPath = @"data\export";
    }
}
