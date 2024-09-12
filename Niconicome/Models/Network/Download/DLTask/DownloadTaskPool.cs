using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions.System.List;
using Reactive.Bindings;
using System.Reactive.Linq;
using Niconicome.Models.Utils.Reactive;
using Niconicome.Models.Utils.Reactive.State;

namespace Niconicome.Models.Network.Download.DLTask
{
    public interface IDownloadTaskPool
    {
        /// <summary>
        /// タスク一覧
        /// </summary>
        IReadOnlyCollection<IDownloadTask> Tasks { get; }

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
        bool DisplayCanceled { get; set; }

        /// <summary>
        /// 完了済みを表示
        /// </summary>
        bool DisplayCompleted { get; set; }

        /// <summary>
        /// 変更監視オブジェクト
        /// </summary>
        IStateChangeNotifyer StateChangeNotifyer { get; }
    }

    public class DownloadTaskPool : IDownloadTaskPool
    {
        public DownloadTaskPool()
        {
            this.Tasks = this._tasksSource.AsReadOnly();
        }


        #region field

        private readonly List<IDownloadTask> _innerList = new();

        private readonly List<IDownloadTask> _tasksSource = new();

        private bool _displayCanceled = true;

        private bool _displayCompleted = true;

        #endregion

        #region Props

        public IReadOnlyCollection<IDownloadTask> Tasks { get; init; }

        public bool DisplayCanceled
        {
            get => this._displayCanceled;
            set
            {
                this._displayCanceled = value;
                this.Refresh();
            }
        }

        public bool DisplayCompleted
        {
            get => this._displayCompleted;
            set
            {
                this._displayCompleted = value;
                this.Refresh();
            }
        }

        public IStateChangeNotifyer StateChangeNotifyer { get; init; } = new StateChangeNotifyer();

        #endregion


        #region Method

        public void AddTask(IDownloadTask task)
        {
            this._innerList.Add(task);

            task.IsCanceled.Subscribe(_ => this.Refresh());
            task.IsCompleted.Subscribe(_ => this.Refresh());

            //設定を参照してコレクションに追加するかどうか判断
            if (!this.DisplayCanceled && task.IsCanceled.Value) return;
            if (!this.DisplayCompleted && task.IsCompleted.Value) return;

            this._tasksSource.Add(task);

            this.StateChangeNotifyer.RaiseChange();

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
                this.StateChangeNotifyer.RaiseChange();
            }

        }

        public void Clear()
        {
            this._innerList.Clear();
            this._tasksSource.Clear();
            this.StateChangeNotifyer.RaiseChange();
        }


        public void Refresh()
        {
            List<IDownloadTask> tasks = this._innerList.Where(t =>
            {
                if (!this.DisplayCanceled && t.IsCanceled.Value)
                {
                    return false;
                }
                else if (!this.DisplayCompleted && t.IsCompleted.Value)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }).ToList();

            this._tasksSource.Clear();
            this._tasksSource.AddRange(tasks);

            this.StateChangeNotifyer.RaiseChange();
        }
        #endregion
    }
}
