using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Reflection;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Windows.Media.Protection.PlayReady;
using Const = Niconicome.Models.Const;


namespace Niconicome.Models.Domain.Niconico
{
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
        Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content, Dictionary<string, string>? headers = null);

        /// <summary>
        /// Option
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        Task<HttpResponseMessage> OptionAsync(Uri uri);

        /// <summary>
        /// カスタムリクエスト
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="optn"></param>
        /// <returns></returns>
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage, HttpCompletionOption optn = HttpCompletionOption.ResponseContentRead);
    }

    public class NicoHttp : INicoHttp
    {
        public NicoHttp(HttpClient client, ISettingsContainer settingsContainer)
        {

            var version = Assembly.GetExecutingAssembly().GetName().Version;

            IAttemptResult<string> result = settingsContainer.GetOnlyValue(SettingNames.UserAgent, string.Empty);
            if (!result.IsSucceeded || string.IsNullOrEmpty(result.Data))
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd($"Mozilla/5.0 (Niconicome/{version?.Major}.{version?.Minor}.{version?.Build})");
            }
            else
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd(result.Data);
            }

            client.DefaultRequestHeaders.Referrer = new Uri(Const::NetConstant.NiconicoBaseURL);
            client.DefaultRequestHeaders.Add("x-frontend-id", "3");
            client.DefaultRequestHeaders.Add("x-frontend-version", "0");
            client.DefaultRequestHeaders.Add("x-client-os-type", "others");

            this._client = client;
        }

        #region Method

        public HttpRequestMessage CreateRequest(HttpMethod method, Uri url)
        {
            var m = new HttpRequestMessage(method, url);
            m.Headers.Referrer = new Uri(Const::NetConstant.NiconicoBaseURL);
            m.Headers.UserAgent.ParseAdd(this._client.DefaultRequestHeaders.UserAgent.ToString());

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

        public async Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content, Dictionary<string, string>? headers = null)
        {
            var message = this.CreateRequest(HttpMethod.Post, uri);
            message.Content = content;
            if (headers is not null)
            {
                foreach (var key in headers.Keys)
                {
                    message.Headers.Add(key, headers[key]);
                }
            }
            var cookie = DIFactory.Resolve<ICookieManager>();
            Debug.WriteLine(message);
            return await this._client.SendAsync(message);
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

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
        {
            return await this._client.SendAsync(requestMessage, completionOption);
        }

        #endregion

        #region field

        private readonly HttpClient _client;

        #endregion
    }

    public class NicoHttpClientHandler : HttpClientHandler
    {
        private readonly bool _skipSSL;

        public NicoHttpClientHandler(CookieContainer container, bool skip)
        {
            this.UseCookies = true;
            this._skipSSL = skip;
            this.CookieContainer = container;
            this.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                    {
                        if (this._skipSSL) return true;
                        return sslPolicyErrors == SslPolicyErrors.None;
                    };
        }
    }

}
