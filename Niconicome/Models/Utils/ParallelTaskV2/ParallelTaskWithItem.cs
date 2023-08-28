using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Utils.ParallelTaskV2
{
    public interface IParallelTask<T>
    {
        /// <summary>
        /// Inxex(自動割当)
        /// </summary>
        int Index { get; set; }

        /// <summary>
        /// タスクアイテム
        /// </summary>
        T TaskItem { get; }

        /// <summary>
        /// タスク関数
        /// T:タスクアイテム
        /// object:localObj
        /// </summary>
        Func<T, object, Task> TaskFunction { get; }

        /// <summary>
        /// 待機ハンドラ
        /// </summary>
        Action<int> OnWait { get; }

        /// <summary>
        /// キャンセルメソッド
        /// </summary>
        void Cancel();
    }

    public class ParallelTask<T> : IParallelTask<T>
    {
        public ParallelTask(T taskItem, Func<T, object, Task> taskFunction, Action<int> onWait)
        {
            this.TaskItem = taskItem;
            this.TaskFunction = taskFunction;
            this.OnWait = onWait;
        }

        public int Index { get; set; }

        public T TaskItem { get; init; }

        public Func<T, object, Task> TaskFunction { get; init; }

        public Action<int> OnWait { get; init; }

        public virtual void Cancel()
        {

        }
    }
}
