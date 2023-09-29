using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Network.Download.DLTask;
using Niconicome.Models.Utils.Reactive;
using WS = Niconicome.Workspaces.Mainpage;


namespace Niconicome.ViewModels.Mainpage.Subwindows.DownloadTask.Pages
{
    public class ViewModelBase
    {
        public ViewModelBase(bool isQueue)
        {
            this._isQueue = isQueue;
            if (isQueue) {

                this.Tasks = new List<IDownloadTaskViewModel>(WS.DownloadManager.Queue.Select(x => this.GetViewModel(x)));
            } else
            {
                this.Tasks = new List<IDownloadTaskViewModel>(WS.DownloadManager.Staged.Select(x => this.GetViewModel(x)));
            }
            WS.DownloadManager.StateChangeNotifyer.Subscribe(() => this.RefreshList());
        }

        private readonly Dictionary<IDownloadTask, IDownloadTaskViewModel> _viewmodels = new();

        private bool _isQueue = false;

        #region Props

        public Bindables Bindables { get; init; } = new();

        /// <summary>
        /// ダウンロードキュー
        /// </summary>
        public List<IDownloadTaskViewModel> Tasks { get; init; }

        #endregion

        #region private

        /// <summary>
        /// リストを更新
        /// </summary>
        private void RefreshList()
        {
            this.Tasks.Clear();
            if (this._isQueue)
            {
                this.Tasks.AddRange(WS.DownloadManager.Queue.Select(x => this.GetViewModel(x)));
            }
            else
            {
                this.Tasks.AddRange(WS.DownloadManager.Staged.Select(x => this.GetViewModel(x)));
            }
            this.Bindables.RaiseChange();
        }

        /// <summary>
        /// VMを取得
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private IDownloadTaskViewModel GetViewModel(IDownloadTask task)
        {
            if (this._viewmodels.ContainsKey(task))
            {
                return this._viewmodels[task];
            }

            var vm = new DownloadTaskViewModel(task);
            this.Bindables.Add(vm.Bindables);

            this._viewmodels.Add(task, vm);

            return vm;
        }

        #endregion
    }
}
