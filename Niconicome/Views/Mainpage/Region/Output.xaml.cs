using System.Windows.Controls;
using Niconicome.ViewModels;
using Niconicome.ViewModels.Mainpage;

namespace Niconicome.Views.Mainpage.Region
{
    /// <summary>
    /// Interaction logic for Output
    /// </summary>
    [ViewModelAttribute(typeof(OutPutViewModel))]
    public partial class Output : UserControl
    {
        public Output()
        {
            this.InitializeComponent();
        }
    }
}
