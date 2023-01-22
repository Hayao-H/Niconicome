using System;
using System.Globalization;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Niconicome.Models.Domain.Niconico.Net.Html;
using Niconicome.Models.Domain.Niconico.Remote.V2.Error;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Niconico.Remote.V2.Series
{
    public interface ISeriesPageHtmlParser
    {
        /// <summary>
        /// シリーズページを解析して情報を取得
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        IAttemptResult<RemotePlaylistInfo> GetSeriesInfo(string source);
    }

    public class SeriesPageHtmlParser : ISeriesPageHtmlParser
    {
        public SeriesPageHtmlParser(IErrorHandler errorHandler)
        {
            this._errorHandler = errorHandler;
        }

        #region field

        private readonly IErrorHandler _errorHandler;

        #endregion


        #region Method

        public IAttemptResult<RemotePlaylistInfo> GetSeriesInfo(string source)
        {
            var series = new RemotePlaylistInfo();

            IHtmlDocument? document = HtmlParser.ParseDocument(source);
            if (document is null)
            {
                this._errorHandler.HandleError(SeriesError.FailedToParseDocument);
                return AttemptResult<RemotePlaylistInfo>.Fail(this._errorHandler.GetMessageForResult(SeriesError.FailedToParseDocument));
            }

            IHtmlCollection<IElement> videos = document.QuerySelectorAll(".SeriesVideoListContainer-video");

            IElement? ownerElm = document.QuerySelector("a.SeriesAdditionalContainer-ownerName");
            string ownerName = ownerElm?.InnerHtml ?? string.Empty;
            string ownerID = ownerElm?.GetAttribute("href")?.Split("/")[^1] ?? "0";

            string seriesName = document?.QuerySelector("div.SeriesDetailContainer-bodyTitle")?.InnerHtml ?? string.Empty;
            series.PlaylistName = seriesName;

            foreach (var videoElm in videos)
            {
                //ID
                string? link = videoElm.QuerySelector("a.NC-MediaObject-contents")?.GetAttribute("href");
                if (link is null)
                {
                    continue;
                }
                else if (link.Contains("?"))
                {
                    link = link[0..link.IndexOf("?")];
                }
                string id = link.Split("/")[^1];

                //タイトル
                string title = videoElm.QuerySelector("h2.NC-VideoMediaObject-title")?.InnerHtml.Trim() ?? string.Empty;

                //サムネ
                string thumb = videoElm.QuerySelector("div.NC-Thumbnail-image")?.GetAttribute("data-background-image") ?? string.Empty;

                //投稿日時
                DateTime.TryParseExact(videoElm.QuerySelector("span.NC-VideoRegisteredAtText-text")?.InnerHtml.Trim() ?? "2000/1/1 00:00", "yyyy/M/d H:m", null, DateTimeStyles.None, out DateTime uploadedDT);

                //再生数等
                int.TryParse(videoElm.QuerySelector("div.NC-VideoMetaCount_view")?.InnerHtml ?? "0", NumberStyles.AllowThousands, null, out int viewCount);
                int.TryParse(videoElm.QuerySelector("div.NC-VideoMetaCount_mylist")?.InnerHtml ?? "0", NumberStyles.AllowThousands, null, out int mylistCount);
                int.TryParse(videoElm.QuerySelector("div.NC-VideoMetaCount_comment")?.InnerHtml ?? "0", NumberStyles.AllowThousands, null, out int commentCount);
                int.TryParse(videoElm.QuerySelector("div.NC-VideoMetaCount_like")?.InnerHtml ?? "0", NumberStyles.AllowThousands, null, out int likeCount);

                var videoinfo = new VideoInfo()
                {
                    Title = title,
                    NiconicoID = id,
                    OwnerID = ownerID,
                    OwnerName = ownerName,
                    UploadedDT = uploadedDT,
                    ViewCount = viewCount,
                    CommentCount = commentCount,
                    MylistCount = mylistCount,
                    ThumbUrl = thumb,
                    LikeCount = likeCount,
                };

                series.Videos.Add(videoinfo);

            }

            return AttemptResult<RemotePlaylistInfo>.Succeeded(series);
        }

        #endregion
    }
}
