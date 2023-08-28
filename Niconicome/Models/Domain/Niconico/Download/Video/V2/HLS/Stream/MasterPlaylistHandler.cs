using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Err = Niconicome.Models.Domain.Niconico.Download.Video.V2.HLS.Error.MasterPlaylistHandlerError;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V2.HLS.Stream
{

    public interface IMasterPlaylisthandler
    {
        /// <summary>
        /// Stream情報を取得する
        /// </summary>
        /// <param name="masterPlaylistURL"></param>
        /// <returns></returns>
        Task<IAttemptResult<IStreamsCollection>> GetStreamInfoAsync(string masterPlaylistURL);
    }

    class MasterPlaylistHandler : IMasterPlaylisthandler
    {

        public MasterPlaylistHandler(IStreamhandler handler, INicoHttp http,IErrorHandler errorHandler)
        {
            this._handler = handler;
            this._http = http;
            this._errorHandler = errorHandler;
        }

        #region field

        private readonly IStreamhandler _handler;

        private readonly INicoHttp _http;

        private readonly IErrorHandler _errorHandler;

        #endregion

        #region Method

        public async Task<IAttemptResult<IStreamsCollection>> GetStreamInfoAsync(string masterPlaylistURL)
        {
            string absPath = new Uri(masterPlaylistURL).AbsoluteUri;
            string baseUrl = absPath.Substring(0, absPath.LastIndexOf('/') + 1);

            //master.m3u8を取得
            IAttemptResult<IEnumerable<IPlaylistInfo>> pResult = await this.GetPlaylistInfos(masterPlaylistURL, baseUrl);
            if (!pResult.IsSucceeded || pResult.Data is null)
            {
                return AttemptResult<IStreamsCollection>.Fail(pResult.Message);
            }

            //playlist.m3u8を取得
            var streams = new List<IStreamInfo>();

            foreach (var childPlaylist in pResult.Data)
            {
                IAttemptResult<IStreamInfo> result = await this.GetStreamInfoAsync(childPlaylist);
                if (result.IsSucceeded && result.Data is not null)
                {
                    streams.Add(result.Data);
                }
            }

            if (streams.Count == 0)
            {
                this._errorHandler.HandleError(Err.FailedToGetStreams, masterPlaylistURL);
                return AttemptResult<IStreamsCollection>.Fail(this._errorHandler.GetMessageForResult(Err.FailedToGetStreams, masterPlaylistURL));
            }

            return AttemptResult<IStreamsCollection>.Succeeded(new StreamsCollection(streams));
        }

        #endregion

        #region private

        /// <summary>
        /// master.m3u8を取得
        /// </summary>
        /// <param name="masterPlaylistURL"></param>
        /// <param name="baseURL"></param>
        /// <returns></returns>
        private async Task<IAttemptResult<IEnumerable<IPlaylistInfo>>> GetPlaylistInfos(string masterPlaylistURL, string baseURL)
        {
            var res = await this._http.GetAsync(new Uri(masterPlaylistURL));
            if (!res.IsSuccessStatusCode)
            {
                this._errorHandler.HandleError(Err.FailedToFetchMasterPlaylist, masterPlaylistURL, (int)res.StatusCode);
                return AttemptResult<IEnumerable<IPlaylistInfo>>.Fail(this._errorHandler.GetMessageForResult(Err.FailedToFetchMasterPlaylist, masterPlaylistURL, (int)res.StatusCode));
            }

            string content = await res.Content.ReadAsStringAsync();


            if (content.IsNullOrEmpty())
            {
                this._errorHandler.HandleError(Err.FailedToLoadMasterPlaylist, masterPlaylistURL);
            }

            IEnumerable<IPlaylistInfo> playlists = this._handler.GetPlaylistInfos(content, baseURL);

            return AttemptResult<IEnumerable<IPlaylistInfo>>.Succeeded(playlists);
        }

        /// <summary>
        /// Stream情報を取得する
        /// </summary>
        /// <param name="playlist"></param>
        /// <returns></returns>
        private async Task<IAttemptResult<IStreamInfo>> GetStreamInfoAsync(IPlaylistInfo playlist)
        {
            var res = await this._http.GetAsync(new Uri(playlist.AbsoluteURL));
            if (!res.IsSuccessStatusCode)
            {
                this._errorHandler.HandleError(Err.FailedToFetchPlaylist, playlist.AbsoluteURL, (int)res.StatusCode);
                return AttemptResult<IStreamInfo>.Fail(this._errorHandler.GetMessageForResult(Err.FailedToFetchPlaylist, playlist.AbsoluteURL, (int)res.StatusCode));
            }

            string baseURL = playlist.AbsoluteURL.Substring(0, playlist.AbsoluteURL.LastIndexOf('/') + 1);

            string content = await res.Content.ReadAsStringAsync();

            if (content.IsNullOrEmpty())
            {
                this._errorHandler.HandleError(Err.FailedToLoadPlaylist, playlist.AbsoluteURL);
            }

            IStreamInfo stream = this._handler.GetStreamInfo(content, baseURL, playlist.Resolution, playlist.BandWidth);

            return AttemptResult<IStreamInfo>.Succeeded(stream);
        }

        #endregion
    }
}
