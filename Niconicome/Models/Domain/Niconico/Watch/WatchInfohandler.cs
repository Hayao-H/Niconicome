using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Niconico.Net.Html;
using Niconicome.Models.Domain.Niconico.Net.Json;
using DmcRequest = Niconicome.Models.Domain.Niconico.Net.Json.WatchPage.DMC.Request;
using Utils = Niconicome.Models.Domain.Utils;
using WatchJson = Niconicome.Models.Domain.Niconico.Net.Json.WatchPage;

namespace Niconicome.Models.Domain.Niconico.Watch
{
    public interface IWatchInfohandler
    {
        Task<IDomainVideoInfo> GetVideoInfoAsync(string id, WatchInfoOptions options);
        WatchInfoHandlerState State { get; }
    }

    public interface IDomainVideoInfo
    {
        string Title { get; set; }
        string Id { get; set; }
        int ViewCount { get; }
        IEnumerable<string> Tags { get; set; }
        IDmcInfo DmcInfo { get; set; }
    }

    public interface IDmcInfo
    {
        string Title { get; set; }
        string Id { get; set; }
        string Owner { get; set; }
        string Userkey { get; }
        string UserId { get; }
        int ViewCount { get; }
        int Duration { get; }
        IEnumerable<string> Tags { get; set; }
        bool IsDownloadsble { get; set; }
        bool IsEncrypted { get; set; }
        DateTime UploadedOn { get; set; }
        IThumbInfo ThumbInfo { get; }
        ISessionInfo SessionInfo { get; }
        List<WatchJson::CommentThread> CommentThreads { get; }
    }

    public interface IThumbInfo
    {
        string? Large { get; set; }
        string? Normal { get; set; }
    }

    public interface ISessionInfo
    {
        string? RecipeId { get; set; }
        string? ContentId { get; set; }
        DmcRequest::Content_Src_Id_Sets ContentSrcIdSets { get; set; }
        int HeartbeatLifetime { get; set; }
        string? Token { get; set; }
        string? Signature { get; set; }
        string? AuthType { get; set; }
        int ContentKeyTimeout { get; set; }
        string? ServiceUserId { get; set; }
        string? PlayerId { get; set; }
        float Priority { get; set; }
    }
    public interface IWatchPageHtmlParser
    {
        IDmcInfo GetDmcInfo(string sourceHtml, WatchInfoOptions options);
        bool HasJsDataElement { get; }
    }

    public enum WatchInfoHandlerState
    {
        RequestHasNotCompleted,
        HttpRequestFailure,
        NoJsDataElement,
        JsonParsingFailure,
        OK
    }

    /// <summary>
    /// 動画情報を取得して解析する
    /// </summary>
    public class WatchInfohandler : IWatchInfohandler
    {
        public WatchInfohandler(INicoHttp http, IWatchPageHtmlParser parser)
        {
            this.http = http;
            this.parser = parser;
        }

        /// <summary>
        /// 視聴ページのパーサー
        /// </summary>
        private readonly IWatchPageHtmlParser parser;

        /// <summary>
        /// httpクライアント
        /// </summary>
        private readonly INicoHttp http;


        public WatchInfoHandlerState State { get; private set; } = WatchInfoHandlerState.RequestHasNotCompleted;

        /// <summary>
        /// 動画情報を取得する
        /// </summary>
        /// <returns></returns>
        public async Task<IDomainVideoInfo> GetVideoInfoAsync(string id, WatchInfoOptions options)
        {
            string source;
            var logger = Utils::DIFactory.Provider.GetRequiredService<Utils::ILogger>();
            Uri url = NiconicoContext.Context.GetPageUri(id);

            try
            {
                source = await this.http.GetStringAsync(url);
            }
            catch (Exception e)
            {
                logger.Error($"動画情報の取得に失敗しました(url: {url.AbsoluteUri})", e);
                this.State = WatchInfoHandlerState.HttpRequestFailure;
                throw new HttpRequestException();
            }

            IDmcInfo info;

            try
            {
                //htmlをパース
                info = this.parser.GetDmcInfo(source,options);
            }
            catch (Exception e)
            {
                if (!this.parser.HasJsDataElement)
                {
                    this.State = WatchInfoHandlerState.NoJsDataElement;
                }
                else
                {
                    this.State = WatchInfoHandlerState.JsonParsingFailure;
                }
                logger.Error($"視聴ページの解析に失敗しました。(id:{id})", e);
                throw new InvalidOperationException();

            }

            this.State = WatchInfoHandlerState.OK;
            return new DomainVideoInfo() { Id = info.Id, Title = info.Title, Tags = info.Tags, DmcInfo = info, ViewCount = info.ViewCount };
        }
    }

    /// <summary>
    /// 視聴ページのhtmlを解析する
    /// </summary>
    public class WatchPageHtmlParser : IWatchPageHtmlParser
    {

        public bool HasJsDataElement { get; private set; }

        /// <summary>
        /// DMCinfo型のインスタンスを取得する
        /// </summary>
        /// <param name="sourceHtml"></param>
        /// <returns></returns>
        public IDmcInfo GetDmcInfo(string sourceHtml, WatchInfoOptions options)
        {
            var document = HtmlParser.ParseDocument(sourceHtml);
            return this.ConvertToDmcData(this.GetApiData(document),options);
        }

        /// <summary>
        /// 視聴ページのapiデータをデシリアライズする
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        private WatchJson::DataApiData GetApiData(IHtmlDocument document)
        {
            var element = document.GetElementById("js-initial-watch-data");
            if (element is null)
            {
                this.WhenNoJsDataElement();
            }
            else
            {
                this.HasJsDataElement = true;
            }

            string sourceJson = element!.GetAttribute("data-api-data");
            WatchJson::DataApiData data = JsonParser.DeSerialize<WatchJson::DataApiData>(sourceJson);
            return data!;
        }

        private void WhenNoJsDataElement()
        {
            this.HasJsDataElement = false;
            throw new InvalidOperationException("ページ内でAPIデータを発見できませんでした。サーバーエラー・権利のない有料動画などの原因が考えられます。");
        }

        /// <summary>
        /// apiデータを変換する
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        private IDmcInfo ConvertToDmcData(WatchJson::DataApiData original, WatchInfoOptions options)
        {
            var info = new DmcInfo
            {
                //タイトル
                Title = original?.Video?.Title ?? string.Empty,
                //ID
                Id = original?.Video?.Id ?? string.Empty
            };

            //投稿日解析
            if (original is not null && original.Video is not null && original.Video.PostedDateTime is not null)
            {
                var result = DateTime.TryParseExact(original.Video.PostedDateTime, "yyyy/MM/dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime d);
                if (result) info.UploadedOn = d;
            }

            //サムネイル
            info.ThumbInfo.Large = original?.Video?.LargeThumbnailUrl ?? string.Empty;
            info.ThumbInfo.Normal = original?.Video?.ThumbnailUrl ?? string.Empty;

            //投稿者
            info.Owner = original?.Owner?.Nickname ?? string.Empty;

            //タグ
            info.Tags = original?.Tags?.Select(t => t.Name ?? string.Empty).Where(t => !t.IsNullOrEmpty()) ?? new List<string>();

            //再生回数
            info.ViewCount = original?.Video?.ViewCount ?? 0;

            //コメント情報
            info.CommentThreads = original?.CommentComposite?.Threads ?? new List<WatchJson::CommentThread>();

            //ユーザー情報
            info.UserId = original?.Video?.DmcInfo?.User?.UserId.ToString() ?? string.Empty;
            info.Userkey = original?.Context?.Userkey ?? string.Empty;

            //時間
            info.Duration = original?.Video?.Duration ?? 0;

            //Session情報
            if (!options.HasFlag(WatchInfoOptions.NoDmcData) && original?.Video?.DmcInfo?.SessionApi is not null)
            {
                info.SessionInfo.RecipeId = original?.Video?.DmcInfo?.SessionApi?.RecipeId;
                info.SessionInfo.ContentId = original?.Video?.DmcInfo?.SessionApi?.ContentId;
                info.SessionInfo.HeartbeatLifetime = original?.Video?.DmcInfo?.SessionApi?.HeartbeatLifetime ?? 0;
                info.SessionInfo.Token = original?.Video?.DmcInfo?.SessionApi?.Token;
                info.SessionInfo.Signature = original?.Video?.DmcInfo?.SessionApi?.Signature;
                info.SessionInfo.AuthType = original?.Video?.DmcInfo?.SessionApi?.AuthTypes?.Http;
                info.SessionInfo.ContentKeyTimeout = original?.Video?.DmcInfo?.SessionApi?.ContentKeyTimeout ?? 0;
                info.SessionInfo.ServiceUserId = original?.Video?.DmcInfo?.SessionApi?.ServiceUserId;
                info.SessionInfo.PlayerId = original?.Video?.DmcInfo?.SessionApi?.PlayerId;
                info.SessionInfo.Priority = original?.Video?.DmcInfo?.SessionApi?.Priority ?? 1f;
                info.SessionInfo.ContentSrcIdSets = this.GetContentSrcIdSets(original?.Video?.DmcInfo?.SessionApi);
                info.IsDownloadsble = original?.Video?.DmcInfo?.Encryption == null;
            }
            else
            {
                info.IsDownloadsble = false;
            }

            //暗号化動画の場合はダウンロード不可
            if (original?.Video?.DmcInfo?.Encryption is not null)
            {
                info.IsDownloadsble = false;
                info.IsEncrypted = true;
            }

            return info;
        }

        /// <summary>
        /// Content_Src_Id_Setsを構成する
        /// </summary>
        /// <param name="sessionApiData"></param>
        /// <returns></returns>
        private DmcRequest::Content_Src_Id_Sets GetContentSrcIdSets(WatchJson::SessionApi? sessionApiData)
        {
            if (sessionApiData is null) throw new InvalidOperationException("SessionAPIDataがnullです。");
            if (sessionApiData.Videos is null) throw new InvalidOperationException($"SessionAPIDataのVideosプロパティーがnullです。");
            if (sessionApiData.Audios is null) throw new InvalidOperationException($"SessionAPIDataのAudiosプロパティーがnullです。");
            if (sessionApiData.Audios.Count == 0) throw new InvalidOperationException($"SessionAPIDataのAudiosにデータが存在しません。");

            var videoSrc = sessionApiData.Videos.Select((value, index) => new { value, index }).ToList();
            string audio = sessionApiData.Audios[0];
            var sets = new DmcRequest::Content_Src_Id_Sets();

            foreach (var video in videoSrc)
            {
                var idsData = new DmcRequest::Content_Src_Ids();
                idsData.Src_id_to_mux.Audio_src_ids.Add(audio);
                int videosCount = videoSrc.Count - video.index;

                foreach (var i in Enumerable.Range(0, videosCount))
                {
                    idsData.Src_id_to_mux.Video_src_ids.Add(videoSrc[i].value);
                }

                sets.Content_src_ids.Add(idsData);

            }

            return sets;
        }
    }

    /// <summary>
    /// 動画情報(ルート)
    /// </summary>
    public class DomainVideoInfo : IDomainVideoInfo
    {
        /// <summary>
        /// 動画タイトル
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 動画ID
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// ファイル名
        /// </summary>
        public string FileName { get; set; } = string.Empty;


        /// <summary>
        /// 再生回数
        /// </summary>
        public int ViewCount { get; set; }

        /// <summary>
        /// タグ
        /// </summary>
        public IEnumerable<string> Tags { get; set; } = new List<string>();

        /// <summary>
        /// DMC情報
        /// </summary>
        public IDmcInfo DmcInfo { get; set; } = new DmcInfo();


    }

    /// <summary>
    /// APi等へのアクセスに必要な情報を格納する
    /// </summary>
    public class DmcInfo : IDmcInfo
    {
        public string Title { get; set; } = string.Empty;

        public string Id { get; set; } = string.Empty;

        public string Owner { get; set; } = string.Empty;

        public string Userkey { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;

        public int ViewCount { get; set; }

        public int Duration { get; set; }

        public IEnumerable<string> Tags { get; set; } = new List<string>();

        /// <summary>
        /// ダウンロード可能フラグ
        /// </summary>
        public bool IsDownloadsble { get; set; } = true;

        /// <summary>
        /// 暗号化フラグ
        /// </summary>
       　public bool IsEncrypted { get; set; }


        /// <summary>
        /// 投稿日時
        /// </summary>
        public DateTime UploadedOn { get; set; }

        /// <summary>
        /// サムネイル
        /// </summary>
        public IThumbInfo ThumbInfo { get; private set; } = new ThumbInfo();

        /// <summary>
        /// セッション情報
        /// </summary>
        public ISessionInfo SessionInfo { get; private set; } = new SessionInfo();

        /// <summary>
        /// コメントスレッド
        /// </summary>
        public List<WatchJson::CommentThread> CommentThreads { get; set; } = new();
    }

    /// <summary>
    /// セッション情報
    /// </summary>
    public class SessionInfo : ISessionInfo
    {
        public string? RecipeId { get; set; }

        public string? ContentId { get; set; }

        public DmcRequest::Content_Src_Id_Sets ContentSrcIdSets { get; set; } = new();

        public int HeartbeatLifetime { get; set; }

        public string? Token { get; set; }

        public string? Signature { get; set; }

        public string? AuthType { get; set; }

        public int ContentKeyTimeout { get; set; }

        public string? ServiceUserId { get; set; }

        public string? PlayerId { get; set; }

        public float Priority { get; set; }
    }

    /// <summary>
    /// サムネイル情報
    /// </summary>
    public class ThumbInfo : IThumbInfo
    {
        /// <summary>
        /// 大サムネイル
        /// </summary>
        public string? Large { get; set; }

        /// <summary>
        /// 通常
        /// </summary>
        public string? Normal { get; set; }
    }



    /// <summary>
    /// オプション
    /// </summary>
    [Flags]
    public enum WatchInfoOptions
    {
        Default,
        NoDmcData,
    }
}
