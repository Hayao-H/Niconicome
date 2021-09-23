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
using Niconicome.Models.Domain.Local.Addons.API.Tab;
using Niconicome.ViewModels.Mainpage.BottomTabs;

namespace Niconicome.Views.Mainpage.Region
{
    /// <summary>
    /// BottomTab.xaml の相互作用ロジック
    /// </summary>
    public partial class BottomTab : UserControl
    {
        public BottomTab(ITabItem tab)
        {
            InitializeComponent();
            this.DataContext = new TabViewModel(tab);
        }
    }
}
