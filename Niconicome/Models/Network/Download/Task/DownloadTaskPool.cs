using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions.System.List;
using Reactive.Bindings;
using System.Reactive.Linq;

namespace Niconicome.Models.Network.Download.DLTask
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
        /// タスクを削除する
        /// </summary>
        /// <param name="task"></param>
        void RemoveTask(IDownloadTask task);

        /// <summary>
        /// タスクをすべて削除
        /// </summary>
        void Clear();

        /// <summary>
        /// 情報を更新
        /// </summary>
        void Refresh();

        /// <summary>
        /// キャンセル済みを表示
        /// </summary>
        ReactiveProperty<bool> DisplayCanceled { get; }

        /// <summary>
        /// 完了済みを表示
        /// </summary>
        ReactiveProperty<bool> DisplayCompleted { get; }
    }

    public class DownloadTaskPool : IDownloadTaskPool
    {
        public DownloadTaskPool()
        {
            this.Tasks = new ReadOnlyObservableCollection<IDownloadTask>(this._tasksSource);
            this.DisplayCanceled.Subscribe(_ => this.Refresh());
            this.DisplayCompleted.Subscribe(_ => this.Refresh());
        }


        #region field

        private readonly List<IDownloadTask> _innerList = new();

        private readonly ObservableCollection<IDownloadTask> _tasksSource = new();

        #endregion

        #region Props

        public ReadOnlyObservableCollection<IDownloadTask> Tasks { get; init; }

        public ReactiveProperty<bool> DisplayCanceled { get; init; } = new(true);

        public ReactiveProperty<bool> DisplayCompleted { get; init; } = new(true);

        #endregion


        #region Method

        public void AddTask(IDownloadTask task)
        {
            this._innerList.Add(task);

            task.IsCanceled.Skip(1).Subscribe(_ => this.Refresh());
            task.IsCompleted.Skip(1).Subscribe(_ => this.Refresh());

            //設定を参照してコレクションに追加するかどうか判断
            if (!this.DisplayCanceled.Value && task.IsCanceled.Value) return;
            if (!this.DisplayCompleted.Value && task.IsCompleted.Value) return;

            this._tasksSource.Add(task);

        }

        public void RemoveTask(IDownloadTask task)
        {
            if (this._innerList.Contains(task))
            {
                this._innerList.Remove(task);
            }

            if (this._tasksSource.Contains(task))
            {
                this._tasksSource.Remove(task);
            }
        }

        public void Clear()
        {
            this._innerList.Clear();
            this._tasksSource.Clear();
        }


        public void Refresh()
        {
            List<IDownloadTask> tasks = this._innerList.Where(t =>
            {
                if (!this.DisplayCanceled.Value && t.IsCanceled.Value)
                {
                    return false;
                }
                else if (!this.DisplayCompleted.Value && t.IsCompleted.Value)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }).ToList();

            this._tasksSource.Clear();
            this._tasksSource.Addrange(tasks);

        }
        #endregion
    }
}
