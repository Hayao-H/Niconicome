using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.IO.V2;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Err = Niconicome.Models.Domain.Niconico.Download.Video.V2.Local.HLS.Error.SegmentDirectoryHandlerError;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V2.Local.HLS
{
    public interface ISegmentDirectoryHandler
    {
        /// <summary>
        /// セグメントファイルの保存先ディレクトリが既に存在するかどうかを確認
        /// </summary>
        /// <param name="niconicoID"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        bool Exists(string niconicoID, uint resolution);

        /// <summary>
        /// ディレクトリを作成
        /// </summary>
        /// <param name="niconicoID"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        IAttemptResult<string> Create(string niconicoID, uint resolution);

        /// <summary>
        /// セグメントファイルの保存先ディレクトリ情報を取得
        /// </summary>
        /// <param name="niconicoID"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        IAttemptResult<ISegmentDirectoryInfo> GetSegmentDirectoryInfo(string niconicoID, uint resolution);

        /// <summary>
        /// 全てのセグメントファイルの保存先ディレクトリを取得
        /// </summary>
        /// <returns></returns>
        IAttemptResult<IEnumerable<ISegmentDirectoryInfo>> GetAllSegmentDirectoryInfos();

        /// <summary>
        /// セグメントファイルの保存先ディレクトリのルートを作成
        /// </summary>
        /// <returns></returns>
        IAttemptResult CreateRootDirectotyIfNotExists();
    }

    public class SegmentDirectoryHandler : ISegmentDirectoryHandler
    {
        public SegmentDirectoryHandler(INiconicomeDirectoryIO directoryIO, IErrorHandler errorHandler)
        {
            this._directoryIO = directoryIO;
            this._errorHandler = errorHandler;
        }

        #region field

        private readonly INiconicomeDirectoryIO _directoryIO;

        private readonly IErrorHandler _errorHandler;

        #endregion

        #region Method

        public bool Exists(string niconicoID, uint resolution)
        {

            IAttemptResult<IEnumerable<string>> dirResult = this._directoryIO.GetDirectories(Path.Combine(AppContext.BaseDirectory, FileFolder.SegmentsFolderPath));

            if (!dirResult.IsSucceeded || dirResult.Data is null)
            {
                return false;
            }

            string? directory = dirResult.Data.FirstOrDefault(p => p.StartsWith($"{niconicoID}-{resolution}-"));

            return directory is not null;
        }

        public IAttemptResult<string> Create(string niconicoID, uint resolution)
        {
            string path = Path.Combine(AppContext.BaseDirectory, FileFolder.SegmentsFolderPath, $"{niconicoID}-{resolution}-{DateTime.Now.ToString("yyyy-MM-dd")}");

            IAttemptResult result = this._directoryIO.CreateDirectory(path);

            if (!result.IsSucceeded)
            {
                return AttemptResult<string>.Fail(result.Message);
            }

            return AttemptResult<string>.Succeeded(path);
        }

        public IAttemptResult<ISegmentDirectoryInfo> GetSegmentDirectoryInfo(string niconicoID, uint resolution)
        {
            if (!this.Exists(niconicoID, resolution))
            {
                this._errorHandler.HandleError(Err.NotExists, niconicoID, resolution);
                return AttemptResult<ISegmentDirectoryInfo>.Fail(this._errorHandler.GetMessageForResult(Err.NotExists, niconicoID, resolution));
            }

            IAttemptResult<IEnumerable<string>> dirResult = this._directoryIO.GetDirectories(Path.Combine(AppContext.BaseDirectory, FileFolder.SegmentsFolderPath));

            if (!dirResult.IsSucceeded || dirResult.Data is null)
            {
                return AttemptResult<ISegmentDirectoryInfo>.Fail(dirResult.Message);
            }

            string directoryName = dirResult.Data.First(p => p.StartsWith($"{niconicoID}-{resolution}-"));

            return this.Parse(directoryName);
        }

        public IAttemptResult<IEnumerable<ISegmentDirectoryInfo>> GetAllSegmentDirectoryInfos()
        {
            IAttemptResult<IEnumerable<string>> dirResult = this._directoryIO.GetDirectories(Path.Combine(AppContext.BaseDirectory, FileFolder.SegmentsFolderPath));

            if (!dirResult.IsSucceeded || dirResult.Data is null)
            {
                return AttemptResult<IEnumerable<ISegmentDirectoryInfo>>.Fail(dirResult.Message);
            }

            var infos = new List<ISegmentDirectoryInfo>();

            foreach (var dir in dirResult.Data)
            {
                IAttemptResult<ISegmentDirectoryInfo> result = this.Parse(dir);
                if (!result.IsSucceeded || result.Data is null)
                {
                    continue;
                }

                infos.Add(result.Data);
            }

            return AttemptResult<IEnumerable<ISegmentDirectoryInfo>>.Succeeded(infos);
        }

       public IAttemptResult CreateRootDirectotyIfNotExists()
        {
            string path = Path.Combine(AppContext.BaseDirectory, FileFolder.SegmentsFolderPath);
            if (this._directoryIO.Exists(path))
            {
                return AttemptResult.Succeeded();
            }

            return this._directoryIO.CreateDirectory(path);
        }



        #endregion

        #region private

        private IAttemptResult<ISegmentDirectoryInfo> Parse(string directoryName)
        {
            string[] splited = directoryName.Split("-");

            if (splited.Length != 5)
            {
                this._errorHandler.HandleError(Err.InvalidDirectoryName, directoryName);
                return AttemptResult<ISegmentDirectoryInfo>.Fail(this._errorHandler.GetMessageForResult(Err.InvalidDirectoryName, directoryName));
            }

            string dt = $"{splited[2]}-{splited[3]}-{splited[4]}";
            bool parse = DateTime.TryParseExact(dt, "yyyy-MM-dd", null, DateTimeStyles.AssumeLocal, out DateTime result);

            if (!parse)
            {
                this._errorHandler.HandleError(Err.InvalidDirectoryName, directoryName);
                return AttemptResult<ISegmentDirectoryInfo>.Fail(this._errorHandler.GetMessageForResult(Err.InvalidDirectoryName, directoryName));
            }

            string dirPath = Path.Combine(AppContext.BaseDirectory, FileFolder.SegmentsFolderPath, directoryName);
            IAttemptResult<IEnumerable<string>> filesResult = this._directoryIO.GetFiles(dirPath, "*.ts");
            if (!filesResult.IsSucceeded || filesResult.Data is null)
            {
                return AttemptResult<ISegmentDirectoryInfo>.Fail(filesResult.Message);
            }


            return AttemptResult<ISegmentDirectoryInfo>.Succeeded(new SegmentDirectoryInfo(dirPath, result, filesResult.Data.ToList().AsReadOnly()));
        }

        #endregion
    }
}
