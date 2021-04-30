using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Attributes;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Helper.Result.Generic;
using Niconicome.Models.Network.Watch;
using Niconicome.Models.Playlist;
using WatchInfo = Niconicome.Models.Domain.Niconico.Watch;

namespace Niconicome.Models.Domain.Niconico.Video.Channel
{
    public interface IChannelResult
    {
        List<IListVideoInfo> RetrievedVideos { get; }

        int FailedCounts { get; }
        bool IsSucceededAll { get; }
    }

    public interface IChannelVideoHandler
    {
        Task<IAttemptResult<string>> GetVideosAsync(string channelId, List<string> ids, IEnumerable<string> registeredVideo, Action<string> onMessage);
    }

    class ChannelVideoHandler : IChannelVideoHandler
    {
        public ChannelVideoHandler(INicoHttp http, ILogger logger, IChannelPageHtmlParser htmlParser)
        {
            this.http = http;
            this.htmlParser = htmlParser;
            this.logger = logger;
        }

        private readonly INicoHttp http;

        private readonly ILogger logger;

        private readonly IChannelPageHtmlParser htmlParser;

        /// <summary>
        /// チャンネル動画を取得する
        /// </summary>
        /// <param name="channelId"></param>
        /// <returns></returns>
        public async Task<IAttemptResult<string>> GetVideosAsync(string channelId, List<string> ids, IEnumerable<string> registeredVideo, Action<string> onMessage)
        {

            string html;
            var index = 0;
            var baseUrl = $"https://ch.nicovideo.jp/{channelId}/video";
            var urlQuery = string.Empty;
            IChannelPageInfo info;

            do
            {
                if ((index + 1) % 5 == 0)
                {
                    onMessage("待機中...(10s)");
                    await Task.Delay(10 * 1000);
                }

                onMessage($"{index + 1}ページ目を取得します");
                try
                {
                    html = await this.GetChannelPage(baseUrl + urlQuery);
                }
                catch (Exception e)
                {
                    return new AttemptResult<string>() { Message = $"チャンネルページ取得に失敗しました。(詳細: {e.Message}" };
                }

                try
                {
                    info = this.GetINfoFromHtml(html);
                }
                catch (Exception e)
                {
                    return new AttemptResult<string>() { Message = $"チャンネルページの解析に失敗しました。(詳細: {e.Message}" };
                }

                urlQuery = info.NextPageQuery ?? string.Empty;
                ids.AddRange(info.IDs);
                onMessage($"{info.IDs.Count()}件の動画を新たに取得しました。(現在の取得数:{ids.Count})");
                ++index;
            }
            while (info.HasNext);

            return new AttemptResult<string>() { IsSucceeded = true, Data = info.ChannelName };
        }



        /// <summary>
        /// チャンネルページのHTMlを取得する
        /// </summary>
        /// <param name="channelId"></param>
        /// <returns></returns>
        private async Task<string> GetChannelPage(string url)
        {
            string content;

            try
            {
                content = await this.http.GetStringAsync(new Uri(url));
            }
            catch (Exception e)
            {
                this.logger.Error("チャンネルページの取得に失敗しました。", e);
                throw new HttpRequestException(e.Message);
            }

            return content;
        }

        /// <summary>
        /// IDの一覧を取得する
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private IChannelPageInfo GetINfoFromHtml(string html)
        {
            return this.htmlParser.ParseAndGetIds(html);
        }
    }

    /// <summary>
    /// チャンネル動画の取得結果
    /// </summary>
    public class ChannelResult : IChannelResult
    {
        public List<IListVideoInfo> RetrievedVideos { get; init; } = new();

        public int FailedCounts { get; set; }

        public bool IsSucceededAll { get; set; }
    }
}
