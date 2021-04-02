using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Attributes;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Network;
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
        Task<IEnumerable<string>> GetVideosAsync(string channelId,IEnumerable<string> registeredVideo, Action<string> onMessage);
        Exception? CurrentException { get; }
    }

    class ChannelVideoHandler : IChannelVideoHandler
    {
        public ChannelVideoHandler(INicoHttp http, ILogger logger, IChannelPageHtmlParser htmlParser, IWatch watch)
        {
            this.http = http;
            this.htmlParser = htmlParser;
            this.logger = logger;
            this.watch = watch;
        }

        private readonly INicoHttp http;

        private readonly ILogger logger;

        private readonly IChannelPageHtmlParser htmlParser;

        private readonly IWatch watch;

        /// <summary>
        /// チャンネル動画を取得する
        /// </summary>
        /// <param name="channelId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetVideosAsync(string channelId, IEnumerable<string> registeredVideo, Action<string> onMessage)
        {
            var result = new ChannelResult();

            string html;
            try
            {
                html = await this.GetChannelPage(channelId);
            }
            catch (Exception e)
            {
                this.CurrentException = e;
                throw new HttpRequestException();
            }

            List<string> ids;
            try
            {
                ids = this.GetIdsFromHtml(html).ToList();
            }
            catch (Exception e)
            {

                this.CurrentException = e;
                throw new InvalidOperationException();
            }

            return ids;
        }


        /// <summary>
        /// 直近の例外
        /// </summary>
        public Exception? CurrentException { get; private set; }


        /// <summary>
        /// チャンネルページのHTMlを取得する
        /// </summary>
        /// <param name="channelId"></param>
        /// <returns></returns>
        private async Task<string> GetChannelPage(string channelId)
        {
            string content;
            try
            {
                content = await this.http.GetStringAsync(new Uri($"https://ch.nicovideo.jp/{channelId}"));
            }
            catch (Exception e)
            {
                this.logger.Error("チャンネルページの取得に失敗しました。", e);
                throw new HttpRequestException("チャンネルページの取得に失敗しました。");
            }

            return content;
        }

        /// <summary>
        /// IDの一覧を取得する
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private IEnumerable<string> GetIdsFromHtml(string html)
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
