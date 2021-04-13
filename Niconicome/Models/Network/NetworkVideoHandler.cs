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
using Niconicome.Models.Local;

namespace Niconicome.Models.Network
{
    public interface INetworkVideoHandler
    {
        bool IsVideoDownloaded(string niconicoId);
        string GetFilePath(string niconicoId);
        Task<INetworkResult> AddVideosAsync(IEnumerable<string> videosId, int playlistId, Action<IResult> onFailed, Action<IListVideoInfo> onSucceed);
        Task AddVideosAsync(IEnumerable<IListVideoInfo> videos, int playlistId);
        Task<INetworkResult> UpdateVideosAsync(IEnumerable<IListVideoInfo> videos, string folderPath, Action<IListVideoInfo> onSucceeded, CancellationToken ct);
        Task<IEnumerable<IListVideoInfo>> GetVideoListInfosAsync(IEnumerable<string> ids, Action<IListVideoInfo, IListVideoInfo, int, object> onSucceeded, Action<IResult, IListVideoInfo> onFailed, Action<IListVideoInfo, int> onStarted, Action<IListVideoInfo> onWaiting);
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

        private readonly IPlaylistHandler playlistTreeHandler;

        private readonly IVideoHandler videoHandler;

        private readonly State::IMessageHandler messageHandler;

        private readonly IVideoFileStorehandler fileStorehandler;

        private readonly IVideoThumnailUtility videoThumnailUtility;

        private readonly ILocalSettingHandler settingHandler;


        public NetworkVideoHandler(IWatch watchPageHandler, IPlaylistHandler playlistTreeHandler, State::IMessageHandler messageHandler, IVideoFileStorehandler fileStorehandler, IVideoHandler videoHandler, IVideoThumnailUtility videoThumnailUtility, ILocalSettingHandler settingHandler)
        {
            this.wacthPagehandler = watchPageHandler;
            this.playlistTreeHandler = playlistTreeHandler;
            this.messageHandler = messageHandler;
            this.fileStorehandler = fileStorehandler;
            this.videoHandler = videoHandler;
            this.videoThumnailUtility = videoThumnailUtility;
            this.settingHandler = settingHandler;
        }


        /// <summary>
        /// 動画を追加する
        /// </summary>
        /// <param name="videoIds"></param>
        /// <param name="playlistId"></param>
        /// <param name="onfailed"></param>
        /// <param name="onSucceed"></param>
        /// <returns></returns>
        public async Task<INetworkResult> AddVideosAsync(IEnumerable<string> videoIds, int playlistId, Action<IResult> onfailed, Action<IListVideoInfo> onSucceed)
        {
            var playlist = this.playlistTreeHandler.GetPlaylist(playlistId);

            var netResult = new NetworkResult();

            if (playlist is null) return netResult;

            int videosCount = videoIds.Count();

            if (videosCount == 0) return netResult;

            this.messageHandler.AppendMessage($"{videosCount}件の動画を追加します。");

            await this.GetVideoListInfosAsync(videoIds.Copy(), async (newVideo, video, i, lockObj) =>
            {
                lock (lockObj)
                {
                    netResult.SucceededCount++;
                }
                await this.videoThumnailUtility.SetThumbPathAsync(newVideo);
                int id = this.videoHandler.AddVideo(newVideo, playlist.Id);
                newVideo.Id = id;
                this.messageHandler.AppendMessage($"{video.NiconicoId}を追加しました。({i + 1}/{videosCount})");
                onSucceed(newVideo);
            }, (result, video) =>
            {
                netResult.FailedCount++;
                onfailed(result);
                this.messageHandler.AppendMessage($"動画の取得に失敗しました。(詳細: {result.Message})");
            }, (video, index) =>
            {
                this.messageHandler.AppendMessage($"{video.NiconicoId}の動画情報を取得中です...({index + 1}/{videosCount})");
            }, video =>
            {
                video.Message = "待機中...(15s)";
                this.messageHandler.AppendMessage("待機中...(15s)");
            });

            if (netResult.SucceededCount == videosCount) netResult.IsSucceededAll = true;

            this.messageHandler.AppendMessage($"動画の追加処理が完了しました。({netResult.SucceededCount}件)");
            this.messageHandler.AppendMessage("-".Repeat(50));

            return netResult;
        }

        /// <summary>
        /// 情報を取得済の動画を追加する
        /// </summary>
        /// <param name="videos"></param>
        /// <param name="playlistId"></param>
        /// <returns></returns>
        public async Task AddVideosAsync(IEnumerable<IListVideoInfo> videos, int playlistId)
        {
            var playlist = this.playlistTreeHandler.GetPlaylist(playlistId);

            if (playlist is null) return;

            videos = videos.Where(v => !playlist.HasVideo(v.NiconicoId)).Copy();
            int videoCount = videos.Count();
            int i = 0;

            this.messageHandler.AppendMessage($"{videoCount}件の動画を追加します。");

            foreach (var video in videos)
            {
                await this.videoThumnailUtility.SetThumbPathAsync(video);
                this.videoHandler.AddVideo(video, playlistId);
                this.messageHandler.AppendMessage($"{video.NiconicoId}を追加しました。({i + 1}/{videoCount})");
                i++;
            }

            this.messageHandler.AppendMessage($"動画の追加処理が完了しました。({videoCount}件)");
        }

        /// <summary>
        /// 動画情報を更新する
        /// </summary>
        /// <param name="videos"></param>
        /// <param name="folderPath"></param>
        /// <param name="onSucceeded"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<INetworkResult> UpdateVideosAsync(IEnumerable<IListVideoInfo> videos, string folderPath, Action<IListVideoInfo> onSucceeded, CancellationToken ct)
        {
            videos = videos.Copy();
            var netResult = new NetworkResult();
            int videosCount = videos.Count();

            this.messageHandler.AppendMessage($"{videosCount}件の動画情報を更新します。");

            await this.GetVideoListInfosAsync(videos.Copy(), async (newVideo, video, i, lockObj) =>
            {
                lock (lockObj)
                {
                    netResult.SucceededCount++;
                }
                video.IsSelected = false;
                video.Message = "情報を更新しました。";
                newVideo.Id = video.Id;
                newVideo.Message = video.Message;
                await this.videoThumnailUtility.SetThumbPathAsync(newVideo);
                this.videoHandler.Update(newVideo);
                this.messageHandler.AppendMessage($"{video.NiconicoId}の情報を更新しました。({i + 1}/{videosCount})");
                onSucceeded(newVideo);
            }, (result, video) =>
            {
                netResult.FailedCount++;
                video.Message = "情報の更新に失敗しました。";
                this.messageHandler.AppendMessage($"{video.NiconicoId}の情報を更新に失敗しました。(詳細: {result.Message ?? "None"})");
            }, (video, i) =>
            {
                video.Message = "情報を取得中...";
                this.messageHandler.AppendMessage($"情報を取得中...({i + 1}/{videosCount})");
            },
            video =>
            {
                video.Message = "待機中...(15s)";
                this.messageHandler.AppendMessage("待機中...(15s)");
            });

            if (netResult.SucceededCount == videosCount) netResult.IsSucceededAll = true;

            this.messageHandler.AppendMessage("-".Repeat(50));

            return netResult;
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
        public async Task<IEnumerable<IListVideoInfo>> GetVideoListInfosAsync(IEnumerable<string> ids, Action<IListVideoInfo, IListVideoInfo, int, object> onSucceeded, Action<IResult, IListVideoInfo> onFailed, Action<IListVideoInfo, int> onStarted, Action<IListVideoInfo> onWaiting)
        {
            return await this.GetVideoListInfosAsync(ids.Select(i => new BindableListVIdeoInfo() { NiconicoId = i }), onSucceeded, onFailed, onStarted, onWaiting);
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
        private async Task<IEnumerable<IListVideoInfo>> GetVideoListInfosAsync(IEnumerable<IListVideoInfo> emptyVideos, Action<IListVideoInfo, IListVideoInfo, int, object> onSucceeded, Action<IResult, IListVideoInfo> onFailed, Action<IListVideoInfo, int> onStarted, Action<IListVideoInfo> onWaiting)
        {

            var registerOnlyID = this.settingHandler.GetBoolSetting(Settings.StoreOnlyNiconicoID);
            if (registerOnlyID)
            {
                return emptyVideos;
            }

            int videosCount = emptyVideos.Count();
            var videos = new List<IListVideoInfo>();
            var maxParallelCount = this.settingHandler.GetIntSetting(Settings.MaxFetchCount);
            var waitInterval = this.settingHandler.GetIntSetting(Settings.FetchSleepInterval);
            if (maxParallelCount < 1) maxParallelCount = 3;
            if (waitInterval < 1) waitInterval = 5;

            var handler = new ParallelTasksHandler<NetworkVideoParallelTask>(maxParallelCount, waitInterval, 15);

            foreach (var item in emptyVideos.Select((video, index) => new { video, index }))
            {

                var task = new NetworkVideoParallelTask(async (_, lockObj, _) =>
                {
                    onStarted(item.video, item.index);

                    IResult result = await this.wacthPagehandler.TryGetVideoInfoAsync(item.video.NiconicoId, item.video, DWatch::WatchInfoOptions.NoDmcData);

                    if (result.IsSucceeded)
                    {
                        onSucceeded(item.video, item.video, item.index, lockObj);
                        videos.Add(item.video);
                    }
                    else
                    {
                        onFailed(result, item.video);
                    }
                }, index => { onWaiting(item.video); });

                handler.AddTaskToQueue(task);

            }

            await handler.ProcessTasksAsync();


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
