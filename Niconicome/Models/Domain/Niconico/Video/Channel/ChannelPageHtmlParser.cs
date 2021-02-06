using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Html.Dom;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Niconico.Net.Html;
using Niconicome.Models.Domain.Utils;

namespace Niconicome.Models.Domain.Niconico.Video.Channel
{

    public interface IChannelPageHtmlParser
    {
        IEnumerable<string> ParseAndGetIds(string html);
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
        public IEnumerable<string> ParseAndGetIds(string html)
        {
            IHtmlDocument document;
            try
            {
                document = HtmlParser.ParseDocument(html);
            }
            catch (Exception e)
            {
                this.logger.Error("チャンネルページの解析に失敗しました。", e);
                throw new InvalidOperationException("チャンネルページの解析に失敗しました。");
            }

            var wrapper = document.GetElementsByClassName("g-videolist").FirstOrDefault();
            if (wrapper is null)
            {
                throw new InvalidOperationException(".g-videolistを発見できませんでした。");
            }
            var elements = wrapper.QuerySelectorAll(".g-video-link");

            var ids = elements.Select((element) =>
           {
               string href = element.GetAttribute("href");
               int lastindex = href.LastIndexOf("/");
               if (lastindex == -1) return string.Empty;
               return href[(lastindex + 1)..];
           }).Where(i => !i.IsNullOrEmpty()).Distinct();

            return ids;

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
