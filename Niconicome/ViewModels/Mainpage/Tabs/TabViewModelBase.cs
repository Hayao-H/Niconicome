using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.ViewModels.Mainpage.Tabs
{
    internal class TabViewModelBase : BindableBase
    {
        public TabViewModelBase(string title)
        {
            this.Title = title;
        }

        public string Title { get; init; }
    }
}
