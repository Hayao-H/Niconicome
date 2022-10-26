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
using Niconicome.Models.Local.Addon;
using Niconicome.Models.Utils.InitializeAwaiter;
using Niconicome.Models.Local.Addon.V2;

namespace Niconicome.Models.Local.Application
{

    public interface IStartUp
    {
        void RunStartUptasks();
        event EventHandler? AutoLoginSucceeded;
    }

    class StartUp : IStartUp
    {

        public StartUp(Store::IPlaylistStoreHandler playlistStoreHandler, Store::IVideoFileStorehandler fileStorehandler, IBackuphandler backuphandler, IAutoLogin autoLogin, ISnackbarHandler snackbarHandler, ILogger logger, ILocalSettingHandler settingHandler, Resume::IStreamResumer streamResumer, NicoIO::INicoDirectoryIO nicoDirectoryIO, IAddonManager addonManager)
        {

            this._playlistStoreHandler = playlistStoreHandler;
            this._fileStorehandler = fileStorehandler;
            this._backuphandler = backuphandler;
            this._autoLogin = autoLogin;
            this._snackbarHandler = snackbarHandler;
            this._logger = logger;
            this._settingHandler = settingHandler;
            this._streamResumer = streamResumer;
            this._nicoDirectoryIO = nicoDirectoryIO;
            this._addonManager = addonManager;
            this.DeleteInvalidbackup();
        }

        #region field

        private readonly Store::IPlaylistStoreHandler _playlistStoreHandler;

        private readonly Store::IVideoFileStorehandler _fileStorehandler;

        private readonly IBackuphandler _backuphandler;

        private readonly IAutoLogin _autoLogin;

        private readonly ISnackbarHandler _snackbarHandler;

        private readonly ILogger _logger;

        private readonly ILocalSettingHandler _settingHandler;

        private readonly Resume::IStreamResumer _streamResumer;

        private readonly NicoIO::INicoDirectoryIO _nicoDirectoryIO;

        private readonly IAddonManager _addonManager;

        #endregion

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
                this.LoadAddon();
                await this.Autologin();
            });
        }

        /// <summary>
        /// 一時フォルダーを削除する
        /// </summary>
        private void RemoveTmpFolder()
        {
            if (this._nicoDirectoryIO.Exists("tmp"))
            {
                var maxTmp = this._settingHandler.GetIntSetting(SettingsEnum.MaxTmpDirCount);
                if (maxTmp < 0) maxTmp = 20;
                var infos = this._streamResumer.GetAllSegmentsDirectoryInfo().ToList();
                if (infos.Count <= maxTmp) return;
                infos = infos.OrderBy(i => i.StartedOn).ToList();
                foreach (var i in Enumerable.Range(0, infos.Count - maxTmp))
                {
                    this._nicoDirectoryIO.Delete(Path.Combine(AppContext.BaseDirectory, "tmp", infos[i].DirectoryName));
                }

            }
        }

        /// <summary>
        /// データを修復する
        /// </summary>
        private void JustifyData()
        {
            this._playlistStoreHandler.Initialize();
        }

        /// <summary>
        /// 存在しない動画ファイルのパスを削除する
        /// </summary>
        private void DeleteInvalidFilePath()
        {
            this._fileStorehandler.Clean();
        }

        /// <summary>
        /// 存在しないバックアップを削除する
        /// </summary>
        private void DeleteInvalidbackup()
        {
            this._backuphandler.Clean();
        }

        /// <summary>
        /// 自動ログインを試行
        /// </summary>
        /// <returns></returns>
        private async Task Autologin()
        {
            if (!this._autoLogin.IsAUtologinEnable) return;
            if (!this._autoLogin.Canlogin())
            {
                this._snackbarHandler.Enqueue("自動ログインが出来ません。");
                return;
            }

            bool result;

            try
            {
                result = await this._autoLogin.LoginAsync();
            }
            catch (Exception e)
            {
                this._logger.Error("自動ログインに失敗しました。", e);
                this._snackbarHandler.Enqueue("自動ログインに失敗しました。");
                return;
            }

            if (!result)
            {
                this._snackbarHandler.Enqueue("自動ログインに失敗しました。");
                return;
            }
            else
            {
                this._snackbarHandler.Enqueue("自動ログインに成功しました。");
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

        /// <summary>
        /// アドオンを読み込む
        /// </summary>
        private void LoadAddon()
        {
            this._addonManager.InitializeAddons();
        }
    }
}
