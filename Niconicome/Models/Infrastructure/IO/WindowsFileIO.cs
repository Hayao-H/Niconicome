using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Niconicome.Models.Domain.Local.IO.V2;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Error = Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Infrastructure.IO
{
    public class WindowsFileIO : INiconicomeFileIO
    {
        public WindowsFileIO(Error::IErrorHandler errorHandler)
        {
            this._errorHandler = errorHandler;
        }

        #region field

        private readonly Error::IErrorHandler _errorHandler;

        #endregion

        #region Method

        public IAttemptResult<int> GetVerticalResolution(string path)
        {
            if (!this.Exists(path))
            {
                this._errorHandler.HandleError(WindowsFileIOError.FileDoesNotExist, path);
                return AttemptResult<int>.Fail(this._errorHandler.GetMessageForResult(WindowsFileIOError.FileDoesNotExist, path));
            }

            Type? type = Type.GetTypeFromProgID("Shell.Application");

            if (type is null)
            {
                this._errorHandler.HandleError(WindowsFileIOError.FailedToGetShellType);
                return AttemptResult<int>.Fail(this._errorHandler.GetMessageForResult(WindowsFileIOError.FailedToGetShellType));
            }

            var s = Activator.CreateInstance(type);

            dynamic? shell = Activator.CreateInstance(type);
            if (shell is null)
            {
                this._errorHandler.HandleError(WindowsFileIOError.FailedToCreateShellInstance);
                return AttemptResult<int>.Fail(this._errorHandler.GetMessageForResult(WindowsFileIOError.FailedToCreateShellInstance));
            }

            try
            {
                string name = Path.GetFileName(path);
                string? dirName = Path.GetDirectoryName(path);

                Shell32.Folder folder = shell.NameSpace(dirName);
                Shell32.FolderItem file = folder.ParseName(name);

                List<string> arrHeaders = new List<string>();
                for (int i = 0; i < 1000; i++)
                {
                    string header = folder.GetDetailsOf(null, i);
                    arrHeaders.Add(header);
                }

                int index = arrHeaders.IndexOf("フレーム高");

                string result = folder.GetDetailsOf(file, index);

                if (string.IsNullOrEmpty(result))
                {
                    this._errorHandler.HandleError(WindowsFileIOError.FailedToGetVerticalResolution);
                    return AttemptResult<int>.Fail(this._errorHandler.GetMessageForResult(WindowsFileIOError.FailedToGetVerticalResolution));
                }

                if (int.TryParse(result, out int resultInt))
                {
                    return AttemptResult<int>.Succeeded(resultInt);
                }
                else
                {
                    this._errorHandler.HandleError(WindowsFileIOError.FailedToParseVerticalResoltion, resultInt);
                    return AttemptResult<int>.Fail(this._errorHandler.GetMessageForResult(WindowsFileIOError.FailedToParseVerticalResoltion, resultInt));
                }
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(WindowsFileIOError.FailedToGetVerticalResolution, ex);
                return AttemptResult<int>.Fail(this._errorHandler.GetMessageForResult(WindowsFileIOError.FailedToGetVerticalResolution, ex));
            }
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
