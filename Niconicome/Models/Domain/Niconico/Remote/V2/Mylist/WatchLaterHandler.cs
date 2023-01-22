using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Domain.Niconico.Remote.V2.Error;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Mylist = Niconicome.Models.Domain.Niconico.Net.Json.API.Mylist;
using WatchLater = Niconicome.Models.Domain.Niconico.Net.Json.API.WatchLater;

namespace Niconicome.Models.Domain.Niconico.Remote.V2.Mylist
{

    public interface IWatchLaterHandler
    {
        /// <summary>
        /// 「あとで見る」から動画を取得
        /// </summary>
        /// <returns></returns>
        Task<IAttemptResult<RemotePlaylistInfo>> GetVideosAsync();
    }

    class WatchLaterHandler : IWatchLaterHandler
    {
        public WatchLaterHandler(INicoHttp http,IErrorHandler errorHandler) {
            this._http = http;
            this._errorHandler = errorHandler;
        }


        #region private

        private readonly INicoHttp _http;

        private readonly IErrorHandler _errorHandler;

        #endregion

        #region Method

        public async Task<IAttemptResult<RemotePlaylistInfo>> GetVideosAsync()
        {
            IAttemptResult<List<Mylist::Item>> result = await this.GetAllWatchLaterVideosAsync();
            if (!result.IsSucceeded || result.Data is null) return AttemptResult<RemotePlaylistInfo>.Fail(result.Message);

            return AttemptResult<RemotePlaylistInfo>.Succeeded(this.ConvertToRemotePlaylistInfo(result.Data));
        }

        #endregion

        /// <summary>
        /// 「あとで見る」のデータを取得
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        private async Task<IAttemptResult<WatchLater::WatchLaterResponse>> GetVideosAsync(string page)
        {
            var url = $"https://nvapi.nicovideo.jp/v1/users/me/watch-later?sortKey=addedAt&pageSize=100&page={page}";

            this._errorHandler.HandleError(WatchLaterError.AccessToAPI);
            var res = await this._http.GetAsync(new Uri(url));

            if (!res.IsSuccessStatusCode)
            {
                this._errorHandler.HandleError(WatchLaterError.FailedToRetrieveData, url, (int)res.StatusCode);
                return AttemptResult<WatchLater::WatchLaterResponse>.Fail(this._errorHandler.GetMessageForResult(WatchLaterError.FailedToRetrieveData, url, (int)res.StatusCode));
            }

            WatchLater::WatchLaterResponse data;

            try
            {
                data = JsonParser.DeSerialize<WatchLater::WatchLaterResponse>(await res.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(WatchLaterError.DataAnalysisFailed, ex);
                return AttemptResult<WatchLater::WatchLaterResponse>.Fail(this._errorHandler.GetMessageForResult(WatchLaterError.DataAnalysisFailed, ex));
            }

            return AttemptResult<WatchLater::WatchLaterResponse>.Succeeded(data);

        }

        /// <summary>
        /// 全ての動画を取得
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<IAttemptResult<List<Mylist::Item>>> GetAllWatchLaterVideosAsync()
        {

            int page = 1;
            var hasNext = true;
            var videos = new List<Mylist::Item>();

            this._errorHandler.HandleError(WatchLaterError.FetchStarted);

            do
            {
                IAttemptResult<WatchLater::WatchLaterResponse> data = await this.GetVideosAsync(page.ToString());
                if (!data.IsSucceeded || data.Data is null) return AttemptResult<List<Mylist::Item>>.Fail(data.Message);

                hasNext = data.Data.Data.WatchLater.HasNext;
                page++;

                videos.AddRange(data.Data!.Data.WatchLater.Items);
            }
            while (hasNext);

            List<Mylist::Item> vList = videos.Distinct(v => v.Video.Id).ToList();
            this._errorHandler.HandleError(WatchLaterError.FetchCompleted, vList.Count);

            return AttemptResult<List<Mylist::Item>>.Succeeded(vList);
        }

        /// <summary>
        /// APIから取得したデータをローカルで扱うことが出来る形式に変換する
        /// </summary>
        /// <param name="videos"></param>
        /// <returns></returns>
        private RemotePlaylistInfo ConvertToRemotePlaylistInfo(List<Mylist::Item> videos)
        {
            return new RemotePlaylistInfo()
            {
                PlaylistName = "あとで見る",
                Videos = videos.Select(item => new VideoInfo()
                {
                    NiconicoID = item.Video.Id,
                    Title = item.Video.Title,
                    OwnerName = item.Video.Owner.Name,
                    OwnerID = item.Video.Owner.Id,
                    ThumbUrl = item.Video.Thumbnail.LargeUrl,
                    UploadedDT = item.AddtedAt,
                    ViewCount = item.Video.Count.View,
                    CommentCount = item.Video.Count.Comment,
                    MylistCount = item.Video.Count.Mylist,
                    LikeCount = item.Video.Count.Like,
                }).ToList()
            };
        }

    }
}
