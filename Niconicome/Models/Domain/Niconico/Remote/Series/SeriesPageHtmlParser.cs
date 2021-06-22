using System;
using System.Globalization;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Niconicome.Models.Domain.Niconico.Net.Html;
using Niconicome.Models.Helper.Result.Generic;

namespace Niconicome.Models.Domain.Niconico.Remote.Series
{
    public interface ISeriesPageHtmlParser
    {
        IAttemptResult<RemotePlaylistInfo> GetSeriesInfo(string source);
    }

    public class SeriesPageHtmlParser : ISeriesPageHtmlParser
    {
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
                string id = videoElm.GetAttribute("data-video-itemdata-video-id");

                if (string.IsNullOrEmpty(id)) continue;

                string title = videoElm.QuerySelector(".VideoMediaObject-title a")?.InnerHtml.Trim() ?? string.Empty;

                DateTime uploadedDT = DateTime.ParseExact(videoElm.QuerySelector(".SeriesVideoListContainer-videoRegisteredAt")?.InnerHtml.Trim() ?? "2000/01/01 00:00 投稿", "yyyy/MM/dd HH:mm 投稿", null);

                long.TryParse(videoElm.QuerySelector(".VideoMetaCount-view")?.InnerHtml ?? "0", NumberStyles.AllowThousands, null, out long viewCount);
                long.TryParse(videoElm.QuerySelector(".VideoMetaCount-mylist")?.InnerHtml ?? "0", NumberStyles.AllowThousands, null, out long mylistCount);
                long.TryParse(videoElm.QuerySelector(".VideoMetaCount-comment")?.InnerHtml ?? "0", NumberStyles.AllowThousands, null, out long commentCount);

                var videoinfo = new VideoInfo()
                {
                    Title = title,
                    ID = id,
                    UserID = ownerID,
                    UserName = ownerName,
                    UploadedDT = uploadedDT,
                    ViewCount = viewCount,
                    CommentCount = commentCount,
                    MylistCount = mylistCount,
                };

                series.Videos.Add(videoinfo);

            }

            return series;
        }
    }
}
