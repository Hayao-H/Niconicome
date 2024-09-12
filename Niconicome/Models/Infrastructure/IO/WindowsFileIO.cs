using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using VB = Microsoft.VisualBasic.FileIO;
using Niconicome.Models.Domain.Local.External.Software.FFmpeg.ffprobe;
using Niconicome.Models.Domain.Local.IO.V2;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Error = Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Domain.Local.Server.RequestHandler.TS;

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
                this._errorHandler.HandleError(WindowsFileIOError.FailedToWrite, ex, path);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(WindowsFileIOError.FailedToWrite, ex, path));
            }

            return AttemptResult.Succeeded();
        }
        public IAttemptResult Write(string path, byte[] content)
        {
            try
            {
                using var fs = File.Create(path);
                fs.Write(content);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(WindowsFileIOError.FailedToWrite, ex, path);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(WindowsFileIOError.FailedToWrite, ex, path));
            }

            return AttemptResult.Succeeded();
        }

        public IAttemptResult WriteToStream(string path, System.IO.Stream stream)
        {
            try
            {
                using var video = new FileStream(path, FileMode.Open, FileAccess.Read);
                video.CopyTo(stream);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(WindowsFileIOError.FailedToWrite, ex, path);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(TSRequestHandlerError.FailedToOpenVodeo, ex));
            }

            return AttemptResult.Succeeded();
        }

        public IAttemptResult<string> Read(string path)
        {
            var content = string.Empty;

            try
            {
                using var fs = new StreamReader(path);
                content = fs.ReadToEnd();
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(WindowsFileIOError.FailedToRead, ex, path);
                return AttemptResult<string>.Fail(this._errorHandler.GetMessageForResult(WindowsFileIOError.FailedToRead, ex, path));
            }

            return AttemptResult<string>.Succeeded(content);
        }

        public IAttemptResult<byte[]> ReadByte(string path)
        {
            byte[] content;

            try
            {
                content = File.ReadAllBytes(path);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(WindowsFileIOError.FailedToRead, ex, path);
                return AttemptResult<byte[]>.Fail(this._errorHandler.GetMessageForResult(WindowsFileIOError.FailedToRead, ex, path));
            }

            return AttemptResult<byte[]>.Succeeded(content);
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

        public IAttemptResult Delete(string path, bool resycycle)
        {
            if (resycycle)
            {
                try
                {
                    VB::FileSystem.DeleteFile(path, VB::UIOption.AllDialogs, VB::RecycleOption.SendToRecycleBin);
                }
                catch (Exception ex)
                {
                    this._errorHandler.HandleError(WindowsFileIOError.FailedToDeleteFile, ex, path);
                    return AttemptResult.Fail(this._errorHandler.GetMessageForResult(WindowsFileIOError.FailedToDeleteFile, ex, path));
                }
            }
            else
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
            }

            return AttemptResult.Succeeded();
        }

        public IAttemptResult Copy(string source, string target, bool overwrite = false)
        {
            try
            {
                File.Copy(source, target, overwrite);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(WindowsFileIOError.FailedToCopyFile, ex, source, target);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(WindowsFileIOError.FailedToCopyFile, ex, source, target));
            }

            return AttemptResult.Succeeded();
        }

        public IAttemptResult SetLastWriteTime(string path, DateTime dt)
        {
            try
            {
                File.SetLastWriteTime(path, dt);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(WindowsFileIOError.FailedToSetLastWriteTime, ex, path);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(WindowsFileIOError.FailedToSetLastWriteTime, ex, path));
            }

            return AttemptResult.Succeeded();
        }


        #endregion
    }
}
