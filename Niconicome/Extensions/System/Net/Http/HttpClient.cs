using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Niconicome.Extensions.System.Net.Http
{
    static class HttpClientExt
    {

        /// <summary>
        /// optionメソッドを実行する
        /// </summary>
        /// <param name="client"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> OptionAsync(this HttpClient client,string url)
        {
            return client.OptionAsync(new Uri(url));
        }

        /// <summary>
        /// optionメソッドを実行する
        /// </summary>
        /// <param name="client"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> OptionAsync(this HttpClient client,Uri url)
        {
            var request = new HttpRequestMessage(HttpMethod.Options, url);
            return client.SendAsync(request);
        }
    }
}
