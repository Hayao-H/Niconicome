using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Domain.Niconico.Remote.V2.Error;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Playlist;
using Mylist = Niconicome.Models.Domain.Niconico.Net.Json.API.Mylist;
using Utils = Niconicome.Models.Domain.Utils;

namespace Niconicome.Models.Domain.Niconico.Remote.V2.Mylist
{
    public interface IMylistHandler
    {
        /// <summary>
        /// マイリストを取得する
        /// </summary>
        /// <param name="playlistId"></param>
        /// <returns></returns>
        Task<IAttemptResult<RemotePlaylistInfo>> GetVideosAsync(string playlistId);
    }

    public class MylistHandler : IMylistHandler
    {
        public MylistHandler(INicoHttp http, Utils::ILogger logger, IVideoInfoContainer videoInfoContainer,IErrorHandler errorHandler)
        {
            this._http = http;
            this._errorHandler = errorHandler;
        }

        #region field

        protected readonly INicoHttp _http;

        protected readonly IErrorHandler _errorHandler;

        #endregion

        #region Method

        public async Task<IAttemptResult<RemotePlaylistInfo>> GetVideosAsync(string playlistId)
        {
            IAttemptResult<FetchResult> result = await this.GetAllVideosAsync(playlistId);
            if (!result.IsSucceeded || result.Data is null) return AttemptResult<RemotePlaylistInfo>.Fail(result.Message);

            RemotePlaylistInfo converted = this.ConvertToRemotePlaylistInfo(result.Data);

            return AttemptResult<RemotePlaylistInfo>.Succeeded(converted);
        }

        #endregion

        #region private

        /// <summary>
        /// マイリストデータを取得する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task<IAttemptResult<Mylist::MylistResponse>> TryGetmylistAsync(string id, string page)
        {

            IAttemptResult<string> getResult = await this.GetMylistDataAsync(id, page);
            if (!getResult.IsSucceeded || getResult.Data is null) return AttemptResult<Mylist::MylistResponse>.Fail(getResult.Message);


            Mylist::MylistResponse data;

            try
            {
                data = JsonParser.DeSerialize<Mylist::MylistResponse>(getResult.Data);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(MylistError.DataAnalysisFailed, ex, id, page);
                return AttemptResult<Mylist::MylistResponse>.Fail(this._errorHandler.GetMessageForResult(MylistError.DataAnalysisFailed, ex, id, page));
            }

            return AttemptResult<Mylist::MylistResponse>.Succeeded(data);
        }

        /// <summary>
        /// マイリストに含まれる全ての動画を取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<IAttemptResult<FetchResult>> GetAllVideosAsync(string id)
        {
            this._errorHandler.HandleError(MylistError.FetchStarted, id);

            int page = 1;
            var videos = new List<Mylist::Item>();
            var playlistName = "";
            var hasNext = true;

            do
            {
                IAttemptResult<Mylist::MylistResponse> result = await this.TryGetmylistAsync(id, page.ToString());

                if (!result.IsSucceeded || result.Data is null) return AttemptResult<FetchResult>.Fail(result.Message);

                hasNext = result.Data.Data.Mylist.HasNext;
                playlistName = result.Data.Data.Mylist.Name;
                page++;
                videos.AddRange(result.Data!.Data.Mylist.Items);
            }
            while (hasNext);

            var resultVideos = videos.Distinct(v => v.WatchId).ToList();

            this._errorHandler.HandleError(MylistError.FetchCompleted, resultVideos.Count, id);


            return AttemptResult<FetchResult>.Succeeded(new FetchResult(playlistName, resultVideos));
        }

        /// <summary>
        /// APIから取得したデータをローカルで扱うことが出来る形式に変換する
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private RemotePlaylistInfo ConvertToRemotePlaylistInfo(FetchResult source)
        {
            var converted = new List<VideoInfo>();

            converted.AddRange(source.Videos.Select(video => new VideoInfo()
            {
                NiconicoID = video.Video.Id,
                Title = video.Video.Title,
                OwnerName = video.Video.Owner.Name,
                OwnerID = video.Video.Owner.Id,
                ThumbUrl = video.Video.Thumbnail.LargeUrl,
                UploadedDT = video.AddtedAt,
                ViewCount = video.Video.Count.View,
                CommentCount = video.Video.Count.Comment,
                MylistCount = video.Video.Count.Mylist,
                LikeCount = video.Video.Count.Like,
            }).ToList());

            return new RemotePlaylistInfo()
            {
                PlaylistName = source.Name,
                Videos= converted
            };
        }

        /// <summary>
        /// マイリストのデータをサーバーから取得する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        private async Task<IAttemptResult<string>> GetMylistDataAsync(string id, string page)
        {
            var urlBase = $"https://nvapi.nicovideo.jp/v2/mylists/{id}?pageSize=100&page={page}";
            var urlMy = $"https://nvapi.nicovideo.jp/v1/users/me/mylists/{id}?pageSize=100&page={page}";

            this._errorHandler.HandleError(MylistError.AccessToAPIV1, urlBase);
            var resBase = await this._http.GetAsync(new Uri(urlBase));

            if (resBase.IsSuccessStatusCode)
            {
                return AttemptResult<string>.Succeeded(await resBase.Content.ReadAsStringAsync());
            }
            else if (resBase.StatusCode != System.Net.HttpStatusCode.Forbidden)
            {
                this._errorHandler.HandleError(MylistError.FailedToRetrieveData, urlBase, (int)resBase.StatusCode);
                return AttemptResult<string>.Fail(this._errorHandler.GetMessageForResult(MylistError.FailedToRetrieveData, urlBase, (int)resBase.StatusCode));
            }

            this._errorHandler.HandleError(MylistError.AccessToAPIV2, urlMy);
            var resMy = await this._http.GetAsync(new Uri(urlMy));

            if (resMy.IsSuccessStatusCode)
            {
                return AttemptResult<string>.Succeeded(await resMy.Content.ReadAsStringAsync());
            }
            else
            {

                this._errorHandler.HandleError(MylistError.FailedToRetrieveData, urlMy, (int)resMy.StatusCode);
                return AttemptResult<string>.Fail(this._errorHandler.GetMessageForResult(MylistError.FailedToRetrieveData, urlMy, (int)resMy.StatusCode));
            }
        }

        #endregion

        #region record

        private record FetchResult(string Name, List<Mylist::Item> Videos);

        #endregion
    }

}
