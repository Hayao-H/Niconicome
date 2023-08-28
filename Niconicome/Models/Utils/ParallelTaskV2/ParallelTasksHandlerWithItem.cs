using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Extensions.System.List;
using Reactive.Bindings.Extensions;
using Windows.ApplicationModel.Contacts;

namespace Niconicome.Models.Utils.ParallelTaskV2
{

    public class ParallelTasksHandler<T>
    {

        public ParallelTasksHandler(int maxPallarelTasksCount, int waitInterval, int waitSeconds, bool createThread = true, bool untilEmpty = false)
        {
            this.maxPallarelTasksCount = maxPallarelTasksCount;
            this.waitInterval = waitInterval;
            this.waitSeconds = waitSeconds;
            this.createThread = createThread;
            this.processUntilEmpty = untilEmpty;
        }

        public ParallelTasksHandler(int maxPallarelTasksCount, bool createThread = true, bool untilEmpty = false) : this(maxPallarelTasksCount, -1, -1, createThread, untilEmpty) { }

        #region private 

        /// <summary>
        /// 最大同時実行数
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
        /// ロックオブジェクト
        /// </summary>
        private readonly object lockobj = new();

        /// <summary>
        /// 別スレッド化フラグ
        /// </summary>
        private readonly bool createThread;

        /// <summary>
        /// タスクが空になるまで続ける
        /// </summary>
        private readonly bool processUntilEmpty;

        /// <summary>
        /// 最後に追加したタスクのインデックス
        /// </summary>
        private int lastTasksIndex = -1;

        #endregion

        /// <summary>
        /// 実行フラグ
        /// </summary>
        public bool IsProcessing { get; private set; }

        /// <summary>
        /// 全ての並列タスク
        /// </summary>
        public virtual Queue<IParallelTask<T>> PallarelTasks { get; init; } = new();

        /// <summary>
        /// タスクを追加する
        /// </summary>
        /// <param name="task"></param>
        public void AddTaskToQueue(IParallelTask<T> task)
        {
            lock (this.lockobj)
            {
                this.lastTasksIndex++;
                task.Index = this.lastTasksIndex;
                this.PallarelTasks.Enqueue(task);
            }
        }

        /// <summary>
        /// 全てのタスクを削除する
        /// </summary>
        public void CancellAllTasks()
        {
            lock (this.lockobj)
            {
                foreach (var t in this.PallarelTasks)
                {
                    try
                    {
                        t.Cancel();
                    }
                    catch { }
                }
                this.PallarelTasks.Clear();
            }
        }

        /// <summary>
        /// 次のタスクを取得する
        /// </summary>
        /// <returns></returns>
        private IParallelTask<T>? GetNextTask()
        {
            lock (this.lockobj)
            {
                if (this.PallarelTasks.Count <= 0) return default;
                var task = this.PallarelTasks.Dequeue();
                return task;
            }
        }

        /// <summary>
        /// 実処理
        /// </summary>
        /// <returns></returns>
        public async Task ProcessTasksAsync(Action? preAction = null, Action? onCancelled = null, CancellationToken ct = default)
        {
            //すでにタスクを実行中の場合はキャンセル
            if (this.IsProcessing) return;

            SemaphoreSlim semaphore = new SemaphoreSlim(this.maxPallarelTasksCount, this.maxPallarelTasksCount);
            ManualResetEventSlim mre = new ManualResetEventSlim(true);
            var tasks = new List<Task>();
            var lockObj = new object();

            lock (this.lockobj)
            {
                this.IsProcessing = true;
                this.lastTasksIndex = -1;
            }

            if (preAction is not null)
            {
                preAction();
            }

            void OnCancel()
            {
                semaphore.Release();
                mre.Set();
            }

            //スレッドを作成して並列実行する
            while (this.PallarelTasks.Count > 0)
            {
                IParallelTask<T>? task = this.GetNextTask();

                //タスクがnullならキャンセル
                if (task is null)
                {
                    continue;
                }

                await Task.Delay(300, CancellationToken.None);

                Func<Task> func = async () =>
                {
                    try
                    {
                        await semaphore.WaitAsync(ct);
                    }
                    catch { }

                    if (ct.IsCancellationRequested)
                    {
                        onCancelled?.Invoke();
                        OnCancel();
                        return;
                    }

                    //待機処理
                    if (this.waitInterval != -1 && task.Index > 0 && task.Index % this.waitInterval == 0)
                    {
                        mre.Reset();
                        task.OnWait(task.Index);

                        try
                        {
                            await Task.Delay(this.waitSeconds * 1000, ct);
                        }
                        catch
                        {
                            OnCancel();
                            return;
                        }

                        mre.Set();

                        if (ct.IsCancellationRequested)
                        {
                            OnCancel();
                            onCancelled?.Invoke();
                            return;
                        }
                    }

                    mre.Wait();

                    if (ct.IsCancellationRequested)
                    {
                        OnCancel();
                        onCancelled?.Invoke();
                        return;
                    }

                    try
                    {
                        await task.TaskFunction(task.TaskItem, lockobj);
                    }
                    catch
                    {
                    }

                    semaphore.Release();
                };

                if (this.createThread)
                {
                    var t = Task.Run(func, CancellationToken.None);
                    tasks.Add(t);
                }
                else
                {
                    var t = func();
                    tasks.Add(t);
                }

            }

            try
            {
                await tasks.WhenAll();
            }
            catch
            {
                lock (this.lockobj)
                {
                    this.IsProcessing = false;
                }
                return;
            }

            lock (this.lockobj)
            {
                this.IsProcessing = false;
            }

            if (this.processUntilEmpty && this.PallarelTasks.Count > 0)
            {
                await this.ProcessTasksAsync(preAction, onCancelled, ct);
            }

        }

    }
}
