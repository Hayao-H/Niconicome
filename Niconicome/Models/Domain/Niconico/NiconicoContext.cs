using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Extensions.DependencyInjection;
using Const = Niconicome.Models.Const;
using Niconicome.Models.Domain.Local;
using Niconicome.Models.Domain.Local.Store.Types;
using Niconicome.Models.Domain.Network;
using Niconicome.Models.Domain.Niconico.Net.Html;
using Niconicome.Models.Domain.Niconico.Net.Xml;
using Niconicome.Models.Domain.Utils;
using Reactive.Bindings;

namespace Niconicome.Models.Domain.Niconico
{
    public interface INiconicoContext
    {
        /// <summary>
        /// ログイン状態
        /// </summary>
        bool IsLogin { get; }

        /// <summary>
        /// ユーザー情報
        /// </summary>
        ReactiveProperty<User?> User { get; }

        /// <summary>
        /// ログインする
        /// </summary>
        /// <param name="u"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        Task<bool> LoginAsync(string u, string p);

        /// <summary>
        /// ログアウトする
        /// </summary>
        /// <returns></returns>
        Task LogoutAsync();

        /// <summary>
        /// ページURLを取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Uri GetPageUri(string id);

        /// <summary>
        /// ユーザー情報を更新
        /// </summary>
        /// <returns></returns>
        Task RefreshUser();
    }

    public interface INicoHttp
    {
        /// <summary>
        /// 非同期に文字列を取得する
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        Task<string> GetStringAsync(Uri uri);

        /// <summary>
        /// GET
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        Task<HttpResponseMessage> GetAsync(Uri uri);

        /// <summary>
        /// POST
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content);

        /// <summary>
        /// Option
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        Task<HttpResponseMessage> OptionAsync(Uri uri);
    }

    public interface ICookieManager
    {
        /// <summary>
        /// Cookieコンテナ
        /// </summary>
        CookieContainer CookieContainer { get; }

        /// <summary>
        /// 指定したCookieを取得
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        string GetCookie(string k);

        /// <summary>
        /// Cookieを追加
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void AddCookie(string name, string value);

        /// <summary>
        /// すべてのCookieを削除
        /// </summary>
        void DeleteAllCookies();

        /// <summary>
        /// Cookieの有無を確認
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        bool HasCookie(string k);
    }

    /// <summary>
    /// クッキーを一元管理する
    /// </summary>
    public class CookieManager : ICookieManager
    {

        public CookieManager()
        {
            this.niconicoBaseUri = new Uri(Const::NetConstant.NiconicoBaseURLNonSSL);
        }

        #region field

        private readonly Uri niconicoBaseUri;

        #endregion

        #region Props

        public CookieContainer CookieContainer { get; private set; } = new();


        #endregion

        #region Method

        public bool HasCookie(string key)
        {
            var cookies = this.GetAllNiconicoCookies();
            foreach (Cookie cookie in cookies)
            {
                if (cookie.Name == key && !cookie.Expired)
                {
                    return true;
                }
            }

            return false;
        }

        public string GetCookie(string key)
        {
            foreach (Cookie cookie in this.GetAllNiconicoCookies())
            {
                if (cookie.Name == key)
                {
                    return cookie.Value;
                }
            }

            return string.Empty;
        }

        public void AddCookie(string name, string value)
        {
            var cookie = new Cookie(name, value)
            {
                Domain = ".nicovideo.jp"
            };
            this.CookieContainer.Add(this.niconicoBaseUri, cookie);
        }

        public void DeleteAllCookies()
        {
            var cookies = this.GetAllNiconicoCookies();
            foreach (Cookie cookie in cookies)
            {
                cookie.Expires = DateTime.Now.Subtract(TimeSpan.FromDays(1));
            }
        }

        #endregion

        #region private

        private IEnumerable<Cookie> GetAllNiconicoCookies()
        {
            var h = this.CookieContainer.GetAllCookies();
            return this.CookieContainer.GetAllCookies().Where(c => c.Domain.Contains(Const::NetConstant.NiconicoDomain));
        }

        #endregion

    }

    /// <summary>
    /// httpClientを提供するクラス
    /// </summary>
    public class NicoHttp : INicoHttp
    {
        public NicoHttp(HttpClient client)
        {

            var version = Assembly.GetExecutingAssembly().GetName().Version;

            client.DefaultRequestHeaders.Referrer = new Uri(Const::NetConstant.NiconicoBaseURL);
            client.DefaultRequestHeaders.UserAgent.ParseAdd($"Mozilla/5.0 (Niconicome/{version?.Major}.{version?.Minor}.{version?.Build})");
            client.DefaultRequestHeaders.Add("X-Frontend-Id", "6");
            client.DefaultRequestHeaders.Add("X-Frontend-Version", "0");

            this._client = client;
        }

        #region Method

        public HttpRequestMessage CreateRequest(HttpMethod method, Uri url)
        {
            var m = new HttpRequestMessage(method, url);
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            m.Headers.UserAgent.ParseAdd($"Mozilla/5.0 (Niconicome/{version?.Major}.{version?.Minor}.{version?.Build})");
            m.Headers.Referrer = new Uri(Const::NetConstant.NiconicoBaseURL);
            m.Headers.Add("X-Frontend-Id", "6");
            m.Headers.Add("X-Frontend-Version", "0");

            return m;
        }

        public async Task<string> GetStringAsync(Uri uri)
        {
            return await this._client.GetStringAsync(uri);
        }

        public async Task<HttpResponseMessage> GetAsync(Uri uri)
        {
            return await this._client.GetAsync(uri);
        }

        public async Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content)
        {
            return await this._client.PostAsync(uri, content);
        }

        public async Task<HttpResponseMessage> OptionAsync(Uri uri)
        {
            var message = new HttpRequestMessage(HttpMethod.Options, uri);
            return await this._client.SendAsync(message);
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return await this._client.SendAsync(request);
        }


        #endregion

        #region field

        private readonly HttpClient _client;

        #endregion
    }

    /// <summary>
    /// ログインセッションを管理する
    /// </summary>
    public class NiconicoContext : INiconicoContext
    {

        public NiconicoContext(INicoHttp http, ICookieManager cookieManager, ILogger logger, INetWorkHelper helper)
        {
            this._http = http;
            this._cookieManager = cookieManager;
            this._logger = logger;
            this._helper = helper;
            this.User = new ReactiveProperty<User?>();
        }

        #region field

        private readonly INicoHttp _http;

        private readonly ICookieManager _cookieManager;

        private readonly ILogger _logger;

        private readonly INetWorkHelper _helper;

        private readonly string UserNameAPI = "https://seiga.nicovideo.jp/api/user/info?id=";

        #endregion

        #region Props

        public static INiconicoContext Context { get; private set; } = DIFactory.Provider.GetRequiredService<INiconicoContext>();

        public ReactiveProperty<User?> User { get; init; }

        public bool IsLogin
        {
            get
            {
                return this._cookieManager.HasCookie("user_session");
            }
        }

        #endregion

        #region Method

        public async Task<bool> LoginAsync(string username, string password)
        {
            if (this.IsLogin) return true;

            //Cookieを削除
            this._cookieManager.DeleteAllCookies();

            //ログイン処理
            var data = new Dictionary<string, string?>()
            {
                {"mail_tel",username },
                {"password",password },
                {"next_url",null }
            };

            var formData = new FormUrlEncodedContent((IEnumerable<KeyValuePair<string?, string?>>)data);

            HttpResponseMessage result = await this._http.PostAsync(new Uri(Const::NetConstant.NiconicoLoginURL), formData);

            //this._cookieManager.SetCookies(new Uri(Const.Net.NiconicoBaseURLNonSSL), result.Headers);
            if (result.IsSuccessStatusCode || result.StatusCode == HttpStatusCode.Found)
            {
                if (this._cookieManager.HasCookie("user_session"))
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

        public async Task LogoutAsync()
        {
            if (!this.IsLogin) return;
            await this._http.GetAsync(new Uri(Const::NetConstant.NiconicoLogoutURL));
            this._cookieManager.DeleteAllCookies();
            this.User.Value = null;
        }


        public async Task RefreshUser()
        {
            if (!this.IsLogin) return;
            string userID = this._cookieManager.GetCookie("user_session").Split('_')[2];
            string userName = await this.GetUserNameAsync(userID) + "さん";

            this.User.Value = new User(userName, userID);
        }

        public Uri GetPageUri(string id)
        {
            return new Uri($"https://nicovideo.jp/watch/{id}");
        }

        #endregion

        #region private
        private async Task<string> GetUserNameAsync(string id)
        {
            HttpResponseMessage result = await this._http.GetAsync(new Uri($"{this.UserNameAPI}{id}"));

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

        #endregion

    }
}
