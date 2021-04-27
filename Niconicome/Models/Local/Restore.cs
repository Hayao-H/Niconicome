using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Niconicome.Extensions.System;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Domain.Local;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Domain.Utils;
using STypes = Niconicome.Models.Domain.Local.Store.Types;

namespace Niconicome.Models.Local
{

    public interface IRestore
    {
        void AddVideoDirectory(string path);
        void DeleteAllVideosAndPlaylists();
        void DeleteVideoDirectory(string path);
        IEnumerable<IBackupData> GetAllBackups();
        List<string> GetAllVideoDirectories();
        void JustifySavedFilePaths();
        void ResetSettings();
        bool TryApplyBackup(string guid);
        bool TryCreateBackup(string name);
        bool TryRemoveBackup(string guid);
    }

    public class Restore : IRestore
    {

        public Restore(IDataBase dataBase, ILogger logger, IBackuphandler backuphandler, INiconicoUtils niconicoUtils, IVideoFileStorehandler fileStorehandler, ILocalSettingHandler settingHandler, IVideoDirectoryStoreHandler videoDirectoryStoreHandler)
        {
            this.dataBase = dataBase;
            this.logger = logger;
            this.backuphandler = backuphandler;
            this.niconicoUtils = niconicoUtils;
            this.fileStorehandler = fileStorehandler;
            this.settingHandler = settingHandler;
            this.videoDirectoryStoreHandler = videoDirectoryStoreHandler;
        }

        /// <summary>
        /// データベースインスタンス
        /// </summary>
        private readonly IDataBase dataBase;

        /// <summary>
        /// ロガー
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// バックアップ
        /// </summary>
        private readonly IBackuphandler backuphandler;

        /// <summary>
        /// ユーティリティー
        /// </summary>
        private readonly INiconicoUtils niconicoUtils;

        /// <summary>
        /// 保存したファイルのハンドラ
        /// </summary>
        private readonly IVideoFileStorehandler fileStorehandler;

        /// <summary>
        /// 設定のハンドラ
        /// </summary>
        private readonly ILocalSettingHandler settingHandler;

        /// <summary>
        /// 保存フォルダー
        /// </summary>
        private readonly IVideoDirectoryStoreHandler videoDirectoryStoreHandler;

        /// <summary>
        /// 全ての設定をリセット
        /// </summary>
        public void ResetSettings()
        {
            this.dataBase.Clear(STypes::AppSettingString.TableName);
            this.dataBase.Clear(STypes::AppSettingBool.TableName);
        }

        /// <summary>
        ///全ての動画・プレイリストを削除する 
        /// </summary>
        public void DeleteAllVideosAndPlaylists()
        {
            this.dataBase.Clear(STypes::Playlist.TableName);
            this.dataBase.Clear(STypes::Video.TableName);
        }

        /// <summary>
        /// バックアップを作成する
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool TryCreateBackup(string name)
        {
            try
            {
                this.backuphandler.CreateBackup(name);
            }
            catch (Exception e)
            {
                this.logger.Error("バックアップの作成に失敗しました。", e);
                return false;
            }

            return true;
        }

        /// <summary>
        /// バックアップの一覧を取得する
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IBackupData> GetAllBackups()
        {
            return this.backuphandler.GetAllBackups();
        }

        /// <summary>
        /// バックアップを削除する
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public bool TryRemoveBackup(string guid)
        {
            try
            {
                this.backuphandler.RemoveBackup(guid);
            }
            catch (Exception e)
            {
                this.logger.Error("バックアップの削除に失敗しました。", e);
                return false;
            }
            return true;
        }

        /// <summary>
        /// バックアップを適用する
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public bool TryApplyBackup(string guid)
        {
            try
            {
                this.backuphandler.ApplyBackup(guid, "niconicome.db");
            }
            catch (Exception e)
            {

                this.logger.Error("バックアップの適用に失敗しました。", e);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 保存ファイルのパスを修正する
        /// </summary>
        public void JustifySavedFilePaths()
        {
            var directories = this.GetAllVideoDirectories();
            var filePaths = new List<string>();
            directories.AddUnique(AppContext.BaseDirectory);
            

            foreach (var directory in directories)
            {
                try
                {
                    filePaths.AddRange(Directory.GetFiles(directory, "*.mp4", SearchOption.AllDirectories));
                }
                catch
                {
                    continue;
                }
            }

            foreach (var file in filePaths)
            {
                string id;
                string format = this.settingHandler.GetStringSetting(SettingsEnum.FileNameFormat) ?? string.Empty;
                try
                {
                    id = this.niconicoUtils.GetIdFromFIleName(format, file);
                }
                catch { continue; }
                this.fileStorehandler.Add(id, file);
            }

            this.fileStorehandler.Clean();

        }

        /// <summary>
        /// 保存ディレクトリーを追加する
        /// </summary>
        /// <param name="path"></param>
        public void AddVideoDirectory(string path)
        {
            this.videoDirectoryStoreHandler.AddDirectory(path);
        }

        /// <summary>
        /// 保存ディレクトリーを削除する
        /// </summary>
        /// <param name="path"></param>
        public void DeleteVideoDirectory(string path)
        {
            this.videoDirectoryStoreHandler.DeleteDirectory(path);
        }

        /// <summary>
        /// 全ての保存ディレクトリーを取得する
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllVideoDirectories()
        {
            var list =this.videoDirectoryStoreHandler.GetVideoDirectories().Select(v => v.Path ?? string.Empty).Where(p => !p.IsNullOrEmpty()).ToList();
            list.AddUnique(AppContext.BaseDirectory);
            return list;
        }



    }
}
