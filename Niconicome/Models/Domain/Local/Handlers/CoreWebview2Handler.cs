using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;
using Niconicome.Models.Domain.Niconico;
using Niconicome.Models.Domain.Utils;

namespace Niconicome.Models.Domain.Local.Handlers
{
    public interface ICoreWebview2Handler
    {
        /// <summary>
        /// 指定したドメインのCookieを削除する
        /// </summary>
        /// <param name="wv2"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        Task DeleteBrowserCookiesAsync(string domain);

        /// <summary>
        /// Cookieを取得する
        /// </summary>
        /// <param name="wv2"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        Task<List<CoreWebView2Cookie>> GetCookiesAsync(string domain);

        /// <summary>
        /// 初期化する
        /// </summary>
        /// <param name="wv2"></param>
        void Initialize(CoreWebView2 wv2);

        /// <summary>
        /// URLフィルター関数を登録する
        /// </summary>
        /// <param name="func"></param>
        void RegisterFilterFunc(Func<string, bool> func);

        /// <summary>
        /// HTMLを表示する
        /// </summary>
        /// <param name="html"></param>
        void NavigateToString(string html);
    }

    public class CoreWebview2Handler : ICoreWebview2Handler
    {
        public CoreWebview2Handler(ILogger logger)
        {
            this.logger = logger;
        }

        #region field

        private readonly ILogger logger;

        private Func<string, bool>? filter;

        private bool isInitialized;

        private CoreWebView2? wv2;

        private SynchronizationContext? ctx;

        #endregion

        public async Task DeleteBrowserCookiesAsync(string domain)
        {
            this.CheckIfInitialized();

            List<CoreWebView2Cookie> cookies = new();
            await this.InternalGetCookiesAsync(cookies, domain);
            this.InternalDeleteCookies(cookies);
        }

        public async Task<List<CoreWebView2Cookie>> GetCookiesAsync(string domain)
        {
            this.CheckIfInitialized();

            var cookies = new List<CoreWebView2Cookie>();

            await this.InternalGetCookiesAsync(cookies, domain);

            return cookies;
        }

        public void Initialize(CoreWebView2 wv2)
        {
            if (this.isInitialized) return;

            this.wv2 = wv2;
            this.ctx = SynchronizationContext.Current;
            this.wv2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
            this.wv2.WebResourceRequested += this.OnNavigate;
            this.isInitialized = true;
        }

        public void RegisterFilterFunc(Func<string, bool> func)
        {
            this.filter = func;
        }

        public void NavigateToString(string html)
        {
            this.CheckIfInitialized();
            this.ctx?.Post(_ => this.wv2!.NavigateToString(html), null);
        }



        #region private

        private void CheckIfInitialized()
        {
            if (!this.isInitialized) throw new InvalidCastException("ハンドラが初期化されていません。");
        }

        /// <summary>
        /// クッキーのリストを取得する内部メソッド
        /// </summary>
        /// <param name="cookies"></param>
        /// <param name="wv2"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        private async Task InternalGetCookiesAsync(List<CoreWebView2Cookie> cookies, string domain)
        {

            try
            {
                cookies.AddRange(await this.wv2!.CookieManager.GetCookiesAsync(domain));
            }
            catch (ArgumentException ex)
            {
                this.logger.Error("ブラウザーからのクッキー取得に失敗しました。", ex);
            }
            catch (Exception ex)
            {
                this.logger.Error("ブラウザーからのクッキー取得に失敗しました。", ex);
            }

        }

        /// <summary>
        /// クッキーを削除する内部メソッド
        /// </summary>
        /// <param name="cookies"></param>
        /// <param name="wv2"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        private void InternalDeleteCookies(List<CoreWebView2Cookie> cookies)
        {
            foreach (var cookie in cookies)
            {
                this.wv2!.CookieManager.DeleteCookie(cookie);
            }
        }

        private void OnNavigate(object? sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            var canAccess = this.filter?.Invoke(e.Request.Uri) ?? true;
            if (!canAccess)
            {
                e.Response = this.wv2!.Environment.CreateWebResourceResponse(new MemoryStream(), 0, "Permission Requred", null);
            }
        }

        #endregion

    }
}
