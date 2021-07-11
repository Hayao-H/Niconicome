using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.Addons.Manifest.V1
{
    public class Manifest : ManifestBase
    {
        [JsonPropertyName("manifest_version")]
        public override string ManifestVersion { get; set; } = "1.0";

        [JsonPropertyName("auto_update_policy")]
        public AutoUpdatePolicy AutoUpdatePolicy { get; set; } = new();

        public Scripts Scripts { get; set; } = new();

    }

    public class AutoUpdatePolicy
    {
        [JsonPropertyName("auto_update")]
        public bool AutoUpdate { get; set; }

        [JsonPropertyName("github_release")]
        public bool GithubRelease { get; set; }

        [JsonPropertyName("github_release_filename_pattern")]
        public string GithubReleaseFileNamePattern { get; set; } = string.Empty;

        [JsonPropertyName("github_owner_name")]
        public string GithubOwnerName { get; set; } = string.Empty;

        [JsonPropertyName("github_repo_name")]
        public string GithubRepoName { get; set; } = string.Empty;
    }

    public class Scripts
    {
        [JsonPropertyName("background_script")]
        public string BackgroundScript { get; set; } = string.Empty;
    }
}
