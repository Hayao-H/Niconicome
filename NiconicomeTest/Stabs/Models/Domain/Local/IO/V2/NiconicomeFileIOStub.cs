using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.IO.V2;
using Niconicome.Models.Helper.Result;

namespace NiconicomeTest.Stabs.Models.Domain.Local.IO.V2
{
    public class NiconicomeFileIOStub : INiconicomeFileIO
    {

        public NiconicomeFileIOStub(Func<string, string> readFunc)
        {
            this._readFunc = readFunc;
        }

        public NiconicomeFileIOStub()
        {
            this._readFunc = _ => string.Empty;
        }

        private readonly Func<string, string> _readFunc;

        public Task<IAttemptResult<int>> GetVerticalResolutionAsync(string path)
        {
            return Task.FromResult(AttemptResult<int>.Succeeded(0));
        }

        public IAttemptResult Write(string path, string content, Encoding? encoding = null)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult Write(string path, byte[] content)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult<string> Read(string path)
        {
            return AttemptResult<string>.Succeeded(this._readFunc(path));
        }

        public IAttemptResult Delete(string path, bool resycycle)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult Copy(string source, string target, bool overwrite = false)
        {
            return AttemptResult.Succeeded();
        }

        public void EnumerateFiles(string path, string searchPattern, Action<string> enumAction, bool searchSubDirectory)
        {

        }

        public Task EnumerateFilesAsync(string path, string searchPattern, Func<string, Task> enumAction, bool searchSubDirectory)
        {
            return Task.CompletedTask;
        }

        public bool Exists(string path)
        {
            return true;
        }

        public IAttemptResult SetLastWriteTime(string path, DateTime dt)
        {
            return AttemptResult.Succeeded();
        }

    }
}
