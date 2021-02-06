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

        private readonly Handlers::CoreWebview2Handler handler = new();

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.NavigationCompleted += this.OnNavigate;
            this.AssociatedObject.CoreWebView2InitializationCompleted += this.OnInitialized;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.NavigationCompleted -= this.OnNavigate;
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
            var cookies = new List<CoreWebView2Cookie>();

            await handler.GetAndSetCookiesAsync(DIFactory.Provider.GetRequiredService<ICookieManager>(), this.AssociatedObject.CoreWebView2, @"https://nicovideo.jp", cookies);

            if (cookies.Any(cookie => cookie.Name == "user_session"))
            {
                await NiconicoContext.Context.RefreshUser();
                (this.AssociatedObject.DataContext as LoginBrowserViewModel)?.RaiseLoginSucceeded();
                this.Window?.Close();
            }
        }

        private async void OnInitialized(object? sender,EventArgs e)
        {
            await this.handler.DeleteBrowserCookiesAsync(this.AssociatedObject.CoreWebView2, @"https://nicovideo.jp");
        }
    }

}
