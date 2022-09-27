using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.Context;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.Infomation;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Loader;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.Addon.API;
using Niconicome.Models.Local.Addon.API.Net.Http.Fetch;

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
        /// <param name="id"></param>
        /// <returns></returns>
        IAttemptResult ShutDown(string id);
    }

    public class AddonManager : IAddonManager
    {

        public AddonManager(IAddonLoader loader, IAddonContextsContainer contextsContainer, IAddonStatusContainer statusContainer)
        {
            this._loader = loader;
            this._contextsContainer = contextsContainer;
            this._statusContainer = statusContainer;
        }

        #region field

        private readonly IAddonLoader _loader;

        private readonly IAddonContextsContainer _contextsContainer;

        private readonly IAddonStatusContainer _statusContainer;

        #endregion

        #region Method

        public IAttemptResult InitializeAddons()
        {
            IAttemptResult<LoadResult> loadResult = this._loader.LoadAddons(infomation =>
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

            return result;
        }

        #endregion
        #region private
        #endregion
    }

}
