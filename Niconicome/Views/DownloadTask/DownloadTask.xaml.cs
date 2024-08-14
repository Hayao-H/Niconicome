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
using Microsoft.Web.WebView2.Core;
using Niconicome.Models.Domain.Utils;
using Niconicome.ViewModels;
using Niconicome.ViewModels.Mainpage.Subwindows.DownloadTask;

namespace Niconicome.Views.DownloadTask
{
    /// <summary>
    /// DownloadTask.xaml の相互作用ロジック
    /// </summary>
    [ViewModel(typeof(DownloadTaskTabViewModel))]
    public partial class DownloadTask : UserControl,IDisposable
    {
        public DownloadTask()
        {
            InitializeComponent();
            Resources.Add("services", DIFactory.Provider);
            webview.BlazorWebViewInitializing += (s, e) =>
            {
                e.EnvironmentOptions = new CoreWebView2EnvironmentOptions()
                {
                    AdditionalBrowserArguments = string.Join(' ', ["--disable-web-security", "--disable-features=AutoupgradeMixedContent"]),
                };
            };
        }

        public void Dispose()
        {
            this.webview.DisposeAsync();
        }
    }
}
