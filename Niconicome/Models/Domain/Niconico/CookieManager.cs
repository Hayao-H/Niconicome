using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Const = Niconicome.Models.Const;
namespace Niconicome.Models.Domain.Niconico
{
    public interface ICookieManager
    {
        /// <summary>
        /// Cookieコンテナ
        /// </summary>
        CookieContainer CookieContainer { get; }

        /// <summary>
        /// 指定したCookieを取得
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        string GetCookie(string k);

        /// <summary>
        /// Cookieを追加
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void AddCookie(string name, string value);

        /// <summary>
        /// すべてのCookieを削除
        /// </summary>
        void DeleteAllCookies();

        /// <summary>
        /// Cookieの有無を確認
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        bool HasCookie(string k);
    }

    public class CookieManager : ICookieManager
    {

        public CookieManager()
        {
            this.niconicoBaseUri = new Uri(Const::NetConstant.NiconicoBaseURLNonSSL);
        }

        #region field

        private readonly Uri niconicoBaseUri;

        #endregion

        #region Props

        public CookieContainer CookieContainer { get; private set; } = new();


        #endregion

        #region Method

        public bool HasCookie(string key)
        {
            var cookies = this.GetAllNiconicoCookies();
            foreach (Cookie cookie in cookies)
            {
                if (cookie.Name == key && !cookie.Expired)
                {
                    return true;
                }
            }

            return false;
        }

        public string GetCookie(string key)
        {
            foreach (Cookie cookie in this.GetAllNiconicoCookies())
            {
                if (cookie.Name == key)
                {
                    return cookie.Value;
                }
            }

            return string.Empty;
        }

        public void AddCookie(string name, string value)
        {
            var cookie = new Cookie(name, value)
            {
                Domain = ".nicovideo.jp"
            };
            this.CookieContainer.Add(this.niconicoBaseUri, cookie);
        }

        public void DeleteAllCookies()
        {
            var cookies = this.GetAllNiconicoCookies();
            foreach (Cookie cookie in cookies)
            {
                cookie.Expires = DateTime.Now.Subtract(TimeSpan.FromDays(1));
            }
        }

        #endregion

        #region private

        private IEnumerable<Cookie> GetAllNiconicoCookies()
        {
            var h = this.CookieContainer.GetAllCookies();
            return this.CookieContainer.GetAllCookies().Where(c => c.Domain.Contains(Const::NetConstant.NiconicoDomain));
        }

        #endregion

    }
}
