using System;
using System.Net.Http;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Local.Addons.API.Hooks;
using Niconicome.Models.Domain.Niconico.Download.Video.V3.DMS;
using Niconicome.Models.Domain.Niconico.Video.Infomations;
using Niconicome.Models.Domain.Niconico.Watch;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.State.MessageV2;
using Err = Niconicome.Models.Domain.Niconico.Download.Video.V3.Error.WatchSessionError;
using Utils = Niconicome.Models.Domain.Utils;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V3.Session
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
        Task<IAttemptResult<IStreamCollection>> GetAvailableStreamsAsync();
    }

    /// <summary>
    /// 視聴セッション全般を管理する
    /// </summary>
    public class WatchSession : IWatchSession
    {
        public WatchSession(IWatchInfohandler watchInfo, INicoHttp http,IStreamParser streamParser, IHooksManager hooksManager, IErrorHandler errorHandler)
        {
            this._watchInfo = watchInfo;
            this._http = http;
            this._hooksManager = hooksManager;
            this._errorHandler = errorHandler;
            this._streamParser = streamParser;
        }

        ~WatchSession()
        {
            this.IsSessionEnsured = false;
        }

        #region field

        private readonly IErrorHandler _errorHandler;

        private readonly IWatchInfohandler _watchInfo;

        private readonly IStreamParser _streamParser;

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
            if (!this._hooksManager.IsRegistered(HookType.SessionEnsuring))
            {
                return AttemptResult.Fail(this._errorHandler.HandleError(Err.AddonNotRegistered));
            }

            IAttemptResult<IWatchSessionInfo> result =await this.EnsureSessionWithAddonAsync(videoInfo);

            if (!result.IsSucceeded || result.Data is null)
            {
                return AttemptResult.Fail(result.Message);
            }
            else
            {
                this._session = result.Data;
            }

            this.IsSessionEnsured = true;

            this._errorHandler.HandleError(Err.SessionEnsured, videoInfo.Id);

            return AttemptResult.Succeeded();
        }

        public async Task<IAttemptResult<IStreamCollection>> GetAvailableStreamsAsync()
        {
            if (this.IsSessionExipired)
            {
                this._errorHandler.HandleError(Err.SessionExpired);
                return AttemptResult<IStreamCollection>.Fail(this._errorHandler.GetMessageForResult(Err.SessionExpired));
            }

            if (this._session is null)
            {
                this._errorHandler.HandleError(Err.SessionNotEnsured);
                return AttemptResult<IStreamCollection>.Fail(this._errorHandler.GetMessageForResult(Err.SessionNotEnsured));
            }

            return await this._streamParser.ParseAsync(this._session.ContentUrl);

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
                if (result.Data.DmcResponseJsonData is not string jsonData || result.Data.ContentUrl is not string contentUrl || result.Data.SessionId is not string sessionID || result.Data.IsDMS is not bool isDMS)
                {
                    this._errorHandler.HandleError(Err.AddonReturnedInvalidInfomation, video.Id);
                    return AttemptResult<IWatchSessionInfo>.Fail(this._errorHandler.GetMessageForResult(Err.AddonReturnedInvalidInfomation, video.Id));
                }

                var info = new WatchSessionInfo()
                {
                    DmcResponseJsonData = jsonData,
                    ContentUrl = contentUrl,
                    SessionId = sessionID,
                    IsDMS = isDMS,
                };

                return new AttemptResult<IWatchSessionInfo>() { IsSucceeded = true, Data = info };
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(Err.AddonReturnedInvalidInfomation, ex, video.Id);
                return AttemptResult<IWatchSessionInfo>.Fail(this._errorHandler.GetMessageForResult(Err.AddonReturnedInvalidInfomation, ex, video.Id));
            }
        }

        #endregion
    }

}
