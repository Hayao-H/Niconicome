using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Niconicome.Models.Const;

namespace Niconicome.Models.Domain.Local.Addons.Manifest.V1
{
    public class Manifest : ManifestBase
    {
        [JsonPropertyName("manifest_version")]
        public string ManifestVersion { get; set; } = AddonConstant.ManiestVersion;

        [JsonPropertyName("remote_update_policy")]
        public AutoUpdatePolicy AutoUpdatePolicy { get; set; } = new();

        public Scripts Scripts { get; set; } = new();

    }

    public class AutoUpdatePolicy
    {

        [JsonPropertyName("updatejson-url")]
        public string UpdateJsonUrl { get; set; } = string.Empty;
    }

    public class Scripts
    {
        [JsonPropertyName("background_script")]
        public string BackgroundScript { get; set; } = string.Empty;
    }
}
