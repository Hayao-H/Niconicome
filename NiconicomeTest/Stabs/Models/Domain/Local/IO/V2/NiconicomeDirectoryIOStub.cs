using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.IO.V2;
using Niconicome.Models.Helper.Result;

namespace NiconicomeTest.Stabs.Models.Domain.Local.IO.V2
{
    public class NiconicomeDirectoryIOStub : INiconicomeDirectoryIO
    {
        public NiconicomeDirectoryIOStub(IEnumerable<string> files, IEnumerable<string> directories)
        {
            this._files = files;
            this._directories = directories;
        }

        private readonly IEnumerable<string> _files;

        private readonly IEnumerable<string> _directories;

        public IAttemptResult CreateDirectory(string path)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult<IEnumerable<string>> GetDirectories(string path)
        {
            return AttemptResult<IEnumerable<string>>.Succeeded(this._directories);
        }

        public IAttemptResult<IEnumerable<string>> GetFiles(string path, string searchPattern = "*")
        {
            return AttemptResult<IEnumerable<string>>.Succeeded(this._files);
        }

        public IAttemptResult Delete(string path, bool recursive = true, bool recycle = false)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult Move(string source, string destination, bool overwrite = true)
        {
            return AttemptResult.Succeeded();
        }

        public bool Exists(string path)
        {
            return true;
        }
    }
}
