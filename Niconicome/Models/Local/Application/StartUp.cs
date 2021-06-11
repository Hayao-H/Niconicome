using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Niconicome.Models.Auth;
using Niconicome.Models.Local.State;
using Niconicome.Models.Domain.Utils;
using Store = Niconicome.Models.Domain.Local.Store;
using Resume = Niconicome.Models.Domain.Niconico.Download.Video.Resume;
using NicoIO = Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Local.Settings;

namespace Niconicome.Models.Local.Application
{

    public interface IStartUp
    {
        void RunStartUptasks();
        event EventHandler? AutoLoginSucceeded;
    }

    class StartUp : IStartUp
    {

        public StartUp(Store::IVideoStoreHandler videoStoreHandler, Store::IPlaylistStoreHandler playlistStoreHandler, Store::IVideoFileStorehandler fileStorehandler, IBackuphandler backuphandler, IAutoLogin autoLogin, ISnackbarHandler snackbarHandler, ILogger logger, ILocalSettingHandler settingHandler, Resume::IStreamResumer streamResumer,NicoIO::INicoDirectoryIO nicoDirectoryIO)
        {

            this.videoStoreHandler = videoStoreHandler;
            this.playlistStoreHandler = playlistStoreHandler;
            this.fileStorehandler = fileStorehandler;
            this.backuphandler = backuphandler;
            this.autoLogin = autoLogin;
            this.snackbarHandler = snackbarHandler;
            this.logger = logger;
            this.settingHandler = settingHandler;
            this.streamResumer = streamResumer;
            this.nicoDirectoryIO = nicoDirectoryIO;
            this.DeleteInvalidbackup();
        }

        private readonly Store::IVideoStoreHandler videoStoreHandler;

        private readonly Store::IPlaylistStoreHandler playlistStoreHandler;

        private readonly Store::IVideoFileStorehandler fileStorehandler;

        private readonly IBackuphandler backuphandler;

        private readonly IAutoLogin autoLogin;

        private readonly ISnackbarHandler snackbarHandler;

        private readonly ILogger logger;

        private readonly ILocalSettingHandler settingHandler;

        private readonly Resume::IStreamResumer streamResumer;

        private readonly NicoIO::INicoDirectoryIO nicoDirectoryIO;

        /// <summary>
        /// 自動ログイン成功時
        /// </summary>
        public event EventHandler? AutoLoginSucceeded;

        /// <summary>
        /// スタートアップタスク
        /// </summary>
        public void RunStartUptasks()
        {
            Task.Run(async () =>
            {
                this.RemoveTmpFolder();
                this.JustifyData();
                this.DeleteInvalidFilePath();
                await this.Autologin();
            });
        }

        /// <summary>
        /// 一時フォルダーを削除する
        /// </summary>
        private void RemoveTmpFolder()
        {
            if (this.nicoDirectoryIO.Exists("tmp"))
            {
                var maxTmp = this.settingHandler.GetIntSetting(SettingsEnum.MaxTmpDirCount);
                if (maxTmp < 0) maxTmp = 20;
                var infos = this.streamResumer.GetAllSegmentsDirectoryInfo().ToList();
                if (infos.Count <= maxTmp) return;
                infos = infos.OrderBy(i => i.StartedOn).ToList();
                foreach (var i in Enumerable.Range(0, infos.Count - maxTmp))
                {
                    this.nicoDirectoryIO.Delete(Path.Combine(AppContext.BaseDirectory, "tmp", infos[i].DirectoryName));
                }

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

        /// <summary>
        /// 自動ログインを試行
        /// </summary>
        /// <returns></returns>
        private async Task Autologin()
        {
            if (!this.autoLogin.IsAUtologinEnable) return;
            if (!this.autoLogin.Canlogin())
            {
                this.snackbarHandler.Enqueue("自動ログインが出来ません。");
                return;
            }

            bool result;

            try
            {
                result = await this.autoLogin.LoginAsync();
            }
            catch (Exception e)
            {
                this.logger.Error("自動ログインに失敗しました。", e);
                this.snackbarHandler.Enqueue("自動ログインに失敗しました。");
                return;
            }

            if (!result)
            {
                this.snackbarHandler.Enqueue("自動ログインに失敗しました。");
                return;
            }
            else
            {
                this.snackbarHandler.Enqueue("自動ログインに成功しました。");
                this.RaiseLoginSucceeded();
            }
        }

        /// <summary>
        /// ログイン成功時
        /// </summary>
        private void RaiseLoginSucceeded()
        {
            this.AutoLoginSucceeded?.Invoke(this, EventArgs.Empty);
        }
    }
}
