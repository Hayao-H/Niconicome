using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
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
    }

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

}
