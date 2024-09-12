using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Niconico.UserAuth;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Infrastructure.Database.LiteDB;
using Types = Niconicome.Models.Domain.Local.Store.Types;
using LocalCnst = Niconicome.Models.Const.LocalConstant;
using Utl = Niconicome.Models.Domain.Utils;
using Err = Niconicome.Models.Infrastructure.Database.Error.CookieDBHandlerError;
using System.Diagnostics;
using Niconicome.Extensions.System;

namespace Niconicome.Models.Infrastructure.Database
{
    public class CookieDBHandler : ICookieStore
    {
        public CookieDBHandler(ILiteDBHandler dataBase, IErrorHandler errorHandler)
        {
            this._dataBase = dataBase;
            this._errorHandler = errorHandler;
        }

        #region field

        private readonly ILiteDBHandler _dataBase;

        private readonly IErrorHandler _errorHandler;

        private readonly int AES_KEY_SIZE_BIT = 128;

        #endregion

        #region Method

        public IAttemptResult<ICookieInfo> GetCookieInfo()
        {
            this.Initialize();

            IAttemptResult<IReadOnlyList<Types.Cookie>> cookieResult = this._dataBase.GetAllRecords<Types.Cookie>(TableNames.Cookie);

            if (!cookieResult.IsSucceeded || cookieResult.Data is null) return AttemptResult<ICookieInfo>.Fail(cookieResult.Message);

            var cookie = cookieResult.Data.First();

            if (cookie.UserSession.IsNullOrEmpty())
            {
                var empty = new CookieInfo(this, string.Empty, string.Empty);
                return AttemptResult<ICookieInfo>.Succeeded(empty);
            }

            Decrypted decrypted;

            try
            {
                decrypted = this.DecryptCookie(cookie);
            }
            catch (Exception ex)
            {
                this.DeleteCookieInfo();
                return AttemptResult<ICookieInfo>.Fail(this._errorHandler.HandleError(Err.FailedToDecrypt, ex));
            }

            var info = new CookieInfo(this, decrypted.UserSession, decrypted.UserSessionSecure);

            return AttemptResult<ICookieInfo>.Succeeded(info);
        }


        public IAttemptResult DeleteCookieInfo()
        {
            return this._dataBase.Clear(TableNames.Cookie);
        }


        public IAttemptResult Update(ICookieInfo cookieInfo)
        {
            if (!this.Exists()) return AttemptResult<ICookieInfo>.Fail(this._errorHandler.HandleError(Err.CookieNotFound));
            if (cookieInfo.UserSession.IsNullOrEmpty() || cookieInfo.UserSessionSecure.IsNullOrEmpty()) return AttemptResult<ICookieInfo>.Succeeded();

            IAttemptResult<IReadOnlyList<Types.Cookie>> cookieResult = this._dataBase.GetAllRecords<Types.Cookie>(TableNames.Cookie);

            if (!cookieResult.IsSucceeded || cookieResult.Data is null) return AttemptResult<ICookieInfo>.Fail(cookieResult.Message);

            var original = cookieResult.Data.First();
            Types.Cookie encrypted;

            try
            {
                encrypted = this.EncryptCookie(cookieInfo.UserSession, cookieInfo.UserSessionSecure);
            }
            catch (Exception ex)
            {
                return AttemptResult.Fail(this._errorHandler.HandleError(Err.FailedToEncrypt, ex));
            }

            encrypted.Id = original.Id;

            return this._dataBase.Update(encrypted);
        }

        public bool Exists()
        {
            IAttemptResult<IReadOnlyList<Types.Cookie>> cookie = this._dataBase.GetAllRecords<Types.Cookie>(TableNames.Cookie);
            if (!cookie.IsSucceeded || cookie.Data is null) return false;

            return cookie.Data.Count > 0;
        }

        #endregion

        #region private

        /// <summary>
        /// Cookieを復号
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        private Decrypted DecryptCookie(Types.Cookie cookie)
        {
            var appKey = Encoding.UTF8.GetBytes(LocalCnst.AppKey);
            var key = ProtectedData.Unprotect(Convert.FromBase64String(cookie.Key), null, DataProtectionScope.CurrentUser);
            key = Convert.FromBase64String(Encoding.UTF8.GetString(this.DecryptAES(key, appKey)));

            var userSession = this.DecryptAES(Convert.FromBase64String(cookie.UserSession), key);
            var userSessionSecure = this.DecryptAES(Convert.FromBase64String(cookie.UserSessionSecure), key);

            return new Decrypted(Encoding.UTF8.GetString(userSession), Encoding.UTF8.GetString(userSessionSecure));
        }

        /// <summary>
        /// Cookieを暗号化
        /// </summary>
        /// <param name="userSession"></param>
        /// <param name="userSessionSecure"></param>
        /// <returns></returns>
        private Types.Cookie EncryptCookie(string userSession, string userSessionSecure)
        {
            var key = this.CreateKey();
            string keyB64 = Convert.ToBase64String(key);
            var appKey = Encoding.UTF8.GetBytes(LocalCnst.AppKey);

            if (appKey.Length * 8 != this.AES_KEY_SIZE_BIT) throw new InvalidOperationException();

            var userSessionChipher = this.EncryptAES(userSession, key);
            var userSessionSecureChipher = this.EncryptAES(userSessionSecure, key);

            return new Types.Cookie()
            {
                Key = Convert.ToBase64String(ProtectedData.Protect(this.EncryptAES(keyB64,appKey), null, DataProtectionScope.CurrentUser)),
                UserSession = Convert.ToBase64String(userSessionChipher),
                UserSessionSecure = Convert.ToBase64String(userSessionSecureChipher),
            };
        }

        /// <summary>
        /// AESで暗号化された文字列を復号
        /// </summary>
        /// <param name="content"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private byte[] DecryptAES(byte[] rawChipher, byte[] key)
        {
            byte[] iv = rawChipher[0..16];
            byte[] chipher = rawChipher[16..];

            using (var aes = Aes.Create())
            {
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, aes.CreateDecryptor(key, iv), CryptoStreamMode.Write))
                {
                    using (var writer = new BinaryWriter(cs))
                    {
                        writer.Write(chipher);
                    }
                    return ms.ToArray();

                }
            }

        }

        /// <summary>
        /// AESで暗号化
        /// </summary>
        /// <param name="content"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private byte[] EncryptAES(string content, byte[] key)
        {
            using (var aes = Aes.Create())
            {
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, aes.CreateEncryptor(key, aes.IV), CryptoStreamMode.Write))
                {
                    ms.Write(aes.IV);

                    using (var writer = new StreamWriter(cs))
                    {
                        writer.Write(content);
                    }

                    byte[] chipher = ms.ToArray();

                    return chipher;
                }
            }

        }

        /// <summary>
        /// キーを生成
        /// </summary>
        /// <returns></returns>
        private byte[] CreateKey()
        {
            using var aes = Aes.Create();
            return aes.Key;
        }

        /// <summary>
        /// 初期化
        /// </summary>
        private IAttemptResult Initialize()
        {
            IAttemptResult<IReadOnlyList<Types.Cookie>> cookie = this._dataBase.GetAllRecords<Types.Cookie>(TableNames.Cookie);
            if (!cookie.IsSucceeded || cookie.Data is null) return AttemptResult.Fail(cookie.Message);

            if (cookie.Data.Count > 0) return AttemptResult.Succeeded();

            var app = new Types.Cookie();
            return this._dataBase.Insert(app);
        }

        private record Decrypted(string UserSession, string UserSessionSecure);

        #endregion
    }
}
