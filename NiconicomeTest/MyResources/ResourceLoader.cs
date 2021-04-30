using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiconicomeTest.MyResources
{
    static class ResourceLoader
    {
        public static string Load(string path, Encoding? encoding = null)
        {

            path = @"MyResources\" + path;
            if (encoding is null) encoding = Encoding.UTF8;
            using var fs = new StreamReader(path, encoding);
            return fs.ReadToEnd();
        }
    }
}
