using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Domain.Niconico.Net.Json;
using LiteDB;
using System.Text.Json;
using System.Runtime.InteropServices.ComTypes;
using Niconicome.Models.Domain.Local;
using System.Diagnostics;

namespace Niconicome.Models.Local
{
    public interface IBackuphandler
    {
        IEnumerable<IBackupData> GetAllBackups();
        void CreateBackup(string name);
        void CreateBackup(string name, string dbFileName);
        void RemoveBackup(string guid);
        void ApplyBackup(string guid,string dbFilename);
        void Clean();
    }

    public interface IBackupData
    {
        string Name { get; }
        string GUID { get; }
        string Filename { get; }
        DateTime CreatedOn { get; }
        float FileSize { get; }
    }

    /// <summary>
    /// バックアップハンドラ
    /// </summary>
    public class BackupHandler : IBackuphandler
    {

        public BackupHandler(ILogger logger, IDataBase dataBase)
        {
            this.logger = logger;
            this.backupFolderName = "backups";
            this.backupIndexFileName = "index.json";
            this.dataBase = dataBase;
        }

        private readonly ILogger logger;

        private readonly string backupFolderName;

        private readonly string backupIndexFileName;

        private readonly IDataBase dataBase;

        /// <summary>
        /// 全てのバックアップを取得する
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IBackupData> GetAllBackups()
        {
            var data = new List<IBackupData>();

            try
            {
                var retlieved = this.GetBackupDatasFromIndex();
                data.AddRange(retlieved);

            }
            catch (Exception e)
            {
                this.logger.Error("バックアップ情報の取得に失敗しました。", e);
            }

            return data;
        }

        /// <summary>
        /// バックアップを作成する
        /// </summary>
        /// <param name="name"></param>
        public void CreateBackup(string name)
        {
            this.CreateBackup(name, "niconicome.db");
        }

        /// <summary>
        /// バックアップを作成する
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbFileName"></param>
        public void CreateBackup(string name, string dbFileName)
        {
            if (!File.Exists(dbFileName)) return;
            float fileSIze = this.TryGetDatabaseSize(dbFileName);
            var data = new BackupData(name)
            {
                FileSize = fileSIze
            };
            this.AddBackup(dbFileName, data);
        }

        /// <summary>
        /// データを修復する
        /// </summary>
        public void Clean()
        {
            if (this.IndexfileExists())
            {
                var backups = this.GetBackupDatasFromIndex().Where(b => File.Exists(Path.Combine(this.backupFolderName, b.Filename))).ToList();
                this.UpdateIndex(backups);
            }
        }

        /// <summary>
        /// バックアップを削除する
        /// </summary>
        /// <param name="guid"></param>
        public void RemoveBackup(string guid)
        {
            var backups = this.GetBackupDatasFromIndex();

            if (!backups.Any(b => b.GUID == guid)) return;

            var targets = backups.Where(b => b.GUID == guid);

            foreach (var target in targets)
            {
                string filePath = Path.Combine(this.backupFolderName, target.Filename);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }

            backups.RemoveAll(b => b.GUID == guid);
            this.UpdateIndex(backups);
        }

        /// <summary>
        /// バックアップを適用する
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="dbFilename"></param>
        public void ApplyBackup(string guid, string dbFilename)
        {
            var target = this.GetBackupDatasFromIndex().FirstOrDefault(b => b.GUID == guid);
            if (target is null)
            {
                throw new InvalidOperationException("そのようなバックアップは存在しません。");
            }

            string fileName = Path.Combine(this.backupFolderName, target.Filename);
            if (!File.Exists(fileName)) throw new IOException($"バックアップファイル({target.Filename})は存在しません。");

            this.dataBase.Dispose();
            File.Delete(dbFilename);
            File.Copy(fileName, dbFilename);
            this.dataBase.Open();

        }

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
        private List<IBackupData> GetBackupDatasFromIndex()
        {
            var data = new List<IBackupData>();
            if (this.IndexfileExists())
            {
                using var fs = new StreamReader(this.GetIndexFilePath());
                string content = fs.ReadToEnd();
                try
                {
                    var backups = JsonParser.DeSerialize<List<BackupData>>(content);
                    data.AddRange(backups);
                }
                catch (Exception e)
                {
                    throw new JsonException($"インデックスファイルの解析に失敗しました。(詳細: {e.Message})");
                }
            }
            return data;
        }

        /// <summary>
        /// インデックスを更新する
        /// </summary>
        /// <param name="data"></param>
        private void UpdateIndex(List<IBackupData> data)
        {
            if (!Directory.Exists(this.backupFolderName))
            {
                try
                {
                    Directory.CreateDirectory("backups");
                }
                catch (Exception e)
                {
                    throw new IOException($"バックアップフォルダーの作成に失敗しました。(詳細: {e.Message})");
                }
            }

            using var fs = new StreamWriter(this.GetIndexFilePath());
            var serialized = JsonParser.Serialize(data);
            fs.Write(serialized);
        }

        /// <summary>
        /// 情報ファイルの存在を確認する
        /// </summary>
        /// <returns></returns>
        private bool IndexfileExists()
        {
            return Directory.Exists(this.backupFolderName) && File.Exists(this.GetIndexFilePath());
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
        private void AddBackup(string dbFilename, IBackupData backupData)
        {
            if (!Directory.Exists("backups"))
            {
                try
                {
                    Directory.CreateDirectory("backups");
                }
                catch (Exception e)
                {
                    throw new IOException($"バックアップフォルダーの作成に失敗しました。(詳細: {e.Message})");
                }
            }

            try
            {
                File.Copy(dbFilename, Path.Combine("backups", backupData.Filename));
            }
            catch (Exception e)
            {
                throw new IOException($"ファイルのコピーに失敗しました。(詳細: {e.Message})");
            }

            var data = this.GetBackupDatasFromIndex();
            data.Add(backupData);
            this.UpdateIndex(data);

        }
    }

    /// <summary>
    /// バックアップデータ
    /// </summary>
    public class BackupData : IBackupData
    {
        public BackupData(string name)
        {
            this.GUID = Guid.NewGuid().ToString("D");
            this.Name = name;
            this.Filename = $"{this.GUID}.dbbackup";
            this.CreatedOn = DateTime.Now;
        }

        public string Name { get; init; }

        public string GUID { get; init; }

        public string Filename { get; init; }

        public DateTime CreatedOn { get; init; }

        public float FileSize { get; set; }
    }
}
