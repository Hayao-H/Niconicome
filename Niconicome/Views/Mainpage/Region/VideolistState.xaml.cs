using System.Windows.Controls;
using Niconicome.ViewModels.Mainpage;
using Niconicome.ViewModels;

namespace Niconicome.Views.Mainpage.Region
{
    /// <summary>
    /// Interaction logic for VideoListState
    /// </summary>
    [ViewModelAttribute(typeof(VideolistStateViewModel))]
    public partial class VideoListState : UserControl
    {
        public VideoListState()
        {
            this.InitializeComponent();
        }
    }
}
