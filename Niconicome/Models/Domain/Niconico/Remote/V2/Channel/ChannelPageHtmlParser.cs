using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Niconico.Net.Html;
using Niconicome.Models.Domain.Niconico.Remote.V2.Error;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Playlist;

namespace Niconicome.Models.Domain.Niconico.Remote.V2.Channel
{

    public interface IChannelPageHtmlParser
    {
        /// <summary>
        /// HTMLを解析して動画情報などを返す
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        IAttemptResult<ChannelPageInfo> GetInfomationFromChannelPage(string source);
    }

    public class ChannelPageHtmlParser : IChannelPageHtmlParser
    {
        public ChannelPageHtmlParser(IErrorHandler errorHandler)
        {
            this._errorHandler = errorHandler;
        }


        #region field

        private readonly IErrorHandler _errorHandler;

        #endregion

        #region Method

        public IAttemptResult<ChannelPageInfo> GetInfomationFromChannelPage(string source)
        {
            IHtmlDocument? document;

            try
            {
                document = HtmlParser.ParseDocument(source);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(ChannelError.FailedToAnalysis, ex);
                return AttemptResult<ChannelPageInfo>.Fail(this._errorHandler.GetMessageForResult(ChannelError.FailedToAnalysis, ex));
            }

            if (document is null)
            {
                this._errorHandler.HandleError(ChannelError.FailedToAnalysis);
                return AttemptResult<ChannelPageInfo>.Fail(this._errorHandler.GetMessageForResult(ChannelError.FailedToAnalysis));
            }

            var wrapper = document.QuerySelector(".p-channelVideo__wrapper>ul");
            if (wrapper is null)
            {
                this._errorHandler.HandleError(ChannelError.FailedToAnalysis);
                return AttemptResult<ChannelPageInfo>.Fail(this._errorHandler.GetMessageForResult(ChannelError.FailedToAnalysisForNoWrapperElement));
            }

            //チャンネル名
            var channnelName = document.QuerySelector(".channel_name>a")?.InnerHtml ?? string.Empty;

            //動画解析
            IHtmlCollection<IElement> elements = wrapper.QuerySelectorAll(".item");
            var videos = new List<VideoInfo>();

            foreach (IElement element in elements)
            {

                IElement? titleElm = element.QuerySelector(".item_right>.title>a");

                if (titleElm is null) continue;

                string? title = titleElm.InnerHtml;
                string? niconicoId = titleElm.GetAttribute("href")?.Split("/")?.LastOrDefault();
                string? thumbUrl = element.QuerySelector(".item_left>.thumb_video>img")?.GetAttribute("src");

                if (title is null || niconicoId is null || niconicoId.IsNullOrEmpty() || thumbUrl is null) continue;

                IElement? countElm = element.QuerySelector(".item_right>ul.counts");

                if (countElm is null) continue;

                int.TryParse(countElm.QuerySelector(".view>var")?.InnerHtml ?? "0", NumberStyles.AllowThousands, null, out int viewCount);
                int.TryParse(countElm.QuerySelector(".comment var")?.InnerHtml ?? "0", NumberStyles.AllowThousands, null, out int commentCount);
                int.TryParse(countElm.QuerySelector(".mylist var")?.InnerHtml ?? "0", NumberStyles.AllowThousands, null, out int mylistCount);

                string? uploadStr = element.QuerySelector(".time var")?.InnerHtml?.Trim();
                if (uploadStr is null) continue;
                DateTime.TryParseExact(uploadStr, "yyyy-MM-dd HH:mm", null, DateTimeStyles.None, out DateTime uploadedAt);

                var video = new VideoInfo()
                {
                    NiconicoID = niconicoId,
                    Title = title,
                    OwnerName = channnelName,
                    ThumbUrl = thumbUrl,
                    UploadedDT = uploadedAt,
                    ViewCount = viewCount,
                    CommentCount = commentCount,
                    MylistCount = mylistCount,
                    LikeCount = 0
                };

                videos.Add(video);
            }

            var hasNext = false;
            var nextPageQuery = "";

            //ページャー
            var pager = wrapper.QuerySelector("footer>menu.pager");
            if (pager is not null)
            {
                var next = pager.QuerySelector("li.next:not(.disabled)>a")?.GetAttribute("href");
                if (next is not null)
                {
                    hasNext = true;
                    nextPageQuery = next;
                }
            }

            return AttemptResult<ChannelPageInfo>.Succeeded(new ChannelPageInfo(videos,channnelName,hasNext,nextPageQuery));

        }

        #endregion

    }
}
