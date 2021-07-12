using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.Core;
using Reactive.Bindings;

namespace Niconicome.ViewModels.Mainpage.Subwindows.AddonManager
{
    public class AddonInfomationViewModel
    {
        public AddonInfomationViewModel(AddonInfomation info)
        {
            this.AddonInfomation = info;
            this.Name = info.Name.ToReadOnlyReactiveProperty();
            this.Version = info.Version.Select(v => v.ToString()).ToReadOnlyReactiveProperty();
            this.Author=info.Author.ToReadOnlyReactiveProperty();
            this.Description = info.Description.ToReadOnlyReactiveProperty();
        }

        public AddonInfomation AddonInfomation { get; init; }

        public ReadOnlyReactiveProperty<string?> Name { get; init; }

        public ReadOnlyReactiveProperty<string?> Version { get; init; }

        public ReadOnlyReactiveProperty<string?> Author { get; init; }

        public ReadOnlyReactiveProperty<string?> Description { get; init; }
    }
}
