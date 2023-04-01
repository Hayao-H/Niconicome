using System;
using System.Collections.Generic;
using Niconicome.Models.Domain.Local.External;

namespace Niconicome.Models.Domain.Local.SQLite
{
    public interface IUserCookieRaw
    {
        bool IsUserSessionExpires { get; }
        bool IsUserSessionSecureExpires { get; }
        bool IsNnicosidExpires { get; }
        byte[]? UserSession { get; }
        byte[]? UserSessionSecure { get; }
    }

    public interface ISqliteCookieLoader
    {
        string GetCookiePath(CookieType type);
        IUserCookieRaw GetCookies(string path, CookieType cookieType = CookieType.Webview2);
    }

    public class SqliteCookieLoader : ISqliteCookieLoader
    {

        public SqliteCookieLoader(ISQliteLoader loader)
        {
            this.loader = loader;
        }

        private readonly ISQliteLoader loader;

        public string GetCookiePath(CookieType type)
        {
            return type switch
            {
                CookieType.Webview2 => @"Niconicome.exe.WebView2\EBWebView\Default\Network\Cookies",
                _ => throw new InvalidOperationException("そのような種別のCookieには対応していません。"),
            };
        }

        /// <summary>
        /// 指定したブラウザーの形式に合わせてSQL分を発行しCookieを取得する
        /// 対応ブラウザー：Firefox 90, Webview2
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cookieType"></param>
        /// <returns></returns>
        public IUserCookieRaw GetCookies(string path, CookieType cookieType = CookieType.Webview2)
        {
            using var loader = this.loader;

            var commandRaw = new List<string>();
            if (cookieType == CookieType.Firefox)
            {
                commandRaw.Add("Select name,value,expiry");
                commandRaw.Add("From moz_cookies");
                commandRaw.Add("Where host = '.nicovideo.jp'");
            }
            else
            {
                commandRaw.Add("Select name,encrypted_value,expires_utc");
                commandRaw.Add("From cookies");
                commandRaw.Add("Where host_key = '.nicovideo.jp'");
            }

            var command = string.Join(' ', commandRaw);

            var reader = loader.GetDataReader(path, command);

            var data = new UserCookieRaw();

            while (reader.Read())
            {
                var name = reader.GetString(0);
                var stream = reader.GetStream(1);
                var len = stream.Length;
                var expires = reader.GetInt64(2);
                var expiresDT = DateTimeOffset.FromUnixTimeSeconds(expires / 1000000 - 11644473600);

                if (name == "user_session")
                {
                    data.UserSession = new byte[stream.Length];
                    stream.Read(data.UserSession);
                    data.IsUserSessionExpires = DateTime.Now > expiresDT.ToLocalTime();
                }
                else if (name == "user_session_secure")
                {
                    data.UserSessionSecure = new byte[stream.Length];
                    stream.Read(data.UserSessionSecure);
                    data.IsUserSessionSecureExpires = DateTime.Now > expiresDT.ToLocalTime();

                }
            }

            return data;
        }
    }

    public class UserCookieRaw : IUserCookieRaw
    {

        public bool IsUserSessionExpires { get; set; }

        public bool IsUserSessionSecureExpires { get; set; }

        public bool IsNnicosidExpires { get; set; }

        public byte[]? UserSession { get; set; }

        public byte[]? UserSessionSecure { get; set; }
    }

    public enum CookieType
    {
        Webview2,
        Firefox
    }
}
