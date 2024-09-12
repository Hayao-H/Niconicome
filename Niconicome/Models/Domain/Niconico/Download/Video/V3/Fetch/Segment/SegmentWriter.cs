using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.IO.V2;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Err = Niconicome.Models.Domain.Niconico.Download.Video.V3.Error.SegmentWriterError;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V3.Fetch.Segment
{
    public interface ISegmentWriter
    {
        /// <summary>
        /// セグメントファイルを書き込む
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        IAttemptResult Write(byte[] data, string path);
    }

    public class SegmentWriter : ISegmentWriter
    {
        public SegmentWriter(IErrorHandler errorHandler,INiconicomeFileIO fileIO,INiconicomeDirectoryIO directoryIO)
        {
            this._errorHandler = errorHandler;
            this._fileIO = fileIO;
            this._directoryIO = directoryIO;
        }

        #region field

        private readonly IErrorHandler _errorHandler;

        private readonly INiconicomeFileIO _fileIO;

        private readonly INiconicomeDirectoryIO _directoryIO;

        #endregion

        #region Method

        public IAttemptResult Write(byte[] data, string path)
        {
            string? dirPath = Path.GetDirectoryName(path);

            if (string.IsNullOrEmpty(dirPath))
            {
                this._errorHandler.HandleError(Err.FailedToGetDirPath, path);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(Err.FailedToGetDirPath, path));
            }

            if (!this._directoryIO.Exists(dirPath))
            {
                IAttemptResult dirResult = this._directoryIO.CreateDirectory(dirPath);
                if (!dirResult.IsSucceeded)
                {
                    return dirResult;
                }
            }

            IAttemptResult result = this._fileIO.Write(path,data);

            return result;
        }

        #endregion
    }
}
