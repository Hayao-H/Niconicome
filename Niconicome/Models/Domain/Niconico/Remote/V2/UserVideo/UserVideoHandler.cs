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
using UVideo = Niconicome.Models.Domain.Niconico.Net.Json.API.Video.V3;

namespace Niconicome.Models.Domain.Niconico.Remote.V2.UserVideo
{
    public interface IUserVideoHandler
    {
        /// <summary>
        /// 全ての投稿動画を取得する
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IAttemptResult<RemotePlaylistInfo>> GetVideosAsync(string userId);
    }

    public class UserVideoHandler : IUserVideoHandler
    {

        public UserVideoHandler(INicoHttp http,IErrorHandler errorHandler,IStringHandler stringHandler)
        {
            this._http = http;
            this._errorHandler = errorHandler;
            this._stringHandler = stringHandler;
        }

        #region field

        private readonly INicoHttp _http;

        private readonly IErrorHandler _errorHandler;

        private readonly IStringHandler _stringHandler;

        #endregion

        #region Method

        public async Task<IAttemptResult<RemotePlaylistInfo>> GetVideosAsync(string id)
        {
            int page = 1;
            long total = 0;
            var videos = new List<VideoInfo>();

            do
            {
                IAttemptResult<FetchResult> result = await this.GetUserVideosAsync(id, page);
                if (!result.IsSucceeded || result.Data is null) return AttemptResult<RemotePlaylistInfo>.Fail(result.Message);

                total = result.Data.TotalCount;

                videos.AddRange(this.ConvertToTreeVideoInfo(result.Data.Videos));
                ++page;
            } while (videos.Count < total);

            string ownerName = videos.FirstOrDefault()?.OwnerName ?? "";

            return AttemptResult<RemotePlaylistInfo>.Succeeded(new RemotePlaylistInfo() { PlaylistName = this._stringHandler.GetContent(UserVideoStringContent.UserVideoTitle, ownerName), Videos = videos });
        }

        #endregion

        #region private

        /// <summary>
        /// 指定したユーザーの投稿動画を取得する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        private async Task<IAttemptResult<FetchResult>> GetUserVideosAsync(string id, int page)
        {
            var url = $"https://nvapi.nicovideo.jp/v3/users/{id}/videos?sortKey=registeredAt&sortOrder=desc&sensitiveContents=mask&pageSize=100&page={page}";

            HttpResponseMessage res = await this._http.GetAsync(new Uri(url));
            if (!res.IsSuccessStatusCode)
            {
                this._errorHandler.HandleError(UserVideoError.FailedToRetrieveData, url, (int)res.StatusCode);
                return AttemptResult<FetchResult>.Fail(this._errorHandler.GetMessageForResult(UserVideoError.FailedToRetrieveData, url, (int)res.StatusCode));
            }

            string rawData = await res.Content.ReadAsStringAsync();

            UVideo::UserVideo data;

            try
            {
                data = JsonParser.DeSerialize<UVideo::UserVideo>(rawData);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(UserVideoError.FailedToAnalysis, ex, id, page);
                return AttemptResult<FetchResult>.Fail(this._errorHandler.GetMessageForResult(UserVideoError.FailedToAnalysis, ex, id, page));
            }

            return AttemptResult<FetchResult>.Succeeded(new FetchResult(data.Data.Items.Select(i => i.Essential).ToList().AsReadOnly(), data.Data.TotalCount));

        }

        /// <summary>
        /// 取得結果
        /// </summary>
        /// <param name="Videos"></param>
        /// <param name="TotalCount"></param>
        private record FetchResult(IReadOnlyList<UVideo::Essential> Videos, int TotalCount);

        /// <summary>
        /// ネットワークから取得したデータをローカルで扱うことが出来る形式に変換する
        /// </summary>
        /// <param name="videos"></param>
        /// <returns></returns>
        private IReadOnlyList<VideoInfo> ConvertToTreeVideoInfo(IEnumerable<UVideo::Essential> videos)
        {
            return videos.Select(v => new VideoInfo()
            {
                Title = v.Title,
                NiconicoID = v.Id,
                UploadedDT = v.RegisteredAt,
                ThumbUrl = v.Thumbnail.LargeUrl,
                ViewCount = v.Count.View,
                CommentCount = v.Count.Comment,
                MylistCount = v.Count.Mylist,
                LikeCount = v.Count.Like,
                OwnerID = v.Owner.Id,
                OwnerName = v.Owner.Name,
            }).ToList().AsReadOnly();
        }

        #endregion
    }
}
