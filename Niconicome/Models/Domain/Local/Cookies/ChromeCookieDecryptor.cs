using System;
using System.Security.Cryptography;
using System.Text;
using Niconicome.Models.Domain.Niconico.Net.Json.WatchPage.DMC.Request;
using Niconicome.Models.Domain.Utils;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace Niconicome.Models.Domain.Local.Cookies
{
    public interface IChromeCookieDecryptor
    {
        byte[] DecryptCookie(byte[] cipherRaw, byte[] key);
        byte[] GetEncryptionKey(byte[] rawkey);
        string ToUtf8String(byte[] data);
    }

    public class ChromeCookieDecryptor : IChromeCookieDecryptor
    {
        public ChromeCookieDecryptor(ILogger logger)
        {
            this.logger = logger;
        }

        private readonly ILogger logger;

        /// <summary>
        /// 暗号化キーを復号する
        /// </summary>
        /// <param name="rawkey"></param>
        /// <returns></returns>
        public byte[] GetEncryptionKey(byte[] rawkey)
        {
            var cipher = rawkey[5..];
            byte[] plainText;

            try
            {
                plainText = ProtectedData.Unprotect(cipher, null, DataProtectionScope.CurrentUser);
            }
            catch (Exception e)
            {
                this.logger.Error("cookie暗号化キーの復号に失敗しました。", e);
                throw new InvalidOperationException($"cookie暗号化キーの復号に失敗しました。(詳細: {e.Message})");
            }

            return plainText;
        }

        /// <summary>
        /// cookieを復号する
        /// </summary>
        /// <param name="cipherRaw"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public byte[] DecryptCookie(byte[] cipherRaw, byte[] key)
        {
            var encrypted = cipherRaw[15..];
            var iv = cipherRaw[3..15];

            var cipher = new GcmBlockCipher(new AesEngine());
            var parameters = new AeadParameters(new KeyParameter(key), 128, iv, null);
            cipher.Init(false, parameters);
            var plainBytes = new byte[cipher.GetOutputSize(encrypted.Length)];
            int retLen = cipher.ProcessBytes(encrypted, 0, encrypted.Length, plainBytes, 0);
            cipher.DoFinal(plainBytes, retLen);

            return plainBytes;
        }

        /// <summary>
        /// バイト配列を文字列に変換する
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string ToUtf8String(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }
    }
}
