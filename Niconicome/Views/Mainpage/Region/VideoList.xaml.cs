using System.Windows.Controls;
using Niconicome.ViewModels;
using Niconicome.ViewModels.Mainpage;

namespace Niconicome.Views.Mainpage.Region
{
    /// <summary>
    /// Interaction logic for VideoList
    /// </summary>
    /// 
    [ViewModelAttribute(typeof(VideoListViewModel))]
    public partial class VideoList : UserControl
    {
        public VideoList()
        {
            this.InitializeComponent();
        }
    }
}
