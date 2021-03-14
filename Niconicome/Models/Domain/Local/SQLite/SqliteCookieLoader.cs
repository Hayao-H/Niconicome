using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.SQLite
{
    public interface IUserCookieRaw
    {
        byte[]? UserSession { get; }
        byte[]? UserSessionSecure { get; }
        byte[]? Nicosid { get; }
    }

    public interface ISqliteCookieLoader
    {
        string GetCookiePath(CookieType type);
        IUserCookieRaw GetCookies(string path);
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
                CookieType.Webview2 => @"Niconicome.exe.WebView2\Niconicome.exe.WebView2\Default\Cookies",
                _ => throw new InvalidOperationException("そのような種別のCookieには対応していません。"),
            };
        }

        public IUserCookieRaw GetCookies(string path)
        {
            using var loader = this.loader;

            var command =
                $"Select name,encrypted_value " +
                $"From cookies " +
                $"Where host_key = '.nicovideo.jp'";

            var reader = loader.GetDataReader(path, command);

            var data = new UserCookieRaw();

            while (reader.Read())
            {
                var name = reader.GetString(0);
                var stream = reader.GetStream(1);
                var len = stream.Length;

                if (name == "user_session")
                {
                    data.UserSession = new byte[stream.Length];
                    stream.Read(data.UserSession);
                }
                else if (name == "user_session_secure")
                {
                    data.UserSessionSecure = new byte[stream.Length];
                    stream.Read(data.UserSessionSecure);

                }
                else if (name == "nicosid")
                {
                    data.Nicosid = new byte[stream.Length];
                    stream.Read(data.Nicosid);
                }
            }

            return data;
        }
    }

    public class UserCookieRaw :IUserCookieRaw{

       public byte[]? UserSession { get; set; }

       public byte[]? UserSessionSecure { get; set; }

       public byte[]? Nicosid { get; set; }
    }

    public enum CookieType
    {
        Webview2
    }
}
