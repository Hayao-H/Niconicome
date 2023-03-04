using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Niconicome.Extensions;
using Niconicome.Extensions.System;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Domain.Local.DataBackup;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Utils.Reactive;
using Niconicome.Models.Utils.Reactive.Command;
using Niconicome.ViewModels.Controls;
using Niconicome.ViewModels.Setting.Pages.String;
using MD = MaterialDesignThemes.Wpf;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Setting.Pages
{
    public class RestorePageViewModel : BindableBase
    {
        public RestorePageViewModel(Func<string, MessageBoxButtons, MessageBoxIcons, Task<MaterialMessageBoxResult>> showMessage)
        {
            WS::SettingPage.Restore.Initialize();

            this.showMessage = showMessage;

            this.Backups = new BindableCollection<BackupDataViewModel, IBackupData>(WS::SettingPage.Restore.Backups, b => new BackupDataViewModel(b));

            this.SnackbarMessageQueue = WS::SettingPage.SnackbarMessageQueue;
            this.VideoDirectories = new BindableCollection<string, string>(WS::SettingPage.Restore.VideoFileDirectories, x => x);

            this.CreatebackupCommand = new BindableCommand(() =>
            {
                if (this.BackupNameInput.Value.IsNullOrEmpty()) return;

                var result = WS::SettingPage.Restore.CreateBackup(this.BackupNameInput.Value);
                if (result.IsSucceeded)
                {
                    this.SnackbarMessageQueue.Enqueue(WS::SettingPage.StringHandler.GetContent(RestorePageVMStringContent.BackupCreated, this.BackupNameInput.Value));
                    this.BackupNameInput.Value = string.Empty;
                }
                else
                {
                    this.SnackbarMessageQueue.Enqueue(WS::SettingPage.StringHandler.GetContent(RestorePageVMStringContent.FailedToCreateBackup));
                }
            }, new BindableProperty<bool>(true));

            this.ResetSettingsCommand = new BindableCommand(async () =>
            {
                var confirm = await this.showMessage(WS::SettingPage.StringHandler.GetContent(RestorePageVMStringContent.ConfirmMessageOfResetSetting), MessageBoxButtons.Yes | MessageBoxButtons.No, MessageBoxIcons.Question);
                if (confirm != MaterialMessageBoxResult.Yes) return;

                IAttemptResult result = WS::SettingPage.Restore.ResetSettings();
                if (result.IsSucceeded)
                {
                    this.SnackbarMessageQueue.Enqueue(WS::SettingPage.StringHandler.GetContent(RestorePageVMStringContent.ResetOfSettingHasCompleted));
                }
            }, new BindableProperty<bool>(true));

            this.RemovebackupCommand = new BindableCommand<BackupDataViewModel>(vm =>
            {
                if (vm.Backup.GUID.IsNullOrEmpty()) return;

                IAttemptResult result = WS::SettingPage.Restore.RemoveBackup(vm.Backup.GUID);

                if (result.IsSucceeded)
                {
                    this.SnackbarMessageQueue.Enqueue(WS::SettingPage.StringHandler.GetContent(RestorePageVMStringContent.BackupDeleted));
                }
                else
                {
                    this.SnackbarMessageQueue.Enqueue(WS::SettingPage.StringHandler.GetContent(RestorePageVMStringContent.FailedToDeleteBackup));
                }

            }, new BindableProperty<bool>(true));

            this.ApplyBackupCommand = new BindableCommand<BackupDataViewModel>(async vm =>
             {

                 if (vm.Backup.GUID.IsNullOrEmpty()) return;

                 var confirm = await this.showMessage(WS::SettingPage.StringHandler.GetContent(RestorePageVMStringContent.ConfirmMessageOfApplyingBackup), MessageBoxButtons.Yes | MessageBoxButtons.No | MessageBoxButtons.Cancel, MessageBoxIcons.Question);
                 if (confirm != MaterialMessageBoxResult.Yes) return;

                 IAttemptResult result = WS::SettingPage.Restore.ApplyBackup(vm.Backup.GUID);

                 if (result.IsSucceeded)
                 {
                     this.SnackbarMessageQueue.Enqueue(WS::SettingPage.StringHandler.GetContent(RestorePageVMStringContent.BackupApplyed), WS::SettingPage.StringHandler.GetContent(RestorePageVMStringContent.Restart), () => WS::SettingPage.PowerManager.Restart());
                 }
                 else
                 {
                     this.SnackbarMessageQueue.Enqueue(WS::SettingPage.StringHandler.GetContent(RestorePageVMStringContent.FailedToApplyBackup));
                 }
             }, this._isBackupProcessing);

            this.ResetDataCommand = new BindableCommand(async () =>
              {

                  var confirm = await this.showMessage(WS::SettingPage.StringHandler.GetContent(RestorePageVMStringContent.ConfirmMessageOfDeletingAllData), MessageBoxButtons.Yes | MessageBoxButtons.No | MessageBoxButtons.Cancel, MessageBoxIcons.Question);
                  if (confirm != MaterialMessageBoxResult.Yes) return;

                  IAttemptResult result = WS::SettingPage.Restore.DeleteAllVideosAndPlaylists();
                  if (result.IsSucceeded)
                  {
                      this.SnackbarMessageQueue.Enqueue(WS::SettingPage.StringHandler.GetContent(RestorePageVMStringContent.DeletingAllDataHasCompleted));
                      WS::SettingPage.PlaylistTreeHandler.Refresh();
                  }
              }, new BindableProperty<bool>(true));

            this.LoadSavedFiles = new BindableCommand(async () =>
            {
                await WS::SettingPage.Restore.GetVideosFromVideoDirectoryAsync();
            }, this._isLoadVideosProcessing);

            this.AddVideoDirCommand = new BindableCommand(async () =>
            {
                if (this.VIdeoDirInput.Value.IsNullOrEmpty())
                {
                    return;
                }

                await WS::SettingPage.Restore.AddVideoDirectoryAsync(this.VIdeoDirInput.Value);
                this.VIdeoDirInput.Value = string.Empty;
            }, new BindableProperty<bool>(true));

            this.DeleteVideoDirectoryCommand = new BindableCommand<string>(arg =>
            {
                if (arg.IsNullOrEmpty()) return;

                WS::SettingPage.Restore.DeleteVideoDirectory(arg);
            }, new BindableProperty<bool>(true));
        }


        public RestorePageViewModel() : this((message, button, image) => MaterialMessageBox.Show(message, button, image))
        {
        }

        private readonly Func<string, MessageBoxButtons, MessageBoxIcons, Task<MaterialMessageBoxResult>> showMessage;

        private readonly IBindableProperty<bool> _isBackupProcessing = WS::SettingPage.Restore.IsApplyingBackupProcessing;

        private readonly IBindableProperty<bool> _isLoadVideosProcessing = WS::SettingPage.Restore.IsGettingVideosProcessing;

        #region Props

        /// <summary>
        /// 保存ディレクトリー名
        /// </summary>
        public IBindableProperty<string> VIdeoDirInput { get; init; } = new BindableProperty<string>(string.Empty);

        /// <summary>
        /// バックアップ名
        /// </summary>
        public IBindableProperty<string> BackupNameInput { get; init; } = new BindableProperty<string>(string.Empty);

        /// <summary>
        /// メッセージキュー
        /// </summary>
        public MD::ISnackbarMessageQueue SnackbarMessageQueue { get; init; }

        /// <summary>
        /// 保存フォルダー
        /// </summary>
        public BindableCollection<string, string> VideoDirectories { get; init; }

        /// <summary>
        /// バックアップ一覧
        /// </summary>
        public BindableCollection<BackupDataViewModel, IBackupData> Backups { get; init; }

        #endregion

        #region Command

        /// <summary>
        /// バックアップを作成する
        /// </summary>
        public IBindableCommand CreatebackupCommand { get; init; }

        /// <summary>
        /// 設定をリセット
        /// </summary>
        public IBindableCommand ResetSettingsCommand { get; init; }

        /// <summary>
        /// バックアップを削除
        /// </summary>
        public IBindableCommand<BackupDataViewModel> RemovebackupCommand { get; init; }

        /// <summary>
        /// バックアップを適用
        /// </summary>
        public IBindableCommand<BackupDataViewModel> ApplyBackupCommand { get; init; }

        /// <summary>
        /// データをリセット
        /// </summary>
        public IBindableCommand ResetDataCommand { get; init; }

        /// <summary>
        /// 保存したファイルを再読み込み
        /// </summary>
        public IBindableCommand LoadSavedFiles { get; init; }

        /// <summary>
        /// 保存ディレクトリを追加
        /// </summary>
        public IBindableCommand AddVideoDirCommand { get; init; }

        /// <summary>
        /// 保存ディレクトリを削除
        /// </summary>
        public IBindableCommand<string> DeleteVideoDirectoryCommand { get; init; }

        #endregion
    }

    [Obsolete("For Design Only")]
    public class RestorePageViewModelD : BindableBase
    {
        public RestorePageViewModelD()
        {
            this.VIdeoDirInput = new BindableProperty<string>(string.Empty);
            this.BackupNameInput = new BindableProperty<string>(string.Empty);
            this.SnackbarMessageQueue = new MD::SnackbarMessageQueue();
            this.VideoDirectories = new BindableCollection<string, string>(new ObservableCollection<string>(), x => x);
            this.Backups = new BindableCollection<BackupDataViewModel, IBackupData>(new ObservableCollection<IBackupData>(), x => new BackupDataViewModel(x));
        }

        #region Props

        public IBindableProperty<string> VIdeoDirInput { get; init; }

        public IBindableProperty<string> BackupNameInput { get; init; }

        public MD::ISnackbarMessageQueue SnackbarMessageQueue { get; init; }

        public BindableCollection<string, string> VideoDirectories { get; init; }

        public BindableCollection<BackupDataViewModel, IBackupData> Backups { get; init; }

        #endregion

        #region Command

        public IBindableCommand? CreatebackupCommand { get; init; }

        public IBindableCommand? ResetSettingsCommand { get; init; }

        public IBindableCommand<string>? RemovebackupCommand { get; init; }

        public IBindableCommand<string>? ApplyBackupCommand { get; init; }

        public IBindableCommand? ResetDataCommand { get; init; }

        public IBindableCommand? LoadSavedFiles { get; init; }

        public IBindableCommand? AddVideoDirCommand { get; init; }

        public IBindableCommand<string>? DeleteVideoDirectoryCommand { get; init; }

        #endregion
    }

    public class BackupDataViewModel
    {
        public BackupDataViewModel(IBackupData backup)
        {
            this._backup = backup;
        }

        private readonly IBackupData _backup;

        public IBackupData Backup => this._backup;

        public string Name => this._backup.Name;

        public string FileSize => $"{this._backup.FileSize}kB";

        public string CatedOn => this._backup.CreatedOn.ToString("yyyy/MM/dd HH:mm");
    }
}
