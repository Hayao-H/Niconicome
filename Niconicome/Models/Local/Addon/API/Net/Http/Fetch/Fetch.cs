using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ClearScript;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.Infomation;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Permisson;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Utils;

namespace Niconicome.Models.Local.Addon.API.Net.Http.Fetch
{
    public interface IFetch
    {
        /// <summary>
        /// リクエストを処理する
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        Task<Response> FetchAsync(string url, IFetchOption? option);

        /// <summary>
        /// 初期化する
        /// </summary>
        /// <param name="hostPermissions"></param>
        void Initialize(IAddonInfomation info);
    }


    public class Fetch : IFetch
    {
        public Fetch(IAddonHttp http,IHostPermissionsHandler permissionsHandler)
        {
            this.http = http;
            this.permissionsHandler = permissionsHandler;
        }

        #region field

        private readonly IAddonHttp http;

        private readonly IHostPermissionsHandler permissionsHandler;

        private bool isInitialized;

        #endregion

        public async Task<Response> FetchAsync(string url, IFetchOption? option = null)
        {
            if (!this.isInitialized)
            {
                throw new InvalidOperationException("未初期化です。");
            }

            if (!this.permissionsHandler.CanAccess(url))
            {
                throw new InvalidOperationException($"{url}は許可されていないホストです。");
            }

            HttpContent? content = null;
            if (option?.method == "POST")
            {
                if (option.body is null or Undefined or not string) throw new InvalidOperationException("POSTメソッドではbodyをnullにできません。");
                content = new StringContent(option.body);
            }

            HttpResponseMessage message;
            if (option?.credentials == "include")
            {
                if (this.http.NicoHttp is null) throw new InvalidOperationException($"{PermissionNames.Session}権限を取得していないため、credentials:includeは無効です。");
                message = option?.method switch
                {
                    "GET" => await this.http.NicoHttp.GetAsync(new Uri(url)),
                    "POST" => await this.http.NicoHttp.PostAsync(new Uri(url), content!),
                    _ => await this.http.NicoHttp.GetAsync(new Uri(url)),
                };
            }

            message = option?.method switch
            {
                "GET" => await this.http.GetAsync(new Uri(url)),
                "POST" => await this.http.PostAsync(new Uri(url), content!),
                _ => await this.http.GetAsync(new Uri(url)),
            };

            var res = new Response(message);

            return res;
        }

        public void Initialize(IAddonInfomation info)
        {
            this.http.Initialize(info);
            this.permissionsHandler.Initialize(info.HostPermissions);
            this.isInitialized = true;
        }


    }
}
