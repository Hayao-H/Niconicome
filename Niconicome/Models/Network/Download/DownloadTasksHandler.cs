using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Niconicome.Models.Playlist;
using Reactive.Bindings;

namespace Niconicome.Models.Network.Download
{
    interface IDownloadTasksHandler
    {
        /// <summary>
        /// ダウンロードタスク
        /// </summary>
        IDownloadTaskPool DownloadTaskPool { get; init; }

        /// <summary>
        /// ステージ済みタスク
        /// </summary>
        IDownloadTaskPool StagedDownloadTaskPool { get; init; }

        /// <summary>
        /// キャンセル済みを表示
        /// </summary>
        ReactiveProperty<bool> DisplayCanceled { get; }

        /// <summary>
        /// 完了済みを表示
        /// </summary>
        ReactiveProperty<bool> DisplayCompleted { get; }

        /// <summary>
        /// ステージ済みタスクをキューに移動
        /// </summary>
        /// <param name="clearAfterMove"></param>
        void MoveStagedToQueue(bool clearAfterMove = true);

        /// <summary>
        /// 条件に一致するステージ済みタスクをキューに移動
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="clearAfterMove"></param>
        void MoveStagedToQueue(Func<IDownloadTask, bool> predicate, bool clearAfterMove = true);

        /// <summary>
        /// 動画をステージ
        /// </summary>
        /// <param name="video"></param>
        /// <param name="settings"></param>
        /// <param name="allowDupe"></param>
        void StageVIdeo(IListVideoInfo video, DownloadSettings settings, bool allowDupe);

        /// <summary>
        /// 複数の動画をステージ
        /// </summary>
        /// <param name="videos"></param>
        /// <param name="settings"></param>
        /// <param name="allowDupe"></param>
        void StageVIdeos(IEnumerable<IListVideoInfo> videos, DownloadSettings settings, bool allowDupe);
    }

    class DownloadTasksHandler : IDownloadTasksHandler
    {

        public DownloadTasksHandler(IDownloadTaskPool staged, IDownloadTaskPool download)
        {
            this.StagedDownloadTaskPool = staged;
            this.DownloadTaskPool = download;

            this.DownloadTaskPool.RegisterFilter(task =>
            {
                if (!this.DisplayCanceled.Value && task.IsCanceled.Value)
                {
                    return false;
                }
                else if (!this.DisplayCompleted.Value && task.IsDone.Value)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            });

            this.DisplayCanceled.Subscribe(_ => this.DownloadTaskPool.Refresh());
            this.DisplayCompleted.Subscribe(_ => this.DownloadTaskPool.Refresh());
        }

        #region Props

        public IDownloadTaskPool StagedDownloadTaskPool { get; init; }

        public IDownloadTaskPool DownloadTaskPool { get; init; }

        public ReactiveProperty<bool> DisplayCanceled { get; init; } = new(true);

        public ReactiveProperty<bool> DisplayCompleted { get; init; } = new(true);

        #endregion

        #region Method

        public void StageVIdeo(IListVideoInfo video, DownloadSettings settings, bool allowDupe)
        {
            var task = new DownloadTask(video, settings);
            task.Message.Subscribe(value =>
            {
                if (value is not null) video.Message.Value = value;
            });
            this.StagedDownloadTaskPool.AddTask(task);
        }

        public void StageVIdeos(IEnumerable<IListVideoInfo> videos, DownloadSettings settings, bool allowDupe)
        {
            foreach (var video in videos)
            {
                this.StageVIdeo(video, settings with { }, allowDupe);
            }
        }

        public void MoveStagedToQueue(bool clearAfterMove = true)
        {
            this.DownloadTaskPool.AddTasks(this.StagedDownloadTaskPool.Tasks);
            this.SubscribeTaskProperty(this.StagedDownloadTaskPool.Tasks);
            if (clearAfterMove) this.ClearStaged();
        }

        public void MoveStagedToQueue(Func<IDownloadTask, bool> predicate, bool clearAfterMove = true)
        {
            var tasks = this.StagedDownloadTaskPool.Tasks.Where(predicate);
            this.DownloadTaskPool.AddTasks(tasks);
            this.SubscribeTaskProperty(tasks);
            if (clearAfterMove) this.RemoveStaged(tasks);
        }

        #endregion

        #region private

        private void RemoveStaged(IEnumerable<IDownloadTask> tasks)
        {
            this.StagedDownloadTaskPool.RemoveTasks(tasks);
        }

        private void ClearStaged()
        {
            this.StagedDownloadTaskPool.Clear(false);
        }

        private void SubscribeTaskProperty(IEnumerable<IDownloadTask> tasks)
        {
            foreach (var task in tasks)
            {
                Observable.Merge(task.IsCanceled, task.IsDone).Subscribe(_ => this.DownloadTaskPool.Refresh());
            }
        }

        #endregion


    }
}
