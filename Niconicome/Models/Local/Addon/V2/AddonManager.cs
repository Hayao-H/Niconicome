using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.Context;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.Infomation;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Loader;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Update;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.Addon.API;
using Niconicome.Models.Local.Addon.API.Net.Http.Fetch;
using Const = Niconicome.Models.Const;

namespace Niconicome.Models.Local.Addon.V2
{
    public interface IAddonManager
    {
        /// <summary>
        /// アドオンをすべてロードして実行
        /// </summary>
        /// <returns></returns>
        IAttemptResult InitializeAddons();

        /// <summary>
        /// アドオンを停止する
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        IAttemptResult ShutDown(string ID);

        /// <summary>
        /// アドオンをリロード
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        IAttemptResult Reload(string ID);

        /// <summary>
        /// アップデートを確認
        /// </summary>
        /// <returns></returns>
        Task CheckForUpdates();
    }

    public class AddonManager : IAddonManager
    {

        public AddonManager(IAddonLoader loader, IAddonContextsContainer contextsContainer, IAddonStatusContainer statusContainer, IAddonUpdateChecker updateChecker)
        {
            this._loader = loader;
            this._contextsContainer = contextsContainer;
            this._statusContainer = statusContainer;
            this._updateChecker = updateChecker;
        }

        #region field

        private readonly IAddonLoader _loader;

        private readonly IAddonContextsContainer _contextsContainer;

        private readonly IAddonStatusContainer _statusContainer;

        private readonly IAddonUpdateChecker _updateChecker;

        #endregion

        #region Method

        public IAttemptResult InitializeAddons()
        {
            IAttemptResult<LoadResult> loadResult = this._loader.LoadAddons(this.FactoryFunction);

            if (!loadResult.IsSucceeded || loadResult.Data is null)
            {
                return AttemptResult.Fail(loadResult.Message);
            }

            //それぞれリストに追加
            this._statusContainer.LoadedAddons.Clear();
            this._statusContainer.LoadFailedAddons.Clear();

            this._statusContainer.LoadedAddons.AddRange(loadResult.Data.Succeeded);
            this._statusContainer.LoadFailedAddons.AddRange(loadResult.Data.Failed);

            return AttemptResult.Succeeded();

        }

        public IAttemptResult ShutDown(string ID)
        {
            IAttemptResult result = this._contextsContainer.ShutDown(ID);

            this._statusContainer.Remove(ID);

            return result;
        }

        public async Task CheckForUpdates()
        {
            this._statusContainer.ToBeUpdatedAddons.Clear();

            foreach (var info in this._statusContainer.LoadedAddons)
            {
                if (!info.AutoUpdate) continue;

                //アップデートを確認
                IAttemptResult<bool> result = await this._updateChecker.CheckForUpdate(info);
                if (!result.IsSucceeded) continue;

                if (result.Data)
                {
                    this._statusContainer.ToBeUpdatedAddons.Add(info);
                }
            }
        }

        public IAttemptResult Reload(string ID)
        {

            IAttemptResult<IAddonContext> stdResult = this._contextsContainer.ShutDown(ID);
            if (!stdResult.IsSucceeded || stdResult.Data is null)
            {
                return stdResult;
            }

            IAddonInfomation infomation = stdResult.Data.AddonInfomation!;
            this._statusContainer.Remove(ID);

            string dirName = infomation.DirectoryName;
            string manifestPath = Path.Combine(AppContext.BaseDirectory, Const::FileFolder.AddonsFolder, dirName, Const::FileFolder.ManifestFileName);

            IAttemptResult<LoadResult> loadResult = this._loader.LoadAddon(manifestPath, dirName, this.FactoryFunction);
            if (!loadResult.IsSucceeded || loadResult.Data is null)
            {
                return AttemptResult.Fail(loadResult.Message);
            }

            if (loadResult.Data.Succeeded.Count == 1)
            {
                this._statusContainer.LoadedAddons.Add(loadResult.Data.Succeeded[0]);
            }
            else if (loadResult.Data.Failed.Count == 1)
            {
                this._statusContainer.LoadFailedAddons.Add(loadResult.Data.Failed[0]);
            }

            return loadResult.Data.Succeeded.Count == 1 ? AttemptResult.Succeeded() : AttemptResult.Fail();
        }



        #endregion

        #region private

        private APIObjectConatainer FactoryFunction(IAddonInfomation infomation)
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
        }

        #endregion
    }

}
