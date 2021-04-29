﻿using System;
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
            try
            {
                html = await this.GetChannelPage(channelId);
            }
            catch (Exception e)
            {
                return new AttemptResult<string>() { Message = $"チャンネルページ取得に失敗しました。(詳細: {e.Message}" };
            }

            try
            {
                var retlieved = this.GetIdsFromHtml(html);
                ids.AddRange(retlieved);
            }
            catch (Exception e)
            {
                return new AttemptResult<string>() { Message = $"チャンネルページの解析に失敗しました。(詳細: {e.Message}" };
            }

            return new AttemptResult<string>() { IsSucceeded = true };
        }



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
                throw new HttpRequestException(e.Message);
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
