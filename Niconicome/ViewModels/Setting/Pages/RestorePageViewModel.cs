using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Niconicome.Extensions;
using Niconicome.Extensions.System;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Local;
using Niconicome.ViewModels.Controls;
using MD = MaterialDesignThemes.Wpf;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Setting.Pages
{
    class RestorePageViewModel : BindableBase
    {
        public RestorePageViewModel(Func<string, MessageBoxButtons, MessageBoxIcons, Task<MaterialMessageBoxResult>> showMessage)
        {
            this.showMessage = showMessage;
            this.Backups = new ObservableCollection<IBackupData>();
            this.Backups.Addrange(WS::SettingPage.Restore.GetAllBackups());
            this.SnackbarMessageQueue = WS::SettingPage.SnackbarMessageQueue;

            this.VideoDirectories = new ObservableCollection<string>(WS::SettingPage.Restore.GetAllVideoDirectories());

            this.CreatebackupCommand = new CommandBase<object>(_ => true, _ =>
            {
                if (this.BackupName.IsNullOrEmpty()) return;
                var result = WS::SettingPage.Restore.TryCreateBackup(this.BackupName);
                if (result)
                {
                    this.Backups.Clear();
                    this.Backups.Addrange(WS::SettingPage.Restore.GetAllBackups());
                    this.SnackbarMessageQueue.Enqueue($"バックアップ「{this.BackupName}」を作成しました。");
                    this.BackupName = string.Empty;
                }
                else
                {
                    this.SnackbarMessageQueue.Enqueue("バックアップの作成に失敗しました。");
                }
            });

            this.ResetSettingsCommand = new CommandBase<object>(_ => true, async _ =>
            {
                var confirm = await this.showMessage("本当に設定を削除しますか？この操作は元に戻すことができません。", MessageBoxButtons.Yes | MessageBoxButtons.No, MessageBoxIcons.Question);
                if (confirm != MaterialMessageBoxResult.Yes) return;
                WS::SettingPage.Restore.ResetSettings();
                this.SnackbarMessageQueue.Enqueue("設定をリセットしました。");
            });

            this.RemovebackupCommand = new CommandBase<IBackupData>(_ => true, arg =>
            {
                if (arg is null) return;
                if (arg.AsNullable<IBackupData>() is not IBackupData backup || backup is null) return;

                bool result = WS::SettingPage.Restore.TryRemoveBackup(backup.GUID);

                if (result)
                {
                    this.SnackbarMessageQueue.Enqueue($"バックアップを削除しました。");
                    this.Backups.Clear();
                    this.Backups.Addrange(WS::SettingPage.Restore.GetAllBackups());
                }
                else
                {
                    this.SnackbarMessageQueue.Enqueue($"バックアップ「{backup.Name}」の削除に失敗しました。");
                }

            });

            this.ApplyBackupCommand = new CommandBase<IBackupData>(_ => true, async arg =>
             {

                 if (arg is null) return;
                 if (arg.AsNullable<IBackupData>() is not IBackupData backup || backup is null) return;

                 var confirm = await this.showMessage("本当にこのバックアップを適用しますか？現在の設定は全て削除され、操作は元に戻すことができません。", MessageBoxButtons.Yes | MessageBoxButtons.No | MessageBoxButtons.Cancel, MessageBoxIcons.Question);
                 if (confirm != MaterialMessageBoxResult.Yes) return;

                 bool result = WS::SettingPage.Restore.TryApplyBackup(backup.GUID);

                 if (result)
                 {
                     this.SnackbarMessageQueue.Enqueue($"バックアップを適用しました。");
                 }
                 else
                 {
                     this.SnackbarMessageQueue.Enqueue($"バックアップの適用に失敗しました。");
                 }
             });

            this.ResetDataCommand = new CommandBase<object>(_ => true, async _ =>
              {

                  var confirm = await this.showMessage("本当に全ての動画・プレイリストを削除しますか？この操作は元に戻すことができません。", MessageBoxButtons.Yes | MessageBoxButtons.No | MessageBoxButtons.Cancel, MessageBoxIcons.Question);
                  if (confirm != MaterialMessageBoxResult.OK) return;
                  WS::SettingPage.Restore.DeleteAllVideosAndPlaylists();
                  this.SnackbarMessageQueue.Enqueue("データをリセットしました。");
                  WS::SettingPage.PlaylistTreeHandler.Refresh();
                  WS::SettingPage.PlaylistTreeHandler.Refresh();
              });

            this.LoadSavedFiles = new CommandBase<object>(_ => true, _ =>
            {
                WS::SettingPage.Restore.JustifySavedFilePaths();
            });

            this.AddVideoDirCommand = new CommandBase<object>(_ => true, _ =>
            {
                if (this.VIdeoDir.IsNullOrEmpty())
                {
                    this.SnackbarMessageQueue.Enqueue("パスを指定してください。");
                    return;
                }

                if (!Directory.Exists(this.VIdeoDir))
                {
                    this.SnackbarMessageQueue.Enqueue("そのようなディレクトリーは存在しません。");
                    return;
                }

                WS::SettingPage.Restore.AddVideoDirectory(this.VIdeoDir);
                this.VideoDirectories.Add(this.VIdeoDir);
                this.VIdeoDir = string.Empty;
            });

            this.DeleteVideoDirectoryCommand = new CommandBase<string>(_ => true, arg =>
            {
                if (arg is null or not string) return;
                WS::SettingPage.Restore.DeleteVideoDirectory(arg);
                this.VideoDirectories.Clear();
                this.VideoDirectories.Addrange(WS::SettingPage.Restore.GetAllVideoDirectories());
            });
        }


        public RestorePageViewModel() : this((message, button, image) => MaterialMessageBox.Show(message, button, image))
        {
        }

        private string backupNameField = string.Empty;

        private string videodirField = string.Empty;

        /// <summary>
        /// 保存ディレクトリー名
        /// </summary>
        public string VIdeoDir { get => this.videodirField; set => this.SetProperty(ref this.videodirField, value); }

        /// <summary>
        /// バックアップ名
        /// </summary>
        public string BackupName { get => this.backupNameField; set => this.SetProperty(ref this.backupNameField, value); }

        /// <summary>
        /// バックアップを作成する
        /// </summary>
        public CommandBase<object> CreatebackupCommand { get; init; }

        /// <summary>
        /// 設定をリセット
        /// </summary>
        public CommandBase<object> ResetSettingsCommand { get; init; }

        /// <summary>
        /// バックアップ一覧
        /// </summary>
        public ObservableCollection<IBackupData> Backups { get; init; }

        /// <summary>
        /// バックアップを削除
        /// </summary>
        public CommandBase<IBackupData> RemovebackupCommand { get; init; }

        /// <summary>
        /// バックアップを適用
        /// </summary>
        public CommandBase<IBackupData> ApplyBackupCommand { get; init; }

        /// <summary>
        /// データをリセット
        /// </summary>
        public CommandBase<object> ResetDataCommand { get; init; }

        /// <summary>
        /// 保存したファイルを再読み込み
        /// </summary>
        public CommandBase<object> LoadSavedFiles { get; init; }

        /// <summary>
        /// 保存ディレクトリを追加
        /// </summary>
        public CommandBase<object> AddVideoDirCommand { get; init; }

        /// <summary>
        /// 保存ディレクトリを削除
        /// </summary>
        public CommandBase<string> DeleteVideoDirectoryCommand { get; init; }

        /// <summary>
        /// 保存フォルダー
        /// </summary>
        public ObservableCollection<string> VideoDirectories { get; init; }

        /// <summary>
        /// メッセージキュー
        /// </summary>
        public MD::ISnackbarMessageQueue SnackbarMessageQueue { get; init; }

        /// <summary>
        /// メッセージボックス
        /// </summary>
        private readonly Func<string, MessageBoxButtons, MessageBoxIcons, Task<MaterialMessageBoxResult>> showMessage;
    }
}
