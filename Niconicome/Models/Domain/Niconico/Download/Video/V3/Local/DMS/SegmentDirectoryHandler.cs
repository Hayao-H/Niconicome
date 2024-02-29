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
using Err = Niconicome.Models.Domain.Niconico.Download.Video.V3.Error.SegmentDirectoryHandlerError;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V3.Local.DMS
{
    public interface ISegmentDirectoryHandler
    {
        /// <summary>
        ///  セグメントファイルの保存先ディレクトリが既に存在するかどうかを確認
        /// </summary>
        /// <param name="niconicoID"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        bool Exists(string niconicoID, int resolution);

        /// <summary>
        /// ディレクトリを作成
        /// </summary>
        /// <param name="niconicoID"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        IAttemptResult<string> Create(string niconicoID, int resolution);

        /// <summary>
        /// セグメントファイルの保存先ディレクトリ情報を取得
        /// </summary>
        /// <param name="niconicoID"></param>
        /// <param name="resolution"></param>
        /// <returns></returns>
        IAttemptResult<ISegmentDirectoryInfo> GetSegmentDirectoryInfo(string niconicoID, int resolution);

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

    internal class SegmentDirectoryHandler : ISegmentDirectoryHandler
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

        public bool Exists(string niconicoID, int resolution)
        {

            IAttemptResult<IEnumerable<string>> dirResult = this._directoryIO.GetDirectories(Path.Combine(AppContext.BaseDirectory, FileFolder.SegmentsFolderPath));

            if (!dirResult.IsSucceeded || dirResult.Data is null)
            {
                return false;
            }

            string? directory = dirResult.Data.Select(p => Path.GetFileName(p)).FirstOrDefault(p => p.StartsWith($"{niconicoID}-{resolution}"));

            return directory is not null;
        }

        public IAttemptResult<string> Create(string niconicoID, int resolution)
        {
            string path = Path.Combine(AppContext.BaseDirectory, FileFolder.SegmentsFolderPath, $"{niconicoID}-{resolution}-{DateTime.Now.ToString("yyyy-MM-dd")}");

            string videoPath = Path.Combine(path, "video");
            string audioPath = Path.Combine(path, "audio");

            IAttemptResult result = this._directoryIO.CreateDirectory(path);
            IAttemptResult vResult = this._directoryIO.CreateDirectory(videoPath);
            IAttemptResult aResult = this._directoryIO.CreateDirectory(audioPath);

            if (new[] { result, vResult, aResult }.Any(p => !p.IsSucceeded))
            {
                return AttemptResult<string>.Fail(new[] { result, vResult, aResult }.First(r => !r.IsSucceeded).Message);
            }

            return AttemptResult<string>.Succeeded(path);
        }

        public IAttemptResult<ISegmentDirectoryInfo> GetSegmentDirectoryInfo(string niconicoID, int resolution)
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

            string directoryName = dirResult.Data.Select(p => Path.GetFileName(p)).First(p => p.StartsWith($"{niconicoID}-{resolution}"));

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

            foreach (var dir in dirResult.Data.Select(p => Path.GetFileName(p)))
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

            if (splited.Length != 6)
            {
                this._errorHandler.HandleError(Err.InvalidDirectoryName, directoryName);
                return AttemptResult<ISegmentDirectoryInfo>.Fail(this._errorHandler.GetMessageForResult(Err.InvalidDirectoryName, directoryName));
            }

            string dt = $"{splited[3]}-{splited[4]}-{splited[5]}";
            bool parse = DateTime.TryParseExact(dt, "yyyy-MM-dd", null, DateTimeStyles.AssumeLocal, out DateTime result);

            if (!parse)
            {
                this._errorHandler.HandleError(Err.InvalidDirectoryName, directoryName);
                return AttemptResult<ISegmentDirectoryInfo>.Fail(this._errorHandler.GetMessageForResult(Err.InvalidDirectoryName, directoryName));
            }

            string dirPath = Path.Combine(AppContext.BaseDirectory, FileFolder.SegmentsFolderPath, directoryName);
            IAttemptResult<IEnumerable<string>> videofilesResult = this._directoryIO.GetFiles(Path.Combine(dirPath, "video"), "*.cmfv");
            if (!videofilesResult.IsSucceeded || videofilesResult.Data is null)
            {
                return AttemptResult<ISegmentDirectoryInfo>.Fail(videofilesResult.Message);
            }

            IAttemptResult<IEnumerable<string>> audiofilesResult = this._directoryIO.GetFiles(Path.Combine(dirPath, "audio"), "*.cmfa");
            if (!audiofilesResult.IsSucceeded || audiofilesResult.Data is null)
            {
                return AttemptResult<ISegmentDirectoryInfo>.Fail(audiofilesResult.Message);
            }




            return AttemptResult<ISegmentDirectoryInfo>.Succeeded(new SegmentDirectoryInfo(dirPath, result, videofilesResult.Data.Select(p => Path.GetFileName(p)).ToList().AsReadOnly(), audiofilesResult.Data.Select(p => Path.GetFileName(p)).ToList().AsReadOnly()));
        }

        #endregion
    }
}
