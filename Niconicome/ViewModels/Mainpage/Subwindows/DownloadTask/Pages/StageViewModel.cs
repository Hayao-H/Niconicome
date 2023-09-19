using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Const;
using Niconicome.Models.Utils.Reactive;
using WS = Niconicome.Workspaces.Mainpage;


namespace Niconicome.ViewModels.Mainpage.Subwindows.DownloadTask.Pages
{
    public class StageViewModel : ViewModelBase
    {
        public StageViewModel() : base(false)
        {
            this.TaskInfoViewModel = new TaskInfoViewModel();
            this.Bindables.Add(this.TaskInfoViewModel.Bindables);
        }

        #region Props

        /// <summary>
        /// タスク情報
        /// </summary>
        public TaskInfoViewModel TaskInfoViewModel { get; init; }

        #endregion

        #region Method

        /// <summary>
        /// 選択したタスクを削除
        /// </summary>
        public void RemoveSelected()
        {
            IDownloadTaskViewModel[] targets = this.Tasks.Where(t => t.IsSelected.Value).ToArray();
            foreach (var task in targets)
            {
                WS.DownloadManager.RemoveFromStaged(task.Task);
            }
        }

        /// <summary>
        /// ステージング済みのタスクをクリア
        /// </summary>
        public void ClearStaged()
        {
            WS.DownloadManager.ClearStaged();
        }

        /// <summary>
        /// ダウンロードを開始
        /// </summary>
        public void StartDownload()
        {
            WS.DownloadManager.StartDownloadAsync(m => WS.SnackbarHandler.Enqueue(m), m => WS.MessageHandler.AppendMessage(m, LocalConstant.SystemMessageDispacher));
        }

        #endregion

    }
}
