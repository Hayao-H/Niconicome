using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;

namespace Niconicome.ViewModels.Mainpage.Tabs
{
    public class TabViewModelBase : BindableBase
    {
        public TabViewModelBase(string title, string id, bool canClose = false)
        {
            this.Title = title;
            this.ID = id;
            this.CanClose = new ReactiveProperty<bool>(canClose);
            this.CloseCommand = this.CanClose.ToReactiveCommand();
        }

        public string Title { get; init; }

        public string ID { get; init; }

        public ReactiveProperty<bool> CanClose { get; init; }

        public ReactiveCommand CloseCommand { get; init; }
    }

    public class TabViewModelBaseD {

        public string Title { get; init; } = "";

        public string ID { get; init; } = "";

        public ReactiveProperty<bool> CanClose { get; init; } = new(true);

        public ReactiveCommand CloseCommand { get; init; } = new();
    }
}
