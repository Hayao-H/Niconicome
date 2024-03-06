using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.IO.V2;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Infrastructure.IO
{
    public class WindowsDirectoryIO : INiconicomeDirectoryIO
    {
        public WindowsDirectoryIO(IErrorHandler errorHandler)
        {
            this._errorHandler = errorHandler;
        }

        #region field

        private readonly IErrorHandler _errorHandler;

        #endregion

        #region Method

        public IAttemptResult CreateDirectory(string path)
        {
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(WindowsDirectoryIOError.FailedToCreateDirectory, ex);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(WindowsDirectoryIOError.FailedToCreateDirectory, ex));
            }

            return AttemptResult.Succeeded();
        }

        public IAttemptResult<IEnumerable<string>> GetDirectories(string path)
        {
            string[] dirs;

            try
            {
                dirs = Directory.GetDirectories(path);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(WindowsDirectoryIOError.FailedToGetDirectories, ex, path);
                return AttemptResult<IEnumerable<string>>.Fail(this._errorHandler.GetMessageForResult(WindowsDirectoryIOError.FailedToGetDirectories, ex, path));
            }

            return AttemptResult<IEnumerable<string>>.Succeeded(dirs);
        }

        public IAttemptResult<IEnumerable<string>> GetFiles(string path, string searchPattern = "*")
        {
            string[] files;

            try
            {
                files = Directory.GetFiles(path, searchPattern);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(WindowsDirectoryIOError.FailedToGetFiles, ex, path);
                return AttemptResult<IEnumerable<string>>.Fail(this._errorHandler.GetMessageForResult(WindowsDirectoryIOError.FailedToGetFiles, ex, path));
            }

            return AttemptResult<IEnumerable<string>>.Succeeded(files);
        }



        public IAttemptResult Delete(string path, bool recursive = true)
        {
            try
            {
                Directory.Delete(path, recursive);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(WindowsDirectoryIOError.FailedToDeleteDirectory, ex, path);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(WindowsDirectoryIOError.FailedToDeleteDirectory, ex, path));
            }

            return AttemptResult.Succeeded();
        }

        public IAttemptResult Move(string source, string destination, bool overwrite = true)
        {
            try
            {
                this.CopyDirectory(source, destination, overwrite);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(WindowsDirectoryIOError.FailedToMoveDirectory, ex, source, destination);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(WindowsDirectoryIOError.FailedToMoveDirectory, ex, source, destination));
            }

            return this.Delete(source);
        }



        public bool Exists(string path)
        {
            return Directory.Exists(path);
        }


        #endregion

        private void CopyDirectory(string sourceDir, string destinationDir, bool overwrite)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            if (!dir.Exists)
            {
                Directory.CreateDirectory(destinationDir);
            }

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath,overwrite);
            }

            // If recursive and copying subdirectories, recursively call this method
            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir,overwrite);
            }
        }
    }
}
