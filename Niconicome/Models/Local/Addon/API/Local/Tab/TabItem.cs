using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Web.WebView2.Core;
using Niconicome.Models.Domain.Local.Addons.API.Tab;
using Niconicome.Models.Domain.Local.Handlers;
using Niconicome.Models.Domain.Utils;

namespace Niconicome.Models.Local.Addon.API.Local.Tab
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
        /// WevView2にメッセージを送信する
        /// </summary>
        /// <param name="message"></param>
        void PostMessage(string message);

        /// <summary>
        /// メッセージハンドラを追加する
        /// </summary>
        /// <param name="handler"></param>
        void AddMessageHandler(Action<string> handler);

        /// <summary>
        /// 初期化処理が完了するまで待機
        /// </summary>
        /// <returns></returns>
        Task WaitUntilInitialize();

        /// <summary>
        /// 仮想ホストを設定する
        /// </summary>
        /// <param name="virtualHostName"></param>
        /// <param name="folderName"></param>
        void SetVirtualHostName(string virtualHostName, string folderName);

        /// <summary>
        /// ID
        /// </summary>
        string ID { get; }

        /// <summary>
        /// タイトル
        /// </summary>
        string Title { get; }

        /// <summary>
        /// フラグ
        /// </summary>
        bool IsClosed { get; }
    }

    public class TabItem : ITabItem
    {

        public TabItem(ITabInfomation infomation, Func<ITabItem, bool> closeFunc)
        {
            this._tabInfomation = infomation;
            this._closeFunc = closeFunc;
        }

        #region field

        private readonly ITabInfomation _tabInfomation;

        private readonly Func<ITabItem, bool> _closeFunc;

        private readonly ICoreWebview2Handler _webView2Handler = DIFactory.Provider.GetRequiredService<ICoreWebview2Handler>();

        private Action? _initializeHandlers;

        private Action<string>? _messageHandler;

        private bool _isInitialized;

        #endregion

        #region private

        private void OnInitialize()
        {
            try
            {
                this._initializeHandlers?.Invoke();
            }
            catch { }
        }

        private void OnMessage(string message)
        {
            try
            {
                this._messageHandler?.Invoke(message);
            }
            catch { }
        }

        private void CheckIfInitialized()
        {
            if (!this._isInitialized) throw new InvalidOperationException("Not Initialized");
        }

        #endregion

        #region Methods

        public bool Close()
        {
            if (this.IsClosed) return false;
            return this.IsClosed = this._closeFunc(this);
        }

        public void NavigateString(string html)
        {
            this._webView2Handler.NavigateToString(html);
        }

        public void Initialize(CoreWebView2 wv2)
        {
            if (this._isInitialized) return;
            this._webView2Handler.Initialize(wv2);
            this._webView2Handler.RegisterFilterFunc(url => this._tabInfomation.CanAccess(url));
            this._webView2Handler.AddMessageHandler(this.OnMessage);
            this.OnInitialize();
            this._isInitialized = true;
        }

        public Task WaitUntilInitialize()
        {
            var tsc = new TaskCompletionSource();

            if (this._isInitialized)
                tsc.TrySetResult();
            else
                this._initializeHandlers += () => tsc.TrySetResult();

            return tsc.Task;
        }

        public void SetVirtualHostName(string virtualHostName, string folderName)
        {
            this.CheckIfInitialized();
            this._webView2Handler.SetVirtualHostName(virtualHostName, folderName);
        }

        public void AddMessageHandler(Action<string> handler)
        {
            this.CheckIfInitialized();
            this._messageHandler += handler;
        }

        public void PostMessage(string message)
        {
            this.CheckIfInitialized();
            this._webView2Handler.PostMessage(message);
        }

        #endregion

        #region Props

        public string ID => this._tabInfomation.ID;

        public string Title => this._tabInfomation.Title;

        public bool IsClosed { get; private set; }

        #endregion

    }
}
