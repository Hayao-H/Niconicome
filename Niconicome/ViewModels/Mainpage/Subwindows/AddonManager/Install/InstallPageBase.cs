using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reactive.Bindings;

namespace Niconicome.ViewModels.Mainpage.Subwindows.AddonManager.Install
{
    abstract class InstallPageBase : BindableBase
    {
        public ReactiveCommand ToNext { get; init; } = new();
    }
}
