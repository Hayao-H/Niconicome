using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Attributes;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Domain.Niconico.Remote.V2.Error;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Domain.Utils.StringHandler;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Network.Watch;
using Niconicome.Models.Playlist;
using WatchInfo = Niconicome.Models.Domain.Niconico.Watch;

namespace Niconicome.Models.Domain.Niconico.Remote.V2.Channel
{
    public interface IChannelVideoHandler
    {
        /// <summary>
        /// チャンネルから動画を取得する
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        Task<IAttemptResult<RemotePlaylistInfo>> GetVideosAsync(string channelId, Action<string, ErrorLevel> onMessage);
    }

    class ChannelVideoHandler : IChannelVideoHandler
    {
        public ChannelVideoHandler(INicoHttp http, IChannelPageHtmlParser htmlParser,IErrorHandler errorHandler,IStringHandler stringHandler)
        {
            this.http = http;
            this._htmlParser = htmlParser;
            this._errorHandler = errorHandler;
            this._stringHandler = stringHandler;
        }

        #region field

        private readonly INicoHttp http;

        private readonly IChannelPageHtmlParser _htmlParser;

        private readonly IErrorHandler _errorHandler;

        private readonly IStringHandler _stringHandler;

        #endregion

        #region Method

        public async Task<IAttemptResult<RemotePlaylistInfo>> GetVideosAsync(string channelId, Action<string, ErrorLevel> onMessage)
        {

            var videos = new List<VideoInfo>();
            var index = 0;
            var channelName = "";
            var baseUrl = $"https://ch.nicovideo.jp/{channelId}/video";
            var urlQuery = string.Empty;
            var hasNext = true;
            const int fetchWaitSpan = 5;
            const int fetchWaitTime = 10;

            this._errorHandler.HandleError(ChannelError.ChannnelInfomationRetrievingHasStarted, channelId);

            do
            {
                if ((index + 1) % fetchWaitSpan == 0)
                {
                    onMessage(this._stringHandler.GetContent(ChannelStringContents.Waiting, fetchWaitTime), ErrorLevel.Log);
                    await Task.Delay(fetchWaitTime * 1000);
                }

                onMessage(this._stringHandler.GetContent(ChannelStringContents.ChannnelPageRetrievingHasStarted, index + 1), ErrorLevel.Log);

                IAttemptResult<string> pageResult = await this.GetChannelPageAsync(baseUrl + urlQuery);
                if (!pageResult.IsSucceeded || pageResult.Data is null)
                {
                    return AttemptResult<RemotePlaylistInfo>.Fail(pageResult.Message);
                }

                IAttemptResult<ChannelPageInfo> parseResult = this._htmlParser.GetInfomationFromChannelPage(pageResult.Data);
                if (!parseResult.IsSucceeded || parseResult.Data is null)
                {
                    return AttemptResult<RemotePlaylistInfo>.Fail(parseResult.Message);
                }

                urlQuery = parseResult.Data.NextPageQuery;
                hasNext = parseResult.Data.HasNext;
                channelName = parseResult.Data.ChannnelName;

                videos.AddRange(parseResult.Data.Videos);
                onMessage(this._stringHandler.GetContent(ChannelStringContents.ChannnelPageRetrievingHasCompleted, index + 1, parseResult.Data.Videos.Count, videos.Count), ErrorLevel.Log);
                ++index;
            }
            while (hasNext);

            this._errorHandler.HandleError(ChannelError.ChannnelInfomationRetrievingHasCompleted, channelId);

            return AttemptResult<RemotePlaylistInfo>.Succeeded(new RemotePlaylistInfo() { PlaylistName = channelName, Videos = videos });
        }

        #endregion

        #region private

        /// <summary>
        /// チャンネルページのHTMlを取得する
        /// </summary>
        /// <param name="channelId"></param>
        /// <returns></returns>
        private async Task<IAttemptResult<string>> GetChannelPageAsync(string url)
        {
            HttpResponseMessage res = await this.http.GetAsync(new Uri(url));

            if (!res.IsSuccessStatusCode)
            {
                this._errorHandler.HandleError(ChannelError.FailedToRetrievingPage, url, (int)res.StatusCode);
                return AttemptResult<string>.Fail(this._errorHandler.GetMessageForResult(ChannelError.FailedToRetrievingPage, url, (int)res.StatusCode));
            }

            return AttemptResult<string>.Succeeded(await res.Content.ReadAsStringAsync());
        }

        #endregion
    }
}
