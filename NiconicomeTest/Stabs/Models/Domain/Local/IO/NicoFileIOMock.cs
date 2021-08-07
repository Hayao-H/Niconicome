using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.IO;

namespace NiconicomeTest.Stabs.Models.Domain.Local.IO
{
    class NicoFileIOMock : INicoFileIO
    {

        public NicoFileIOMock(Func<bool> existsFunc, Func<string> readFunc)
        {
            this.existsFunc = existsFunc;
            this.readFunc = readFunc;
        }

        private readonly Func<bool> existsFunc;

        private readonly Func<string> readFunc;

        public bool Exists(string path)
        {
            return this.existsFunc();
        }

        public void Delete(string path)
        {

        }

        public string OpenRead(string path)
        {
            return this.readFunc();
        }

        public void Write(string path, string content, bool append = false)
        {

        }

        public void Move(string path, string destPath, bool overwrite = false)
        {

        }

        public void AppendText(string path, string text)
        {

        }


    }
}
