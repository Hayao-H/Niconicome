using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Models.Domain.Niconico;
using Microsoft.Xaml.Behaviors;
using Microsoft.Web.WebView2.Wpf;
using Microsoft.Web.WebView2.Core;
using System.Windows;
using Niconicome.Models.Domain.Utils;
using Handlers = Niconicome.Models.Domain.Local.Handlers;
using Utils = Niconicome.Models.Domain.Utils;
using System.Threading.Tasks;
using Niconicome.ViewModels.Controls;

namespace Niconicome.ViewModels.Login
{
    class LoginBrowserViewModel
    {
        /// <summary>
        /// ログイン成功イベント
        /// </summary>
        public event EventHandler? LoginSucceeded;

        /// <summary>
        /// ログイン成功イベントを発行
        /// </summary>
        public void RaiseLoginSucceeded()
        {
            this.LoginSucceeded?.Invoke(this, EventArgs.Empty);
        }
    }

    class WebViewBehavior : Behavior<WebView2>
    {

        private readonly Handlers::ICoreWebview2Handler handler = Utils::DIFactory.Provider.GetRequiredService<Handlers::ICoreWebview2Handler>();

        private bool isInitializeCompleted = false;

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.NavigationStarting += this.OnNavigate;
            this.AssociatedObject.CoreWebView2InitializationCompleted += this.OnInitialized;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.NavigationStarting -= this.OnNavigate;
            this.AssociatedObject.CoreWebView2InitializationCompleted -= this.OnInitialized;
        }

        public Window? Window
        {
            get
            {
                return (Window)this.GetValue(WindowProperty);
            }
            set
            {
                this.SetValue(WindowProperty, value);
            }
        }

        public static readonly DependencyProperty WindowProperty = DependencyProperty.Register("Window", typeof(Window), typeof(WebViewBehavior), new PropertyMetadata(null));

        private async void OnNavigate(object? sender, EventArgs e)
        {
            if (!this.isInitializeCompleted)
            {
                return;
            }

            var cookies = await this.handler.GetCookiesAsync(@"https://nicovideo.jp");


            if (cookies.Any(cookie => cookie.Name == "user_session"))
            {
                await this.SetCookiesAndExitAsync(cookies);
            }
        }

        private async void OnInitialized(object? sender, EventArgs e)
        {
            this.handler.Initialize(this.AssociatedObject.CoreWebView2);

            var cookies = await this.handler.GetCookiesAsync(@"https://nicovideo.jp");

            if (cookies.Any(cookie => cookie.Name == "user_session" && cookie.Expires > DateTime.Now))
            {
                var result = await MaterialMessageBox.Show("有効なセッションが存在します。ログインをスキップしますか？", MessageBoxButtons.Yes | MessageBoxButtons.No, MessageBoxIcons.Question);

                if (result == MaterialMessageBoxResult.Yes)
                {
                    await this.SetCookiesAndExitAsync(cookies);
                    return;
                }
            }

            await this.handler.DeleteBrowserCookiesAsync( @"https://nicovideo.jp");

            this.isInitializeCompleted = true;
        }

        private async Task SetCookiesAndExitAsync(List<CoreWebView2Cookie> cookies)
        {
            var cookieManager = DIFactory.Provider.GetRequiredService<ICookieManager>();
            foreach (var cookie in cookies)
            {
                cookieManager.AddCookie(cookie.Name, cookie.Value);
            }
            await NiconicoContext.Context.RefreshUser();
            (this.AssociatedObject.DataContext as LoginBrowserViewModel)?.RaiseLoginSucceeded();
            this.Window?.Close();
        }
    }

}
