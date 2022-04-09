using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Models.Domain.Network;
using Niconicome.Models.Domain.Niconico.Net.Xml;
using Niconicome.Models.Domain.Utils;
using Reactive.Bindings;
using Const = Niconicome.Models.Const;

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
