using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.External.Software.Mozilla.Firefox
{
    public interface IStoreFirefoxProfileManager : IFirefoxProfileManager
    {

    }

    public class StoreFirefoxProfileManager : FirefoxProfileManager, IStoreFirefoxProfileManager
    {
        public StoreFirefoxProfileManager(INicoDirectoryIO directoryIO,ILogger logger) : base(directoryIO, logger) { }

        public override IAttemptResult Initialize()
        {

            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            List<string> packages;

            try
            {
                packages = this._directoryIO.GetDirectorys(Path.Combine(appData, "Packages"), "Mozilla.Firefox*");
            }
            catch (Exception e)
            {
                this._logger.Error("Microsoft Store版Firefoxアプリフォルダーの取得に失敗しました。", e);
                return AttemptResult.Fail("Microsoft Store版Firefoxアプリフォルダーの取得に失敗しました。");
            }

            if (packages.Count == 0)
            {
                return AttemptResult.Fail("プロファイルの取得に失敗しました。");
            }

            string profileRoot = Path.Combine(packages[0], @"LocalCache\Roaming\Mozilla\Firefox\Profiles");

            if (this._directoryIO.Exists(profileRoot))
            {
                this.profileFolder = profileRoot;
                this.isInitialized = true;
                return AttemptResult.Succeeded();
            }
            else
            {
                return AttemptResult.Fail("プロファイルフォルダーが存在しません、");
            }
        }
    }
}
