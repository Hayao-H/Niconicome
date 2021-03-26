using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Niconico.Net.Html;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Domain.Niconico.Net.Json.WatchPage.V2;
using Niconicome.Models.Domain.Utils;
using DmcRequest = Niconicome.Models.Domain.Niconico.Net.Json.WatchPage.DMC.Request;
using WatchJson = Niconicome.Models.Domain.Niconico.Net.Json.WatchPage.V2;

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
        string ChannelID { get; set; }
        string ChannelName { get; set; }
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
        string ChannelID { get; }
        string ChannelName { get; }
        int ViewCount { get; }
        int Duration { get; }
        IEnumerable<string> Tags { get; set; }
        bool IsDownloadsble { get; set; }
        bool IsEncrypted { get; set; }
        bool IsOfficial { get; set; }
        DateTime UploadedOn { get; set; }
        DateTime DownloadStartedOn { get; set; }
        IThumbInfo ThumbInfo { get; }
        ISessionInfo SessionInfo { get; }
        List<WatchJson::Thread> CommentThreads { get; }
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
        string? TransferPriset { get; set; }
        double Priority { get; set; }
    }
    public interface IWatchPageHtmlParser
    {
        IDmcInfo GetDmcInfo(string sourceHtml, string niconicoId, WatchInfoOptions options);
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
        public WatchInfohandler(INicoHttp http, IWatchPageHtmlParser parser, ILogger logger)
        {
            this.http = http;
            this.parser = parser;
            this.logger = logger;
        }

        /// <summary>
        /// 視聴ページのパーサー
        /// </summary>
        private readonly IWatchPageHtmlParser parser;

        /// <summary>
        /// httpクライアント
        /// </summary>
        private readonly INicoHttp http;

        private readonly ILogger logger;


        public WatchInfoHandlerState State { get; private set; } = WatchInfoHandlerState.RequestHasNotCompleted;

        /// <summary>
        /// 動画情報を取得する
        /// </summary>
        /// <returns></returns>
        public async Task<IDomainVideoInfo> GetVideoInfoAsync(string id, WatchInfoOptions options)
        {
            string source;
            Uri url = NiconicoContext.Context.GetPageUri(id);

            var res = await this.http.GetAsync(url);

            try
            {
                source = await this.http.GetStringAsync(url);
            }
            catch (Exception e)
            {
                this.logger.Error($"動画情報の取得に失敗しました(url: {url.AbsoluteUri})", e);
                this.State = WatchInfoHandlerState.HttpRequestFailure;
                throw new HttpRequestException();
            }

            IDmcInfo info;

            try
            {
                //htmlをパース
                info = this.parser.GetDmcInfo(source, id, options);
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
                this.logger.Error($"視聴ページの解析に失敗しました。(id:{id})", e);
                throw new InvalidOperationException();

            }

            this.State = WatchInfoHandlerState.OK;
            return new DomainVideoInfo() { Id = info.Id, Title = info.Title, Tags = info.Tags, DmcInfo = info, ViewCount = info.ViewCount,ChannelID = info.ChannelID,ChannelName = info.ChannelName };
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
        public IDmcInfo GetDmcInfo(string sourceHtml, string niconicoId, WatchInfoOptions options)
        {
            var document = HtmlParser.ParseDocument(sourceHtml);
            return this.ConvertToDmcData(this.GetApiData(document), niconicoId, options);
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
        private IDmcInfo ConvertToDmcData(WatchJson::DataApiData original, string niconicoId, WatchInfoOptions options)
        {
            var info = new DmcInfo
            {
                //タイトル
                Title = original?.Video?.Title ?? string.Empty,
                //ID
                Id = niconicoId,
            };

            //投稿日解析
            if (original is not null && original.Video is not null)
            {
                info.UploadedOn = original.Video.RegisteredAt.DateTime;
            }

            //サムネイル
            info.ThumbInfo.Large = original?.Video?.Thumbnail?.LargeUrl ?? (original?.Video?.Thumbnail?.Url ?? string.Empty);
            info.ThumbInfo.Normal = original?.Video?.Thumbnail?.MiddleUrl ?? (original?.Video?.Thumbnail?.Url ?? string.Empty);

            //投稿者
            info.Owner = original?.Owner?.Nickname ?? string.Empty;

            //タグ
            info.Tags = original?.Tag.Items?.Select(t => t.Name ?? string.Empty).Where(t => !t.IsNullOrEmpty()) ?? new List<string>();

            //再生回数
            info.ViewCount = original?.Video?.Count?.View ?? 0;

            //コメント情報
            info.CommentThreads = original?.Comment?.Threads ?? new List<WatchJson::Thread>();

            //ユーザー情報
            //info.UserId = original?.Media?.Delivery?.Movie?.Session?.ServiceUserId ?? string.Empty;
            info.UserId = NiconicoContext.User?.ID ?? string.Empty;
            info.Userkey = original?.Comment?.Keys?.UserKey ?? string.Empty;

            //公式フラグ
            info.IsOfficial = original?.Channel is null;

            //時間
            info.Duration = original?.Video?.Duration ?? 0;

            //チャンネル情報
            info.ChannelID = original?.Channel?.Id ?? string.Empty;
            info.ChannelName = original?.Channel?.Name ?? string.Empty;

            //Session情報
            if (!options.HasFlag(WatchInfoOptions.NoDmcData) && original?.Media?.Delivery is not null)
            {
                info.SessionInfo.RecipeId = original?.Media?.Delivery.RecipeId;
                info.SessionInfo.ContentId = original?.Media?.Delivery?.Movie?.ContentId;
                info.SessionInfo.HeartbeatLifetime = original?.Media?.Delivery?.Movie?.Session?.HeartbeatLifetime ?? 0;
                info.SessionInfo.Token = original?.Media?.Delivery?.Movie?.Session?.Token;
                info.SessionInfo.Signature = original?.Media?.Delivery?.Movie?.Session?.Signature;
                info.SessionInfo.AuthType = original?.Media?.Delivery?.Movie?.Session?.AuthTypes?.Http;
                info.SessionInfo.ContentKeyTimeout = original?.Media?.Delivery?.Movie?.Session?.ContentKeyTimeout ?? 0;
                info.SessionInfo.PlayerId = original?.Media?.Delivery?.Movie?.Session?.PlayerId;
                info.SessionInfo.Priority = Math.Floor((original?.Media?.Delivery?.Movie?.Session?.Priority ?? 1) * 10) / 10;
                info.SessionInfo.ContentSrcIdSets = this.GetContentSrcIdSets(original?.Media?.Delivery?.Movie?.Session);
                info.SessionInfo.ServiceUserId = original?.Media?.Delivery?.Movie?.Session?.ServiceUserId;
                info.SessionInfo.TransferPriset = original?.Media?.Delivery?.Movie?.Session?.TransferPrisets?.FirstOrDefault() ?? string.Empty;
                info.IsDownloadsble = original?.Media?.Delivery?.Encryption == null;
            }
            else
            {
                info.IsDownloadsble = false;
            }

            //暗号化動画の場合はダウンロード不可
            if (original?.Media?.Delivery?.Encryption is not null)
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
        private DmcRequest::Content_Src_Id_Sets GetContentSrcIdSets(WatchJson::Session? sessionApiData)
        {
            if (sessionApiData is null) throw new InvalidOperationException("SessionAPIDataがnullです。");
            if (sessionApiData.Videos is null) throw new InvalidOperationException($"SessionAPIDataのVideosプロパティーがnullです。");
            if (sessionApiData.Audios is null) throw new InvalidOperationException($"SessionAPIDataのAudiosプロパティーがnullです。");
            if (sessionApiData.Audios.Count == 0) throw new InvalidOperationException($"SessionAPIDataのAudiosにデータが存在しません。");

            sessionApiData.Videos.Sort((a, b) =>
            {
                if (a.EndsWith("_low")) return -1;
                if (b.EndsWith("_low")) return 1;
                return string.Compare(a, b);
            });
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

                //idsData.Src_id_to_mux.Video_src_ids = idsData.Src_id_to_mux.Video_src_ids.OrderByDescending(s => s).ToList();
                idsData.Src_id_to_mux.Video_src_ids.Sort((a, b) =>
                {
                    if (a.EndsWith("_low")) return 1;
                    if (b.EndsWith("_low")) return -1;
                    return string.Compare(b, a);
                });

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
        /// チャンネルID
        /// </summary>
        public string ChannelID { get; set; } = string.Empty;

        /// <summary>
        /// チャンネル名
        /// </summary>
        public string ChannelName { get; set; } = string.Empty;

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

        public string ChannelID { get; set; } = string.Empty;

        public string ChannelName { get; set; } = string.Empty;

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
        /// 公式動画フラグ
        /// </summary>
        public bool IsOfficial { get; set; }


        /// <summary>
        /// 投稿日時
        /// </summary>
        public DateTime UploadedOn { get; set; }

        /// <summary>
        /// DL開始日時
        /// </summary>
        public DateTime DownloadStartedOn { get; set; }


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
        public List<WatchJson::Thread> CommentThreads { get; set; } = new();
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

        public string? TransferPriset { get; set; }

        public int ContentKeyTimeout { get; set; }

        public string? ServiceUserId { get; set; }

        public string? PlayerId { get; set; }

        public double Priority { get; set; }
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
