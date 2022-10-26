using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.Context;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.Infomation;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Install;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Loader;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Uninstall;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Update;
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
        /// <param name="deleteArchiveFile"></param>
        /// <returns></returns>
        IAttemptResult UpdateAndLoad(string ID, string archivePath, bool deleteArchiveFile = false);

        /// <summary>
        /// アドオンをアンインストールする
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        IAttemptResult Uninstall(string ID);

        /// <summary>
        /// アップデートをダウンロードして情報を読み込む
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        Task<IAttemptResult<UpdateInfomation>> DownloadUpdate(string ID);

        /// <summary>
        /// インストール状況を確認
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        bool IsInstalled(string identifier);

    }


    public class AddonInstallManager : IAddonInstallManager
    {
        public AddonInstallManager(IAddonContextsContainer contextsContainer, IAddonInstaller installer, IAddonLoader addonLoader, IAddonStatusContainer statusContainer, IAddonUpdator updator,IAddonUninstaller uninstaller)
        {
            this._contextsContainer = contextsContainer;
            this._installer = installer;
            this._addonLoader = addonLoader;
            this._statusContainer = statusContainer;
            this._updator = updator;
            this._uninstaller = uninstaller;
        }

        #region field

        private readonly IAddonContextsContainer _contextsContainer;

        private readonly IAddonInstaller _installer;

        private readonly IAddonLoader _addonLoader;

        private readonly IAddonStatusContainer _statusContainer;

        private readonly IAddonUpdator _updator;

        private readonly IAddonUninstaller _uninstaller;

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

        public IAttemptResult UpdateAndLoad(string ID, string archivePath, bool deleteArchiveFile = false)
        {

            //シャットダウンまで請け負う
            IAttemptResult<IAddonContext> sResult = this._contextsContainer.ShutDown(ID);
            if (!sResult.IsSucceeded || sResult.Data is null)
            {
                return AttemptResult.Fail(sResult.Message);
            }
            this._statusContainer.Remove(ID);

            string dirName = sResult.Data.AddonInfomation!.DirectoryName;

            IAttemptResult<InstallInfomation> iResult = this._installer.InstallToSpecifiedDiectory(archivePath, Path.Combine(AppContext.BaseDirectory, FileFolder.AddonsFolder, dirName), deleteArchiveFile);

            if (!iResult.IsSucceeded || iResult.Data is null)
            {
                return AttemptResult.Fail(iResult.Message);
            }

            return this.LoadInternal(iResult.Data);
        }

        public IAttemptResult Uninstall(string ID)
        {
            this._statusContainer.Remove(ID);
            return this._uninstaller.Uninstall(ID);
        }


        public async Task<IAttemptResult<UpdateInfomation>> DownloadUpdate(string ID)
        {
            IAttemptResult<IAddonContext> ctxResult = this._contextsContainer.Get(ID);
            if (!ctxResult.IsSucceeded || ctxResult.Data is null)
            {
                return AttemptResult<UpdateInfomation>.Fail(ctxResult.Message);
            }

            IAttemptResult<UpdateInfomation> uResult = await this._updator.DownloadAndLoadUpdate(ctxResult.Data.AddonInfomation!);
            return uResult;
        }

        public bool IsInstalled(string identifier)
        {
            return this._statusContainer.LoadedAddons.Any(i => i.Identifier == identifier);
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
                this._statusContainer.Add(failed, ListType.Failed);
                return AttemptResult.Fail(failed.Message);
            }
            else if (succeeded is not null)
            {
                this._statusContainer.Add(succeeded,ListType.Loaded);
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
