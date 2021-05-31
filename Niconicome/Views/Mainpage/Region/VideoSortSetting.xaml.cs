using System.Windows.Controls;
using Niconicome.ViewModels.Mainpage;
using Niconicome.ViewModels;

namespace Niconicome.Views.Mainpage.Region
{
    /// <summary>
    /// Interaction logic for VideoSortSetting
    /// </summary>
    [ViewModelAttribute(typeof(SortViewModel))]
    public partial class VideoSortSetting : UserControl
    {
        public VideoSortSetting()
        {
            InitializeComponent();
        }
    }
}
