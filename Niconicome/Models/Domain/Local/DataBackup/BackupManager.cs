using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Net.Json;
using LiteDB;
using System.Text.Json;
using System.Runtime.InteropServices.ComTypes;
using System.Diagnostics;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Domain.Local.IO.V2;
using Niconicome.Models.Domain.Niconico.Net.Xml;
using Niconicome.Models.Infrastructure.Database.LiteDB;
using Windows.ApplicationModel.Background;

namespace Niconicome.Models.Domain.Local.DataBackup
{
    public interface IBackupManager
    {
        /// <summary>
        /// バックアップ一覧を取得
        /// </summary>
        /// <returns></returns>
        IAttemptResult<IEnumerable<IBackupData>> GetAllBackups();

        /// <summary>
        /// バックアップを作成
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IAttemptResult<IBackupData> CreateBackup(string name);

        /// <summary>
        /// バックアップを削除
        /// </summary>
        /// <param name="guid"></param>
        IAttemptResult RemoveBackup(string guid);

        /// <summary>
        /// バックアップを適応
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="dbFilename"></param>
        IAttemptResult ApplyBackup(string guid);

        /// <summary>
        /// 存在しないバックアップをリストから削除
        /// </summary>
        void Clean();
    }


    /// <summary>
    /// バックアップハンドラ
    /// </summary>
    public class BackupManager : IBackupManager
    {

        public BackupManager(IErrorHandler errorHandler, INiconicomeDirectoryIO directoryIO, INiconicomeFileIO fileIO, ILiteDBHandler dataBase)
        {
            this._errorHandler = errorHandler;
            this._directoryIO = directoryIO;
            this._fileIO = fileIO;
            this._dataBase = dataBase;

            this.backupFolderName = "backups";
            this.backupIndexFileName = "index.json";
            this.dbFileName = "niconicome.db";
        }

        #region field

        private readonly IErrorHandler _errorHandler;

        private readonly INiconicomeFileIO _fileIO;

        private readonly INiconicomeDirectoryIO _directoryIO;

        private readonly ILiteDBHandler _dataBase;

        private readonly string backupFolderName;

        private readonly string backupIndexFileName;

        private readonly string dbFileName;

        #endregion

        #region Method

        public IAttemptResult<IEnumerable<IBackupData>> GetAllBackups()
        {
            IAttemptResult<List<IBackupData>> result = this.GetBackupDatasFromIndex();
            if (!result.IsSucceeded || result.Data is null)
            {
                return AttemptResult<IEnumerable<IBackupData>>.Fail(result.Message);
            }
            else
            {
                return AttemptResult<IEnumerable<IBackupData>>.Succeeded(result.Data);
            }
        }

        public IAttemptResult<IBackupData> CreateBackup(string name)
        {

            if (!File.Exists(this.dbFileName))
            {
                this._errorHandler.HandleError(BackupManagerError.DBFileNotExists);
                return AttemptResult<IBackupData>.Fail(this._errorHandler.GetMessageForResult(BackupManagerError.DBFileNotExists));
            }

            float fileSIze = this.TryGetDatabaseSize(this.dbFileName);

            var data = new BackupData(name)
            {
                FileSize = fileSIze
            };

            IAttemptResult result = this.AddBackup(this.dbFileName, data);
            if (result.IsSucceeded)
            {
                return AttemptResult<IBackupData>.Succeeded(data);
            } else
            {
                return AttemptResult<IBackupData>.Fail(result.Message);
            }
        }

        public IAttemptResult RemoveBackup(string guid)
        {

            IAttemptResult<List<IBackupData>> backupsResult = this.GetBackupDatasFromIndex();
            if (!backupsResult.IsSucceeded || backupsResult.Data is null)
            {
                return AttemptResult.Fail(backupsResult.Message);
            }

            List<IBackupData> backups = backupsResult.Data;

            if (!backups.Any(b => b.GUID == guid))
            {
                this._errorHandler.HandleError(BackupManagerError.SpecifiedBackupDoesNotExists, guid);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(BackupManagerError.SpecifiedBackupDoesNotExists, guid));
            }

            var targets = backups.Where(b => b.GUID == guid);

            foreach (var target in targets)
            {
                string filePath = Path.Combine(this.backupFolderName, target.Filename);
                if (!this._fileIO.Exists(filePath))
                {
                    continue;
                }

                IAttemptResult dResult = this._fileIO.Delete(filePath);
                if (!dResult.IsSucceeded)
                {
                    return dResult;
                }

            }

            backups.RemoveAll(b => b.GUID == guid);

            return this.UpdateIndex(backups);
        }

        public IAttemptResult ApplyBackup(string guid)
        {
            IAttemptResult<List<IBackupData>> backupsResult = this.GetBackupDatasFromIndex();
            if (!backupsResult.IsSucceeded || backupsResult.Data is null)
            {
                return AttemptResult.Fail(backupsResult.Message);
            }

            var target = backupsResult.Data.FirstOrDefault(b => b.GUID == guid);
            if (target is null)
            {
                this._errorHandler.HandleError(BackupManagerError.SpecifiedBackupDoesNotExists, guid);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(BackupManagerError.SpecifiedBackupDoesNotExists, guid));
            }

            string fileName = Path.Combine(this.backupFolderName, target.Filename);
            if (!File.Exists(fileName))
            {

                this._errorHandler.HandleError(BackupManagerError.BackupFileDoesNotExist, target.Filename);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(BackupManagerError.BackupFileDoesNotExist, target.Filename));
            }

            this._dataBase.Dispose();

            IAttemptResult deleteResult = this._fileIO.Delete(this.dbFileName);
            if (!deleteResult.IsSucceeded)
            {
                return deleteResult;
            }

            IAttemptResult copyResult = this._fileIO.Copy(fileName, this.dbFileName);
            if (!copyResult.IsSucceeded)
            {
                return copyResult;
            }

            return this._dataBase.ReOpen();
        }

        public void Clean()
        {
            if (!this.IndexfileExists())
            {
                return;
            }

            IAttemptResult<List<IBackupData>> result = this.GetBackupDatasFromIndex();
            if (!result.IsSucceeded || result.Data is null)
            {
                return;
            }

            var backups = result.Data.Where(b => this._fileIO.Exists(Path.Combine(this.backupFolderName, b.Filename))).ToList();
            this.UpdateIndex(backups);
        }

        #endregion


        #region privaye

        /// <summary>
        /// ファイルサイズを取得する
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private float TryGetDatabaseSize(string filename)
        {
            try
            {
                var file = new FileInfo(filename);
                return file.Length / 1024;
            }
            catch
            {
                return 0f;
            }
        }

        /// <summary>
        /// バックアップ情報を取得する
        /// </summary>
        /// <returns></returns>
        private IAttemptResult<List<IBackupData>> GetBackupDatasFromIndex()
        {
            var data = new List<IBackupData>();
            if (this.IndexfileExists())
            {
                string content;
                try
                {

                    using var fs = new StreamReader(this.GetIndexFilePath());
                    content = fs.ReadToEnd();
                }
                catch (Exception ex)
                {
                    this._errorHandler.HandleError(BackupManagerError.FailedToLoadIndex, ex);
                    return AttemptResult<List<IBackupData>>.Fail(this._errorHandler.GetMessageForResult(BackupManagerError.FailedToLoadIndex, ex));
                }

                try
                {
                    var backups = JsonParser.DeSerialize<List<BackupData>>(content);
                    data.AddRange(backups);
                }
                catch (Exception ex)
                {
                    this._errorHandler.HandleError(BackupManagerError.FailedToLoadIndex, ex);
                    return AttemptResult<List<IBackupData>>.Fail(this._errorHandler.GetMessageForResult(BackupManagerError.FailedToLoadIndex, ex));
                }
            }

            return AttemptResult<List<IBackupData>>.Succeeded(data);
        }

        /// <summary>
        /// インデックスを更新する
        /// </summary>
        /// <param name="data"></param>
        private IAttemptResult UpdateIndex(List<IBackupData> data)
        {
            IAttemptResult dbResult = this.CreateBackupDirectory();
            if (!dbResult.IsSucceeded)
            {
                return dbResult;
            }

            try
            {
                using var fs = new StreamWriter(this.GetIndexFilePath());
                var serialized = JsonParser.Serialize(data);
                fs.Write(serialized);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(BackupManagerError.FailedToWriteIndex, ex);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(BackupManagerError.FailedToWriteIndex, ex));
            }

            return AttemptResult.Succeeded();
        }

        /// <summary>
        /// 情報ファイルの存在を確認する
        /// </summary>
        /// <returns></returns>
        private bool IndexfileExists()
        {
            var dir = this._directoryIO.Exists(this.backupFolderName);
            var name = this.GetIndexFilePath();
            var file  = this._fileIO.Exists(name);

            return this._directoryIO.Exists(this.backupFolderName) && this._fileIO.Exists(this.GetIndexFilePath());
        }

        /// <summary>
        /// インデックスファイルを取得する
        /// </summary>
        /// <returns></returns>
        private string GetIndexFilePath()
        {
            return Path.Combine(this.backupFolderName, this.backupIndexFileName);
        }

        /// <summary>
        /// バックアップを追加する
        /// </summary>
        /// <param name="dbFilename"></param>
        /// <param name="backupData"></param>
        private IAttemptResult AddBackup(string dbFilename, IBackupData backupData)
        {
            IAttemptResult dbResult = this.CreateBackupDirectory();
            if (!dbResult.IsSucceeded)
            {
                return dbResult;
            }

            IAttemptResult copyResult = this._fileIO.Copy(dbFilename, Path.Combine("backups", backupData.Filename));
            if (!copyResult.IsSucceeded)
            {
                return copyResult;
            }

            var result = this.GetBackupDatasFromIndex();
            if (!result.IsSucceeded || result.Data is null)
            {
                return AttemptResult.Fail(result.Message);
            }

            result.Data.Add(backupData);
            return this.UpdateIndex(result.Data);

        }

        /// <summary>
        /// バックアップディレクトリを作成
        /// </summary>
        /// <returns></returns>
        private IAttemptResult CreateBackupDirectory()
        {
            if (this._directoryIO.Exists("backups"))
            {
                return AttemptResult.Succeeded();
            }

            try
            {
                this._directoryIO.CreateDirectory("backups");
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(BackupManagerError.FailedToCreateDirectory, ex);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(BackupManagerError.FailedToCreateDirectory, ex));
            }

            return AttemptResult.Succeeded();
        }

        #endregion

    }

}
