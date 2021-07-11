using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.Addons.Manifest
{
    public abstract class ManifestBase
    {
        public virtual string ManifestVersion { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Version { get; set; } = "1.0.0";

        public string Identifier { get; set; } = string.Empty;

        public List<string> Permissions { get; set; } = new();

    }
}
