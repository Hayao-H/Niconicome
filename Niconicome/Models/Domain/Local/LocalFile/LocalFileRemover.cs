using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Local.IO.V2;
using Utils = Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Err = Niconicome.Models.Domain.Local.LocalFile.Error.LocalFileRemoverError;
using Niconicome.Models.Domain.Utils.Error;
using ABI.System;

namespace Niconicome.Models.Domain.Local.LocalFile
{
    public interface ILocalFileRemover
    {
        /// <summary>
        /// 除外指定した動画以外の実体ファイルを削除する
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="exceptID"></param>
        /// <returns></returns>
        Task<IAttemptResult> RemoveFilesAsync(string directoryPath, IEnumerable<string> exceptID);

        /// <summary>
        /// 指定したIDを含む実体ファイルを削除
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="niconicoID"></param>
        /// <returns></returns>
        Task<IAttemptResult> RemoveFileAsync(string directoryPath, string niconicoID);
    }

    public class LocalFileRemover : ILocalFileRemover
    {
        public LocalFileRemover(INiconicomeDirectoryIO directoryIO, INiconicomeFileIO fileIO, Utils::INiconicoUtils utils, IErrorHandler errorHandler)
        {
            this._directoryIO = directoryIO;
            this._fileIO = fileIO;
            this._utils = utils;
            this._errorHandler = errorHandler;
        }

        #region field

        private readonly INiconicomeFileIO _fileIO;

        private readonly INiconicomeDirectoryIO _directoryIO;

        private readonly Utils::INiconicoUtils _utils;

        private readonly IErrorHandler _errorHandler;

        #endregion

        #region Method

        public Task<IAttemptResult> RemoveFilesAsync(string directoryPath, IEnumerable<string> exceptID)
        {
            return Task.Run(() =>
            {
                IAttemptResult<IEnumerable<string>> fResult = this._directoryIO.GetFiles(directoryPath);
                if (!fResult.IsSucceeded || fResult.Data is null)
                {
                    return AttemptResult.Fail(fResult.Message);
                }

                IAttemptResult<IEnumerable<string>> dResult = this._directoryIO.GetDirectories(directoryPath);
                if (!dResult.IsSucceeded || dResult.Data is null)
                {
                    return AttemptResult.Fail(dResult.Message);
                }

                foreach (var filePath in fResult.Data)
                {
                    string? fileName = Path.GetFileName(filePath);
                    if (fileName is null) continue;

                    string id = this._utils.GetIdFromFIleName(fileName);
                    if (string.IsNullOrEmpty(id)) continue;

                    if (exceptID.Contains(id)) continue;

                    IAttemptResult result = this._fileIO.Delete(filePath, true);
                    if (result.IsSucceeded)
                    {
                        this._errorHandler.HandleError(Err.RemovedFile, fileName);
                    }
                }


                foreach (var filePath in dResult.Data)
                {
                    string? fileName = Path.GetFileName(filePath);
                    if (fileName is null) continue;

                    string id = this._utils.GetIdFromFIleName(fileName);
                    if (string.IsNullOrEmpty(id)) continue;

                    if (exceptID.Contains(id)) continue;

                    IAttemptResult result = this._directoryIO.Delete(filePath, true, true);
                    if (result.IsSucceeded)
                    {
                        this._errorHandler.HandleError(Err.RemovedFile, fileName);
                    }
                }

                return AttemptResult.Succeeded();
            });
        }

        public Task<IAttemptResult> RemoveFileAsync(string directoryPath, string niconicoID)
        {
            return Task.Run(() =>
            {
                IAttemptResult<IEnumerable<string>> fResult = this._directoryIO.GetFiles(directoryPath);
                if (!fResult.IsSucceeded || fResult.Data is null)
                {
                    return AttemptResult.Fail(fResult.Message);
                }

                IAttemptResult<IEnumerable<string>> dResult = this._directoryIO.GetDirectories(directoryPath);
                if (!dResult.IsSucceeded || dResult.Data is null)
                {
                    return AttemptResult.Fail(dResult.Message);
                }

                foreach (var filePath in fResult.Data)
                {
                    string? fileName = Path.GetFileName(filePath);
                    if (fileName is null) continue;

                    string id = this._utils.GetIdFromFIleName(fileName);
                    if (string.IsNullOrEmpty(id)) continue;

                    if (id != niconicoID) continue;

                    IAttemptResult result = this._fileIO.Delete(filePath, true);
                    if (result.IsSucceeded)
                    {
                        this._errorHandler.HandleError(Err.RemovedFile, fileName);
                    }
                }

                foreach (var filePath in dResult.Data)
                {
                    string? fileName = Path.GetFileName(filePath);
                    if (fileName is null) continue;

                    string id = this._utils.GetIdFromFIleName(fileName);
                    if (string.IsNullOrEmpty(id)) continue;

                    if (id != niconicoID) continue;

                    IAttemptResult result = this._directoryIO.Delete(filePath, true,true);
                    if (result.IsSucceeded)
                    {
                        this._errorHandler.HandleError(Err.RemovedFile, fileName);
                    }
                }

                return AttemptResult.Succeeded();
            });
        }



        #endregion
    }
}
