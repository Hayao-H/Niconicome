using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using Niconicome.Models.Playlist;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Mylist = Niconicome.Models.Domain.Niconico.Net.Json.API.Mylist;
using Utils = Niconicome.Models.Domain.Utils;
using Niconicome.Extensions.System;
using Niconicome.Models.Helper.Result.Generic;

namespace Niconicome.Models.Domain.Niconico.Remote.Mylist
{
    public interface IMylistHandler
    {
        Task<IAttemptResult<string>> GetVideosAsync(string playlistId, List<IListVideoInfo> videos);
    }

    public class MylistHandler : IMylistHandler
    {
        public MylistHandler(INicoHttp http, Utils::ILogger logger,IVideoInfoContainer videoInfoContainer)
        {
            this.http = http;
            this.logger = logger;
            this.videoInfoContainer = videoInfoContainer;
        }

        #region DI

        protected readonly INicoHttp http;

        protected readonly Utils::ILogger logger;

        protected readonly IVideoInfoContainer videoInfoContainer;

        #endregion

        /// <summary>
        /// マイリストを取得する
        /// </summary>
        /// <param name="mylistId"></param>
        /// <returns></returns>
        public virtual async Task<IAttemptResult<string>> GetVideosAsync(string playlistId, List<IListVideoInfo> videos)
        {
            var rawVideos = new List<Mylist::Video>();

            var result = await this.GetAllMylistVideos(playlistId, rawVideos);
            var converted = this.ConvertToTreeVideoInfo(rawVideos);
            videos.AddRange(converted);

            return result;
        }


        /// <summary>
        /// マイリストに含まれる全ての動画を取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected virtual async Task<IAttemptResult<string>> GetAllMylistVideos(string id, List<Mylist::Video> videos)
        {

            IAttemptResult<Mylist::MylistResponse> data;
            int page = 1;

            do
            {
                data = await this.TryGetmylist(id, page.ToString());
                if (!data.IsSucceeded) return new AttemptResult<string>() { Message = data.Message };
                page++;
                videos.AddRange(data.Data!.Data.Mylist.Items.Select(i => i.Video));
            }
            while (data.Data!.Data.Mylist.HasNext);

            var vList = videos.Distinct(v => v.Id).ToList();
            this.logger.Log($"{vList.Count}件の動画をマイリスト(id:{id})から取得しました。");
            return new AttemptResult<string>() { Data = data.Data!.Data.Mylist.Name, IsSucceeded = true };
        }

        /// <summary>
        /// マイリストデータを取得する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task<IAttemptResult<Mylist::MylistResponse>> TryGetmylist(string id, string page)
        {
            Mylist::MylistResponse? data;
            string rawData;

            try
            {
                rawData = await this.GetMylistdataAsync(id, page);
            }
            catch (Exception e)
            {
                this.logger.Error($"マイリストデータの取得に失敗しました。(id:{id}, page:{page})", e);
                return new AttemptResult<Mylist::MylistResponse>() { Message = $"マイリストデータの取得に失敗しました。(id:{id}, page:{page})", Exception = e };
            }

            try
            {
                data = JsonParser.DeSerialize<Mylist::MylistResponse>(rawData);
            }
            catch (Exception e)
            {
                this.logger.Error($"マイリストデータの解析に失敗しました。(id:{id}, page:{page})", e);
                return new AttemptResult<Mylist::MylistResponse>() { Message = $"マイリストデータの解析に失敗しました。(id:{id}, page:{page})", Exception = e };

            }

            return new AttemptResult<Mylist::MylistResponse>() { IsSucceeded = true, Data = data };
        }

        /// <summary>
        /// APIから取得したデータをローカルで扱うことが出来る形式に変換する
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        protected List<IListVideoInfo> ConvertToTreeVideoInfo(IEnumerable<Mylist::Video> source)
        {
            var converted = new List<IListVideoInfo>();

            converted.AddRange(source.Select(video =>
            {
                var v = this.videoInfoContainer.GetVideo(video.Id);

                v.UploadedOn.Value = video.RegisteredAt;
                v.NiconicoId.Value = video.Id;
                v.LargeThumbUrl.Value = video.Thumbnail.LargeUrl.IsNullOrEmpty() ? video.Thumbnail.Url : video.Thumbnail.LargeUrl;
                v.ThumbUrl.Value = video.Thumbnail.MiddleUrl.IsNullOrEmpty() ? video.Thumbnail.Url : video.Thumbnail.MiddleUrl;
                v.Title.Value = video.Title;
                return v;
            }).ToList());

            return converted;
        }

        /// <summary>
        /// マイリストのデータをサーバーから取得する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        protected virtual async Task<string> GetMylistdataAsync(string id, string page)
        {
            var urlBase = $"https://nvapi.nicovideo.jp/v2/mylists/{id}?pageSize=100&page={page}";
            var urlMy = $"https://nvapi.nicovideo.jp/v1/users/me/mylists/{id}?pageSize=100&page={page}";

            var resBase = await this.http.GetAsync(new Uri(urlBase));

            if (resBase.IsSuccessStatusCode)
            {
                return await resBase.Content.ReadAsStringAsync();
            }
            else if (resBase.StatusCode != System.Net.HttpStatusCode.Forbidden)
            {
                throw new HttpRequestException($"マイリストデータの取得に失敗しました。(詳細: status:{(int)resBase.StatusCode} message: {resBase.ReasonPhrase})");
            }

            var resMy = await this.http.GetAsync(new Uri(urlMy));

            if (resMy.IsSuccessStatusCode)
            {
                return await resMy.Content.ReadAsStringAsync();
            }

            throw new HttpRequestException($"マイリストデータの取得に失敗しました。(詳細: status:{(int)resMy.StatusCode} message: {resMy.ReasonPhrase})");
        }
    }

}
