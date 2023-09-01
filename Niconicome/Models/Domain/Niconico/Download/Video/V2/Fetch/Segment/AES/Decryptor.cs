using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Niconicome.Models.Domain.Niconico.Download.Video.V2.Fetch.Segment.AES.Error;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V2.Fetch.Segment.AES
{
    public interface IDecryptor
    {
        /// <summary>
        /// 復号する
        /// </summary>
        /// <param name="cipher"></param>
        /// <param name="AESInfomation"></param>
        /// <returns></returns>
        IAttemptResult<byte[]> Decrypt(byte[] cipher, IAESInfomation AESInfomation);
    }

    public class Decryptor : IDecryptor
    {
        public Decryptor(IErrorHandler errorHandler)
        {
            this._errorHandler = errorHandler;
        }

        #region field

        private readonly IErrorHandler _errorHandler;

        #endregion

        #region Method

        public IAttemptResult<byte[]> Decrypt(byte[] cipher, IAESInfomation AESInfomation)
        {
            try
            {
                using var aes = Aes.Create();

                // HLS uses AES-128 w/ CBC & PKCS7
                // https://www.rfc-editor.org/rfc/rfc8216#section-4.3.2.4
                aes.BlockSize = 128;
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;
                aes.KeySize = 128;

                aes.Key = AESInfomation.Key;
                aes.IV = AESInfomation.IV;

                using var ms = new MemoryStream(cipher);
                using var msPlain = new MemoryStream();
                using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);

                cs.CopyTo(msPlain);

                return AttemptResult<byte[]>.Succeeded(msPlain.ToArray());
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(DecryptorError.FailedToDecrypt, ex);
                return AttemptResult<byte[]>.Fail(this._errorHandler.GetMessageForResult(DecryptorError.FailedToDecrypt, ex));
            }
        }

        #endregion
    }
}
