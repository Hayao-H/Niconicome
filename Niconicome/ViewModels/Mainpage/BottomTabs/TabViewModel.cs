using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.API.Tab;
using Reactive.Bindings;

namespace Niconicome.ViewModels.Mainpage.BottomTabs
{
    public class TabViewModel
    {
        public TabViewModel(ITabItem tabItem)
        {
            this.Title = new ReactiveProperty<string>(tabItem.Title);
        }

        public ReactiveProperty<string> Title { get; init; }
    }
}
