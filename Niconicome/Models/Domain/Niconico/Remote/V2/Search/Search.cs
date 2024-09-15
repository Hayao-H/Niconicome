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
using ApiV2 = Niconicome.Models.Domain.Niconico.Net.Json.API.Search.V2;

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
        public Search(ISearchUrlConstructor urlConstructor, INicoHttp http, IErrorHandler errorHandler, IStringHandler stringHandler)
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

            string url = this._urlConstructor.GetUrlV2(query);

            IAttemptResult<ApiV2::Response> searchResult = await this.GetResponseFromNVAsync(url);
            if (!searchResult.IsSucceeded || searchResult.Data is null) return AttemptResult<RemotePlaylistInfo>.Fail(searchResult.Message);

            List<VideoInfo> videos = searchResult.Data.Data.Items.Select(v => new VideoInfo()
            {
                NiconicoID = v.Id,
                UploadedDT = v.RegisteredAt,
                Title = v.Title,
                ViewCount = v.Count.View,
                CommentCount = v.Count.Comment,
                MylistCount = v.Count.Mylist,
                LikeCount = v.Count.Like,
                ThumbUrl = v.Thumbnail.Url,
                Duration = v.Duration,
            }).ToList();

            return AttemptResult<RemotePlaylistInfo>.Succeeded(new RemotePlaylistInfo()
            {
                PlaylistName = this._stringHandler.GetContent(query.SearchType switch
                {
                    SearchType.Tag => SearchStringContent.ResultTitleTag,
                    _ => SearchStringContent.ResultTitleKw
                }, query.Query),
                Videos = videos,
                TotalCount = searchResult.Data.Data.TotalCount,
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

        /// <summary>
        /// nvapiから検索結果を取得する
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private async Task<IAttemptResult<ApiV2::Response>> GetResponseFromNVAsync(string url)
        {
            HttpResponseMessage res;
            res = await this._http.GetAsync(new Uri(url));

            if (!res.IsSuccessStatusCode)
            {
                this._errorHandler.HandleError(SearchError.FailedToRetrievingData, url, (int)res.StatusCode);
                return AttemptResult<ApiV2::Response>.Fail(this._errorHandler.GetMessageForResult(SearchError.FailedToRetrievingData2, url, (int)res.StatusCode));
            }

            string rawData = await res.Content.ReadAsStringAsync();
            ApiV2::Response data;

            try
            {
                data = JsonParser.DeSerialize<ApiV2::Response>(rawData);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(SearchError.FailedToAnalysisOfData, ex);
                return AttemptResult<ApiV2::Response>.Fail(this._errorHandler.GetMessageForResult(SearchError.FailedToAnalysisOfData, ex));
            }

            return AttemptResult<ApiV2::Response>.Succeeded(data);
        }
        #endregion

    }
}
