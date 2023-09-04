using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Domain.Niconico.Video.Infomations;
using Niconicome.Models.Domain.Niconico.Watch.V2;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using API = Niconicome.Models.Domain.Niconico.Net.Json.API.Video.V3;

namespace Niconicome.Models.Domain.Niconico.Download.Comment.V2.Fetch.V3.Threadkey
{
    public interface IThreadKeyHandler
    {
        /// <summary>
        /// スレッドキーを取得
        /// </summary>
        /// <param name="videoID"></param>
        /// <returns></returns>
        Task<IAttemptResult<string>> GetThreadKeyAsync(string videoID);
    }

    public class ThreadKeyHandler : IThreadKeyHandler
    {
        public ThreadKeyHandler(IWatchPageInfomationHandler handler, IErrorHandler errorHandler)
        {
            this._handler = handler;
            this._errorHandler = errorHandler;
        }

        private readonly IWatchPageInfomationHandler _handler;

        private readonly IErrorHandler _errorHandler;

        public async Task<IAttemptResult<string>> GetThreadKeyAsync(string videoID)
        {

            IAttemptResult<IDomainVideoInfo> result = await this._handler.GetVideoInfoAsync(videoID);

            if (!result.IsSucceeded||result.Data is null)
            {
                return AttemptResult<string>.Fail(result.Message);
            }

            this._errorHandler.HandleError(ThreadKeyError.GetThreadKey, videoID, result.Data.DmcInfo.Threadkey);

            return AttemptResult<string>.Succeeded(result.Data.DmcInfo.Threadkey);
        }
    }


}
