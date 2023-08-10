using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Net.Json;
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
        public ThreadKeyHandler(INicoHttp http, IErrorHandler errorHandler)
        {
            this._http = http;
            this._errorHandler = errorHandler;
        }

        private readonly INicoHttp _http;

        private readonly IErrorHandler _errorHandler;

        public async Task<IAttemptResult<string>> GetThreadKeyAsync(string videoID)
        {
            var id = this.GetTrackID();

            HttpResponseMessage res = await this._http.GetAsync(new Uri($"https://www.nicovideo.jp/api/watch/v3/{videoID}?actionTrackId={id}"));

            if (!res.IsSuccessStatusCode)
            {
                this._errorHandler.HandleError(ThreadKeyError.FailedToGetData, videoID, (int)res.StatusCode);
                return AttemptResult<string>.Fail(this._errorHandler.GetMessageForResult(ThreadKeyError.FailedToGetData, videoID, (int)res.StatusCode));
            }

            API::WatchAPI data;

            try
            {
                var content = await res.Content.ReadAsStringAsync();
                data = JsonParser.DeSerialize<API::WatchAPI>(content);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(ThreadKeyError.FailedToLoadData, ex.Message);
                return AttemptResult<string>.Fail(this._errorHandler.GetMessageForResult(ThreadKeyError.FailedToLoadData, ex.Message));
            }

            return AttemptResult<string>.Succeeded(data.Data.Comment.NvComment.ThreadKey);
        }

        #region private

        private string GetTrackID()
        {
            var source = new List<string>();
            source.AddRange(Enumerable.Range('a', 26).Select(x => ((char)x).ToString()));
            source.AddRange(Enumerable.Range('A', 26).Select(x => ((char)x).ToString()));
            source.AddRange(Enumerable.Range(0, 10).Select(x => x.ToString()));

            var ramd = new Random();
            var id = new StringBuilder();

            foreach (var _ in Enumerable.Range(0, 10))
            {
                id.Append(source[ramd.Next(26 + 26 + 10 - 1)]);
            }

            id.Append("_");

            id.Append(ramd.NextInt64((long)Math.Pow(10, 12), (long)Math.Pow(10, 13)).ToString());

            return id.ToString();
        }

        #endregion

    }


}
