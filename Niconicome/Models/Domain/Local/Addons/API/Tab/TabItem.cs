using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Web.WebView2.Core;
using Niconicome.Models.Domain.Local.Handlers;
using Niconicome.Models.Domain.Utils;

namespace Niconicome.Models.Domain.Local.Addons.API.Tab
{
    public interface ITabItem
    {
        /// <summary>
        /// タブを閉じる
        /// </summary>
        /// <returns></returns>
        bool Close();

        /// <summary>
        /// 初期化する
        /// </summary>
        /// <param name="wv2"></param>
        void Initialize(CoreWebView2 wv2);

        /// <summary>
        /// HTMLを表示する
        /// </summary>
        /// <param name="html"></param>
        void NavigateString(string html);

        /// <summary>
        /// ID
        /// </summary>
        string ID { get; init; }

        /// <summary>
        /// タイトル
        /// </summary>
        string Title { get; }
    }

    internal class TabItem : ITabItem
    {

        public TabItem(ITabInfomation infomation, Func<bool> closeFunc, string id)
        {
            this._tabInfomation = infomation;
            this._closeFunc = closeFunc;
            this.ID = id;
        }

        #region field

        private readonly ITabInfomation _tabInfomation;

        private readonly Func<bool> _closeFunc;

        private readonly ICoreWebview2Handler _webView2Handler = DIFactory.Provider.GetRequiredService<ICoreWebview2Handler>();

        #endregion

        public bool Close()
        {
            return this._closeFunc();
        }

        public void NavigateString(string html)
        {
            this._webView2Handler.NavigateToString(html);
        }

        public void Initialize(CoreWebView2 wv2)
        {
            this._webView2Handler.Initialize(wv2);
        }

        public string ID { get; init; }

        public string Title => this._tabInfomation.Title;

    }
}
