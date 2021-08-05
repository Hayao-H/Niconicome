using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico;

namespace Niconicome.Models.Local.Addon.API.Net.Http.Fetch
{
    public interface IFetch
    {
        /// <summary>
        /// リクエストを処理する
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        Task<Response> FetchAsync(string url);

        /// <summary>
        /// 初期化する
        /// </summary>
        /// <param name="hostPermissions"></param>
        void Initialize(List<string> hostPermissions);
    }


    public class Fetch : IFetch
    {
        public Fetch(INicoHttp http)
        {
            this.http = http;
        }

        #region field

        private readonly INicoHttp http;

        private readonly List<string> hosts = new();

        private bool isInitialized;

        #endregion

        public async Task<Response> FetchAsync(string url)
        {
            if (!this.isInitialized)
            {
                throw new InvalidOperationException("未初期化です。");
            }

            if (!this.hosts.Any(host => Regex.IsMatch(url,host)))
            {
                throw new InvalidOperationException($"{url}は許可されていないホストです。");
            }

            HttpResponseMessage message = await this.http.GetAsync(new Uri(url));

            var res = new Response(message);

            return res;
        }

        public void Initialize(List<string> hostPermissions)
        {
            this.hosts.AddRange(hostPermissions.Select(host =>
            {
                string replaced = host.Replace("*", ".*");

                replaced = Regex.Replace(replaced, @"\.(?!\*)", @"\.");

                return replaced;
            }));
            this.isInitialized = true;
        }


    }
}
