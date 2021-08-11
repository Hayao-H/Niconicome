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
using Niconicome.ViewModels;
using Niconicome.ViewModels.Mainpage;

namespace Niconicome.Views.Mainpage.Region
{
    /// <summary>
    /// TimerSettings.xaml の相互作用ロジック
    /// </summary>
    [ViewModel(typeof(TimerSettingsViewModel))]
    public partial class TimerSettings : UserControl
    {
        public TimerSettings()
        {
            InitializeComponent();
        }
    }
}
