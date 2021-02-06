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

namespace Niconicome.Models.Domain.Niconico.Video
{
    public interface IUserVideoHandler
    {
        Task<List<ITreeVideoInfo>> GetVideosAsync(string userId);
        Exception? CurrentException { get; }
    }

    public class UserVideoHandler : IUserVideoHandler
    {

        public UserVideoHandler(INicoHttp http)
        {
            this.http = http;
        }

        private readonly INicoHttp http;

        private int allVideos = -1;

        public Exception? CurrentException { get; private set; }


        /// <summary>
        /// 全ての投稿動画を取得する
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<ITreeVideoInfo>> GetVideosAsync(string userId)
        {
            var rawData = await this.GetAllVideosInternalAsync(userId);
            return this.ConvertToTreeVideoInfo(rawData);
        }

        /// <summary>
        /// 全ての投稿動画を取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<List<UVideo::Video>> GetAllVideosInternalAsync(string id)
        {
            var videos = new List<UVideo::Video>();
            int page = 1;
            do
            {
                List<UVideo::Video> data;
                data = await this.GetUserVideosAsync(id, page);
                if (data.Count == 0) break;
                videos.AddRange(data);
                ++page;
            } while (videos.Count < this.allVideos);

            return videos.Distinct(v => v.Id).ToList();
        }

        /// <summary>
        /// 指定したユーザーの投稿動画を取得する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        private async Task<List<UVideo::Video>> GetUserVideosAsync(string id, int page)
        {
            string rawData;
            UVideo::UserVideo? data;
            var logger = Utils::DIFactory.Provider.GetRequiredService<Utils::ILogger>();

            try
            {
                rawData = await this.http.GetStringAsync(new Uri($"https://nvapi.nicovideo.jp/v1/users/{id}/videos?pageSize=100&page={page}"));
            }
            catch (Exception e)
            {
                logger.Error($"投稿動画一覧の取得に失敗しました。(id:{id}, page:{page})", e);
                this.CurrentException = e;
                throw new HttpRequestException();
            }

            try
            {
                data = JsonParser.DeSerialize<UVideo::UserVideo>(rawData);
            }
            catch (Exception e)
            {
                logger.Error($"投稿動画一覧の解析に失敗しました。(id:{id}, page:{page})", e);
                this.CurrentException = e;
                throw new JsonException();
            }

            if (this.allVideos == -1) this.allVideos = data?.Data?.TotalCount ?? -1;
            return data?.Data?.Items ?? new List<UVideo.Video>();
        }

        /// <summary>
        /// ネットワークから取得したデータをローカルで扱うことが出来る形式に変換する
        /// </summary>
        /// <param name="videos"></param>
        /// <returns></returns>
        private List<ITreeVideoInfo> ConvertToTreeVideoInfo(List<UVideo::Video> videos)
        {
            var converted = new List<ITreeVideoInfo>();
            converted.AddRange(videos.Select(v => new BindableTreeVideoInfo() { Title = v.Title, NiconicoId = v.Id, UploadedOn = v.RegisteredAt, ThumbUrl = v.Thumbnail.MiddleUrl, LargeThumbUrl = v.Thumbnail.LargeUrl }));
            return converted;
        }
    }
}
