using System.Windows.Controls;
using Niconicome.Extensions;

namespace Niconicome.Views.Mainpage.Region
{
    /// <summary>
    /// Tab.xaml の相互作用ロジック
    /// </summary>
    public partial class Tab : UserControl
    {
        public Tab(ViewModels.Mainpage.Tabs.TabViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
            this.WebView.NavigationCompleted += (_, _) => this.DataContext.As<ViewModels.Mainpage.Tabs.TabViewModel>().Initialize(this.WebView.CoreWebView2);
        }
    }
}
