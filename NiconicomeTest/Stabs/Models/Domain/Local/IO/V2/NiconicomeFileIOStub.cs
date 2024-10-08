﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.IO.V2;
using Niconicome.Models.Domain.Local.Server.RequestHandler.TS;
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

        public IAttemptResult<byte[]> ReadByte(string path)
        {
            return AttemptResult<byte[]>.Succeeded(new byte[0]);
        }

        public IAttemptResult Delete(string path, bool resycycle)
        {
            this.DeleteMethodCall?.Invoke(this, new NiconicomeFileIOEventArgs() { Path = path });
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

        #region Test

        public event EventHandler<NiconicomeFileIOEventArgs>? DeleteMethodCall;

        #endregion

    }

    public class NiconicomeFileIOEventArgs : EventArgs
    {
        public string Path { get; init; } = string.Empty;
    }
}
