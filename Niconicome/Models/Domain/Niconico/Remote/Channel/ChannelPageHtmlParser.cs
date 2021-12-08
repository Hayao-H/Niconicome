using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Niconico.Net.Html;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Playlist;

namespace Niconicome.Models.Domain.Niconico.Remote.Channel
{

    public interface IChannelPageHtmlParser
    {
        /// <summary>
        /// HTMLを解析して動画情報などを返す
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        IChannelPageInfo ParseAndGetIds(string html);
    }

    public class ChannelPageHtmlParser : IChannelPageHtmlParser
    {
        public ChannelPageHtmlParser(ILogger logger, INiconicoUtils utils, IVideoInfoContainer container)
        {
            this._logger = logger;
            this._utils = utils;
            this._container = container;
        }


        #region field

        private readonly ILogger _logger;

        private readonly INiconicoUtils _utils;

        private readonly IVideoInfoContainer _container;

        #endregion

        #region Method

        public IChannelPageInfo ParseAndGetIds(string html)
        {
            IHtmlDocument document;
            var info = new ChannelPageInfo();

            try
            {
                document = HtmlParser.ParseDocument(html);
            }
            catch (Exception e)
            {
                this._logger.Error("チャンネルページの解析に失敗しました。", e);
                throw new InvalidOperationException("チャンネルページの解析に失敗しました。");
            }

            var wrapper = document.QuerySelectorAll(".items").FirstOrDefault();
            if (wrapper is null)
            {
                throw new InvalidOperationException(".itemsを発見できませんでした。");
            }

            //チャンネル名
            var cName = document.QuerySelector(".channel_name")?.Children.FirstOrDefault()?.TextContent ?? string.Empty;
            info.ChannelName = cName;

            //動画解析
            IHtmlCollection<IElement> elements = wrapper.QuerySelectorAll(".item");
            var videos = new List<IListVideoInfo>();

            foreach (var element in elements)
            {
                try
                {
                    IElement titleElm = element.QuerySelector(".title a");
                    string title = titleElm.InnerHtml;
                    string niconicoId = titleElm.GetAttribute("href").Split("/").LastOrDefault() ?? "";

                    string thumbUrl = element.QuerySelector(".thumb_video img").GetAttribute("src");

                    IElement countElm = element.QuerySelector(".counts");

                    int viewCount = int.Parse(countElm.QuerySelector(".view var").InnerHtml, NumberStyles.AllowThousands);
                    int commentCount = int.Parse(countElm.QuerySelector(".comment var").InnerHtml, NumberStyles.AllowThousands);
                    int mylistCount = int.Parse(countElm.QuerySelector(".mylist var").InnerHtml, NumberStyles.AllowThousands);

                    DateTime uploadedAt = DateTime.ParseExact(element.QuerySelector(".time var").InnerHtml.Trim(), "yyyy-MM-dd HH:mm", null);

                    if (niconicoId.IsNullOrEmpty()) continue;

                    IListVideoInfo video = this._container.GetVideo(niconicoId);
                    video.Title.Value = title;
                    video.NiconicoId.Value = niconicoId;
                    video.ViewCount.Value = viewCount;
                    video.CommentCount.Value = commentCount;
                    video.MylistCount.Value = mylistCount;
                    video.UploadedOn.Value = uploadedAt;
                    video.ThumbUrl.Value = thumbUrl;

                    videos.Add(video);
                }
                catch (Exception e)
                {
                    this._logger.Error($"チャンネル情報を解析中にエラが発生しました。(channel:{cName})", e);
                }
            }

            info.Videos = videos;

            //ページャー
            var pager = document.QuerySelector(".pager");
            if (pager is not null)
            {
                var next = pager.QuerySelector(".next")?.Children?.FirstOrDefault()?.GetAttribute("href");
                if (next is not null && !next.Contains("最後のページです"))
                {
                    info.HasNext = true;
                    info.NextPageQuery = next;
                }
            }

            return info;

        }

        #endregion

    }
}
