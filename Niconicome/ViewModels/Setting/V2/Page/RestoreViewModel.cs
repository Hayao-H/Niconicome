using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.DataBackup;
using Niconicome.Models.Utils.Reactive;
using Niconicome.ViewModels.Shared;
using WS = Niconicome.Workspaces.SettingPageV2;
using SC = Niconicome.ViewModels.Setting.V2.Page.StringContent.RestoreViewModelStringContent;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Utils.Error;
using System.Threading.Tasks.Sources;
using Windows.Devices.Printers;

namespace Niconicome.ViewModels.Setting.V2.Page
{
    public class RestoreViewModel : AlertViewModel, IDisposable
    {
        public RestoreViewModel()
        {
            WS.RestoreManager.Initialize();

            this.Bindables.Add(this._alertBindables);

            this.Backups = new BindableCollection<BackupViewModel, IBackupData>(WS.RestoreManager.Backups, b =>
            {
                var vm = new BackupViewModel(b);
                this.Bindables.Add(vm.IsChecked);
                return vm;
            });
            this.Bindables.Add(this.Backups);
            this.VideoDirectories = new BindableCollection<VideoDirectoryViewModel, string>(WS.RestoreManager.VideoFileDirectories, d =>
            {
                var vm = new VideoDirectoryViewModel(d);
                this.Bindables.Add(vm.IsChecked);
                return vm;
            });
            this.Bindables.Add(this.VideoDirectories);

            this.BackupNameInput = new BindableProperty<string>(string.Empty).AddTo(this.Bindables);
            this.VideoDirectoryPathInput = new BindableProperty<string>(string.Empty).AddTo(this.Bindables);
            this.IsProcessing = WS.RestoreManager.IsApplyingBackupProcessing.AsReadOnly().AddTo(this.Bindables).AddTo(this._disposable);
            this.IsVideoDirProcessing = WS.RestoreManager.IsGettingVideosProcessing.AsReadOnly().AddTo(this.Bindables).AddTo(this._disposable);
            this.IsDataCleaningProcessing = WS.RestoreManager.IsDataCleaningProcessing.AsReadOnly().AddTo(this.Bindables).AddTo(this._disposable);
        }

        #region field

        private readonly Disposable _disposable = new();

        #endregion

        #region Props

        /// <summary>
        /// 変更監視オブジェクト
        /// </summary>
        public Bindables Bindables { get; init; } = new();

        /// <summary>
        /// バックアップの一覧
        /// </summary>
        public BindableCollection<BackupViewModel, IBackupData> Backups { get; init; }

        /// <summary>
        /// 動画ディレクトリの一覧
        /// </summary>
        public BindableCollection<VideoDirectoryViewModel, string> VideoDirectories { get; init; }

        /// <summary>
        /// バックアップ名
        /// </summary>
        public IBindableProperty<string> BackupNameInput { get; init; }

        /// <summary>
        /// 動画ディレクトリのパス
        /// </summary>
        public IBindableProperty<string> VideoDirectoryPathInput { get; init; }

        /// <summary>
        /// 処理中フラグ
        /// </summary>
        public IReadonlyBindablePperty<bool> IsProcessing { get; init; }

        /// <summary>
        /// 処理中フラグ
        /// </summary>
        public IReadonlyBindablePperty<bool> IsVideoDirProcessing { get; init; }

        /// <summary>
        /// クリーニング中フラグ
        /// </summary>
        public IReadonlyBindablePperty<bool> IsDataCleaningProcessing { get; init; }

        #endregion

        #region Method

        public void OnAddBackupButtonClick()
        {
            if (string.IsNullOrEmpty(this.BackupNameInput.Value))
            {
                this.ShowAlert(WS.StringHandler.GetContent(SC.InputIsEmpty), AlertType.Error);
                return;
            }

            IAttemptResult result = WS.RestoreManager.CreateBackup(this.BackupNameInput.Value);
            if (result.IsSucceeded)
            {
                string message = WS.StringHandler.GetContent(SC.BackupCreationSucceeded);
                WS.MessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher);
                this.ShowAlert(message, AlertType.Info);
            }
            else
            {
                string message = WS.StringHandler.GetContent(SC.BackupCreationFailed);
                string detail = WS.StringHandler.GetContent(SC.DetailInfo, result.Message);
                WS.MessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher, ErrorLevel.Error);
                WS.MessageHandler.AppendMessage(detail, LocalConstant.SystemMessageDispacher, ErrorLevel.Error);
                this.ShowAlert(message, AlertType.Error);
            }

            this.BackupNameInput.Value = string.Empty;
        }

        public void OnDeleteBackupButtonClick()
        {
            foreach (var backup in this.Backups)
            {
                if (!backup.IsChecked.Value)
                {
                    continue;
                }

                IAttemptResult result = WS.RestoreManager.RemoveBackup(backup.ID);
                if (result.IsSucceeded)
                {
                    string message = WS.StringHandler.GetContent(SC.BackupDeletionSucceeded);
                    WS.MessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher);
                    this.ShowAlert(message, AlertType.Info);
                }
                else
                {
                    string message = WS.StringHandler.GetContent(SC.BackupDeletionFailed);
                    string detail = WS.StringHandler.GetContent(SC.DetailInfo, result.Message);
                    WS.MessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher, ErrorLevel.Error);
                    WS.MessageHandler.AppendMessage(detail, LocalConstant.SystemMessageDispacher, ErrorLevel.Error);
                    this.ShowAlert(message, AlertType.Error);
                }
            }
        }

        public async Task OnApplyBackupButtonClickAsync()
        {
            string? backupID = this.Backups.FirstOrDefault(b => b.IsChecked.Value)?.ID;
            if (backupID is null)
            {
                this.ShowAlert(WS.StringHandler.GetContent(SC.TargetIsNotSelected), AlertType.Error);
                return;
            }

            IAttemptResult result = await WS.RestoreManager.ApplyBackupAsync(backupID);
            if (result.IsSucceeded)
            {
                string message = WS.StringHandler.GetContent(SC.BackupApplyingSuceeded);
                WS.MessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher);
                this.ShowAlert(message, AlertType.Info);
            }
            else
            {
                string message = WS.StringHandler.GetContent(SC.BackupApplyingFailed);
                string detail = WS.StringHandler.GetContent(SC.DetailInfo, result.Message);
                WS.MessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher, ErrorLevel.Error);
                WS.MessageHandler.AppendMessage(detail, LocalConstant.SystemMessageDispacher, ErrorLevel.Error);
                this.ShowAlert(message, AlertType.Error);
            }

        }

        public async Task OnAddVideoDirectoryButtonClickAsync()
        {

            if (string.IsNullOrEmpty(this.VideoDirectoryPathInput.Value))
            {
                this.ShowAlert(WS.StringHandler.GetContent(SC.InputIsEmpty), AlertType.Error);
                return;
            }

            IAttemptResult<int> result = await WS.RestoreManager.AddVideoDirectoryAsync(this.VideoDirectoryPathInput.Value);
            if (result.IsSucceeded)
            {
                string message = WS.StringHandler.GetContent(SC.AddingVideoDirectorySucceeded, result.Data);
                WS.MessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher);
                this.ShowAlert(message, AlertType.Info);
            }
            else
            {
                string message = WS.StringHandler.GetContent(SC.AddingVideoDirectoryFailed);
                string detail = WS.StringHandler.GetContent(SC.DetailInfo, result.Message);
                WS.MessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher, ErrorLevel.Error);
                WS.MessageHandler.AppendMessage(detail, LocalConstant.SystemMessageDispacher, ErrorLevel.Error);
                this.ShowAlert(message, AlertType.Error);
            }

            this.VideoDirectoryPathInput.Value = string.Empty;
        }

        public void OnDeleteVideoDirButtonClick()
        {
            foreach (var d in this.VideoDirectories)
            {
                if (!d.IsChecked.Value)
                {
                    continue;
                }

                IAttemptResult result = WS.RestoreManager.DeleteVideoDirectory(d.Path);
                if (result.IsSucceeded)
                {
                    string message = WS.StringHandler.GetContent(SC.DeletingVideoDirectorySucceeded);
                    WS.MessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher);
                    this.ShowAlert(message, AlertType.Info);
                }
                else
                {
                    string message = WS.StringHandler.GetContent(SC.DeletingVideoDirectoryFailed);
                    string detail = WS.StringHandler.GetContent(SC.DetailInfo, result.Message);
                    WS.MessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher, ErrorLevel.Error);
                    WS.MessageHandler.AppendMessage(detail, LocalConstant.SystemMessageDispacher, ErrorLevel.Error);
                    this.ShowAlert(message, AlertType.Error);
                }
            }
        }

        public async Task OnLoadVideosButtonClickedASync()
        {
            IAttemptResult<int> result = await WS.RestoreManager.GetVideosFromVideoDirectoryAsync();
            if (result.IsSucceeded)
            {
                string message = WS.StringHandler.GetContent(SC.LoadingVideosSucceeded, result.Data);
                WS.MessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher);
                this.ShowAlert(message, AlertType.Info);
            }
            else
            {
                string message = WS.StringHandler.GetContent(SC.LoadingVideosFailed);
                string detail = WS.StringHandler.GetContent(SC.DetailInfo, result.Message);
                WS.MessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher, ErrorLevel.Error);
                WS.MessageHandler.AppendMessage(detail, LocalConstant.SystemMessageDispacher, ErrorLevel.Error);
                this.ShowAlert(message, AlertType.Error);
            }
        }

        public async Task OnCleanDataButtonClickedAsync()
        {
            if (this.IsDataCleaningProcessing.Value)
            {
                return;
            }

            IAttemptResult result = await WS.RestoreManager.CleanDataAsync();
            if (result.IsSucceeded)
            {
                string message = WS.StringHandler.GetContent(SC.DataCleaningSucceeded);
                WS.MessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher);
                this.ShowAlert(message, AlertType.Info);
            }
            else
            {
                string message = WS.StringHandler.GetContent(SC.DataCleaningFailed);
                string detail = WS.StringHandler.GetContent(SC.DetailInfo,result.Message);
                this.ShowAlert(message, AlertType.Error);
                WS.MessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher, ErrorLevel.Error);
                WS.MessageHandler.AppendMessage(detail, LocalConstant.SystemMessageDispacher, ErrorLevel.Error);
            }
        }

        public void OnResetDataButtonClick()
        {
            IAttemptResult result = WS.RestoreManager.DeleteAllVideosAndPlaylists();
            if (result.IsSucceeded)
            {
                string message = WS.StringHandler.GetContent(SC.DataResetSucceeded);
                WS.MessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher);
                this.ShowAlert(message, AlertType.Info);
            }
            else
            {
                string message = WS.StringHandler.GetContent(SC.DataResetFailed);
                string detail = WS.StringHandler.GetContent(SC.DetailInfo, result.Message);
                this.ShowAlert(message, AlertType.Error);
                WS.MessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher, ErrorLevel.Error);
                WS.MessageHandler.AppendMessage(detail, LocalConstant.SystemMessageDispacher, ErrorLevel.Error);
            }
        }

        public void OnResetSettingButtonClick()
        {
            IAttemptResult result = WS.RestoreManager.ResetSettings();
            if (result.IsSucceeded)
            {
                string message = WS.StringHandler.GetContent(SC.SettingResetSucceeded);
                WS.MessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher);
                this.ShowAlert(message, AlertType.Info);
            }
            else
            {
                string message = WS.StringHandler.GetContent(SC.SettingResetFailed);
                string detail = WS.StringHandler.GetContent(SC.DetailInfo, result.Message);
                this.ShowAlert(message, AlertType.Error);
                WS.MessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher, ErrorLevel.Error);
                WS.MessageHandler.AppendMessage(detail, LocalConstant.SystemMessageDispacher, ErrorLevel.Error);
            }
        }

        #endregion

        public void Dispose()
        {
            this._disposable.Dispose();
            this.Bindables.Dispose();
            this.Backups.Dispose();
            this.VideoDirectories.Dispose();
        }
    }

    public class BackupViewModel
    {
        public BackupViewModel(IBackupData data)
        {
            this.ID = data.GUID;
            this.Name = data.Name;
            this.CreatedOn = data.CreatedOn.ToString("yyyy/MM/dd HH:mm");
            this.FileSize = $"{data.FileSize:#,0}kB";
        }

        public string ID { get; init; }

        public string Name { get; init; }

        public string CreatedOn { get; init; }

        public string FileSize { get; init; }

        public IBindableProperty<bool> IsChecked { get; set; } = new BindableProperty<bool>(false);

    }

    public class VideoDirectoryViewModel
    {
        public VideoDirectoryViewModel(string path)
        {
            this.Path = path;
        }

        public string Path { get; init; }

        public IBindableProperty<bool> IsChecked { get; set; } = new BindableProperty<bool>(false);
    }
}
