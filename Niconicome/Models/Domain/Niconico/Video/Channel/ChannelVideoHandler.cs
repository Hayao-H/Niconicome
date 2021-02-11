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
    public interface IChannelVideoHandler
    {
        Task<List<ITreeVideoInfo>> GetVideosAsync(string channelId);
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
        public async Task<List<ITreeVideoInfo>> GetVideosAsync(string channelId)
        {
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

            IEnumerable<string> ids;
            try
            {
                ids = this.GetIdsFromHtml(html).ToList();
            }
            catch (Exception e)
            {

                this.CurrentException = e;
                throw new InvalidOperationException();
            }

            IEnumerable<ITreeVideoInfo> videos;
            try
            {
                videos = await this.ConvertToTreeVideoInfo(ids);
            }
            catch (Exception e)
            {
                this.CurrentException = e;
                throw new InvalidOperationException();
            }

            return videos.ToList();
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

        /// <summary>
        /// 動画情報に変換する
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        private async Task<IEnumerable<ITreeVideoInfo>> ConvertToTreeVideoInfo(IEnumerable<string> ids)
        {
            return await ids.Select(async (id, index) =>
                {
                    if (index % 4 == 0)
                    {
                        await Task.Delay(10 * 1000);
                    }
                    var videoInfo = new VIdeoInfo();
                    var result = await this.watch.TryGetVideoInfoAsync(id, videoInfo, WatchInfo::WatchInfoOptions.NoDmcData);
                    return videoInfo.ConvertToTreeVideoInfo();
                }).WhenAll();
        }
    }
}
