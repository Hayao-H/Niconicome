using System;
using System.Collections.Generic;
using Niconicome.Models.Domain.Local.Addons.Manifest.V1;
using Reactive.Bindings;

namespace Niconicome.Models.Domain.Local.Addons.Core
{
    public class AddonInfomation
    {
        public AddonInfomation()
        {
            this.PackageID = new ReactiveProperty<string>(Guid.NewGuid().ToString("D"));
        }

        public ReactiveProperty<int> ID { get; init; } = new();

        public ReactiveProperty<string> Name { get; init; } = new(string.Empty);

        public ReactiveProperty<string> Author { get; init; } = new(string.Empty);

        public ReactiveProperty<string> Description { get; init; } = new(string.Empty);

        public ReactiveProperty<Version> Version { get; init; } = new();

        public ReactiveProperty<string> Identifier { get; init; } = new();

        public ReactiveProperty<string> PackageID { get; init; }

        public List<string> Permissions { get; init; } = new();

        public AutoUpdatePolicy AutoUpdatePolicy { get; set; } = new();

        public Scripts Scripts { get; set; } = new();
    }
}
