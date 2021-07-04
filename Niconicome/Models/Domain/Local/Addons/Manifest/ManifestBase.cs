using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.Addons.Manifest
{
    abstract class ManifestBase
    {
        public virtual string ManifestVersion { get; set; } = string.Empty;
    }
}
