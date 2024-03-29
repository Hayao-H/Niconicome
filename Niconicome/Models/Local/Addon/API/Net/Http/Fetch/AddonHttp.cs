﻿using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.Infomation;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Permisson;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Niconico;
using Niconicome.Models.Helper.Result;
using Const = Niconicome.Models.Const;

namespace Niconicome.Models.Local.Addon.API.Net.Http.Fetch
{
    public interface IAddonHttp
    {
        INicoHttp? NicoHttp { get; }
        void Initialize(IAddonInfomation addon);
        Task<HttpResponseMessage> GetAsync(Uri uri);
        Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content);
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead);
    }

    public class AddonHttp : IAddonHttp
    {
        public AddonHttp(HttpClient client, INicoHttp nicoHttp, ISettingsContainer settingsContainer)
        {
            this.client = client;
            this.NicoHttp = nicoHttp;
            this._settingsContainer = settingsContainer;
        }

        #region field

        private readonly HttpClient client;

        private readonly ISettingsContainer _settingsContainer;

        #endregion

        #region Props

        public INicoHttp? NicoHttp { get; private set; }

        #endregion

        #region Method

        public void Initialize(IAddonInfomation addon)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;

            IAttemptResult<string> result = this._settingsContainer.GetOnlyValue(SettingNames.UserAgent, string.Empty);
            if (!result.IsSucceeded || string.IsNullOrEmpty(result.Data))
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd($"Mozilla/5.0 (Niconicome/{version?.Major}.{version?.Minor}.{version?.Build}) Addon");
            }
            else
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd(result.Data);
            }

            this.client.DefaultRequestHeaders.Referrer = new Uri(Const::NetConstant.NiconicoBaseURL);
            this.client.DefaultRequestHeaders.Add("X-Frontend-Id", "6");
            this.client.DefaultRequestHeaders.Add("X-Frontend-Version", "0");

            if (!addon.Permissions.Contains(Permissions.Session))
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

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
        {
            return await this.client.SendAsync(requestMessage, completionOption);
        }

        #endregion
    }
}
