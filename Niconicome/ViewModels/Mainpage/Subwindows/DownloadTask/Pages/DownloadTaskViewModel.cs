using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Network.Download.DLTask;
using Niconicome.Models.Utils.Reactive;

namespace Niconicome.ViewModels.Mainpage.Subwindows.DownloadTask.Pages
{
    public interface IDownloadTaskViewModel
    {
        /// <summary>
        /// タスク
        /// </summary>
        IDownloadTask Task { get; }

        /// <summary>
        /// タイトル
        /// </summary>
        string Title { get; }

        /// <summary>
        /// ID
        /// </summary>
        string NiconicoID { get; }

        /// <summary>
        /// 保存先
        /// </summary>
        string DirectoryPath { get; set; }

        /// <summary>
        /// ファイル名のフォーマット
        /// </summary>
        string FileNameFormat { get; set; }

        /// <summary>
        /// 解像度
        /// </summary>
        uint Resolution { get; set; }

        /// <summary>
        /// 選択フラグ
        /// </summary>
        IBindableProperty<bool> IsSelected { get; }

        /// <summary>
        /// 状態
        /// </summary>
        IReadonlyBindablePperty<DLTaskStatus> Status { get; }

        /// <summary>
        /// メッセージ
        /// </summary>
        IReadonlyBindablePperty<string> Message { get; }

        /// <summary>
        /// メッセージ(フル)
        /// </summary>
        IReadOnlyCollection<string> FullMessage { get; }

        /// <summary>
        /// 変更監視オブジェクト
        /// </summary>
        Bindables Bindables { get; }

        /// <summary>
        /// キャンセル
        /// </summary>
        void Cancel();
    }

    public enum DLTaskStatus
    {
        Pending,
        Processing,
        Succeeded,
        Failed,
        Canceled,
    }

    public class DownloadTaskViewModel : IDownloadTaskViewModel
    {
        public DownloadTaskViewModel(IDownloadTask task)
        {
            this.Task = task;
            this.Task.IsCanceled.Subscribe(_ => this.SetStatus());
            this.Task.IsProcessing.Subscribe(_ => this.SetStatus());
            this.Task.IsCompleted.Subscribe(_ => this.SetStatus());

            this.SetStatus();

            this.Bindables.Add(task.Bindables);

            this.FullMessage = task.FullMessage;
            this.Message = task.Message.AddTo(this.Bindables);
            this.Status = this._status.AsReadOnly().AddTo(this.Bindables);

            this.IsSelected = new BindableProperty<bool>(false).AddTo(this.Bindables);
        }

        #region field

        private readonly IBindableProperty<DLTaskStatus> _status = new BindableProperty<DLTaskStatus>(DLTaskStatus.Pending);

        #endregion

        #region Props

        public IDownloadTask Task { get; init; }


        public string Title => this.Task.Title;

        public string NiconicoID => this.Task.NiconicoID;

        public string DirectoryPath
        {
            get => this.Task.DirectoryPath;
            set => this.Task.DirectoryPath = value;
        }

        public string FileNameFormat
        {
            get => this.Task.FileNameFormat;
            set => this.Task.FileNameFormat = value;
        }

        public uint Resolution
        {
            get => this.Task.Resolution;
            set => this.Task.Resolution = value;
        }

        public IReadonlyBindablePperty<DLTaskStatus> Status { get; init; }

        public IReadonlyBindablePperty<string> Message { get; init; }

        public IReadOnlyCollection<string> FullMessage { get; init; }

        public IBindableProperty<bool> IsSelected { get; init; }

        public Bindables Bindables { get; init; } = new();

        #endregion

        #region Method

        public void Cancel()
        {
            this.Task.Cancel();
        }

        #endregion

        private void SetStatus()
        {
            if (this.Task.IsCanceled.Value)
            {
                this._status.Value = DLTaskStatus.Canceled;
            }
            else if (this.Task.IsCompleted.Value && this.Task.IsSuceeded)
            {
                this._status.Value = DLTaskStatus.Succeeded;
            }
            else if (this.Task.IsCompleted.Value && !this.Task.IsSuceeded)
            {
                this._status.Value = DLTaskStatus.Failed;
            }
            else if (this.Task.IsProcessing.Value)
            {
                this._status.Value = DLTaskStatus.Processing;
            }
            else
            {
                this._status.Value = DLTaskStatus.Pending;
            }
        }
    }

}
