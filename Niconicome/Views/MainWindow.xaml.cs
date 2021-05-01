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
using Niconicome.ViewModels.Mainpage;

namespace Niconicome
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.downloadSettings.DataContext = new DownloadSettingsViewModel();
            this.videoList.DataContext = new VideoListViewModel();
        }

        private void Videolist_db_click(object sender, MouseButtonEventArgs e)
        {
            ((VideoListViewModel)this.videolist.DataContext).OnDoubleClick(sender);
        }
    }
}
