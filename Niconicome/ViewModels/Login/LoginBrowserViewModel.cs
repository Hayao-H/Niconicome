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
using WS = Niconicome.Workspaces.Mainpage;

namespace Niconicome.ViewModels.Login
{
    class LoginBrowserViewModel
    {
        
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

            if (await WS.NiconicoCookieManager.HandleNavigate())
            {
                this.Exit();
            }
        }

        private async void OnInitialized(object? sender, EventArgs e)
        {
            this.handler.Initialize(this.AssociatedObject.CoreWebView2);

            WS.NiconicoCookieManager.Wire(this.handler);

            if (await WS.NiconicoCookieManager.IsLoggedIn())
            {
                var result = await MaterialMessageBox.Show("有効なセッションが存在します。ログインをスキップしますか？", MessageBoxButtons.Yes | MessageBoxButtons.No, MessageBoxIcons.Question);

                if (result == MaterialMessageBoxResult.Yes)
                {
                    this.Exit();
                    return;
                }
            }

            await this.handler.DeleteBrowserCookiesAsync(@"https://nicovideo.jp");

            this.isInitializeCompleted = true;
        }

        private void Exit()
        {
            WS.NiconicoCookieManager.UnWire();
            this.Window?.Close();
        }
    }

}
