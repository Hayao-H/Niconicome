using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Domain.Niconico.Remote.V2.Error;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Domain.Utils.StringHandler;
using Niconicome.Models.Helper.Result;
using Api = Niconicome.Models.Domain.Niconico.Net.Json.API.Search;

namespace Niconicome.Models.Domain.Niconico.Remote.V2.Search
{
    public interface ISearch
    {
        /// <summary>
        /// 検索する
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IAttemptResult<RemotePlaylistInfo>> SearchAsync(SearchQuery query);
    }

    /// <summary>
    /// 検索API
    /// </summary>
    class Search : ISearch
    {
        public Search(ISearchUrlConstructor urlConstructor, INicoHttp http,IErrorHandler errorHandler,IStringHandler stringHandler)
        {
            this._urlConstructor = urlConstructor;
            this._http = http;
            this._errorHandler = errorHandler;
            this._stringHandler = stringHandler;
        }

        #region field

        private readonly ISearchUrlConstructor _urlConstructor;

        private readonly INicoHttp _http;

        private readonly IErrorHandler _errorHandler;

        private readonly IStringHandler _stringHandler;

        #endregion

        #region Method

        public async Task<IAttemptResult<RemotePlaylistInfo>> SearchAsync(SearchQuery query)
        {

            string url = this._urlConstructor.GetUrl(query);

            IAttemptResult<Api::Response> searchResult = await this.GetResponseAsync(url);
            if (!searchResult.IsSucceeded || searchResult.Data is null) return AttemptResult<RemotePlaylistInfo>.Fail(searchResult.Message);

            List<VideoInfo> videos = searchResult.Data.Data.Select(v => new VideoInfo()
            {
                NiconicoID = v.ContentId,
                UploadedDT = v.StartTime,
                Title = v.Title,
                ViewCount = v.ViewCounter,
                CommentCount = v.CommentCounter,
                MylistCount = v.MylistCounter,
                LikeCount = v.LikeCounter,
                ThumbUrl = v.ThumbnailUrl,
            }).ToList();

            return AttemptResult<RemotePlaylistInfo>.Succeeded(new RemotePlaylistInfo()
            {
                PlaylistName = this._stringHandler.GetContent(query.SearchType switch
                {
                    SearchType.Tag => SearchStringContent.ResultTitleTag,
                    _ => SearchStringContent.ResultTitleKw
                },query.Query),
                Videos = videos
            });
        }

        #endregion

        #region private


        /// <summary>
        /// APIから検索結果を取得する
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private async Task<IAttemptResult<Api::Response>> GetResponseAsync(string url)
        {
            HttpResponseMessage res;
            res = await this._http.GetAsync(new Uri(url));

            if (!res.IsSuccessStatusCode)
            {
                this._errorHandler.HandleError(SearchError.FailedToRetrievingData, url, (int)res.StatusCode);
                return AttemptResult<Api::Response>.Fail(this._errorHandler.GetMessageForResult(SearchError.FailedToRetrievingData, url, (int)res.StatusCode));
            }

            string rawData = await res.Content.ReadAsStringAsync();
            Api::Response data;

            try
            {
                data = JsonParser.DeSerialize<Api::Response>(rawData);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(SearchError.FailedToAnalysisOfData, ex);
                return AttemptResult<Api::Response>.Fail(this._errorHandler.GetMessageForResult(SearchError.FailedToAnalysisOfData, ex));
            }

            return AttemptResult<Api::Response>.Succeeded(data);
        }

        #endregion

    }
}
