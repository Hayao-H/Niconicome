using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.Addons.Manifest.V1
{
    class Manifest : ManifestBase
    {
        public override string ManifestVersion { get; set; } = "1.0";

        public string Name { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Version { get; set; } = "1.0.0";

        public AutoUpdatePolicy AutoUpdatePolicy { get; set; } = new();

        public List<string> Permissions { get; set; } = new();
    }

    class AutoUpdatePolicy
    {
        public bool AutoUpdate { get; set; }

        public bool GithubRelease { get; set; }

        public string GithubReleaseFileName { get; set; } = string.Empty;

        public string GithubOwnerName { get; set; } = string.Empty;

        public string GithubRepoName { get; set; } = string.Empty;
    }

    class Scripts
    {
        public string BackgroundScript { get; set; } = string.Empty;
    }
}
