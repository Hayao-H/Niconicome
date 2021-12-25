using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Niconicome.Models.Domain.Niconico.Net.Html;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Niconico.Video.Ichiba
{
    public interface IIchibaHtmlParser
    {
        IAttemptResult<INiconicoIchibaInfo> ParseHtml(string source);
    }

    /// <summary>
    /// 市場情報の解析を担当する
    /// </summary>
    public class IchibaHtmlParser : IIchibaHtmlParser
    {
        /// <summary>
        /// 市場情報のHTMlを解析する
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public IAttemptResult<INiconicoIchibaInfo> ParseHtml(string source)
        {
            IHtmlDocument? document;
            try
            {
                document = HtmlParser.ParseDocument(source);
            }
            catch (Exception e)
            {
                return new AttemptResult<INiconicoIchibaInfo>() { Exception = e, Message = "市場ページのHtml解析中にエラーが発生しました。" };
            }

            INiconicoIchibaInfo ichibaInfo;
            try
            {
                ichibaInfo = this.GetNiconicoIchibaInfo(document);
            }
            catch (Exception e)
            {
                return new AttemptResult<INiconicoIchibaInfo>() { Exception = e, Message = "市場ページのHtmlを詳細解析中にエラーが発生しました。" };
            }

            return new AttemptResult<INiconicoIchibaInfo>() { IsSucceeded = true, Data = ichibaInfo };
        }

        /// <summary>
        /// documentを受け取ってJSっぽく解析する
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        private INiconicoIchibaInfo GetNiconicoIchibaInfo(IHtmlDocument? document)
        {
            IHtmlCollection<IElement>? items = document?.QuerySelectorAll(".IchibaMainItem");
            var info = new NiconicoIchibaInfo();

            if (items is null) return info;

            foreach (var item in items)
            {
                IElement? titleElm = item.QuerySelector(".IchibaMainItem_Name");
                string? title = titleElm?.InnerHtml;
                string? link = titleElm?.GetAttribute("href");
                string? category = item?.QuerySelector(".IchibaMainItem_Info_Category")?.InnerHtml;
                string price = item?.QuerySelector(".IchibaMainItem_Price_Number")?.InnerHtml ?? "NaN";
                string? thumb = item?.QuerySelector(".IchibaMainItem_Thumbnail img")?.GetAttribute("src");

                if (title is null || link is null || category is null || price is null || thumb is null) continue;

                var itemInfo = new IchibaItem()
                {
                    Name = title,
                    LinkUrl = link,
                    Category = category,
                    Price = price,
                    ThumbUrl = thumb,
                };
                info.IchibaItems.Add(itemInfo);
            }

            return info;
        }
    }
}
