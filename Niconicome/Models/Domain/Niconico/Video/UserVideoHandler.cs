using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Playlist;
using Niconicome.Models.Domain.Niconico.Net.Json;
using UVideo = Niconicome.Models.Domain.Niconico.Net.Json.API.Video;
using Utils = Niconicome.Models.Domain.Utils;
using Microsoft.Xaml.Behaviors;
using Niconicome.Models.Helper.Result.Generic;
using Niconicome.Models.Domain.Utils;
using System.Reflection.Metadata.Ecma335;

namespace Niconicome.Models.Domain.Niconico.Video
{
    public interface IUserVideoHandler
    {
        Task<IAttemptResult<string>> GetVideosAsync(string userId, List<IListVideoInfo> videos);
    }

    public class UserVideoHandler : IUserVideoHandler
    {

        public UserVideoHandler(INicoHttp http, ILogger logger)
        {
            this.http = http;
            this.logger = logger;
        }

        private readonly INicoHttp http;

        private readonly ILogger logger;


        /// <summary>
        /// 全ての投稿動画を取得する
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IAttemptResult<string>> GetVideosAsync(string userId, List<IListVideoInfo> videos)
        {
            var result = await this.GetAllVideosInternalAsync(userId, videos);
            return result;
        }

        /// <summary>
        /// 全ての投稿動画を取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<IAttemptResult<string>> GetAllVideosInternalAsync(string id, List<IListVideoInfo> videos)
        {
            int page = 1;
            long total = 0;
            IAttemptResult<UVideo::UserVideo> result;
            do
            {
                result = await this.GetUserVideosAsync(id, page, v => total = v.Data.TotalCount);
                if (!result.IsSucceeded) return new AttemptResult<string>() { Message = result.Message, Exception = result.Exception };
                videos.AddRange(this.ConvertToTreeVideoInfo(result.Data!.Data.Items));
                ++page;
            } while (videos.Count < total);

            return new AttemptResult<string>() { Data = videos.FirstOrDefault()?.OwnerName.Value ?? string.Empty, IsSucceeded = true };
        }

        /// <summary>
        /// 指定したユーザーの投稿動画を取得する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        private async Task<IAttemptResult<UVideo::UserVideo>> GetUserVideosAsync(string id, int page, Action<UVideo::UserVideo> onGet)
        {
            string rawData;

            try
            {
                rawData = await this.http.GetStringAsync(new Uri($"https://nvapi.nicovideo.jp/v2/users/{id}/videos?pageSize=100&page={page}"));
            }
            catch (Exception e)
            {
                this.logger.Error($"投稿動画一覧の取得に失敗しました。(id:{id}, page:{page})", e);
                return new AttemptResult<UVideo::UserVideo>() { Message = $"投稿動画一覧の取得に失敗しました。(id:{id}, page:{page})", Exception = e };
            }

            UVideo::UserVideo data;

            try
            {
                data = JsonParser.DeSerialize<UVideo::UserVideo>(rawData);
            }
            catch (Exception e)
            {
                logger.Error($"投稿動画一覧の解析に失敗しました。(id:{id}, page:{page})", e);
                return new AttemptResult<UVideo::UserVideo>() { Message = $"投稿動画一覧の解析に失敗しました。(id:{id}, page:{page})", Exception = e };
            }

            onGet(data);
            return new AttemptResult<UVideo::UserVideo>() { IsSucceeded = true, Data = data };

        }

        /// <summary>
        /// ネットワークから取得したデータをローカルで扱うことが出来る形式に変換する
        /// </summary>
        /// <param name="videos"></param>
        /// <returns></returns>
        private List<IListVideoInfo> ConvertToTreeVideoInfo(List<UVideo::Item> videos)
        {
            var converted = new List<IListVideoInfo>();
            converted.AddRange(videos.Select(v => v.Essential).Select(v =>
            {
                var video = new NonBindableListVideoInfo();
                video.Title.Value = v.Title;
                video.NiconicoId.Value = v.Id;
                video.UploadedOn.Value = v.RegisteredAt.DateTime;
                video.ThumbUrl.Value = v.Thumbnail.MiddleUrl;
                video.LargeThumbUrl.Value = v.Thumbnail.LargeUrl;
                video.ViewCount.Value = (int)v.Count.View;
                video.CommentCount.Value = (int)v.Count.Comment;
                video.MylistCount.Value = (int)v.Count.Mylist;
                video.LikeCount.Value = (int)v.Count.Like;
                video.OwnerName.Value = v.Owner.Name;
                return video;
            }));
            return converted;
        }
    }
}
