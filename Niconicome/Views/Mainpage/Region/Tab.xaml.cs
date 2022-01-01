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
using Niconicome.Extensions;
using Niconicome.Models.Local.Addon.API.Local.Tab;
using Niconicome.ViewModels.Mainpage.BottomTabs;
using Niconicome.ViewModels.Mainpage.Tabs;

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
