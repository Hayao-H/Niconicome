using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.External.Software.FFmpeg.ffprobe;
using Niconicome.Models.Domain.Local.IO.V2;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Error = Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Infrastructure.IO
{
    public class WindowsFileIO : INiconicomeFileIO
    {
        public WindowsFileIO(Error::IErrorHandler errorHandler, IFFprobeHandler fFprobeHandler)
        {
            this._errorHandler = errorHandler;
            this._fFprobeHandler = fFprobeHandler;
        }

        #region field

        private readonly Error::IErrorHandler _errorHandler;

        private readonly IFFprobeHandler _fFprobeHandler;

        #endregion

        #region Method

        public async Task<IAttemptResult<int>> GetVerticalResolutionAsync(string path)
        {
            if (!this.Exists(path))
            {
                this._errorHandler.HandleError(WindowsFileIOError.FileDoesNotExist, path);
                return AttemptResult<int>.Fail(this._errorHandler.GetMessageForResult(WindowsFileIOError.FileDoesNotExist, path));
            }

            IAttemptResult<IFFprobeResult> result = await this._fFprobeHandler.GetVideoInfomationAsync(path);
            if (!result.IsSucceeded || result.Data is null)
            {
                return AttemptResult<int>.Fail(result.Message);
            }

            return AttemptResult<int>.Succeeded(result.Data.Height);

        }

        public IAttemptResult Write(string path, string content, Encoding? encoding = null)
        {
            try
            {
                using var fs = new StreamWriter(path, false, encoding ?? Encoding.UTF8);
                fs.Write(content);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(WindowsFileIOError.FailedToWrite, ex);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(WindowsFileIOError.FailedToWrite, ex));
            }

            return AttemptResult.Succeeded();
        }

        public void EnumerateFiles(string path, string searchPattern, Action<string> enumAction, bool searchSubDirectory)
        {
            SearchOption option = searchSubDirectory ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            try
            {
                foreach (var file in Directory.EnumerateFiles(path, searchPattern, option))
                {
                    enumAction(file);
                }
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(WindowsFileIOError.ErrorWhenEnumerateVideoFiles, ex, path);
            }
        }

        public async Task EnumerateFilesAsync(string path, string searchPattern, Func<string, Task> enumAction, bool searchSubDirectory)
        {
            SearchOption option = searchSubDirectory ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            try
            {
                foreach (var file in Directory.EnumerateFiles(path, searchPattern, option))
                {
                    await enumAction(file);
                }
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(WindowsFileIOError.ErrorWhenEnumerateVideoFiles, ex, path);
            }

        }


        public bool Exists(string path)
        {
            if (File.Exists(path))
            {
                return true;
            }

            return File.Exists(IOUtils.GetPrefixedPath(path));
        }

        public IAttemptResult Delete(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(WindowsFileIOError.FailedToDeleteFile, ex, path);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(WindowsFileIOError.FailedToDeleteFile, ex, path));
            }

            return AttemptResult.Succeeded();
        }

        public IAttemptResult Copy(string source, string target)
        {
            try
            {
                File.Copy(source, target);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(WindowsFileIOError.FailedToCopyFile, ex, source, target);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(WindowsFileIOError.FailedToCopyFile, ex, source, target));
            }

            return AttemptResult.Succeeded();
        }

        #endregion
    }
}
