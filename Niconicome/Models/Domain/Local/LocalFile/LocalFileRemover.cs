using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Local.IO.V2;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;

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
    }

    public class LocalFileRemover : ILocalFileRemover
    {
        public LocalFileRemover(INiconicomeDirectoryIO directoryIO, INiconicomeFileIO fileIO, INiconicoUtils utils)
        {
            this._directoryIO = directoryIO;
            this._fileIO = fileIO;
            this._utils = utils;
        }

        #region field

        private readonly INiconicomeFileIO _fileIO;

        private readonly INiconicomeDirectoryIO _directoryIO;

        private readonly INiconicoUtils _utils;

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

                foreach (var filePath in fResult.Data)
                {
                    string? fileName = Path.GetFileName(filePath);
                    if (fileName is null) continue;

                    string id = this._utils.GetIdFromFIleName(fileName);
                    if (string.IsNullOrEmpty(id)) continue;

                    if (exceptID.Contains(id)) continue;

                    this._fileIO.Delete(filePath, true);
                }

                return AttemptResult.Succeeded();
            });
        }


        #endregion
    }
}
