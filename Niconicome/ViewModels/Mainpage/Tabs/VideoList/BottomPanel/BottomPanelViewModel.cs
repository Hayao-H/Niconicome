using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Utils.Reactive;

namespace Niconicome.ViewModels.Mainpage.Tabs.VideoList.BottomPanel
{
    public class BottomPanelViewModel
    {
        public IBindableProperty<bool> IsDownloadSelected { get; init; } = new BindableProperty<bool>(true);

        public IBindableProperty<bool> IsOutputSelected { get; init; } = new BindableProperty<bool>(false);

        public IBindableProperty<bool> IsStateSelected { get; init; } = new BindableProperty<bool>(false);

        public IBindableProperty<bool> IsTimerSelected { get; init; } = new BindableProperty<bool>(false);

        public void OnPanelSelected(PanelType panelType)
        {
            this.IsDownloadSelected.Value = panelType == PanelType.Download;
            this.IsOutputSelected.Value = panelType == PanelType.Output;
            this.IsStateSelected.Value = panelType == PanelType.State;
            this.IsTimerSelected.Value = panelType == PanelType.Timer;
        }
    }

    public enum PanelType
    {
        Download,
        Output,
        State,
        Timer,
    }
}
