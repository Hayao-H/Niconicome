using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.LocalFile.Error;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Error = Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Local.LocalFile
{
    public interface ILocalDirectoryHandler
    {
        /// <summary>
        /// ローカルディレクトリーからID一覧を取得する
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="searchSubfolder"></param>
        /// <returns></returns>
        IAttemptResult<IImmutableList<string>> GetVideoIdsFromDirectory(string directoryPath, bool searchSubfolder = true);
    }

    public class LocalDirectoryHandler : ILocalDirectoryHandler
    {
        public LocalDirectoryHandler(INiconicoUtils niconicoUtils, Error::IErrorHandler errorHandler)
        {
            this._errorHandler = errorHandler;
            this._niconicoUtils = niconicoUtils;
        }

        #region field

        private readonly INiconicoUtils _niconicoUtils;

        private readonly Error::IErrorHandler _errorHandler;

        #endregion

        public IAttemptResult<IImmutableList<string>> GetVideoIdsFromDirectory(string directoryPath, bool searchSubfolder = true)
        {
            var option = searchSubfolder ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var files = new List<string>();

            try
            {
                files.AddRange(Directory.GetFiles(directoryPath, $"*{FileFolder.Mp4FileExt}", option));
                files.AddRange(Directory.GetFiles(directoryPath, $"*{FileFolder.TsFileExt}", option));
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(LocalDirectoryHandlerError.FailedToSearchLocalDirectory, ex);
                return AttemptResult<IImmutableList<string>>.Fail(this._errorHandler.GetMessageForResult(LocalDirectoryHandlerError.FailedToSearchLocalDirectory, ex));
            }

            var list = files.Select(f => this._niconicoUtils.GetIdFromFIleName(f)).Where(f => !f.IsNullOrEmpty()).ToList();

            return AttemptResult<IImmutableList<string>>.Succeeded(list.ToImmutableList());

        }
    }
}
