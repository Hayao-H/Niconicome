using System;
using System.Security.Cryptography;
using System.Text;
using Niconicome.Models.Domain.Utils;

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
            var cipher = cipherRaw[15..^16];
            var nonce = cipherRaw[3..15];
            var tag = cipherRaw[^16..^0];
            var plainText = new byte[cipher.Length];
            var aes = new AesGcm(key);

            aes.Decrypt(nonce, cipher, tag, plainText, null);

            return plainText;
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
