using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Helper.Result.Generic;
using Reactive.Bindings;

namespace Niconicome.ViewModels.Mainpage.Subwindows.AddonManager
{
    class FailedAddonViewModel
    {
        public FailedAddonViewModel(IAttemptResult<string> result)
        {
            this.PackageID = new ReactiveProperty<string>(result.Data ?? string.Empty);
            this.Details = new ReactiveProperty<string>(result.Message ?? "None");
        }
            

        public ReactiveProperty<string> PackageID { get; init; }

        public ReactiveProperty<string> Details { get; init;  }
    }
}
