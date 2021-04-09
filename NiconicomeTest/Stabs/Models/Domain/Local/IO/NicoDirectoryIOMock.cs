using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Extensions;
using Microsoft.VisualBasic.FileIO;

namespace NiconicomeTest.Stabs.Models.Domain.Local.IO
{
    class NicoDirectoryIOMock : INicoDirectoryIO
    {
        public NicoDirectoryIOMock(Func<string, bool> existsFunc, Func<string, string, bool, IList<string>> getfilesFunc, Func<string, string, bool, IList<string>> getdirFunc)
        {
            this.existsFunc = existsFunc;
            this.getfilesFunc = getfilesFunc;
            this.getdirFunc = getdirFunc;
        }

        private readonly Func<string, bool> existsFunc;

        private readonly Func<string, string, bool, IList<string>> getfilesFunc;

        private readonly Func<string, string, bool, IList<string>> getdirFunc;

        public bool Exists(string path)
        {
            return this.existsFunc(path);
        }

        public void Create(string path)
        {
        }

        public void Delete(string path, bool recurse = true)
        {
        }

        public IList<string> GetFiles(string path, string pattern = "*", bool recurse = false)
        {
            return this.getfilesFunc(path, pattern, recurse);
        }

        public IList<string> GetDirectorys(string path, string pattern = "*", bool recurse = false)
        {
            return this.getdirFunc(path, pattern, recurse);
        }
    }
}
