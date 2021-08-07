using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Niconicome.Models.Domain.Local.Addons.Core.AutoUpdate.Github
{

    public partial class Release
    {

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("prerelease")]
        public bool Prerelease { get; set; }

        [JsonPropertyName("assets")]
        public List<Asset> Assets { get; set; } = new();
    }

    public partial class Asset
    {
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("size")]
        public long Size { get; set; }
    }


}
