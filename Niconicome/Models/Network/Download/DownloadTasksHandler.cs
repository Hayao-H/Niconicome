using System;
using System.Collections.Generic;
using System.Linq;
using Niconicome.Models.Playlist;

namespace Niconicome.Models.Network.Download
{
    interface IDownloadTasksHandler
    {
        IDownloadTaskPool DownloadTaskPool { get; init; }
        IDownloadTaskPool StagedDownloadTaskPool { get; init; }

        void ClearStaged();
        void MoveStagedToQueue(bool clearAfterMove = true);
        void MoveStagedToQueue(Func<IDownloadTask, bool> predicate, bool clearAfterMove = true);
        void StageVIdeo(IVideoListInfo video, DownloadSettings settings, bool allowDupe);
        void StageVIdeos(IEnumerable<IVideoListInfo> videos, DownloadSettings settings, bool allowDupe);
        bool ContainsStage(Func<IDownloadTask, bool> predicate);
    }

    class DownloadTasksHandler : IDownloadTasksHandler
    {

        public DownloadTasksHandler(IDownloadTaskPool staged, IDownloadTaskPool download)
        {
            this.StagedDownloadTaskPool = staged;
            this.DownloadTaskPool = download;
        }

        /// <summary>
        /// ステージング済み
        /// </summary>
        public IDownloadTaskPool StagedDownloadTaskPool { get; init; }

        /// <summary>
        /// ダウンロード対象
        /// </summary>
        public IDownloadTaskPool DownloadTaskPool { get; init; }

        /// <summary>
        /// 動画をステージする
        /// </summary>
        /// <param name="video"></param>
        /// <param name="settings"></param>
        public void StageVIdeo(IVideoListInfo video, DownloadSettings settings, bool allowDupe)
        {
            var task = new DownloadTask(video.NiconicoId, video.Title, video.Id, settings);
            void onMessageChange(object? sender, DownloadTaskMessageChangedEventArgs e)
            {
                if (e.Message is not null) video.Message = e.Message;
            }
            task.MessageChange += onMessageChange;
            task.ProcessingEnd += (_, _) =>
            {
                task.MessageChange -= onMessageChange;
            };
            this.StagedDownloadTaskPool.AddTask(task);
        }

        /// <summary>
        /// 複数の動画をステージする
        /// </summary>
        /// <param name="videos"></param>
        /// <param name="settings"></param>
        public void StageVIdeos(IEnumerable<IVideoListInfo> videos, DownloadSettings settings, bool allowDupe)
        {
            foreach (var video in videos)
            {
                this.StageVIdeo(video, settings with { }, allowDupe);
            }
        }

        /// <summary>
        /// ステージング済みをクリア
        /// </summary>
        public void ClearStaged()
        {
            this.StagedDownloadTaskPool.Clear(false);
        }

        /// <summary>
        /// フィルターして削除
        /// </summary>
        /// <param name="predicate"></param>
        public void RemoveStaged(Predicate<IDownloadTask> predicate)
        {
            this.StagedDownloadTaskPool.RemoveTasks(predicate);
        }

        /// <summary>
        /// ステージング済みをキューに追加
        /// </summary>
        /// <param name="clearAfterMove"></param>
        public void MoveStagedToQueue(bool clearAfterMove = true)
        {
            this.DownloadTaskPool.AddTasks(this.StagedDownloadTaskPool.GetAllTasks());
            if (clearAfterMove) this.ClearStaged();
        }

        /// <summary>
        /// ステージング済みをフィルターして追加
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="clearAfterMove"></param>
        public void MoveStagedToQueue(Func<IDownloadTask, bool> predicate, bool clearAfterMove = true)
        {
            var videos = this.StagedDownloadTaskPool.GetAllTasks().Where(predicate);
            this.DownloadTaskPool.AddTasks(videos);
            if (clearAfterMove) this.RemoveStaged(t => predicate(t));

        }

        /// <summary>
        /// ステージング済みタスクを確認
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool ContainsStage(Func<IDownloadTask, bool> predicate)
        {
            return this.StagedDownloadTaskPool.HasTask(predicate);
        }


    }
}
