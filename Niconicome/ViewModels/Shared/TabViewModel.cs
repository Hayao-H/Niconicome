using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Niconicome.Models.Local.State;
using Niconicome.Models.Local.State.Tab.V1;
using Niconicome.Models.Utils.Reactive;
using WS = Niconicome.Workspaces.Mainpage;

namespace Niconicome.ViewModels.Shared
{
    public class TabViewModel : IDisposable
    {
        public TabViewModel(IBlazorPageManager manager)
        {
            this.Tabs = new BindableCollection<TabItemViewModel, ITab>(WS.TabControler.Tabs, tab => new TabItemViewModel(tab, manager));
        }

        /// <summary>
        /// タブの一覧
        /// </summary>
        public BindableCollection<TabItemViewModel, ITab> Tabs { get; init; }

        public void Dispose()
        {
            this.Tabs.Dispose();
        }
    }

    public class TabItemViewModel
    {
        public TabItemViewModel(ITab tab, IBlazorPageManager manager)
        {
            this.tab = tab;
            this.manager = manager;
        }

        private readonly ITab tab;

        private readonly IBlazorPageManager manager;

        public string Name => this.tab.Name;

        public void Close(MouseEventArgs e)
        {
            this.manager.RequestBlazorToNavigate("/videos");
            this.tab.Close();
        }

        public void OnClick()
        {
            var url = this.GetUrl();
            this.manager.RequestBlazorToNavigate(url);
        }

        private string GetUrl()
        {
            return this.tab.Type switch
            {
                TabType.Main => "/videos",
                TabType.Settings => "/settings/general",
                TabType.Download => "/downloadtask/download",
                TabType.Addon => "/addons",
                _ => ""
            };
        }
    }
}
