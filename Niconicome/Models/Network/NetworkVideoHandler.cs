using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Playlist;
using State = Niconicome.Models.Local.State;
using DWatch = Niconicome.Models.Domain.Niconico.Watch;
using Niconicome.Extensions.System;
using Niconicome.Models.Utils;
using Niconicome.Models.Playlist.VideoList;
using Niconicome.Models.Network.Watch;
using Niconicome.Models.Local.Settings;

namespace Niconicome.Models.Network
{
    public interface INetworkVideoHandler
    {
        bool IsVideoDownloaded(string niconicoId);
        string GetFilePath(string niconicoId);
        Task<IEnumerable<IListVideoInfo>> GetVideoListInfosAsync(IEnumerable<string> ids, bool uncheck = false, int? playlistID = null, CancellationToken? ct = null);
        Task<IEnumerable<IListVideoInfo>> GetVideoListInfosAsync(IEnumerable<IListVideoInfo> videos, bool uncheck = false, int? playlistID = null, CancellationToken? ct = null);
    }

    public interface INetworkResult
    {
        bool IsFailed { get; set; }
        bool IsSucceededAll { get; set; }
        bool IsCanceled { get; set; }
        int SucceededCount { get; set; }
        int FailedCount { get; set; }
        IListVideoInfo? FirstVideo { get; set; }
    }

    /// <summary>
    /// 動画情報の操作で、ネットワークに関する機能を提供する
    /// </summary>
    class NetworkVideoHandler : INetworkVideoHandler
    {

        private readonly IWatch wacthPagehandler;

        private readonly State::IMessageHandler messageHandler;

        private readonly IVideoFileStorehandler fileStorehandler;

        private readonly ILocalSettingHandler settingHandler;

        private readonly IVideoListContainer videoListContainer;


        public NetworkVideoHandler(IWatch watchPageHandler, State::IMessageHandler messageHandler, IVideoFileStorehandler fileStorehandler, ILocalSettingHandler settingHandler, IVideoListContainer videoListContainer)
        {
            this.wacthPagehandler = watchPageHandler;
            this.messageHandler = messageHandler;
            this.fileStorehandler = fileStorehandler;
            this.settingHandler = settingHandler;
            this.videoListContainer = videoListContainer;
        }


        /// <summary>
        /// IDから動画情報を取得する
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="onSucceeded"></param>
        /// <param name="onFailed"></param>
        /// <param name="onStarted"></param>
        /// <param name="onWaiting"></param>
        /// <returns></returns>
        public async Task<IEnumerable<IListVideoInfo>> GetVideoListInfosAsync(IEnumerable<string> ids, bool uncheck = false, int? playlistID = null, CancellationToken? ct = null)
        {
            return await this.GetVideoListInfosAsync(ids.Select(i =>
            {
                var v = new NonBindableListVideoInfo();
                v.NiconicoId.Value = i;
                return v;
            }), uncheck, playlistID, ct);
        }

        /// <summary>
        /// ダウンロード済かどうかを確かめる
        /// </summary>
        /// <param name="niconicoId"></param>
        /// <returns></returns>
        public bool IsVideoDownloaded(string niconicoId)
        {
            return this.fileStorehandler.Exists(niconicoId);
        }

        /// <summary>
        /// ファイルパスを取得する
        /// </summary>
        /// <param name="niconicoId"></param>
        /// <returns></returns>
        public string GetFilePath(string niconicoId)
        {
            if (!this.fileStorehandler.Exists(niconicoId)) throw new InvalidOperationException($"{niconicoId}は保存されていません。");
            return this.fileStorehandler.GetFilePath(niconicoId)!;
        }

        /// <summary>
        ///　指定したフォルダーに存在するファイルパスを取得する
        /// </summary>
        /// <param name="niconicoId"></param>
        /// <param name="foldername"></param>
        /// <returns></returns>
        public string GetFilePath(string niconicoId, string foldername)
        {
            var paths = this.fileStorehandler.GetFilePaths(niconicoId);
            return paths.First(p =>
                (Path.GetDirectoryName(p) ?? string.Empty) == foldername);
        }

        #region

        /// <summary>
        /// 動画情報を取得して処理する（実装）
        /// </summary>
        /// <param name="emptyVideos"></param>
        /// <param name="onSucceeded"></param>
        /// <param name="onFailed"></param>
        /// <param name="onStarted"></param>
        /// <param name="onWaiting"></param>
        /// <returns></returns>
        public async Task<IEnumerable<IListVideoInfo>> GetVideoListInfosAsync(IEnumerable<IListVideoInfo> emptyVideos, bool uncheck = false, int? playlistID = null, CancellationToken? ct = null)
        {

            var registerOnlyID = this.settingHandler.GetBoolSetting(SettingsEnum.StoreOnlyNiconicoID);
            if (registerOnlyID)
            {
                return emptyVideos;
            }

            int videosCount = emptyVideos.Count();
            var videos = new List<IListVideoInfo>();
            var maxParallelCount = this.settingHandler.GetIntSetting(SettingsEnum.MaxFetchCount);
            var waitInterval = this.settingHandler.GetIntSetting(SettingsEnum.FetchSleepInterval);
            if (maxParallelCount < 1) maxParallelCount = 3;
            if (waitInterval < 1) waitInterval = 5;

            var handler = new ParallelTasksHandler<NetworkVideoParallelTask>(maxParallelCount, waitInterval, 15);

            foreach (var item in emptyVideos.Select((video, index) => new { video, index }))
            {

                var task = new NetworkVideoParallelTask(async (_, lockObj, _) =>
                {
                    this.messageHandler.AppendMessage($"{item.video.NiconicoId.Value}の取得を開始します。");

                    IResult result = await this.wacthPagehandler.TryGetVideoInfoAsync(item.video.NiconicoId.Value, item.video, DWatch::WatchInfoOptions.NoDmcData);

                    if (result.IsSucceeded)
                    {
                        this.messageHandler.AppendMessage($"{item.video.NiconicoId.Value}の取得に成功しました。");
                        videos.Add(item.video);
                        if (uncheck)
                        {
                            this.videoListContainer.Uncheck(item.video.Id.Value, playlistID ?? -1);
                        }
                    }
                    else
                    {
                        this.messageHandler.AppendMessage($"{item.video.NiconicoId.Value}の取得に失敗しました。(詳細:{result.Message})");
                    }
                }, index => this.messageHandler.AppendMessage("待機中...(15s)"));

                handler.AddTaskToQueue(task);

            }

            await handler.ProcessTasksAsync(() => { }, () => this.messageHandler.AppendMessage("動画情報の取得処理が中断されました。"), ct);


            return videos;
        }

        #endregion
    }

    public class NetworkResult : INetworkResult
    {
        /// <summary>
        /// 処理失敗フラグ
        /// </summary>
        public bool IsFailed { get; set; }


        /// <summary>
        /// 全ての処理に成功
        /// </summary>
        public bool IsSucceededAll { get; set; }

        /// <summary>
        /// キャンセルフラグ
        /// </summary>
        public bool IsCanceled { get; set; }


        /// <summary>
        /// 成功したコンテンツの数
        /// </summary>
        public int SucceededCount { get; set; }

        /// <summary>
        /// 失敗したコンテンツの数
        /// </summary>
        public int FailedCount { get; set; }

        /// <summary>
        /// 動画情報
        /// </summary>
        public IListVideoInfo? FirstVideo { get; set; }

    }

    public class NetworkVideoParallelTask : IParallelTask<NetworkVideoParallelTask>
    {
        public NetworkVideoParallelTask(Func<NetworkVideoParallelTask, object, IParallelTaskToken, Task> taskFunction, Action<int> onwait)
        {
            this.TaskFunction = taskFunction;
            this.OnWait = onwait;
        }

        public Guid TaskId { get; init; } = Guid.NewGuid();

        public Func<NetworkVideoParallelTask, object, IParallelTaskToken, Task> TaskFunction { get; init; }

        public Action<int> OnWait { get; init; }
    }

}
