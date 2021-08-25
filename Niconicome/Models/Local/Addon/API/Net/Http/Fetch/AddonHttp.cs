﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.Core;
using Niconicome.Models.Domain.Local.Addons.Core.Permisson;
using Niconicome.Models.Domain.Niconico;

namespace Niconicome.Models.Local.Addon.API.Net.Http.Fetch
{
    public interface IAddonHttp
    {
        INicoHttp? NicoHttp { get; }
        void Initialize(AddonInfomation addon);
        Task<HttpResponseMessage> GetAsync(Uri uri);
        Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content);
    }

    public class AddonHttp : IAddonHttp
    {
        public AddonHttp(HttpClient client, INicoHttp nicoHttp)
        {
            this.client = client;
            this.NicoHttp = nicoHttp;
        }

        #region field

        private readonly HttpClient client;

        #endregion

        #region Props

        public INicoHttp? NicoHttp { get; private set; }

        #endregion

        #region Method

        public void Initialize(AddonInfomation addon)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;

            this.client.DefaultRequestHeaders.Referrer = this.NicoHttp!.NiconicoBaseUri;
            this.client.DefaultRequestHeaders.UserAgent.ParseAdd($"Mozilla/5.0 (Niconicome/{version?.Major}.{version?.Minor}.{version?.Build})");
            this.client.DefaultRequestHeaders.Add("X-Frontend-Id", "6");
            this.client.DefaultRequestHeaders.Add("X-Frontend-Version", "0");

            if (!addon.Permissions.Contains(PermissionNames.Session))
            {
                this.NicoHttp = null;
            }
        }

        public async Task<HttpResponseMessage> GetAsync(Uri uri)
        {
            return await this.client.GetAsync(uri);
        }

        public async Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content)
        {
            return await this.client.PostAsync(uri, content);
        }

        #endregion
    }
}