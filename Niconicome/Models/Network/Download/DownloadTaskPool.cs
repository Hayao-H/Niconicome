using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions.System.List;

namespace Niconicome.Models.Network.Download
{
    public interface IDownloadTaskPool
    {
        /// <summary>
        /// タスク一覧
        /// </summary>
        ReadOnlyObservableCollection<IDownloadTask> Tasks { get; }

        /// <summary>
        /// タスクを追加する
        /// </summary>
        /// <param name="task"></param>
        void AddTask(IDownloadTask task);

        /// <summary>
        /// 複数のタスクを追加する
        /// </summary>
        /// <param name="tasks"></param>
        void AddTasks(IEnumerable<IDownloadTask> tasks);

        /// <summary>
        /// タスクを削除する
        /// </summary>
        /// <param name="task"></param>
        void RemoveTask(IDownloadTask task);

        /// <summary>
        /// 複数のタスクを削除する
        /// </summary>
        /// <param name="tasks"></param>
        void RemoveTasks(IEnumerable<IDownloadTask> tasks);

        /// <summary>
        /// 条件に一致するタスクを削除する
        /// </summary>
        /// <param name="predicate"></param>
        void RemoveTasks(Predicate<IDownloadTask> predicate);

        /// <summary>
        /// すべてのタスクをキャンセルする
        /// </summary>
        void CancelAllTasks();

        /// <summary>
        /// すべてのタスクを削除する
        /// </summary>
        /// <param name="cancel"></param>
        void Clear(bool cancel = true);

        /// <summary>
        /// Taskフィルターを登録する
        /// </summary>
        /// <param name="filter"></param>
        void RegisterFilter(Func<IDownloadTask, bool> filter);

        /// <summary>
        /// 追加時のハンドラを登録する
        /// </summary>
        /// <param name="handler"></param>
        void RegisterAddHandler(Action<IDownloadTask> handler);

        /// <summary>
        /// タスクを更新する
        /// </summary>
        void Refresh();
    }

    /// <summary>
    /// DLタスクプール
    /// </summary>
    public class DownloadTaskPool : IDownloadTaskPool
    {
        public DownloadTaskPool()
        {
            this._innerList = new List<IDownloadTask>();
            this._innerObservable = new ObservableCollection<IDownloadTask>(this._innerList);
            this.Tasks = new ReadOnlyObservableCollection<IDownloadTask>(this._innerObservable);
            this.filterFunc = _ => true;
        }


        #region field

        private readonly ObservableCollection<IDownloadTask> _innerObservable;

        private readonly List<IDownloadTask> _innerList;

        private Action<IDownloadTask>? addhandler;

        private Func<IDownloadTask, bool> filterFunc;

        #endregion

        #region Props

        public ReadOnlyObservableCollection<IDownloadTask> Tasks { get; init; }

        #endregion


        #region Method

        public void CancelAllTasks()
        {
            foreach (var task in this._innerList)
            {
                task.Cancel();
            }
        }

        public void AddTask(IDownloadTask task)
        {
            this._innerList.Add(task);
            this.addhandler?.Invoke(task);
            this.Refresh();
        }

        public void AddTasks(IEnumerable<IDownloadTask> tasks)
        {
            foreach (var task in tasks)
            {
                this.AddTask(task);
            }
        }

        public void RemoveTask(IDownloadTask task)
        {
            this._innerList.RemoveAll(t => t == task);
            this.Refresh();
        }

        public void RemoveTasks(IEnumerable<IDownloadTask> tasks)
        {
            foreach (var task in tasks)
            {
                this.RemoveTask(task);
            }
        }

        public void RemoveTasks(Predicate<IDownloadTask> predicate)
        {
            this._innerList.RemoveAll(predicate);
            this.Refresh();
        }

        public void Clear(bool cancel = true)
        {
            if (cancel)
            {
                this.CancelAllTasks();
            }
            this._innerList.Clear();
            this._innerObservable.Clear();
        }

        public void Refresh()
        {
            this._innerObservable.Clear();
            this._innerObservable.Addrange(this._innerList.Where(this.filterFunc));
        }

        public void RegisterFilter(Func<IDownloadTask, bool> filter)
        {
            this.filterFunc = filter;
        }

        public void RegisterAddHandler(Action<IDownloadTask> handler)
        {
            this.addhandler += handler;
        }



        #endregion
    }
}
