using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Download.Video.V2.Fetch.Segment.AES.Error;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V2.Fetch.Segment.AES
{
    public interface IAESInfomationHandler
    {
        /// <summary>
        /// AES情報を取得
        /// </summary>
        /// <param name="IV"></param>
        /// <param name="KeyURL"></param>
        /// <returns></returns>
        Task<IAttemptResult<IAESInfomation>> GetAESInfomationAsync(string IV, string KeyURL);
    }

    public class AESInfomationHandler : IAESInfomationHandler
    {
        public AESInfomationHandler(INicoHttp http, IErrorHandler errorHandler)
        {
            this._http = http;
            this._error = errorHandler;
        }

        #region field

        private readonly INicoHttp _http;

        private readonly IErrorHandler _error;

        #endregion

        #region Method

        public async Task<IAttemptResult<IAESInfomation>> GetAESInfomationAsync(string IV, string KeyURL)
        {
            HttpResponseMessage res = await this._http.GetAsync(new Uri(KeyURL));
            if (!res.IsSuccessStatusCode)
            {
                this._error.HandleError(AESInfomationHandlerError.FailedToGetKey, (int)res.StatusCode);
                return AttemptResult<IAESInfomation>.Fail(this._error.GetMessageForResult(AESInfomationHandlerError.FailedToGetKey, (int)res.StatusCode));
            }

            byte[] key;

            try
            {
                string content = await res.Content.ReadAsStringAsync();
                key = Convert.FromBase64String(content);
            }
            catch (Exception ex)
            {
                this._error.HandleError(AESInfomationHandlerError.FailedToParse, ex);
                return AttemptResult<IAESInfomation>.Fail(this._error.GetMessageForResult(AESInfomationHandlerError.FailedToParse, ex));
            }

            //IVは0x....(0x + 16*2)になっているはず
            if (IV.Length != 16 * 2 + 2)
            {
                this._error.HandleError(AESInfomationHandlerError.InvalidIV, IV);
                return AttemptResult<IAESInfomation>.Fail(this._error.GetMessageForResult(AESInfomationHandlerError.InvalidIV, IV));
            }

            byte[] IVByte = this.ConvertIVToByte(IV);

            return AttemptResult<IAESInfomation>.Succeeded(new AESInfomation(key, IVByte));
        }


        #endregion

        #region private

        private byte[] ConvertIVToByte(string IV)
        {
            var converted = new byte[16];
            var index = 0;

            for (var i = 2; i < IV.Length; i += 2)
            {
                string x = IV[i..(i + 2)];
                converted[index]= Convert.ToByte(x,16);
                index++;
            }

            return converted;
        }

        #endregion
    }
}
