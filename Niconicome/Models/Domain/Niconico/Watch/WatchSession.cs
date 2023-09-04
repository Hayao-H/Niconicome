using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Local.Addons.API.Hooks;
using Niconicome.Models.Domain.Niconico.Video.Infomations;
using Niconicome.Models.Helper.Result;
using Dmc = Niconicome.Models.Domain.Niconico.Dmc;
using Utils = Niconicome.Models.Domain.Utils;
using Video = Niconicome.Models.Domain.Niconico.Video.Infomations;

namespace Niconicome.Models.Domain.Niconico.Watch
{
    public interface IWatchSession : IDisposable
    {
        Task EnsureSessionAsync(string id);
        Task GetVideoDataAsync(string id);
        bool IsSessionEnsured { get; }
        bool IsSessionExipired { get; }
        public IDomainVideoInfo? Video { get; }
        WatchSessionState State { get; }
        Task<Dmc::IStreamsCollection> GetAvailableStreamsAsync();
    }

    public enum WatchSessionState
    {
        NotInitialized,
        HttpRequestOrPageAnalyzingFailure,
        EncryptedVideo,
        AuthenticationFailure,
        PaymentNeeded,
        SessionEnsuringFailure,
        GotPage,
        OK
    }

    /// <summary>
    /// 視聴セッション全般を管理する
    /// </summary>
    public class WatchSession : IWatchSession
    {
        public WatchSession(IWatchInfohandler watchInfo, INicoHttp http, Utils::ILogger logger, IDmcDataHandler dmchandler, IWatchPlaylisthandler playlisthandler, IHooksManager hooksManager)
        {
            this.watchInfo = watchInfo;
            this.http = http;
            this.logger = logger;
            this.dmchandler = dmchandler;
            this.playlisthandler = playlisthandler;
            this.hooksManager = hooksManager;
        }

        ~WatchSession()
        {
            this.IsSessionEnsured = false;
        }

        #region field

        private readonly Utils::ILogger logger;

        private readonly IDmcDataHandler dmchandler;

        private readonly IWatchInfohandler watchInfo;

        private readonly IWatchPlaylisthandler playlisthandler;

        private readonly INicoHttp http;

        private readonly IHooksManager hooksManager;

        private IWatchSessionInfo? watchSessionInfo;

        #endregion

        #region Props

        /// <summary>
        /// 動画情報
        /// </summary>
        public IDomainVideoInfo? Video { get; private set; }

        /// <summary>
        /// セッション確立済みフラグ
        /// </summary>
        public bool IsSessionEnsured { get; private set; }

        /// <summary>
        /// セッション失効フラグ
        /// </summary>
        public bool IsSessionExipired { get; private set; }

        /// <summary>
        /// 状態
        /// </summary>
        public WatchSessionState State { get; private set; } = WatchSessionState.NotInitialized;

        #endregion

        #region Method

        /// <summary>
        /// 動画情報を取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task GetVideoDataAsync(string id)
        {
            IAttemptResult<IDomainVideoInfo> result;

            try
            {
                result = await this.watchInfo.GetVideoInfoAsync(id);
            }
            catch
            {
                this.State = WatchSessionState.HttpRequestOrPageAnalyzingFailure;
                this.logger.Error($"動画情報を取得中に不明なエラーが発生しました。(id:{id})");
                return;
            }

            if (!result.IsSucceeded || result.Data is null)
            {
                this.State = WatchSessionState.HttpRequestOrPageAnalyzingFailure;
                return;
            }

            this.Video = result.Data;
            this.State = WatchSessionState.GotPage;

        }

        /// <summary>
        /// 視聴セッションを確立する
        /// </summary>
        /// <param name="nicoId"></param>
        /// <returns></returns>
        public async Task EnsureSessionAsync(string nicoId)
        {
            this.CheckSessionExpired();

            if (this.Video is null && this.State == WatchSessionState.NotInitialized)
            {
                await this.GetVideoDataAsync(nicoId);
            }

            if (this.Video is null)
            {
                this.State = WatchSessionState.HttpRequestOrPageAnalyzingFailure;
                return;
            }

            //ダウンロード不可能な場合は処理をキャンセル
            if (!this.Video.DmcInfo.IsDownloadable)
            {
                if (this.Video.DmcInfo.IsEncrypted)
                {
                    this.State = WatchSessionState.EncryptedVideo;
                }
                else
                {
                    this.State = WatchSessionState.PaymentNeeded;
                }
                return;
            }

            IAttemptResult<IWatchSessionInfo> result;

            if (this.hooksManager.IsRegistered(HookType.SessionEnsuring))
            {
                result = await this.EnsureSessionWithAddonAsync(this.Video);
            }
            else
            {
                result = await this.EnsureSessionDefaultAsync(this.Video);
            }

            if (!result.IsSucceeded || result.Data is null)
            {
                this.State = WatchSessionState.SessionEnsuringFailure;
                return;
            }
            else
            {
                this.watchSessionInfo = result.Data;
            }

            this.IsSessionEnsured = true;
            this.State = WatchSessionState.OK;
            this.StartHeartBeat();
            this.logger.Log($"{nicoId}の視聴セッションを確立しました。");
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

            await this.GetAvailableStreamsAsync(this.watchSessionInfo.ContentUrl, availableStreams);

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

        #endregion

        #region private

        /// <summary>
        /// セッションを確立する
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        private async Task<IAttemptResult<IWatchSessionInfo>> EnsureSessionDefaultAsync(IDomainVideoInfo video)
        {
            IWatchSessionInfo sessionInfo;

            try
            {
                sessionInfo = await this.dmchandler.GetSessionInfoAsync(video);
            }
            catch (Exception e)
            {
                this.logger.Error("セッションの確立に失敗しました。", e);
                return new AttemptResult<IWatchSessionInfo>() { Message = "セッションの確立に失敗しました。", Exception = e };
            }

            return new AttemptResult<IWatchSessionInfo>() { IsSucceeded = true, Data = sessionInfo };

        }

        /// <summary>
        /// アドオンでセッションを確立する
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        private async Task<IAttemptResult<IWatchSessionInfo>> EnsureSessionWithAddonAsync(IDomainVideoInfo video)
        {
            IAttemptResult<dynamic> result = await this.hooksManager.EnsureSessionAsync(video.RawDmcInfo);
            if (!result.IsSucceeded || result.Data is null)
            {
                return new AttemptResult<IWatchSessionInfo>() { Message = $"セッションの確立に失敗しました。(詳細:{result.Message})", Exception = result.Exception };
            }

            try
            {
                if (result.Data.DmcResponseJsonData is not string jsonData) return new AttemptResult<IWatchSessionInfo>() { Message = $"HeratBeatコンテントが正しくありません。" };
                if (result.Data.ContentUrl is not string contentUrl) return new AttemptResult<IWatchSessionInfo>() { Message = $"コンテンツURLが正しくありません。" };
                if (result.Data.SessionId is not string sessionID) return new AttemptResult<IWatchSessionInfo>() { Message = $"セッションIDが正しくありません。" };

                var info = new WatchSessionInfo()
                {
                    DmcResponseJsonData = jsonData,
                    ContentUrl = contentUrl,
                    SessionId = sessionID,
                };

                return new AttemptResult<IWatchSessionInfo>() { IsSucceeded = true, Data = info };
            }
            catch (Exception e)
            {
                this.logger.Error("アドオンから返却されたオブジェクトの解析に失敗しました。", e);
                return new AttemptResult<IWatchSessionInfo>() { Message = $"アドオンから返却されたオブジェクトの解析に失敗しました。", Exception = e };
            }
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

        #endregion
    }

}
