using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Dmc = Niconicome.Models.Domain.Niconico.Dmc;
using DmcRequest = Niconicome.Models.Domain.Niconico.Net.Json.WatchPage.DMC.Request;
using DmcResponse = Niconicome.Models.Domain.Niconico.Net.Json.WatchPage.DMC.Response;
using Utils = Niconicome.Models.Domain.Utils;
using Video = Niconicome.Models.Domain.Niconico.Video.Infomations;

namespace Niconicome.Models.Domain.Niconico.Watch
{
    public interface IWatchSession : IDisposable
    {
        Task EnsureSessionAsync(string id, bool isVideoDownload);
        Task GetVideoDataAsync(string id, bool isVideoDownload);
        bool IsSessionEnsured { get; }
        bool IsSessionExipired { get; }
        string PlaylistUrl { get; }
        WatchSessionState State { get; }
        public IDomainVideoInfo? Video { get; }

        Task<Dmc::IStreamsCollection> GetAvailableStreamsAsync();
    }

    public interface IWatchSessionInfo
    {
        string DmcResponseJsonData { get; }
        string ContentUrl { get; }
        string SessionId { get; }
    }

    public interface IDmcDataHandler
    {
        DmcRequest::DmcPostData GetPostData(IDomainVideoInfo videoInfo);
        Task<IWatchSessionInfo> GetSessionInfoAsync(DmcRequest::DmcPostData dmcPostData);
        Task<IWatchSessionInfo> GetSessionInfoAsync(IDomainVideoInfo videoinfo);
    }

    public enum WatchSessionState
    {
        NotInitialized,
        HttpRequestFailure,
        PageAnalyzingFailure,
        EncryptedVideo,
        AuthenticationFailure,
        PaymentNeeded,
        SessionEnsuringFailure,
        UnknownError,
        GotPage,
        OK
    }

    /// <summary>
    /// 視聴セッション全般を管理する
    /// </summary>
    public class WatchSession : IWatchSession
    {
        public WatchSession(IWatchInfohandler watchInfo, INicoHttp http, Utils::ILogger logger, IDmcDataHandler dmchandler, IWatchPlaylisthandler playlisthandler)
        {
            this.watchInfo = watchInfo;
            this.http = http;
            this.logger = logger;
            this.dmchandler = dmchandler;
            this.playlisthandler = playlisthandler;
        }

        ~WatchSession()
        {
            this.IsSessionEnsured = false;
        }

        /// <summary>
        /// ロガー
        /// </summary>
        private readonly Utils::ILogger logger;

        /// <summary>
        /// DMC
        /// </summary>
        private readonly IDmcDataHandler dmchandler;

        /// <summary>
        /// 視聴情報取得
        /// </summary>
        private readonly IWatchInfohandler watchInfo;

        private readonly IWatchPlaylisthandler playlisthandler;

        /// <summary>
        /// httpクライアント
        /// </summary>
        private readonly INicoHttp http;

        /// <summary>
        /// 動画情報
        /// </summary>
        public IDomainVideoInfo? Video { get; private set; }

        /// <summary>
        /// セッション情報
        /// </summary>
        private IWatchSessionInfo? watchSessionInfo;

        /// <summary>
        /// セッション確立済みフラグ
        /// </summary>
        public bool IsSessionEnsured { get; private set; }

        /// <summary>
        /// セッション失効フラグ
        /// </summary>
        public bool IsSessionExipired { get; private set; }

        /// <summary>
        /// プレイリストのURL
        /// </summary>
        public string PlaylistUrl
        {
            get
            {
                if (this.watchSessionInfo is null) throw new InvalidOperationException("セッション情報がnullのため、プレイリストのURLを取得できません。");
                return this.watchSessionInfo.ContentUrl;
            }
        }

        /// <summary>
        /// 状態
        /// </summary>
        public WatchSessionState State { get; private set; } = WatchSessionState.NotInitialized;

        /// <summary>
        /// 動画情報を取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task GetVideoDataAsync(string id, bool isVideoDownload)
        {
            var options = isVideoDownload ? WatchInfoOptions.Default : WatchInfoOptions.NoDmcData;

            try
            {
                this.Video = await this.watchInfo.GetVideoInfoAsync(id, options);
            }
            catch
            {
                this.State = this.watchInfo.State switch
                {
                    WatchInfoHandlerState.NoJsDataElement => WatchSessionState.PaymentNeeded,
                    WatchInfoHandlerState.HttpRequestFailure => WatchSessionState.HttpRequestFailure,
                    _ => WatchSessionState.PageAnalyzingFailure
                };
                return;
            }

            this.State = WatchSessionState.GotPage;
        }

        /// <summary>
        /// 視聴セッションを確立する
        /// </summary>
        /// <param name="nicoId"></param>
        /// <returns></returns>
        public async Task EnsureSessionAsync(string nicoId, bool isVideoDownload)
        {
            this.CheckSessionExpired();

            if (this.Video is null && this.State == WatchSessionState.NotInitialized)
            {
                await this.GetVideoDataAsync(nicoId, isVideoDownload);

                if (!isVideoDownload)
                {
                    this.State = WatchSessionState.OK;
                    this.IsSessionEnsured = true;
                    this.logger.Log($"{nicoId}の取得に成功しましたが、セッションの確立は行いませんでした。");
                    return;
                }
            }

            if (this.Video is null)
            {
                this.State = WatchSessionState.PageAnalyzingFailure;
                return;
            }

            //ダウンロード不可能な場合は処理をキャンセル
            if (!this.Video.DmcInfo?.IsDownloadsble ?? false)
            {
                if (this.Video.DmcInfo?.IsEncrypted ?? false)
                {
                    this.State = WatchSessionState.EncryptedVideo;
                }
                else
                {
                    this.State = WatchSessionState.PaymentNeeded;
                }
                return;
            }

            try
            {
                this.watchSessionInfo = await this.dmchandler.GetSessionInfoAsync(this.Video);
            }
            catch (Exception e)
            {
                this.State = WatchSessionState.SessionEnsuringFailure;
                this.logger.Error("セッションの確立に失敗しました。", e);
                return;
            }

            this.IsSessionEnsured = true;
            this.State = WatchSessionState.OK;
            this.StartHeartBeat();
            this.logger.Log($"{nicoId}の視聴セッションを確立しました。");
        }

        /// <summary>
        /// 取得可能なストリームの一覧を再帰的に構築
        /// </summary>
        /// <param name="url"></param>
        /// <param name="streams"></param>
        /// <returns></returns>
        private async Task GetAvailableStreamsAsync(string url, List<Dmc::IStreamInfo> streams, Video::IResolution? resolution = null, long bandWidth = 0)
        {
            Dmc::IStreamInfo stream;

            try
            {
                stream = await this.playlisthandler.GetStreamInfoAsync(url);

            }
            catch (Exception e)
            {
                this.logger.Error("プレイリストの取得に失敗しました。", e);
                return;
            }

            if (stream.IsMasterPlaylist)
            {
                foreach (var childStream in stream.PlaylistUrls)
                {
                    await this.GetAvailableStreamsAsync(childStream.AbsoluteUri, streams, childStream.Resolution, childStream.BandWidth);
                }
            }
            else
            {
                if (resolution is not null)
                {
                    stream.Resolution = resolution;
                    stream.BandWidth = bandWidth;
                }
                streams.Add(stream);
            }
        }

        /// <summary>
        /// 取得可能なストリームの一覧を返す
        /// </summary>
        /// <returns></returns>
        public async Task<Dmc::IStreamsCollection> GetAvailableStreamsAsync()
        {
            this.CheckSessionExpired();

            if (this.watchSessionInfo is null) throw new InvalidCastException("セッション情報がnullのため、ストリームの一覧を取得できません。");
            var availableStreams = new List<Dmc::IStreamInfo>();

            await this.GetAvailableStreamsAsync(this.PlaylistUrl, availableStreams);

            var streams = new Dmc::StreamsCollection();
            streams.AddRange(availableStreams);

            return streams;

        }

        /// <summary>
        /// セッションを破棄する
        /// </summary>
        public void Dispose()
        {
            this.IsSessionEnsured = false;
            this.IsSessionExipired = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// ハートビートを送信する
        /// </summary>
        private void StartHeartBeat()
        {
            if (this.watchSessionInfo is null || this.watchSessionInfo.DmcResponseJsonData.IsNullOrEmpty() || this.watchSessionInfo.SessionId.IsNullOrEmpty())
            {
                throw new InvalidOperationException("セッション情報が未取得のため、ハートビートを送信することが出来ません。");
            }
            else if (!this.IsSessionEnsured)
            {
                throw new InvalidOperationException("セッションが確立されていないため、ハートビートを送信することが出来ません。");
            }

            Task.Run(async () =>
            {
                StringContent content = new StringContent(this.watchSessionInfo.DmcResponseJsonData);
                string url = $"https://api.dmc.nico/api/sessions/{this.watchSessionInfo.SessionId}?_format=json&_method=PUT";
                //ハートビートの間隔(40秒)
                const int heartbeatInterval = 40 * 1000;
                await this.http.OptionAsync(new Uri(url));

                while (this.IsSessionEnsured)
                {
                    var res = await this.http.PostAsync(new Uri(url), content);
                    if (!res.IsSuccessStatusCode)
                    {
                        this.logger.Error($"ハートビートの送信に失敗しました。(status: {(int)res.StatusCode}, reason_phrase: {res.ReasonPhrase})");
                        this.IsSessionEnsured = false;
                        return;
                    }
                    else
                    {
                        this.logger.Log($"ハートビートを送信しました。(url: {url})");
                    }
                    await Task.Delay(heartbeatInterval);
                }
            });
        }

        /// <summary>
        /// セッションが失効している場合例外をスローする
        /// </summary>
        private void CheckSessionExpired()
        {
            if (this.IsSessionExipired)
            {
                throw new InvalidOperationException("セッションが失効しているため操作を完了できません。オブジェクトを再生成して下さい。");
            }
        }
    }

    /// <summary>
    /// セッション情報
    /// </summary>
    public class WatchSessionInfo : IWatchSessionInfo
    {
        public string DmcResponseJsonData { get; set; } = string.Empty;
        public string ContentUrl { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;

    }

    /// <summary>
    /// DMCサーバーとの通信を管理する
    /// </summary>
    public class DmcDataHandler : IDmcDataHandler
    {
        public DmcDataHandler(INicoHttp http)
        {
            this.http = http;
        }

        /// <summary>
        /// httpクライアント
        /// </summary>
        private readonly INicoHttp http;
        public DmcRequest::DmcPostData GetPostData(IDomainVideoInfo videoInfo)
        {
            var data = DmcRequest::DmcPostData.GetInstance();
            var sessionIfnfo = videoInfo.DmcInfo.SessionInfo;

            //nullチェック
            var nullList = new List<string>();
            var type = sessionIfnfo.GetType();
            var properties = type.GetProperties();
            foreach (var prop in properties)
            {
                string name = prop.Name;
                object? value = prop.GetValue(sessionIfnfo);
                if (value is null)
                {
                    nullList.Add(name);
                }
            }

            if (nullList.Count > 0)
            {
                throw new InvalidOperationException($"DMCサーバーへのPOSTに必要なデータがnullです。(一覧: {string.Join(',', nullList)})");
            }

            data.Session.Recipe_id = sessionIfnfo.RecipeId;
            data.Session.Content_id = sessionIfnfo.ContentId;
            data.Session.Content_type = "movie";
            data.Session.Content_src_id_sets.Add(sessionIfnfo.ContentSrcIdSets);
            data.Session.Timing_constraint = "unlimited";
            data.Session.Keep_method.Heartbeat.Lifetime = sessionIfnfo.HeartbeatLifetime;
            data.Session.Protocol.Name = "http";
            data.Session.Protocol.Parameters.Http_parameters.Parameters.Hls_parameters.Use_ssl = "yes";
            data.Session.Protocol.Parameters.Http_parameters.Parameters.Hls_parameters.Use_well_known_port = "yes";
            data.Session.Protocol.Parameters.Http_parameters.Parameters.Hls_parameters.Transfer_preset = sessionIfnfo.TransferPriset;
            data.Session.Protocol.Parameters.Http_parameters.Parameters.Hls_parameters.Segment_duration = 6000;
            data.Session.Session_operation_auth.Session_operation_auth_by_signature.Signature = sessionIfnfo.Signature;
            data.Session.Session_operation_auth.Session_operation_auth_by_signature.Token = sessionIfnfo.Token;
            data.Session.Content_auth.Auth_type = sessionIfnfo.AuthType;
            data.Session.Content_auth.Service_id = "nicovideo";
            data.Session.Content_auth.Service_user_id = sessionIfnfo.ServiceUserId;
            data.Session.Content_auth.Content_key_timeout = sessionIfnfo.ContentKeyTimeout;
            data.Session.Client_info.Player_id = sessionIfnfo.PlayerId;
            data.Session.Priority = sessionIfnfo.Priority;


            return data;
        }

        /// <summary>
        /// セッション情報を取得する
        /// </summary>
        /// <param name="dmcPostData"></param>
        /// <returns></returns>
        public async Task<IWatchSessionInfo> GetSessionInfoAsync(DmcRequest::DmcPostData dmcPostData)
        {

            string json = JsonParser.Serialize(dmcPostData);

            var res = await this.http.PostAsync(new Uri("https://api.dmc.nico/api/sessions?_format=json"), new StringContent(json));
            if (!res.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"DMCサーバーへのPOSTに失敗しました。(status: {(int)res.StatusCode}, reason_phrase: {res.ReasonPhrase})");
            }

            string responseString = await res.Content.ReadAsStringAsync();

            var deserialized = JsonParser.DeSerialize<DmcResponse::DmcResponseData>(responseString);

            if (deserialized.Data.Session.ContentUri.IsNullOrEmpty())
            {
                throw new HttpRequestException("DMCサーバーからの情報取得に失敗しました。(Data.Session.ContentUriがnullまたは空白です。)");
            }
            else if (deserialized.Data.Session.Id.IsNullOrEmpty())
            {
                throw new HttpRequestException("DMCサーバーからの情報取得に失敗しました。(Data.Idがnullまたは空白です。)");
            }

            return new WatchSessionInfo()
            {
                DmcResponseJsonData = JsonParser.Serialize(deserialized.Data),
                ContentUrl = deserialized.Data.Session.ContentUri,
                SessionId = deserialized.Data.Session.Id,
            };
        }

        /// <summary>
        /// セッション情報を取得する
        /// </summary>
        /// <param name="videoinfo"></param>
        /// <returns></returns>
        public Task<IWatchSessionInfo> GetSessionInfoAsync(IDomainVideoInfo videoinfo)
        {
            var postData = this.GetPostData(videoinfo);
            return this.GetSessionInfoAsync(postData);
        }
    }

}
