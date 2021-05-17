using System.Windows.Controls;
using Niconicome.ViewModels;
using Niconicome.ViewModels.Mainpage;

namespace Niconicome.Views.Mainpage.Region
{
    /// <summary>
    /// Interaction logic for DownloadSettings
    /// </summary>
    [ViewModelAttribute(typeof(DownloadSettingsViewModel))]
    public partial class DownloadSettings : UserControl
    {
        public DownloadSettings()
        {
            InitializeComponent();
        }
    }
}
