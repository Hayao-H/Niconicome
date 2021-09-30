using System;
using System.Globalization;
using System.Linq;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Niconicome.Models.Domain.Niconico.Net.Html;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Niconico.Remote.Series
{
    public interface ISeriesPageHtmlParser
    {
        IAttemptResult<RemotePlaylistInfo> GetSeriesInfo(string source);
    }

    public class SeriesPageHtmlParser : ISeriesPageHtmlParser
    {
        public SeriesPageHtmlParser(INiconicoUtils niconicoUtils)
        {
            this.niconicoUtils = niconicoUtils;
        }

        #region field

        private readonly INiconicoUtils niconicoUtils;

        #endregion

        /// <summary>
        /// シリーズページを解析して情報を取得
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public IAttemptResult<RemotePlaylistInfo> GetSeriesInfo(string source)
        {

            RemotePlaylistInfo info;

            try
            {
                info = this.AnalyzePage(source);
            }
            catch (Exception e)
            {
                return new AttemptResult<RemotePlaylistInfo>() { Exception = e, Message = $"シリーズページ情報の解析に失敗しました。(詳細：{e.Message})" };
            }

            return new AttemptResult<RemotePlaylistInfo>() { IsSucceeded = true, Data = info };

        }

        /// <summary>
        /// ページを解析してJSONを取得する
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private RemotePlaylistInfo AnalyzePage(string source)
        {
            var series = new RemotePlaylistInfo();

            IHtmlDocument document = HtmlParser.ParseDocument(source);
            IHtmlCollection<IElement> videos = document.QuerySelectorAll(".SeriesVideoListContainer-video");

            IElement? ownerElm = document.QuerySelector(".SeriesAdditionalContainer-ownerName");
            string ownerName = ownerElm?.InnerHtml ?? string.Empty;
            int ownerID = int.Parse((ownerElm?.GetAttribute("href").Split("/")[^1]) ?? "0");

            string seriesName = document.QuerySelector(".SeriesDetailContainer-bodyTitle")?.InnerHtml ?? string.Empty;
            series.PlaylistName = seriesName;

            foreach (var videoElm in videos)
            {
                IElement? link = videoElm.QuerySelector("a.NC-MediaObject-contents");
                string? id = this.niconicoUtils.GetNiconicoIdsFromText(link?.GetAttribute("href") ?? string.Empty).FirstOrDefault();

                if (string.IsNullOrEmpty(id)) continue;

                string title = videoElm.QuerySelector(".NC-VideoMediaObject-title")?.InnerHtml.Trim() ?? string.Empty;
                string thumb = videoElm.QuerySelector(".NC-Thumbnail-image")?.GetAttribute("data-background-image") ?? string.Empty;

                DateTime uploadedDT = DateTime.ParseExact(videoElm.QuerySelector(".NC-VideoRegisteredAtText-text")?.InnerHtml.Trim() ?? "2000/1/1 00:00", "yyyy/M/d H:m", null);

                long.TryParse(videoElm.QuerySelector(".NC-VideoMetaCount_view")?.InnerHtml ?? "0", NumberStyles.AllowThousands, null, out long viewCount);
                long.TryParse(videoElm.QuerySelector(".NC-VideoMetaCount_mylist")?.InnerHtml ?? "0", NumberStyles.AllowThousands, null, out long mylistCount);
                long.TryParse(videoElm.QuerySelector(".NC-VideoMetaCount_comment")?.InnerHtml ?? "0", NumberStyles.AllowThousands, null, out long commentCount);

                var videoinfo = new VideoInfo()
                {
                    Title = title,
                    ID = id,
                    OwnerID = ownerID,
                    OwnerName = ownerName,
                    UploadedDT = uploadedDT,
                    ViewCount = viewCount,
                    CommentCount = commentCount,
                    MylistCount = mylistCount,
                    ThumbUrl = thumb
                };

                series.Videos.Add(videoinfo);

            }

            return series;
        }
    }
}
