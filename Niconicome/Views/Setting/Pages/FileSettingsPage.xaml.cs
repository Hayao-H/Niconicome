using System.Windows.Controls;
using Niconicome.ViewModels.Setting.Pages;

namespace Niconicome.Views.Setting.Pages
{
    /// <summary>
    /// FileSettingsPage.xaml の相互作用ロジック
    /// </summary>
    public partial class FileSettingsPage : Page
    {
        public FileSettingsPage()
        {
            this.InitializeComponent();
            this.DataContext = new FileSettingsViewModel();
        }
    }
}
