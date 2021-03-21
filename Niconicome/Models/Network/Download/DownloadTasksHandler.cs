using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Playlist;

namespace Niconicome.Models.Network.Download
{
    interface IDownloadTasksHandler
    {
        IDownloadTaskPool DownloadTaskPool { get; init; }
        IDownloadTaskPool StagedDownloadTaskPool { get; init; }

        void ClearStaged();
        void MoveStagedToQueue(bool clearAfterMove = true);
        void StageVIdeo(IVideoListInfo video, DownloadSettings settings);
        void StageVIdeos(IEnumerable<IVideoListInfo> videos, DownloadSettings settings);
    }

    class DownloadTasksHandler : IDownloadTasksHandler
    {

        public DownloadTasksHandler(IDownloadTaskPool staged,IDownloadTaskPool download) {
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
        public void StageVIdeo(IVideoListInfo video, DownloadSettings settings)
        {
            var task = new BindableDownloadTask(video, settings);
            this.StagedDownloadTaskPool.AddTask(task);
        }

        /// <summary>
        /// 複数の動画をステージする
        /// </summary>
        /// <param name="videos"></param>
        /// <param name="settings"></param>
        public void StageVIdeos(IEnumerable<IVideoListInfo> videos, DownloadSettings settings)
        {
            foreach (var video in videos)
            {
                this.StageVIdeo(video, settings);
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
        /// ステージング済みをキューに追加
        /// </summary>
        /// <param name="clearAfterMove"></param>
        public void MoveStagedToQueue(bool clearAfterMove = true)
        {
            this.DownloadTaskPool.AddTasks(this.StagedDownloadTaskPool.GetAllTasks());
            if (clearAfterMove) this.ClearStaged();
        }
    }
}
