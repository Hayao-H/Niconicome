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

        public ReactiveProperty<Version> TargetAPIVersion { get; init; } = new();

        public List<string> Permissions { get; init; } = new();

        public AutoUpdatePolicy AutoUpdatePolicy { get; set; } = new();

        public Scripts Scripts { get; set; } = new();

        public void SetData(AddonInfomation infomation)
        {
            this.ID.Value = infomation.ID.Value;
            this.Name.Value = infomation.Name.Value;
            this.Author.Value = infomation.Author.Value;
            this.Description.Value = infomation.Description.Value;
            this.Version.Value = infomation.Version.Value;
            this.Identifier.Value = infomation.Identifier.Value;
            this.PackageID.Value=infomation.PackageID.Value;
            this.Permissions.Clear();
            this.Permissions.AddRange(infomation.Permissions);
            this.AutoUpdatePolicy = infomation.AutoUpdatePolicy;
            this.Scripts = infomation.Scripts;
        }
    }
}
