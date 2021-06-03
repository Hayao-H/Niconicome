using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Utils
{
    public static class Compatibility
    {
        private static Version? osVersion;

        /// <summary>
        /// OSのバージョンを返す
        /// </summary>
        public static Version OsVersion => Compatibility.osVersion ?? (Compatibility.osVersion = Environment.OSVersion.Version);

        /// <summary>
        /// OSのバージョンを比較する
        /// </summary>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <param name="build"></param>
        /// <returns></returns>
        public static bool IsOsVersionLargerThan(int major, int minor = 0, int build = 0)
        {
            var version = new Version(major, minor, build);
            return Compatibility.OsVersion.CompareTo(version) >= 0;
        }
    }
}
