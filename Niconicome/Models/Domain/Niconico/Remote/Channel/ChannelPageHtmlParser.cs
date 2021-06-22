using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Html.Dom;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Niconico.Net.Html;
using Niconicome.Models.Domain.Utils;

namespace Niconicome.Models.Domain.Niconico.Remote.Channel
{

    public interface IChannelPageHtmlParser
    {
        IChannelPageInfo ParseAndGetIds(string html);
    }

    public class ChannelPageHtmlParser : IChannelPageHtmlParser
    {
        public ChannelPageHtmlParser(ILogger logger, INiconicoUtils utils)
        {
            this.logger = logger;
            this.utils = utils;
        }


        /// <summary>
        /// ロガー
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// ユーティリティー
        /// </summary>
        private readonly INiconicoUtils utils;

        /// <summary>
        /// htmlを解析してIDのリストを返す
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
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
                this.logger.Error("チャンネルページの解析に失敗しました。", e);
                throw new InvalidOperationException("チャンネルページの解析に失敗しました。");
            }

            var wrapper = document.QuerySelectorAll(".items").FirstOrDefault();
            if (wrapper is null)
            {
                throw new InvalidOperationException(".itemsを発見できませんでした。");
            }
            var elements = wrapper.QuerySelectorAll(".thumb_video");

            var ids = elements.Select((element) =>
           {
               string href = element.GetAttribute("href");
               int lastindex = href.LastIndexOf("/");
               if (lastindex == -1) return string.Empty;
               return href[(lastindex + 1)..];
           }).Where(i => !i.IsNullOrEmpty()).Distinct();

            info.IDs = ids;

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

            var cName = document.QuerySelector(".channel_name")?.Children.FirstOrDefault()?.TextContent ?? string.Empty;
            info.ChannelName = cName;

            return info;

        }

        /// <summary>
        /// htmlからidを抽出する
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private IEnumerable<string> GetIdsFromText(string html)
        {
            return this.utils.GetNiconicoIdsFromText(html);
        }
    }
}
