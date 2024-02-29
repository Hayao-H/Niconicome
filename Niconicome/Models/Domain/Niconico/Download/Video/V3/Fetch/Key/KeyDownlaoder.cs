using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V3.Fetch.Key
{
    public interface IKeyDownlaoder
    {
        /// <summary>
        /// Keyをダウンロードする
        /// </summary>
        /// <param name="videoKeyURL"></param>
        /// <param name="audioKeyURL"></param>
        /// <returns></returns>
        Task<IAttemptResult<KeyInfomation>> DownloadKeyASync(string videoKeyURL, string audioKeyURL);
    }

    public class KeyDownlaoder : IKeyDownlaoder
    {
        public KeyDownlaoder(INicoHttp http, IErrorHandler errorHandler)
        {
            this._http = http;
            this._errorHandler = errorHandler;
        }

        #region field

        private readonly INicoHttp _http;
        private readonly IErrorHandler _errorHandler;

        #endregion

        public async Task<IAttemptResult<KeyInfomation>> DownloadKeyASync(string videoKeyURL, string audioKeyURL)
        {
            string videoKey;
            string audioKey;

            try
            {
                var res = await this._http.GetAsync(new Uri(videoKeyURL));
                videoKey = Convert.ToBase64String(await res.Content.ReadAsByteArrayAsync());
            }
            catch (Exception ex)
            {
                return AttemptResult<KeyInfomation>.Fail(this._errorHandler.HandleError(Error.KeyDownlaoderError.FailedToDownloadKey, ex));
            }

            try
            {
                var res = await this._http.GetAsync(new Uri(audioKeyURL));
                audioKey = Convert.ToBase64String(await res.Content.ReadAsByteArrayAsync());
            }
            catch (Exception ex)
            {
                return AttemptResult<KeyInfomation>.Fail(this._errorHandler.HandleError(Error.KeyDownlaoderError.FailedToDownloadKey, ex));
            }



            return AttemptResult<KeyInfomation>.Succeeded(new KeyInfomation(videoKey,audioKey));
        }
    }
}
