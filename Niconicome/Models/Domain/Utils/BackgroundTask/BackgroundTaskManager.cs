using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Niconicome.Models.Utils.Reactive;

namespace Niconicome.Models.Domain.Utils.BackgroundTask
{
    public interface IBackgroundTaskManager
    {
        /// <summary>
        /// タスクを追加
        /// </summary>
        /// <param name="action"></param>
        /// <param name="highPriority"></param>
        /// <returns></returns>
        BackgroundTask AddTask(Action action, bool highPriority = false);

        /// <summary>
        /// 時間指定でタスクを追加
        /// </summary>
        /// <param name="action"></param>
        /// <param name="when"></param>
        /// <returns></returns>
        BackgroundTask AddTimerTask(Action action, DateTime when);

        /// <summary>
        /// タスクをキャンセル
        /// </summary>
        /// <param name="taskID"></param>
        void CancelTask(string taskID);
    }

    public class BackgroundTaskManager : IBackgroundTaskManager
    {
        public BackgroundTask AddTask(Action action, bool highPriority = false)
        {
            var task = new BackgroundTask(new BindableProperty<bool>(false), Utils.GetRandomString(5));
            if (highPriority)
            {
                this.EnqueueHighPriorityTask(new TaskObject(action, task));
            } else
            {
                this.EnqueueTask(new TaskObject(action, task));
            }

            if (!this._isRunning)
            {
               _= this.WorkRoop();
            }
            return task;
        }

        public BackgroundTask AddTimerTask(Action action, DateTime when)
        {
            var task = new BackgroundTask(new BindableProperty<bool>(false), Utils.GetRandomString(5));
            var timer = new Timer((when - DateTime.Now).TotalMilliseconds);
            timer.AutoReset = false;
            timer.Elapsed += (sender, e) =>
            {
                this.EnqueueTask(new TaskObject(action, task));
                if (!this._isRunning)
                {
                    _ = this.WorkRoop();
                }
                this._timers.Remove(task.TaskID);
            };
            timer.Start();
            this._timers.Add(task.TaskID, timer);
            return task;
        }

        public void CancelTask(string taskID)
        {
            this._tasks.RemoveAll(_tasks => _tasks.taskInfo.TaskID == taskID);
            if (this._timers.ContainsKey(taskID))
            {
                var timer = this._timers[taskID];
                timer.Stop();
                timer.Dispose();
                this._timers.Remove(taskID);
            }
        }

        /// <summary>
        /// キュー
        /// </summary>
        private readonly List<TaskObject> _tasks = new();

        /// <summary>
        /// タイマー
        /// </summary>
        private readonly Dictionary<string, Timer> _timers = new();

        private bool _isRunning = false;

        #region private

        /// <summary>
        /// タスク処理
        /// </summary>
        private async Task WorkRoop()
        {
            this._isRunning = true;

            if (this._tasks.Count == 0)
            {
                this._isRunning = false;
                return;
            }

            var task = this.DequeueTask();

            await Task.Run(() =>
            {
                try
                {
                    task.action();
                }
                catch
                { }
            });

            task.taskInfo.IsDone.Value = true;

            if (this._tasks.Count > 0)
            {
                _ = this.WorkRoop();
                return;
            }

            this._isRunning = false;
        }

        /// <summary>
        /// タスクを追加
        /// </summary>
        /// <param name="task"></param>
        private void EnqueueTask(TaskObject task)
        {
            this._tasks.Add(task);
        }

        /// <summary>
        /// 優先度の高いタスクを追加
        /// </summary>
        /// <param name="task"></param>
        private void EnqueueHighPriorityTask(TaskObject task)
        {
            this._tasks.Insert(0, task);
        }

        /// <summary>
        /// タスクを取得
        /// </summary>
        /// <returns></returns>
        private TaskObject DequeueTask()
        {
            if (this._tasks.Count == 0)
            {
                throw new InvalidOperationException();
            }

            var task = this._tasks.First();
            this._tasks.RemoveAt(0);
            return task;

        }

        /// <summary>
        /// タスクIDを取得
        /// </summary>
        /// <returns></returns>
        private string GetTaskID()
        {
            return $"{Utils.GetRandomString(5)}-{DateTime.Now.ToString("HHmmss")}";
        }

        #endregion

        /// <summary>
        /// タスクオブジェクト
        /// </summary>
        /// <param name="action"></param>
        /// <param name="taskInfo"></param>
        private record TaskObject(Action action, BackgroundTask taskInfo);
    }
}
