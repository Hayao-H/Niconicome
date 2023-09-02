using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Utils.ParallelTaskV2
{
    public interface IParallelTask
    {
        /// <summary>
        /// Inxex(自動割当)
        /// </summary>
        int Index { get; set; }

        /// <summary>
        /// タスク関数
        /// object:localObj
        /// </summary>
        Func<object, Task> TaskFunction { get; }

        /// <summary>
        /// 待機ハンドラ
        /// </summary>
        Action<int> OnWait { get; }

        /// <summary>
        /// キャンセルメソッド
        /// </summary>
        void Cancel();
    }

    public class ParallelTask : IParallelTask
    {
        public ParallelTask(Func<object, Task> taskFunction, Action<int> onWait)
        {
            this.TaskFunction = taskFunction;
            this.OnWait = onWait;
        }

        public int Index { get; set; }

        public Func<object, Task> TaskFunction { get; init; }

        public Action<int> OnWait { get; init; }

        public virtual void Cancel()
        {

        }
    }
}
