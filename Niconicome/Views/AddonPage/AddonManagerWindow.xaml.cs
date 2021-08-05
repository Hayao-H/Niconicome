using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Niconicome.ViewModels;
using Niconicome.ViewModels.Mainpage.Subwindows.AddonManager;
using Prism.Services.Dialogs;

namespace Niconicome.Views.AddonPage
{
    /// <summary>
    /// Interaction logic for AddonManagerWindow.xaml
    /// </summary>
    [ViewModel(typeof(AddonManagerViewModel))]
    public partial class AddonManagerWindow : UserControl
    {
        public AddonManagerWindow()
        {
            InitializeComponent();
        }
    }
}
