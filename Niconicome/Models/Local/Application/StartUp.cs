using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Store = Niconicome.Models.Domain.Local.Store;

namespace Niconicome.Models.Local.Application
{

    public interface IStartUp
    {
        void RunStartUptasks();
    }

    class StartUp : IStartUp
    {

        public StartUp(Store::IVideoStoreHandler videoStoreHandler, Store::IPlaylistStoreHandler playlistStoreHandler, Store::IVideoFileStorehandler fileStorehandler, IBackuphandler backuphandler)
        {
            this.videoStoreHandler = videoStoreHandler;
            this.playlistStoreHandler = playlistStoreHandler;
            this.fileStorehandler = fileStorehandler;
            this.backuphandler = backuphandler;
            this.DeleteInvalidbackup();
        }

        private readonly Store::IVideoStoreHandler videoStoreHandler;

        private readonly Store::IPlaylistStoreHandler playlistStoreHandler;

        private readonly Store::IVideoFileStorehandler fileStorehandler;

        private readonly IBackuphandler backuphandler;

        /// <summary>
        /// スタートアップタスク
        /// </summary>
        public void RunStartUptasks()
        {
            Task.Run(() =>
            {
                this.RemoveTmpFolder();
                this.JustifyData();
                this.DeleteInvalidFilePath();
            });
        }

        /// <summary>
        /// 一時フォルダーを削除する
        /// </summary>
        private void RemoveTmpFolder()
        {
            if (Directory.Exists("tmp"))
            {
                try
                {
                    Directory.Delete("tmp", true);
                }
                catch { }
            }
        }

        /// <summary>
        /// データを修復する
        /// </summary>
        private void JustifyData()
        {
            this.videoStoreHandler.JustifyVideos();
            var playlists = this.playlistStoreHandler.GetAllPlaylists();
            if (playlists.Any())
            {
                this.playlistStoreHandler.JustifyPlaylists(playlists.Select(p => p.Id));
            }
        }

        /// <summary>
        /// 存在しない動画ファイルのパスを削除する
        /// </summary>
        private void DeleteInvalidFilePath()
        {
            this.fileStorehandler.Clean();
        }

        /// <summary>
        /// 存在しないバックアップを削除する
        /// </summary>
        private void DeleteInvalidbackup()
        {
            this.backuphandler.Clean();
        }
    }
}
