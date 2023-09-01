using System;
using System.Net.Http;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Local.Addons.API.Hooks;
using Niconicome.Models.Domain.Niconico.Download.Video.V2.HLS.Stream;
using Niconicome.Models.Domain.Niconico.Video.Infomations;
using Niconicome.Models.Domain.Niconico.Watch;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Err = Niconicome.Models.Domain.Niconico.Download.Video.V2.Error.WatchSessionError;
using Utils = Niconicome.Models.Domain.Utils;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V2.Session
{
    public interface IWatchSession : IDisposable
    {
        /// <summary>
        /// セッション確立フラグ
        /// </summary>
        bool IsSessionEnsured { get; }

        /// <summary>
        /// セッション失効フラグ
        /// </summary>
        bool IsSessionExipired { get; }

        /// <summary>
        /// セッションを確立する
        /// </summary>
        /// <param name="videoInfo"></param>
        /// <returns></returns>
        Task<IAttemptResult> EnsureSessionAsync(IDomainVideoInfo videoInfo);

        /// <summary>
        /// 取得可能なStreamの一覧
        /// </summary>
        /// <returns></returns>
        Task<IAttemptResult<IStreamsCollection>> GetAvailableStreamsAsync();
    }

    /// <summary>
    /// 視聴セッション全般を管理する
    /// </summary>
    public class WatchSession : IWatchSession
    {
        public WatchSession(IWatchInfohandler watchInfo, INicoHttp http, Utils::ILogger logger, IDmcDataHandler dmchandler, IMasterPlaylisthandler playlisthandler, IHooksManager hooksManager, IErrorHandler errorHandler)
        {
            this._watchInfo = watchInfo;
            this._http = http;
            this._dmchandler = dmchandler;
            this._playlisthandler = playlisthandler;
            this._hooksManager = hooksManager;
            this._errorHandler = errorHandler;
        }

        ~WatchSession()
        {
            this.IsSessionEnsured = false;
        }

        #region field

        private readonly IErrorHandler _errorHandler;

        private readonly IDmcDataHandler _dmchandler;

        private readonly IWatchInfohandler _watchInfo;

        private readonly IMasterPlaylisthandler _playlisthandler;

        private readonly INicoHttp _http;

        private readonly IHooksManager _hooksManager;

        private IWatchSessionInfo? _session;

        #endregion

        #region Props

        public bool IsSessionEnsured { get; private set; }

        public bool IsSessionExipired { get; private set; }


        #endregion

        #region Method

        public async Task<IAttemptResult> EnsureSessionAsync(IDomainVideoInfo videoInfo)
        {
            //ダウンロード不可能な場合は処理をキャンセル
            if (!videoInfo.DmcInfo.IsDownloadable)
            {
                if (videoInfo.DmcInfo.IsEncrypted)
                {
                    this._errorHandler.HandleError(Err.VideoIsEncrypted, videoInfo.Id);
                    return AttemptResult.Fail(this._errorHandler.GetMessageForResult(Err.VideoIsEncrypted, videoInfo.Id));
                }
                else
                {
                    this._errorHandler.HandleError(Err.VideoRequirePayment, videoInfo.Id);
                    return AttemptResult.Fail(this._errorHandler.GetMessageForResult(Err.VideoRequirePayment, videoInfo.Id));
                }
            }

            //セッション確立
            IAttemptResult<IWatchSessionInfo> result;

            if (this._hooksManager.IsRegistered(HookType.SessionEnsuring))
            {
                result = await this.EnsureSessionWithAddonAsync(videoInfo);
            }
            else
            {
                result = await this.EnsureSessionDefaultAsync(videoInfo);
            }

            if (!result.IsSucceeded || result.Data is null)
            {
                return AttemptResult.Fail(result.Message);
            }
            else
            {
                this._session = result.Data;
            }

            this.IsSessionEnsured = true;

            this.StartHeartBeat();

            this._errorHandler.HandleError(Err.SessionEnsured, videoInfo.Id);

            return AttemptResult.Succeeded();
        }

        public async Task<IAttemptResult<IStreamsCollection>> GetAvailableStreamsAsync()
        {
            if (this.IsSessionExipired)
            {
                this._errorHandler.HandleError(Err.SessionExpired);
                return AttemptResult<IStreamsCollection>.Fail(this._errorHandler.GetMessageForResult(Err.SessionExpired));
            }

            if (this._session is null)
            {
                this._errorHandler.HandleError(Err.SessionNotEnsured);
                return AttemptResult<IStreamsCollection>.Fail(this._errorHandler.GetMessageForResult(Err.SessionNotEnsured));
            }

            return await this._playlisthandler.GetStreamInfoAsync(this._session.ContentUrl);

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
                sessionInfo = await this._dmchandler.GetSessionInfoAsync(video);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(Err.SessionEnsuringFailure, ex, video.Id);
                return AttemptResult<IWatchSessionInfo>.Fail(this._errorHandler.GetMessageForResult(Err.SessionEnsuringFailure, ex, video.Id));
            }

            return AttemptResult<IWatchSessionInfo>.Succeeded(sessionInfo);

        }

        /// <summary>
        /// アドオンでセッションを確立する
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        private async Task<IAttemptResult<IWatchSessionInfo>> EnsureSessionWithAddonAsync(IDomainVideoInfo video)
        {
            IAttemptResult<dynamic> result = await this._hooksManager.EnsureSessionAsync(video.RawDmcInfo);
            if (!result.IsSucceeded || result.Data is null)
            {
                if (result.Exception is not null)
                {
                    this._errorHandler.HandleError(Err.SessionEnsuringFailure, result.Exception, video.Id);
                    return AttemptResult<IWatchSessionInfo>.Fail(this._errorHandler.GetMessageForResult(Err.SessionEnsuringFailure, result.Exception, video.Id));
                }
                else
                {
                    this._errorHandler.HandleError(Err.SessionEnsuringFailure, video.Id);
                    return AttemptResult<IWatchSessionInfo>.Fail(this._errorHandler.GetMessageForResult(Err.SessionEnsuringFailure, video.Id));
                }
            }

            try
            {
                if (result.Data.DmcResponseJsonData is not string jsonData || result.Data.ContentUrl is not string contentUrl || result.Data.SessionId is not string sessionID)
                {
                    this._errorHandler.HandleError(Err.AddonReturnedInvalidInfomation, video.Id);
                    return AttemptResult<IWatchSessionInfo>.Fail(this._errorHandler.GetMessageForResult(Err.AddonReturnedInvalidInfomation, video.Id));
                }

                var info = new WatchSessionInfo()
                {
                    DmcResponseJsonData = jsonData,
                    ContentUrl = contentUrl,
                    SessionId = sessionID,
                };

                return new AttemptResult<IWatchSessionInfo>() { IsSucceeded = true, Data = info };
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(Err.AddonReturnedInvalidInfomation, ex, video.Id);
                return AttemptResult<IWatchSessionInfo>.Fail(this._errorHandler.GetMessageForResult(Err.AddonReturnedInvalidInfomation, ex, video.Id));
            }
        }

        /// <summary>
        /// ハートビートを送信する
        /// </summary>
        private void StartHeartBeat()
        {
            if (this._session is null || this._session.DmcResponseJsonData.IsNullOrEmpty() || this._session.SessionId.IsNullOrEmpty())
            {
                throw new InvalidOperationException();
            }
            else if (!this.IsSessionEnsured)
            {
                throw new InvalidOperationException();
            }

            Task.Run(async () =>
            {
                StringContent content = new StringContent(this._session.DmcResponseJsonData);
                string url = $"https://api.dmc.nico/api/sessions/{this._session.SessionId}?_format=json&_method=PUT";
                //ハートビートの間隔(40秒)
                const int heartbeatInterval = 40 * 1000;
                await this._http.OptionAsync(new Uri(url));

                while (this.IsSessionEnsured)
                {
                    var res = await this._http.PostAsync(new Uri(url), content);
                    if (!res.IsSuccessStatusCode)
                    {
                        this._errorHandler.HandleError(Err.FailedToSendHeartBeat, this._session.SessionId, (int)res.StatusCode);
                        this.IsSessionEnsured = false;
                        return;
                    }
                    else
                    {
                        this._errorHandler.HandleError(Err.SucceededToSendHeartBeat, this._session.SessionId);
                    }

                    await Task.Delay(heartbeatInterval);
                }
            });
        }

        #endregion
    }

}
