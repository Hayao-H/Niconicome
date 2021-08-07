using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Const = Niconicome.Models.Const;

namespace Niconicome.Models.Domain.Local.Addons.Manifest
{
    public abstract class ManifestBase
    {
        public string Name { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Version { get; set; } = "1.0.0";

        public string Identifier { get; set; } = string.Empty;

        [JsonPropertyName("target_api_version")]
        public string TargetAPIVersion { get; set; } = Const::Adddon.APIVersion.ToString();

        public List<string> Permissions { get; set; } = new();

        [JsonPropertyName("host_permissions")]
        public List<string> HostPermissions { get; set; } = new();

        public Dictionary<string, string> Icons { get; set; } = new();

    }
}
