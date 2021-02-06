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

namespace Niconicome.Models.Domain.Niconico.Mylist
{
    public interface IMylistHandler
    {
        Task<List<ITreeVideoInfo>> GetVideosAsync(string playlistId);
        Exception? CurrentException { get; }
    }

    public class MylistHandler : IMylistHandler
    {
        public MylistHandler(INicoHttp http,Utils::ILogger logger)
        {
            this.http = http;
            this.logger = logger;
        }

        /// <summary>
        /// httpクライアント
        /// </summary>
        protected readonly INicoHttp http;

        /// <summary>
        /// ロガー
        /// </summary>
        protected readonly Utils::ILogger logger;

        /// <summary>
        /// マイリストを取得する
        /// </summary>
        /// <param name="mylistId"></param>
        /// <returns></returns>
        public virtual async Task<List<ITreeVideoInfo>> GetVideosAsync(string mylistId)
        {
            var rawdata = await this.GetAllMylistVideos(mylistId);
            var converted = this.ConvertToTreeVideoInfo(rawdata);
            return converted;
        }

        /// <summary>
        /// 例外情報
        /// </summary>
        public Exception? CurrentException { get; protected set; }


        /// <summary>
        /// マイリストに含まれる全ての動画を取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected virtual async Task<List<Mylist::Video>> GetAllMylistVideos(string id)
        {
            var videos = new List<Mylist::Video>();

            var data = new Mylist::MylistResponse();
            int page = 1;

            do
            {
                data = await this.TryGetmylist(id, page.ToString());
                page++;
                videos.AddRange(data.Data.Mylist.Items.Select(i => i.Video));
            }
            while (data.Data.Mylist.HasNext);

            var vList= videos.Distinct(v => v.Id).ToList();
            this.logger.Log($"{vList.Count}件の動画をマイリスト(id:{id})から取得しました。");
            return vList;
        }

        /// <summary>
        /// マイリストデータを取得する
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task<Mylist::MylistResponse> TryGetmylist(string id, string page)
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
                this.CurrentException = e;
                throw new HttpRequestException();
            }

            try
            {
                data = JsonParser.DeSerialize<Mylist::MylistResponse>(rawData);
            }
            catch (Exception e)
            {
                this.logger.Error($"マイリストデータの解析に失敗しました。(id:{id}, page:{page})", e);
                this.CurrentException = e;
                throw new System.Text.Json.JsonException();
            }

            return data;
        }

        /// <summary>
        /// APIから取得したデータをローカルで扱うことが出来る形式に変換する
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        protected List<ITreeVideoInfo> ConvertToTreeVideoInfo(IEnumerable<Mylist::Video> source)
        {
            var converted = new List<ITreeVideoInfo>();

            converted.AddRange(source.Select(video => new BindableTreeVideoInfo()
            {
                Title = video.Title,
                UploadedOn = video.RegisteredAt,
                NiconicoId = video.Id,
                LargeThumbUrl = video.Thumbnail.LargeUrl.IsNullOrEmpty()?video.Thumbnail.Url: video.Thumbnail.LargeUrl,
                ThumbUrl = video.Thumbnail.MiddleUrl.IsNullOrEmpty()?video.Thumbnail.Url:video.Thumbnail.MiddleUrl,

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
