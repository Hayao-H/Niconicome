using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Local.SQLite;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Domain.Utils;
using NiconicomeTest.Local.Cookies;

namespace Niconicome.Models.Domain.Local.LocalFile
{
    public class CookieJsonLoader
    {
        public CookieJsonLoader(ILogger logger)
        {
            this.logger = logger;
        }

        private readonly ILogger logger;

        /// <summary>
        /// JSONファイルのパスを取得する
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetJsonPath(CookieType type)
        {
            return type switch
            {
                CookieType.Webview2 => @"Niconicome.exe.WebView2\EBWebView\Local State",
                _ => throw new InvalidOperationException("そのような種別のcookieには対応していません。")
            };
        }

        /// <summary>
        /// Local Stateファイルを開いて取得する
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public byte[] GetEncryptedKey(string path)
        {
            if (!File.Exists(path))
            {
                throw new IOException($"指定されたファイルは存在しません。({path})");
            }

            LocalStateType data;

            try
            {
                var source = File.OpenText(path).ReadToEnd();
                data = JsonParser.DeSerialize<LocalStateType>(source);
            } catch(Exception e)
            {
                this.logger.Error("Local Stateファイルの読み込みでエラーが発生しました。", e);
                throw new IOException($"Local Stateファイルの読み込みでエラーが発生しました。(詳細:{e.Message})");
            }

            if (data.OsCrypt.EncryptedKey.IsNullOrEmpty())
            {
                throw new IOException("キーの読み取りに失敗しました。");
            }

            return Convert.FromBase64String(data.OsCrypt.EncryptedKey);
        }
    }
}
