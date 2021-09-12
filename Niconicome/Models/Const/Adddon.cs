using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Const
{
    class Adddon
    {
        public static Version APIVersion { get; private set; } = new("1.1.0");

        public static string UninstalledAddonsFile { get; private set; } = "uninstalled.dat";

        public static string LocalStorageFileName { get; private set; } = "locaStorage.json";
    }
}
