using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Playlist;
using System.Net.Http;
using WatchLater = Niconicome.Models.Domain.Niconico.Net.Json.API.WatchLater;
using Mylist = Niconicome.Models.Domain.Niconico.Net.Json.API.Mylist;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Extensions.System.List;
using Utils = Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result.Generic;

namespace Niconicome.Models.Domain.Niconico.Remote.Mylist
{

    public interface IWatchLaterHandler
    {
        Task<IAttemptResult<string>> GetVideosAsync(List<IListVideoInfo> videos);
    }

    /// <summary>
    /// 「後で見る」から動画を取得
    /// </summary>
    class WatchLaterHandler : MylistHandler, IWatchLaterHandler
    {
        public WatchLaterHandler(INicoHttp http, Utils::ILogger logger, IVideoInfoContainer videoInfoContainer) : base(http, logger, videoInfoContainer) { }

        /// <summary>
        /// 「あとで見る」を取得する
        /// </summary>
        /// <returns></returns>
        public async Task<IAttemptResult<string>> GetVideosAsync(List<IListVideoInfo> videos)
        {
            return await this.GetVideosAsync("0", videos);
        }

        /// <summary>
        /// 「あとで見る」のデータを取得
        /// </summary>
        /// <param name="id"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        protected async override Task<string> GetMylistdataAsync(string id, string page)
        {
            var url = $"https://nvapi.nicovideo.jp/v1/users/me/watch-later?sortKey=addedAt&pageSize=100&page={page}";

            var res = await this.http.GetAsync(new Uri(url));

            if (res.IsSuccessStatusCode)
            {
                return await res.Content.ReadAsStringAsync();
            }
            else
            {
                throw new HttpRequestException($"「あとで見る」の取得に失敗しました。(詳細: status:{(int)res.StatusCode} message: {res.ReasonPhrase})");
            }
        }

        /// <summary>
        /// 全ての動画を取得
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected async override Task<IAttemptResult<string>> GetAllMylistVideos(string id, List<Net.Json.API.Mylist.Video> videos)
        {

            IAttemptResult<WatchLater::WatchLaterResponse>? data;
            int page = 1;

            do
            {
                data = await this.TryGetWatchLater(id, page.ToString());
                if (!data.IsSucceeded) return new AttemptResult<string>() { Message = data.Message };
                page++;
                videos.AddRange(data.Data!.Data.WatchLater.Items.Select(i => i.Video));
            }
            while (data.Data!.Data.WatchLater.HasNext);

            var vList = videos.Distinct(v => v.Id).ToList();
            this.logger.Log($"{vList.Count}件の動画を「あとで見る」から取得しました。");
            return new AttemptResult<string>() { IsSucceeded = true, Data = "あとで見る" };
        }


        private async Task<IAttemptResult<WatchLater::WatchLaterResponse>> TryGetWatchLater(string id, string page)
        {
            WatchLater::WatchLaterResponse? data;

            string rawData;

            try
            {
                rawData = await this.GetMylistdataAsync(id, page);
            }
            catch (Exception e)
            {
                this.logger.Error($"「あとで見る」の取得に失敗しました。(page:{page})", e);
                return new AttemptResult<WatchLater::WatchLaterResponse>() { Message = $"「あとで見る」の取得に失敗しました。(page:{page})", Exception = e };
            }

            try
            {
                data = JsonParser.DeSerialize<WatchLater::WatchLaterResponse>(rawData);
            }
            catch (Exception e)
            {
                this.logger.Error($"「あとで見る」の解析に失敗しました。(page:{page})", e);
                return new AttemptResult<WatchLater::WatchLaterResponse>() { Message = $"「あとで見る」の解析に失敗しました。(page:{page})", Exception = e };
            }

            return new AttemptResult<WatchLater::WatchLaterResponse>() { IsSucceeded = true, Data = data };
        }
    }
}
