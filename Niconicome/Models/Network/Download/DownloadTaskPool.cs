using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Network.Download
{
    public interface IDownloadTaskPool
    {
        IEnumerable<IDownloadTask> GetAllTasks();
        IEnumerable<IDownloadTask> GetTaskWhere(Func<IDownloadTask, bool> predicate);
        void AddTask(IDownloadTask task);
        void AddTasks(IEnumerable<IDownloadTask> tasks);
        void RemoveTask(IDownloadTask task);
        void RemoveTasks(IEnumerable<IDownloadTask> tasks);
        void RemoveTasks(Predicate<IDownloadTask> predicate);
        event EventHandler<TaskPoolChangeEventargs>? TaskPoolChange;
        void CancelAllTasks();
        void Clear(bool cancel = true);
        bool HasTask(Func<IDownloadTask, bool> predicate);
        int Count { get; }
    }

    public class TaskPoolChangeEventargs : EventArgs
    {
        public TaskPoolChangeEventargs(TaskPoolChangeType changeType, IDownloadTask task)
        {
            this.ChangeType = changeType;
            this.Task = task;
        }


        public TaskPoolChangeType ChangeType { get; init; }

        public IDownloadTask Task { get; init; }

    }

    public enum TaskPoolChangeType
    {
        Remove,
        Add,
    }

    /// <summary>
    /// DLタスクプール
    /// </summary>
   public  class DownloadTaskPool : IDownloadTaskPool
    {

        private readonly List<IDownloadTask> innerList = new();

        /// <summary>
        /// 全てのタスクを取得する
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IDownloadTask> GetAllTasks()
        {
            return this.innerList;
        }

        /// <summary>
        /// 条件を指定してタスクを取得
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<IDownloadTask> GetTaskWhere(Func<IDownloadTask, bool> predicate)
        {
            return this.innerList.Where(predicate);
        }

        /// <summary>
        /// 全てのタスクをキャンセル
        /// </summary>
        public void CancelAllTasks()
        {
            foreach (var task in this.innerList)
            {
                task.Cancel();
            }
        }

        /// <summary>
        ///タスクを追加する 
        /// </summary>
        /// <param name="task"></param>
        public void AddTask(IDownloadTask task)
        {
            this.innerList.Add(task);
            this.RaiseTaskPoolChanged(TaskPoolChangeType.Add, task);

        }

        /// <summary>
        /// 複数のタスクを追加する
        /// </summary>
        /// <param name="tasks"></param>
        public void AddTasks(IEnumerable<IDownloadTask> tasks)
        {
            foreach (var task in tasks)
            {
                this.AddTask(task);
            }
        }

        /// <summary>
        /// タスクを削除する
        /// </summary>
        /// <param name="task"></param>
        public void RemoveTask(IDownloadTask task)
        {
            this.innerList.RemoveAll(t => t == task);
            this.RaiseTaskPoolChanged(TaskPoolChangeType.Remove, task);
        }

        /// <summary>
        /// 複数のタスクを削除する
        /// </summary>
        /// <param name="tasks"></param>
        public void RemoveTasks(IEnumerable<IDownloadTask> tasks)
        {
            foreach (var task in tasks)
            {
                this.RemoveTask(task);
            }
        }

        /// <summary>
        /// 複数のタスクを削除する
        /// </summary>
        /// <param name="predicate"></param>
        public void RemoveTasks(Predicate<IDownloadTask> predicate)
        {
            this.innerList.RemoveAll(predicate);
        }


        /// <summary>
        /// プール変更イベント
        /// </summary>
        public event EventHandler<TaskPoolChangeEventargs>? TaskPoolChange;

        /// <summary>
        /// プール変更イベントを発火させる
        /// </summary>
        /// <param name="changeType"></param>
        /// <param name="task"></param>
        private void RaiseTaskPoolChanged(TaskPoolChangeType changeType, IDownloadTask task)
        {
            this.TaskPoolChange?.Invoke(this, new TaskPoolChangeEventargs(changeType, task));
        }

        /// <summary>
        /// タスクの存在をチェックする
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool HasTask(Func<IDownloadTask, bool> predicate)
        {
            return this.innerList.Any(predicate);
        }

        /// <summary>
        /// 全て削除する
        /// </summary>
        public void Clear(bool cancel = true)
        {
            if (cancel)
            {
                this.CancelAllTasks();
            }
            this.innerList.Clear();
        }


        /// <summary>
        /// タスク数を取得
        /// </summary>
        public int Count { get => this.innerList.Count; }
    }
}
