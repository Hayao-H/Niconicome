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
using Niconicome.Models.Network.Watch;
using Niconicome.Models.Playlist;
using WatchInfo = Niconicome.Models.Domain.Niconico.Watch;

namespace Niconicome.Models.Domain.Niconico.Remote.Channel
{
    public interface IChannelVideoHandler
    {
        /// <summary>
        /// チャンネルから動画を取得する
        /// </summary>
        /// <param name="channelId">チャンネルID</param>
        /// <param name="videos">取得した動画を追加するリスト</param>
        /// <param name="onMessage">メッセージハンドラー</param>
        /// <returns>チャンネル名と成功状態</returns>
        Task<IAttemptResult<string>> GetVideosAsync(string channelId, List<IListVideoInfo> videos, Action<string> onMessage);
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
        public async Task<IAttemptResult<string>> GetVideosAsync(string channelId, List<IListVideoInfo> videos, Action<string> onMessage)
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
                videos.AddRange(info.Videos);
                onMessage($"{info.Videos.Count()}件の動画を新たに取得しました。(現在の取得数:{videos.Count})");
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
}
