using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Web.WebView2.Core;
using Niconicome.Models.Domain.Utils;
using Niconicome.ViewModels;
using Niconicome.ViewModels.Mainpage.Subwindows.AddonManager;

namespace Niconicome.Views.AddonPage.V2
{
    /// <summary>
    /// MainManager.xaml の相互作用ロジック
    /// </summary>
    [ViewModel(typeof(MainManagerViewModel))]
    public partial class MainManager : UserControl,IDisposable
    {
        public MainManager()
        {
            InitializeComponent();
            Resources.Add("services", DIFactory.Provider);
            webview.BlazorWebViewInitializing += (s, e) =>
            {
                e.EnvironmentOptions = new CoreWebView2EnvironmentOptions()
                {
                    AdditionalBrowserArguments = "--disable-web-security"
                };
            };
        }

        public void Dispose()
        {
            this.webview.DisposeAsync();
        }
    }
}
