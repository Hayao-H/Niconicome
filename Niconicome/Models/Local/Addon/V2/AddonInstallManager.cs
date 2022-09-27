using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.Context;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.Infomation;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Install;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Loader;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.Addon.API;
using Niconicome.Models.Local.Addon.API.Net.Http.Fetch;

namespace Niconicome.Models.Local.Addon.V2
{
    public interface IAddonInstallManager
    {
        /// <summary>
        /// アドオンの情報を取得する
        /// </summary>
        /// <param name="archivePath"></param>
        /// <returns></returns>
        IAttemptResult<IAddonInfomation> LoadInfomation(string archivePath);

        /// <summary>
        /// インストールして読み込み
        /// </summary>
        /// <param name="archivePath"></param>
        /// <returns></returns>
        IAttemptResult InstallAndLoad(string archivePath);

        /// <summary>
        /// アドオンをアップデートして実行
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="archivePath"></param>
        /// <returns></returns>
        IAttemptResult UpdateAndLoad(string ID, string archivePath);

    }


    public class AddonInstallManager : IAddonInstallManager
    {
        public AddonInstallManager(IAddonContextsContainer contextsContainer, IAddonInstaller installer, IAddonLoader addonLoader, IAddonStatusContainer statusContainer)
        {
            this._contextsContainer = contextsContainer;
            this._installer = installer;
            this._addonLoader = addonLoader;
            this._statusContainer = statusContainer;
        }

        #region field

        private readonly IAddonContextsContainer _contextsContainer;

        private readonly IAddonInstaller _installer;

        private readonly IAddonLoader _addonLoader;

        private readonly IAddonStatusContainer _statusContainer;

        #endregion

        #region Method

        public IAttemptResult<IAddonInfomation> LoadInfomation(string archivePath)
        {
            return this._installer.LoadInfomation(archivePath);
        }

        public IAttemptResult InstallAndLoad(string archivePath)
        {
            IAttemptResult<InstallInfomation> iResult = this._installer.Install(archivePath);

            if (!iResult.IsSucceeded || iResult.Data is null)
            {
                return AttemptResult.Fail(iResult.Message);
            }


            return this.LoadInternal(iResult.Data);

        }

       public IAttemptResult UpdateAndLoad(string ID, string archivePath)
        {
            IAttemptResult sResult = this._contextsContainer.ShutDown(ID);
            if (!sResult.IsSucceeded)
            {
                return AttemptResult.Fail(sResult.Message);
            }

            IAttemptResult<IAddonContext> cResult = this._contextsContainer.Get(ID);
            if (!cResult.IsSucceeded||cResult.Data is null||cResult.Data.AddonInfomation is null)
            {
                return AttemptResult.Fail(cResult.Message); 
            }

            string dirName = cResult.Data.AddonInfomation.DirectoryName;

            IAttemptResult<InstallInfomation> iResult = this._installer.InstallToSpecifiedDiectory(archivePath, Path.Combine(AppContext.BaseDirectory, FileFolder.AddonsFolder, dirName));

            if (!iResult.IsSucceeded || iResult.Data is null)
            {
                return AttemptResult.Fail(iResult.Message);
            }

            return this.LoadInternal(iResult.Data);
        }


        #endregion

        #region private

        private IAttemptResult LoadInternal(InstallInfomation iInfo)
        {
            IAttemptResult<LoadResult> lResult = this._addonLoader.LoadAddon(iInfo.ManifestPath, iInfo.DirectoryPath, infomation =>
            {
                //API
                var entryPont = DIFactory.Provider.GetRequiredService<IAPIEntryPoint>();
                entryPont.Initialize(infomation);

                //Fetch
                var fetch = DIFactory.Provider.GetRequiredService<IFetch>();
                Func<string, dynamic?, Task<Response>> fetchFunc = (url, optionObj) =>
                {
                    var option = new FetchOption()
                    {
                        method = optionObj?.method,
                        body = optionObj?.body,
                        credentials = optionObj?.credentials,
                    };

                    return fetch.FetchAsync(url, option);
                };

                return new APIObjectConatainer(entryPont, fetchFunc, fetch);
            });

            if (!lResult.IsSucceeded || lResult.Data is null)
            {
                return AttemptResult.Fail(lResult.Message);
            }

            IAddonInfomation? succeeded = lResult.Data.Succeeded.FirstOrDefault();
            FailedResult? failed = lResult.Data.Failed.FirstOrDefault();

            if (succeeded is null && failed is not null)
            {
                this._statusContainer.LoadFailedAddons.Add(failed);
                return AttemptResult.Fail(failed.Message);
            }
            else if (succeeded is not null)
            {
                this._statusContainer.LoadedAddons.Add(succeeded);
                return AttemptResult.Succeeded();
            }
            else
            {
                return AttemptResult.Fail("不明な理由によりアドオンの読み込みに失敗しました。");
            }
        }

        #endregion
    }
}
