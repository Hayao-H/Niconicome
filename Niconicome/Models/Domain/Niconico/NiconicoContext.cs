using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Models.Domain.Local;
using Niconicome.Models.Domain.Local.Store.Types;
using Niconicome.Models.Domain.Niconico.Net.Xml;
using Niconicome.Models.Domain.Utils;

namespace Niconicome.Models.Domain.Niconico
{
    public interface INiconicoContext
    {
        Task<bool> Login(string u, string p);
        Task Logout();
        bool IsLogin { get; }
        Task<string> GetUserName(string i);
        static User? User { get; }
        Uri GetPageUri(string id);
        Task RefreshUser();
    }

    public interface INicoHttp
    {
        Task<string> GetStringAsync(Uri uri);
        Task<HttpResponseMessage> GetAsync(Uri uri);
        Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content);
        Task<HttpResponseMessage> OptionAsync(Uri uri);
        Uri NiconicoLoginUri { get; }
        Uri NiconicoBaseUri { get; }
        Uri NiconicoLogoutUri { get; }
    }

    public interface ICookieManager
    {
        CookieContainer CookieContainer { get; }
        string GetCookie(string k);
        void AddCookie(string name, string value);
        void DeleteAllCookies();
        void DeleteCookies(Uri domain);
        bool HasCookie(string k);
    }

    public interface IDbUrlHandler
    {
        Uri GetUriFromDB(string settingName, string defaultUri);
    }

    public class DbUrlHandler : IDbUrlHandler
    {

        public DbUrlHandler(IDataBase dataBase)
        {
            this.dataBase = dataBase;
        }

        private readonly IDataBase dataBase;

        /// <summary>
        /// データベースから各種URLを取得する
        /// </summary>
        /// <param name="settingName"></param>
        /// <param name="defaultUri"></param>
        /// <returns></returns>
        public Uri GetUriFromDB(string settingName, string defaultUri)
        {
            bool exists = this.dataBase.Exists<UrlSetting>(UrlSetting.TableName, setting => setting.SettingName == settingName);
            if (exists)
            {
                return this.dataBase.GetRecord<UrlSetting>(UrlSetting.TableName, setting => setting.SettingName == settingName)!.GetUrl();
            }
            else
            {
                this.dataBase.Store(new UrlSetting() { SettingName = settingName, UrlString = defaultUri }, UrlSetting.TableName);
                return new Uri(defaultUri);
            }
        }

    }

    /// <summary>
    /// クッキーを一元管理する
    /// </summary>
    public class CookieManager : ICookieManager
    {

        public CookieManager()
        {
            this.niconicoBaseUri = new Uri("http://nicovideo.jp"); 
        }

        private readonly Uri niconicoBaseUri;

        public CookieContainer CookieContainer { get; private set; } = new();


        /// <summary>
        /// cookieを検索
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool HasCookie(string key)
        {
            var cookies = this.CookieContainer.GetCookies(this.niconicoBaseUri);
            foreach (Cookie cookie in cookies)
            {
                if (cookie.Name == key && !cookie.Expired)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// クッキーを取得
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetCookie(string key)
        {
            foreach (Cookie cookie in this.CookieContainer.GetCookies(this.niconicoBaseUri))
            {
                if (cookie.Name == key)
                {
                    return cookie.Value;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// クッキーを追加する
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddCookie(string name, string value)
        {
            var cookie = new Cookie(name, value)
            {
                Domain = ".nicovideo.jp"
            };
            this.CookieContainer.Add(this.niconicoBaseUri, cookie);
        }

        /// <summary>
        /// 全てのクッキーを削除する
        /// </summary>
        public void DeleteAllCookies()
        {
            var cookies = this.CookieContainer.GetCookies(this.niconicoBaseUri);
            foreach (Cookie cookie in cookies)
            {
                cookie.Expires = DateTime.Now.Subtract(TimeSpan.FromDays(1));
            }
        }

        /// <summary>
        /// クッキーを削除する
        /// </summary>
        public void DeleteCookies(Uri domain)
        {
            var cookies = this.CookieContainer.GetCookies(domain);
            foreach (Cookie cookie in cookies)
            {
                cookie.Expires = DateTime.Now.Subtract(TimeSpan.FromDays(1));
            }
        }

    }

    /// <summary>
    /// httpClientを提供するクラス
    /// </summary>
    public class NicoHttp : INicoHttp
    {
        public NicoHttp(HttpClient client, IDbUrlHandler dbUrlHandler)
        {
            //ニコニコのアドレスを設定
            this.NiconicoBaseUri = dbUrlHandler.GetUriFromDB(nameof(this.NiconicoBaseUri), "https://nicovideo.jp");
            this.NiconicoLoginUri = dbUrlHandler.GetUriFromDB(nameof(this.NiconicoLoginUri), "https://secure.nicovideo.jp/secure/login?site=niconico");
            this.NiconicoLogoutUri = dbUrlHandler.GetUriFromDB(nameof(this.NiconicoLogoutUri), "https://account.nicovideo.jp/logout");

            var version = Assembly.GetExecutingAssembly().GetName().Version;

            client.DefaultRequestHeaders.Referrer = this.NiconicoBaseUri;
            client.DefaultRequestHeaders.UserAgent.ParseAdd($"Mozilla/5.0 (Niconicome/{version?.Major}.{version?.Minor}.{version?.Build})");
            client.DefaultRequestHeaders.Add("X-Frontend-Id", "6");
            client.DefaultRequestHeaders.Add("X-Frontend-Version", "0");

            this.client = client;
        }

        /// <summary>
        /// ログイン用URL
        /// </summary>
        public Uri NiconicoLoginUri { get; init; }

        /// <summary>
        /// ベースURL
        /// </summary>
        public Uri NiconicoBaseUri { get; init; }

        /// <summary>
        /// ログアウト用URL
        /// </summary>
        public Uri NiconicoLogoutUri { get; init; }

        /// <summary>
        /// GETメソッド(string)
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task<string> GetStringAsync(Uri uri)
        {
            return await this.client.GetStringAsync(uri);
        }


        /// <summary>
        /// GETメソッド
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> GetAsync(Uri uri)
        {
            return await this.client.GetAsync(uri);
        }

        /// <summary>
        /// POSTメソッド
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content)
        {
            return await this.client.PostAsync(uri, content);
        }

        /// <summary>
        /// OPTIONメソッドを送信する
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> OptionAsync(Uri uri)
        {
            var message = new HttpRequestMessage(HttpMethod.Options, uri);
            return await this.client.SendAsync(message);
        }

        /// <summary>
        /// HttpClient
        /// </summary>
        private readonly HttpClient client;
    }

    /// <summary>
    /// ログインセッションを管理する
    /// </summary>
    public class NiconicoContext : INiconicoContext
    {

        public NiconicoContext(INicoHttp http, ICookieManager cookieManager)
        {
            this.http = http;
            this.cookieManager = cookieManager;

            //ニコニコのアドレスを設定
            this.niconicoLoginUri = this.http.NiconicoLoginUri;
            this.niconicoLogoutUri = this.http.NiconicoLogoutUri;
        }

        /// <summary>
        /// Httpクライアント
        /// </summary>
        private readonly INicoHttp http;

        /// <summary>
        /// cookie管理
        /// </summary>
        private readonly ICookieManager cookieManager;

        /// <summary>
        /// 外部から取得
        /// </summary>
        public static INiconicoContext Context { get; private set; } = DIFactory.Provider.GetRequiredService<INiconicoContext>();

        /// <summary>
        /// ログイン用URL
        /// </summary>
        private readonly Uri niconicoLoginUri;

        /// <summary>
        /// ログアウト用URL
        /// </summary>
        private readonly Uri niconicoLogoutUri;

        /// <summary>
        /// ユーザー名取得API
        /// </summary>
        private readonly string UserNameAPI = "https://seiga.nicovideo.jp/api/user/info?id=";

        /// <summary>
        /// ユーザー情報
        /// </summary>
        public static User? User { get; private set; }

        /// <summary>
        /// ログイン状態を取得する
        /// </summary>
        public bool IsLogin
        {
            get
            {
                return this.cookieManager.HasCookie("user_session");
            }
        }

        /// <summary>
        /// ログイン
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> Login(string username, string password)
        {
            if (this.IsLogin) return true;

            this.cookieManager.DeleteAllCookies();

            var data = new Dictionary<string, string>()
            {
                {"mail_tel",username },
                {"password",password }
            };

            var formData = new FormUrlEncodedContent((IEnumerable<KeyValuePair<string?, string?>>)data);

            HttpResponseMessage result = await this.http.PostAsync(this.niconicoLoginUri, formData);

            if (result.IsSuccessStatusCode)
            {
                if (this.cookieManager.HasCookie("user_session"))
                {
                    await this.RefreshUser();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// ログアウト
        /// </summary>
        /// <returns></returns>
        public async Task Logout()
        {
            if (!this.IsLogin) return;
            await this.http.GetAsync(this.niconicoLogoutUri);
            this.cookieManager.DeleteAllCookies();
        }

        /// <summary>
        /// ユーザー名を取得
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<string> GetUserName(string id)
        {
            HttpResponseMessage result = await this.http.GetAsync(new Uri($"{this.UserNameAPI}{id}"));

            if (result.IsSuccessStatusCode)
            {
                var response = await result.Content.ReadAsStringAsync();
                Response? xmlData;

                try
                {
                    xmlData = Xmlparser.Deserialize<Response>(response);
                }
                catch
                {
                    return string.Empty;
                }

                if (xmlData is null)
                {
                    return string.Empty;
                }

                return xmlData.User.Nickname;

            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// ユーザー名を設定する
        /// </summary>
        /// <returns></returns>
        public async Task RefreshUser()
        {
            if (!this.IsLogin) return;
            string userID = this.cookieManager.GetCookie("user_session").Split('_')[2];
            string userName = await this.GetUserName(userID) + "さん";

            NiconicoContext.User = new User(userName, userID);
        }

        /// <summary>
        /// ニコニコ動画のURIを取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Uri GetPageUri(string id)
        {
            return new Uri($"https://nicovideo.jp/watch/{id}");
        }

    }
}
