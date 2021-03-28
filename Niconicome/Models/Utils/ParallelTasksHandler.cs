using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Extensions.System.List;

namespace Niconicome.Models.Utils
{
    public interface IParallelTask<T>
    {
        Guid TaskId { get; }
        Func<T, object, Task> TaskFunction { get; }
        Action<int> OnWait { get; }
    }

    class ParallelTasksHandler<T> where T : IParallelTask<T>
    {

        public ParallelTasksHandler(int maxPallarelTasksCount, int waitInterval, int waitSeconds, bool createThread = true)
        {
            this.maxPallarelTasksCount = maxPallarelTasksCount;
            this.waitInterval = waitInterval;
            this.waitSeconds = waitSeconds;
            this.createThread = createThread;
        }

        public ParallelTasksHandler(int maxPallarelTasksCount, bool createThread = true) : this(maxPallarelTasksCount, -1, -1, createThread) { }

        /// <summary>
        /// 最大動機実行数
        /// </summary>
        private readonly int maxPallarelTasksCount;

        /// <summary>
        /// 待機間隔
        /// </summary>
        private readonly int waitInterval;

        /// <summary>
        /// 待機時間
        /// </summary>
        private readonly int waitSeconds;

        /// <summary>
        /// 実行フラグ
        /// </summary>
        public bool IsProcessing { get; private set; }

        /// <summary>
        /// ロックオブジェクト
        /// </summary>
        private readonly object lockobj = new();

        /// <summary>
        /// 別スレッド化フラグ
        /// </summary>
        private readonly bool createThread;

        /// <summary>
        /// 全ての並列タスク
        /// </summary>
        public virtual List<T> PallarelTasks { get; init; } = new();

        /// <summary>
        /// タスクを追加する
        /// </summary>
        /// <param name="task"></param>
        public void AddTaskToQueue(T task)
        {
            lock (this.lockobj)
            {
                this.PallarelTasks.Add(task);
            }
        }

        /// <summary>
        /// 複数のタスクを追加する
        /// </summary>
        /// <param name="tasks"></param>
        public void AddTasksToQueue(IEnumerable<T> tasks)
        {
            foreach (var task in tasks)
            {
                this.AddTaskToQueue(task);
            }
        }

        /// <summary>
        /// 全てのタスクを削除する
        /// </summary>
        public void CancellAllTasks()
        {
            lock (this.lockobj)
            {
                this.PallarelTasks.Clear();
            }
        }

        /// <summary>
        /// 次のタスクを取得する
        /// </summary>
        /// <returns></returns>
        private T? GetNextTask()
        {
            lock (this.lockobj)
            {
                var task = this.PallarelTasks.FirstOrDefault();
                if (task is not null)
                {
                    this.PallarelTasks.RemoveAt(0);
                }
                return task;
            }
        }

        /// <summary>
        /// 実処理
        /// </summary>
        /// <returns></returns>
        public async Task ProcessTasksAsync(Action? preAction = null)
        {
            //すでにタスクを実行中の場合はキャンセル
            if (this.IsProcessing) return;

            var semaphore = new SemaphoreSlim(this.maxPallarelTasksCount, this.maxPallarelTasksCount);
            int index = 0;
            var mre = new ManualResetEventSlim(true);
            var tasks = new List<Task>();
            var lockObj = new object();

            lock (this.lockobj)
            {
                this.IsProcessing = true;
            }

            if (preAction is not null)
            {
                preAction();
            }

            //スレッドを作成して並列実行する
            while (this.PallarelTasks.Count > 0)
            {
                var task = this.GetNextTask();
                //タスクがnullならキャンセル
                if (task is null)
                {
                    continue;
                }

                Func<Task> func = async () =>
                {

                    await semaphore.WaitAsync();

                    lock (this.lockobj)
                    {
                        index++;
                    }

                    //待機処理
                    if (this.waitInterval != -1 && index % this.waitInterval == 0)
                    {
                        mre.Reset();
                        task.OnWait(index);
                        await Task.Delay(this.waitSeconds * 1000);
                        mre.Set();
                    }

                    mre.Wait();

                    await Task.Run(async () => await task.TaskFunction(task, lockobj));

                    semaphore.Release();
                };

                if (this.createThread)
                {
                    var t = Task.Run(func);
                    tasks.Add(t);
                }
                else
                {
                    var t = func();
                    tasks.Add(t);
                }

            }

            await tasks.WhenAll();

            lock (this.lockobj)
            {
                this.IsProcessing = false;
            }
        }

    }
}
