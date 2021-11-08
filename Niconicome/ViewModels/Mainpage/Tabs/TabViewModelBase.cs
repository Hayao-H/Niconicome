using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.ViewModels.Mainpage.Tabs
{
    internal class TabViewModelBase : BindableBase
    {
        public TabViewModelBase(string title, string id)
        {
            this.Title = title;
            this.ID = id;
        }

        public string Title { get; init; }

        public string ID { get; init; }
    }
}
