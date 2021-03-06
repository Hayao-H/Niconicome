﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;
using Niconicome.Models.Domain.Niconico;
using Niconicome.Models.Domain.Utils;

namespace Niconicome.Models.Domain.Local.Handlers
{
    public interface ICoreWebview2Handler
    {
        Task GetAndSetCookiesAsync(ICookieManager cookieManager, CoreWebView2 wv2,string domain);
        Task GetAndSetCookiesAsync(ICookieManager cookieManager, CoreWebView2 wv2,string domain,List<CoreWebView2Cookie> cookies);
        Task DeleteCookiesAsync(ICookieManager cookieManager, CoreWebView2 wv2, string domain);
        Task DeleteBrowserCookiesAsync(CoreWebView2 wv2, string domain);
        Task<List<CoreWebView2Cookie>> GetCookiesAsync(CoreWebView2 wv2, string domain);
    }

    public class CoreWebview2Handler:ICoreWebview2Handler
    {
        public CoreWebview2Handler(ILogger logger)
        {
            this.logger = logger;
        }

        private readonly ILogger logger;

        /// <summary>
        /// WebView2のクッキーをコンテキストに設定する
        /// </summary>
        /// <param name="context"></param>
        /// <param name="wv2"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public async Task GetAndSetCookiesAsync(ICookieManager cookieManager, CoreWebView2 wv2, string domain)
        {
            List<CoreWebView2Cookie> cookies = new();
            await this.InternalGetCookiesAsync(cookies,wv2,domain);

            foreach (var cookie in cookies)
            {
                cookieManager.AddCookie(cookie.Name, cookie.Value);
            }
        }

        /// <summary>
        /// WebView2のクッキーをコンテキストに設定する
        /// </summary>
        /// <param name="context"></param>
        /// <param name="wv2"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public async Task GetAndSetCookiesAsync(ICookieManager cookieManager, CoreWebView2 wv2, string domain,List<CoreWebView2Cookie> cookies)
        {
            await this.InternalGetCookiesAsync(cookies, wv2, domain);

            foreach (var cookie in cookies)
            {
                cookieManager.AddCookie(cookie.Name, cookie.Value);
            }
        }

        /// <summary>
        /// コンテキストのクッキーを削除する
        /// </summary>
        /// <param name="context"></param>
        /// <param name="wv2"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public async Task DeleteCookiesAsync(ICookieManager cookieManager, CoreWebView2 wv2,string domain)
        {
            List<CoreWebView2Cookie> cookies = new();
            await this.InternalGetCookiesAsync(cookies, wv2, domain);
            this.InternalDeleteCookies(cookies, wv2);
            cookieManager.DeleteAllCookies();
        }

        /// <summary>
        /// ブラウザーのクッキーを削除する
        /// </summary>
        /// <param name="wv2"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public async Task DeleteBrowserCookiesAsync(CoreWebView2 wv2, string domain)
        {
            List<CoreWebView2Cookie> cookies = new();
            await this.InternalGetCookiesAsync(cookies, wv2, domain);
            this.InternalDeleteCookies(cookies, wv2);
        }

        /// <summary>
        /// クッキーのリストを取得する内部メソッド
        /// </summary>
        /// <param name="cookies"></param>
        /// <param name="wv2"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        private async Task InternalGetCookiesAsync(List<CoreWebView2Cookie> cookies, CoreWebView2 wv2, string domain)
        {

            try
            {
                cookies.AddRange(await wv2.CookieManager.GetCookiesAsync(domain));
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
        private void InternalDeleteCookies(List<CoreWebView2Cookie> cookies, CoreWebView2 wv2)
        {
            foreach (var cookie in cookies)
            {
                wv2.CookieManager.DeleteCookie(cookie);
            }
        }

        /// <summary>
        /// Cookieを取得する
        /// </summary>
        /// <param name="wv2"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public async Task<List<CoreWebView2Cookie>> GetCookiesAsync(CoreWebView2 wv2, string domain)
        {
            var cookies = new List<CoreWebView2Cookie>();

            await this.InternalGetCookiesAsync(cookies, wv2, domain);

            return cookies;
        }

    }
}
