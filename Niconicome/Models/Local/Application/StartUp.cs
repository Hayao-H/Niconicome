using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Niconicome.Models.Auth;
using Niconicome.Models.Domain.Utils;
using Resume = Niconicome.Models.Domain.Niconico.Download.Video.Resume;
using NicoIO = Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Local.Addon;
using Niconicome.Models.Utils.InitializeAwaiter;
using Niconicome.Models.Local.Addon.V2;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Utils.NicoLogger;
using Niconicome.Models.Local.State.Toast;
using Niconicome.Models.Domain.Local.DataBackup;
using Niconicome.Models.Domain.Local.Server.Core;
using Niconicome.Models.Domain.Niconico.Download.Video.V2.Local.HLS;
using System.Collections;
using System.Collections.Generic;

namespace Niconicome.Models.Local.Application
{

    public interface IStartUp
    {
        void RunStartUptasks();
        event EventHandler? AutoLoginSucceeded;
    }

    class StartUp : IStartUp
    {

        public StartUp(IBackupManager backuphandler, IAutoLogin autoLogin, IToastHandler snackbarHandler, ILogger logger, Resume::IStreamResumer streamResumer, NicoIO::INicoDirectoryIO nicoDirectoryIO, IAddonManager addonManager, IAddonInstallManager installManager, ISettingsContainer settingsConainer, IServer server,ISegmentDirectoryHandler segmentDirectoryHandler)
        {
            this._backuphandler = backuphandler;
            this._autoLogin = autoLogin;
            this._snackbarHandler = snackbarHandler;
            this._logger = logger;
            this._streamResumer = streamResumer;
            this._nicoDirectoryIO = nicoDirectoryIO;
            this._addonManager = addonManager;
            this._installManager = installManager;
            this._settingsConainer = settingsConainer;
            this._server = server;
            this._segmentDirectoryHandler = segmentDirectoryHandler;
            this.DeleteInvalidbackup();
        }

        #region field

        private readonly IBackupManager _backuphandler;

        private readonly IAutoLogin _autoLogin;

        private readonly IToastHandler _snackbarHandler;

        private readonly ILogger _logger;

        private readonly Resume::IStreamResumer _streamResumer;

        private readonly NicoIO::INicoDirectoryIO _nicoDirectoryIO;

        private readonly IAddonManager _addonManager;

        private readonly IAddonInstallManager _installManager;

        private readonly ISettingsContainer _settingsConainer;

        private readonly IServer _server;

        private readonly ISegmentDirectoryHandler _segmentDirectoryHandler;

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
                this.StartServer();
                this.RemoveTmpFolder();
                await this.LoadAddon();
                await this.Autologin();
            });
        }

        /// <summary>
        /// 一時フォルダーを削除する
        /// </summary>
        private void RemoveTmpFolder()
        {
            if (this._nicoDirectoryIO.Exists(FileFolder.SegmentsFolderPath))
            {
                IAttemptResult<ISettingInfo<int>> settingResult = this._settingsConainer.GetSetting<int>(SettingNames.MaxTmpSegmentsDirCount, 20);
                if (!settingResult.IsSucceeded || settingResult.Data is null) return;

                int maxTmp = settingResult.Data.Value;

                if (maxTmp < 0) maxTmp = 20;

                IAttemptResult<IEnumerable<ISegmentDirectoryInfo>> result = this._segmentDirectoryHandler.GetAllSegmentDirectoryInfos();
                if (!result.IsSucceeded||result.Data is null)
                {
                    return;
                }

                var infos = result.Data.OrderBy(i => i.DownloadStartedOn).ToList();

                if (infos.Count <= maxTmp) return;

                foreach (var i in Enumerable.Range(0, infos.Count - maxTmp))
                {
                    this._nicoDirectoryIO.Delete(infos[i].DirectoryPath);
                }

            }
        }

        /// <summary>
        /// 存在しないバックアップを削除する
        /// </summary>
        private void DeleteInvalidbackup()
        {
            this._backuphandler.Clean();
        }

        /// <summary>
        /// ローカルサーバーを起動する
        /// </summary>
        private void StartServer()
        {
            this._server.Start();
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
        private async Task LoadAddon()
        {
            this._addonManager.InitializeAddons();
            await this._installManager.InstallEssensialAddons();
            await this._addonManager.CheckForUpdates();
        }
    }
}
