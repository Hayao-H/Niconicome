using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.Manifest;

namespace Niconicome.Models.Domain.Local.Store.Types
{
    public class Addon : ManifestBase, IStorable
    {
        public int Id { get; set; }

        public static string TableName { get; set; } = "addons";

        public string PackageID { get; set; } = string.Empty;
    }
}
