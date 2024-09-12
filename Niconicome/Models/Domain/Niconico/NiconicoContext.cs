using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Network;
using Niconicome.Models.Domain.Niconico.Net.Xml;
using Niconicome.Models.Domain.Niconico.UserAuth;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Utils.Reactive;
using Reactive.Bindings;
using Const = Niconicome.Models.Const;
using DI = Niconicome.Models.Domain.Utils.DIFactory;

namespace Niconicome.Models.Domain.Niconico
{
    public interface INiconicoContext
    {
        /// <summary>
        /// ログイン状態
        /// </summary>
        IBindableProperty<bool> IsLogin { get; }

        /// <summary>
        /// ユーザー情報
        /// </summary>
        User? User { get; }

        /// <summary>
        /// ログアウトする
        /// </summary>
        /// <returns></returns>
        Task LogoutAsync();

        /// <summary>
        /// 引数のCookieでログインして、Cookieを保存する
        /// </summary>
        /// <returns></returns>
        Task<IAttemptResult> LoginAndSaveCookieAsync(string userSession, string userSessionSecure);
    }

    public class NiconicoContext : INiconicoContext
    {

        public NiconicoContext(INicoHttp http, ICookieManager cookieManager, IErrorHandler errorHandler, ICookieStore cookieStore)
        {
            this._http = http;
            this._cookieManager = cookieManager;
            this._errorHandler = errorHandler;
            this._cookieStore = cookieStore;
        }

        #region field

        private readonly INicoHttp _http;

        private readonly ICookieManager _cookieManager;

        private readonly IErrorHandler _errorHandler;

        private readonly ICookieStore _cookieStore;

        private readonly string UserNameAPI = "https://seiga.nicovideo.jp/api/user/info?id=";

        #endregion

        #region Props

        public User? User { get; private set; }

        public IBindableProperty<bool> IsLogin { get; init; } = new BindableProperty<bool>(false);

        #endregion

        #region Method

        public async Task LogoutAsync()
        {
            if (!this.IsLogin.Value) return;
            await this._http.GetAsync(new Uri(Const::NetConstant.NiconicoLogoutURL));
            this._cookieManager.DeleteAllCookies();
            this._cookieStore.DeleteCookieInfo();
            this.User = null;
            this.IsLogin.Value = false;
        }

        public async Task<IAttemptResult> LoginAndSaveCookieAsync(string userSession, string userSessionSecure)
        {
            bool loginOK;

            try
            {
                var result = await this._http.GetAsync(new Uri(@"https://nicovideo.jp/my"));
                loginOK = result.IsSuccessStatusCode && result.StatusCode != HttpStatusCode.Found;
            }
            catch (Exception ex)
            {
                return AttemptResult.Fail(this._errorHandler.HandleError(NiconicoContextError.FailedToCheckLoginStatus, ex));
            }

            if (!loginOK)
            {
                return AttemptResult.Fail(this._errorHandler.HandleError(NiconicoContextError.InvalidCookie));
            }

            IAttemptResult<ICookieInfo> cookieResult = this._cookieStore.GetCookieInfo();
            if (!cookieResult.IsSucceeded || cookieResult.Data is null) return AttemptResult.Fail(cookieResult.Message);

            ICookieInfo cookie = cookieResult.Data;
            cookie.UserSession = userSession;
            cookie.UserSessionSecure = userSessionSecure;

            this._cookieManager.AddCookie("user_session", userSession);
            this._cookieManager.AddCookie("user_session_secure", userSessionSecure);

            string userID = userSession.Split("_")[2];

            IAttemptResult<string> userNameResult = await this.GetUserNameAsync(userID);

            if (!userNameResult.IsSucceeded || userNameResult.Data is null)
            {
                return AttemptResult.Fail(userNameResult.Message);
            }

            this.User = new User(userNameResult.Data, userID);

            this.IsLogin.Value = true;

            return AttemptResult.Succeeded();
        }


        #endregion

        #region private

        /// <summary>
        /// ニコニコ静画のAPIを使ってユーザー名を取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<IAttemptResult<string>> GetUserNameAsync(string id)
        {
            HttpResponseMessage result;

            try
            {
                result = await this._http.GetAsync(new Uri($"{this.UserNameAPI}{id}"));
            }
            catch (Exception ex)
            {
                return AttemptResult<string>.Fail(this._errorHandler.HandleError(NiconicoContextError.FailedToGetUserName, ex));
            }

            if (result.IsSuccessStatusCode)
            {
                var response = await result.Content.ReadAsStringAsync();
                Response? xmlData;

                try
                {
                    xmlData = Xmlparser.Deserialize<Response>(response);
                }
                catch (Exception ex)
                {
                    return AttemptResult<string>.Fail(this._errorHandler.HandleError(NiconicoContextError.FailedToGetUserName, ex));
                }

                if (xmlData is null)
                {
                    return AttemptResult<string>.Fail(this._errorHandler.HandleError(NiconicoContextError.FailedToGetUserName));
                }

                return AttemptResult<string>.Succeeded(xmlData.User.Nickname);

            }
            else
            {
                return AttemptResult<string>.Fail(this._errorHandler.HandleError(NiconicoContextError.FailedToGetUserName));
            }
        }

        #endregion

    }

    public enum NiconicoContextError
    {
        [ErrorEnum(ErrorLevel.Error, "有効なCookieではありません。")]
        InvalidCookie,
        [ErrorEnum(ErrorLevel.Error, "ログイン状態の確認に失敗しました。")]
        FailedToCheckLoginStatus,
        [ErrorEnum(ErrorLevel.Error, "ユーザー名の取得に失敗しました。")]
        FailedToGetUserName,
    }
}
