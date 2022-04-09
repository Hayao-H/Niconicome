using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Const
{
    public class AddonConstant
    {
        public static Version APIVersion { get; private set; } = new("1.2.0");

        public static string UninstalledAddonsFile { get; private set; } = "uninstalled.dat";

        public static string LocalStorageFileName { get; private set; } = "locaStorage.db";

        public static string ResourceDirectoryName { get; private set; } = "resource";

        public static string ResourceHost { get; private set; } = "nc-resource.nico";
    }
}
