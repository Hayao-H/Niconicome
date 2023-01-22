using System;
using System.Net.Http;
using System.Threading.Tasks;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Niconico.Remote.V2.Error;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Niconico.Remote.V2.Series
{
    public interface ISeriesHandler
    {
        /// <summary>
        /// シリーズを取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IAttemptResult<RemotePlaylistInfo>> GetSeries(string id);
    }

    public class SeriesHandler : ISeriesHandler
    {
        public SeriesHandler(INicoHttp http, IErrorHandler errorHandler, ISeriesPageHtmlParser seriesPageHtmlParser)
        {
            this._http = http;
            this._seriesPageHtmlParser = seriesPageHtmlParser;
            this._errorHandler = errorHandler;
        }

        #region field

        private readonly INicoHttp _http;

        private readonly ISeriesPageHtmlParser _seriesPageHtmlParser;

        private readonly IErrorHandler _errorHandler;

        #endregion

        #region Method

        public async Task<IAttemptResult<RemotePlaylistInfo>> GetSeries(string id)
        {

            this._errorHandler.HandleError(SeriesError.RetrievingHasStarted, id);
            IAttemptResult<string> netresult = await this.GetSeriesPage(id);

            if (!netresult.IsSucceeded || netresult.Data is null)
            {
                return AttemptResult<RemotePlaylistInfo>.Fail(netresult.Message);
            }

            IAttemptResult<RemotePlaylistInfo> infoResult;

            try
            {
                infoResult = this._seriesPageHtmlParser.GetSeriesInfo(netresult.Data);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(SeriesError.FailedToParseDocumentWithException, ex, id);
                return AttemptResult<RemotePlaylistInfo>.Fail(this._errorHandler.GetMessageForResult(SeriesError.FailedToParseDocumentWithException, ex, id));
            }

            this._errorHandler.HandleError(SeriesError.RetrievingHasCompleted, id);
            return infoResult;
        }

        #endregion

        #region private

        /// <summary>
        /// シリーズページを取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<IAttemptResult<string>> GetSeriesPage(string id)
        {
            var url = new Uri(APIConstant.SeriesUrl + id);

            HttpResponseMessage response = await this._http.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                this._errorHandler.HandleError(SeriesError.FailedToParseDocument, url.AbsoluteUri, (int)response.StatusCode);
                return AttemptResult<string>.Fail(this._errorHandler.GetMessageForResult(SeriesError.FailedToParseDocument, url.AbsoluteUri, (int)response.StatusCode));
            }

            string content = await response.Content.ReadAsStringAsync();

            return AttemptResult<string>.Succeeded(content);
        }

        #endregion
    }
}
