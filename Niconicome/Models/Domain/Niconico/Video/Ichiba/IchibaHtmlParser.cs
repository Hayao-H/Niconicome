using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using Niconicome.Models.Domain.Niconico.Net.Html;
using Niconicome.Models.Helper.Result.Generic;

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
            IHtmlDocument document;
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
        private INiconicoIchibaInfo GetNiconicoIchibaInfo(IHtmlDocument document)
        {
            var items = document.QuerySelectorAll(".IchibaMainItem");
            var info = new NiconicoIchibaInfo();

            foreach (var item in items)
            {
                var titleElm = item.QuerySelector(".IchibaMainItem_Name");
                var title = titleElm.InnerHtml;
                var link = titleElm.GetAttribute("href");
                var category = item.QuerySelector(".IchibaMainItem_Info_Category").InnerHtml;
                var price = item.QuerySelector(".IchibaMainItem_Price_Number")?.InnerHtml??"NaN";
                var thumb = item.QuerySelector(".IchibaMainItem_Thumbnail img").GetAttribute("src");
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
