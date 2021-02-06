using System;
using System.Collections.Generic;
using System.IO;
using Niconicome.Models.Domain.Local;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Domain.Utils;
using STypes = Niconicome.Models.Domain.Local.Store.Types;

namespace Niconicome.Models.Local
{

    public interface IRestore
    {
        void ResetSettings();
        void DeleteAllVideosAndPlaylists();
        void JustifySavedFilePaths();
        bool TryCreateBackup(string name);
        bool TryRemoveBackup(string guid);
        bool TryApplyBackup(string guid);
        IEnumerable<IBackupData> GetAllBackups();
    }

    public class Restore : IRestore
    {

        public Restore(IDataBase dataBase, ILogger logger, IBackuphandler backuphandler, INiconicoUtils niconicoUtils, IVideoFileStorehandler fileStorehandler, ILocalSettingHandler settingHandler)
        {
            this.dataBase = dataBase;
            this.logger = logger;
            this.backuphandler = backuphandler;
            this.niconicoUtils = niconicoUtils;
            this.fileStorehandler = fileStorehandler;
            this.settingHandler = settingHandler;
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
            string[] filePaths;
            try
            {
                filePaths = Directory.GetFiles(AppContext.BaseDirectory, "*.mp4", SearchOption.AllDirectories);
            }
            catch
            {
                filePaths = new string[0];
            }

            foreach (var file in filePaths)
            {
                string id;
                string format = this.settingHandler.GetStringSetting(Settings.FileNameFormat) ?? string.Empty;
                try
                {
                    id = this.niconicoUtils.GetIdFromFIleName(format, file);
                }
                catch { continue; }
                this.fileStorehandler.Add(id, file);
            }

            this.fileStorehandler.Clean();


        }




    }
}
