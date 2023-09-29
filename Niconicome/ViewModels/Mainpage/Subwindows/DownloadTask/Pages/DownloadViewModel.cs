using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Network.Download.DLTask;
using Niconicome.Models.Utils.Reactive;
using WS = Niconicome.Workspaces.Mainpage;

namespace Niconicome.ViewModels.Mainpage.Subwindows.DownloadTask.Pages
{
    public class DownloadViewModel : ViewModelBase
    {
        public DownloadViewModel() : base(true)
        {
            this.TaskInfoViewModel = new TaskInfoViewModel();
            this.Bindables.Add(this.TaskInfoViewModel.Bindables);
        }

        private readonly Dictionary<IDownloadTask, IDownloadTaskViewModel> _viewmodels = new();

        #region Props

        /// <summary>
        /// タスク情報
        /// </summary>
        public TaskInfoViewModel TaskInfoViewModel { get; init; }

        /// <summary>
        /// キャンセル済みを表示
        /// </summary>
        public bool DisplayCanceled
        {
            get => WS.DownloadManager.DisplayCanceled;
            set => WS.DownloadManager.DisplayCanceled = value;
        }

        /// <summary>
        /// 完了済みを表示
        /// </summary>
        public bool DisplayCompleted
        {
            get => WS.DownloadManager.DisplayCompleted;
            set => WS.DownloadManager.DisplayCompleted = value;
        }

        #endregion

        #region Method

        /// <summary>
        /// 全てのダウンロードをキャンセル
        /// </summary>
        public void CancelAll()
        {
            WS.DownloadManager.CancelDownload();
        }

        /// <summary>
        /// 完了済みタスクを削除
        /// </summary>
        public void ClearCompleted()
        {
            WS.DownloadManager.ClearCompleted();
        }

        #endregion
    }
}
