using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.Addons.API.Hooks;
using Niconicome.Models.Domain.Niconico.Video.Infomations;
using Niconicome.Models.Domain.Niconico.Watch.V2.Error;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Niconico.Watch.V2
{
    public interface IWatchPageInfomationHandler
    {
        /// <summary>
        /// 視聴ページの情報を取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IAttemptResult<IDomainVideoInfo>> GetVideoInfoAsync(string id);

    }

    public class WatchPageInfomationHandler : IWatchPageInfomationHandler
    {
        public WatchPageInfomationHandler(INicoHttp http,IHooksManager hooks,IErrorHandler errorHandler)
        {
            this._http = http;
            this._hooks = hooks;
            this._errorHandler = errorHandler;
        }

        #region field

        private readonly INicoHttp _http;

        private readonly IHooksManager _hooks;

        private readonly IErrorHandler _errorHandler;

        #endregion

        #region Method

        public async Task<IAttemptResult<IDomainVideoInfo>> GetVideoInfoAsync(string id)
        {
            IAttemptResult<string> getResult = await this.GetWatchPageAsync(id);
            if (!getResult.IsSucceeded || getResult.Data is null) return AttemptResult<IDomainVideoInfo>.Fail(getResult.Message);

            if (!this._hooks.IsRegistered(HookType.WatchPageParser))
            {
                this._errorHandler.HandleError(WatchPageInfomationHandlerError.PluginNotFount);
                return AttemptResult<IDomainVideoInfo>.Fail(this._errorHandler.GetMessageForResult(WatchPageInfomationHandlerError.PluginNotFount));
            }

            IAttemptResult<dynamic> parseResult;

            if (this._hooks.IsRegistered(HookType.VIdeoInfoFetcher))
            {
                parseResult = await this._hooks.GetVideoInfoAsync(id);
            } else
            {
                parseResult = this._hooks.ParseWatchPage(getResult.Data);
            }

            if (!parseResult.IsSucceeded || parseResult.Data is null) return AttemptResult<IDomainVideoInfo>.Fail(parseResult.Message);

            return AttemptResult<IDomainVideoInfo>.Succeeded(new DomainVideoInfo() { RawDmcInfo = parseResult.Data });
        }


        #endregion

        #region private

        /// <summary>
        /// 視聴ページを取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<IAttemptResult<string>> GetWatchPageAsync(string id)
        {
            this._errorHandler.HandleError(WatchPageInfomationHandlerError.RetrievingWatchPageHasStarted, id);

            var url = $"{NetConstant.NiconicoWatchUrl}{id}";
            HttpResponseMessage res = await this._http.GetAsync(new Uri(url));

            if (!res.IsSuccessStatusCode)
            {
                this._errorHandler.HandleError(WatchPageInfomationHandlerError.FailedToRetrieveWatchPage, url, (int)res.StatusCode);
                return AttemptResult<string>.Fail(this._errorHandler.GetMessageForResult(WatchPageInfomationHandlerError.FailedToRetrieveWatchPage, url, (int)res.StatusCode));
            }

            this._errorHandler.HandleError(WatchPageInfomationHandlerError.RetrievingWatchPageHasCompleted, id);
            return AttemptResult<string>.Succeeded(await res.Content.ReadAsStringAsync());
        }

        #endregion
    }
}
