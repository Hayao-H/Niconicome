using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
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
        Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content);

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
        public NicoHttp(HttpClient client)
        {

            var version = Assembly.GetExecutingAssembly().GetName().Version;

            client.DefaultRequestHeaders.Referrer = new Uri(Const::NetConstant.NiconicoBaseURL);
            client.DefaultRequestHeaders.UserAgent.ParseAdd($"Mozilla/5.0 (Niconicome/{version?.Major}.{version?.Minor}.{version?.Build})");
            client.DefaultRequestHeaders.Add("x-frontend-id", "6");
            client.DefaultRequestHeaders.Add("x-frontend-version", "0");
            client.DefaultRequestHeaders.Add("x-client-os-type", "others");

            this._client = client;
        }

        #region Method

        public HttpRequestMessage CreateRequest(HttpMethod method, Uri url)
        {
            var m = new HttpRequestMessage(method, url);
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            m.Headers.UserAgent.ParseAdd($"Mozilla/5.0 (Niconicome/{version?.Major}.{version?.Minor}.{version?.Build})");
            m.Headers.Referrer = new Uri(Const::NetConstant.NiconicoBaseURL);
            m.Headers.Add("x-frontend-id", "6");
            m.Headers.Add("x-frontend-version", "0");
            m.Headers.Add("x-client-os-type", "others");

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

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
        {
            return await this._client.SendAsync(requestMessage, completionOption);
        }

        #endregion

        #region field

        private readonly HttpClient _client;

        #endregion
    }

}
