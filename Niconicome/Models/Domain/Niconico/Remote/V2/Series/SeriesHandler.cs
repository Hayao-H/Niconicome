using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Domain.Niconico.Remote.V2.Error;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Series = Niconicome.Models.Domain.Niconico.Net.Json.API.Series;

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

        public SeriesHandler(INicoHttp http, IErrorHandler errorHandler)
        {
            this._http = http;
            this._errorHandler = errorHandler;
        }

        #region private

        private readonly INicoHttp _http;

        private readonly IErrorHandler _errorHandler;

        #endregion

        #region Method

        public async Task<IAttemptResult<RemotePlaylistInfo>> GetSeries(string id)
        {
            IAttemptResult<FetchResult> result = await this.GetAllSeriesVideosAsync(id);
            if (!result.IsSucceeded || result.Data is null) return AttemptResult<RemotePlaylistInfo>.Fail(result.Message);

            return AttemptResult<RemotePlaylistInfo>.Succeeded(this.ConvertToRemotePlaylistInfo(result.Data));
        }

        #endregion

        /// <summary>
        /// シリーズのデータを取得
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        private async Task<IAttemptResult<Series::SeriesResponse>> GetVideosAsync(string id, string page)
        {
            var url = $"https://nvapi.nicovideo.jp/v2/series/{id}?page={page}&pageSize=100";

            this._errorHandler.HandleError(SeriesError.AccessToAPI, url);
            var res = await this._http.GetAsync(new Uri(url));

            if (!res.IsSuccessStatusCode)
            {
                this._errorHandler.HandleError(SeriesError.FailedToRetrieveData, url, (int)res.StatusCode);
                return AttemptResult<Series::SeriesResponse>.Fail(this._errorHandler.GetMessageForResult(SeriesError.FailedToRetrieveData, url, (int)res.StatusCode));
            }

            Series::SeriesResponse data;

            try
            {
                var content = await res.Content.ReadAsStringAsync();
                data = JsonParser.DeSerialize<Series::SeriesResponse>(content);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(SeriesError.DataAnalysisFailed, ex, id, page);
                return AttemptResult<Series::SeriesResponse>.Fail(this._errorHandler.GetMessageForResult(SeriesError.DataAnalysisFailed, ex, id, page));
            }

            return AttemptResult<Series::SeriesResponse>.Succeeded(data);

        }

        /// <summary>
        /// 全ての動画を取得
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<IAttemptResult<FetchResult>> GetAllSeriesVideosAsync(string id)
        {

            int page = 1;
            var hasNext = true;
            var videos = new List<Series::Item>();
            var title = "";

            this._errorHandler.HandleError(SeriesError.RetrievingHasStarted, id);

            do
            {
                IAttemptResult<Series::SeriesResponse> data = await this.GetVideosAsync(id, page.ToString());
                if (!data.IsSucceeded || data.Data is null) return AttemptResult<FetchResult>.Fail(data.Message);

                videos.AddRange(data.Data.Data.Items);

                hasNext = videos.Count < data.Data.Data.TotalCount;
                title = data.Data.Data.Detail.Title;
                page++;

            }
            while (hasNext);

            List<Series::Item> vList = videos.Distinct(v => v.Video.Id).ToList();
            this._errorHandler.HandleError(SeriesError.RetrievingHasCompleted, vList.Count, title);

            return AttemptResult<FetchResult>.Succeeded(new FetchResult(vList, title));
        }

        /// <summary>
        /// APIから取得したデータをローカルで扱うことが出来る形式に変換する
        /// </summary>
        /// <param name="videos"></param>
        /// <returns></returns>
        private RemotePlaylistInfo ConvertToRemotePlaylistInfo(FetchResult result)
        {
            return new RemotePlaylistInfo()
            {
                PlaylistName = result.Title,
                Videos = result.Videos.Select(item => new VideoInfo()
                {
                    NiconicoID = item.Video.Id,
                    Title = item.Video.Title,
                    OwnerName = item.Video.Owner.Name,
                    OwnerID = item.Video.Owner.Id,
                    ThumbUrl = item.Video.Thumbnail.GetURL(),
                    UploadedDT = item.Video.RegisteredAt,
                    ViewCount = item.Video.Count.View,
                    CommentCount = item.Video.Count.Comment,
                    MylistCount = item.Video.Count.Mylist,
                    LikeCount = item.Video.Count.Like,
                    Duration = item.Video.Duration,
                }).ToList()
            };
        }

        private record FetchResult(List<Series::Item> Videos, string Title);
    }
}
