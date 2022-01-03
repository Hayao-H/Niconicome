using System;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Extensions;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Local.Machine;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Local.State;
using Niconicome.Models.Playlist;
using Niconicome.Models.Playlist.Playlist;
using Niconicome.Models.Utils;
using Niconicome.ViewModels;
using Reactive.Bindings;

namespace Niconicome.Models.Network.Download
{
    public interface IContentDownloader
    {
        Task<INetworkResult?> DownloadVideosFriendly(Action<string> onMessage, Action<string> onMessageShort);
        ReactiveProperty<bool> CanDownload { get; }
        void Cancel();
    }

    /// <summary>
    /// DL実処理
    /// </summary>
    class ContentDownloader : BindableBase, IContentDownloader
    {

        public ContentDownloader(ILocalSettingHandler settingHandler, ILogger logger, IMessageHandler messageHandler, IVideoHandler videoHandler, IDownloadTasksHandler downloadTasksHandler, IContentDownloadHelper downloadHelper, IPlaylistHandler playlistHandler, ILightVideoListinfoHandler lightVideoListinfoHandler)
        {
            this.settingHandler = settingHandler;
            this.logger = logger;
            this.videoHandler = videoHandler;
            this.messageHandler = messageHandler;
            this.downloadTasksHandler = downloadTasksHandler;
            this.downloadHelper = downloadHelper;
            this.playlistHandler = playlistHandler;
            this.lightVideoListinfoHandler = lightVideoListinfoHandler;

            int maxParallel = this.settingHandler.GetIntSetting(SettingsEnum.MaxParallelDL);
            var sleepInterval = this.settingHandler.GetIntSetting(SettingsEnum.FetchSleepInterval);
            if (maxParallel <= 0)
            {
                maxParallel = 3;
            }
            if (sleepInterval <= 0)
            {
                sleepInterval = 5;
            }
            this.parallelTasksHandler = new ParallelTasksHandler<IDownloadTask>(maxParallel, sleepInterval, 15, untilEmpty: true);
            this.downloadTasksHandler.DownloadTaskPool.RegisterAddHandler(this.DownloadTaskPoolChangedEventHandler);
        }

        #region フィールド
        private readonly ILogger logger;

        private readonly ILocalSettingHandler settingHandler;

        private readonly IVideoHandler videoHandler;

        private readonly IMessageHandler messageHandler;

        private readonly IDownloadTasksHandler downloadTasksHandler;

        private readonly IContentDownloadHelper downloadHelper;

        private readonly IPlaylistHandler playlistHandler;

        private readonly ILightVideoListinfoHandler lightVideoListinfoHandler;

        private readonly ParallelTasksHandler<IDownloadTask> parallelTasksHandler;

        private INetworkResult? currentResult;

        private CancellationTokenSource? cts;
        #endregion

        #region Props

        /// <summary>
        /// DL可能フラグ
        /// </summary>
        public ReactiveProperty<bool> CanDownload { get; init; } = new(true);

        #endregion

        #region Methods

        /// <summary>
        /// メッセージを表示しながらDL
        /// </summary>
        /// <param name="onMessage"></param>
        /// <param name="onMessageShort"></param>
        /// <returns></returns>
        public async Task<INetworkResult?> DownloadVideosFriendly(Action<string> onMessage, Action<string> onMessageShort)
        {
            var videoCount = this.parallelTasksHandler.PallarelTasks.Count;

            onMessage($"動画のダウンロードを開始します。({videoCount}件)");
            onMessageShort($"動画のダウンロードを開始します。({videoCount}件)");

            INetworkResult? result = null;

            try
            {
                result = await this.DownloadVideos();

            }
            catch (Exception e)
            {
                this.logger.Error("ダウンロード中にエラーが発生しました", e);
                onMessage($"ダウンロード中にエラーが発生しました。(詳細: {e.Message})");
                onMessageShort($"ダウンロード中にエラーが発生しました。");
            }

            if (result?.SucceededCount > 1)
            {
                if (result?.FirstVideo is not null)
                {
                    onMessage($"{result.FirstVideo.NiconicoId.Value}ほか{result.SucceededCount - 1}件の動画をダウンロードしました。");
                    onMessageShort($"{result.FirstVideo.NiconicoId.Value}ほか{result.SucceededCount - 1}件の動画をダウンロードしました。");

                    if (!result.IsSucceededAll)
                    {
                        onMessage($"{result.FailedCount}件の動画のダウンロードに失敗しました。");
                    }
                }
            }
            else if (result?.SucceededCount == 1)
            {
                if (result?.FirstVideo is not null)
                {
                    onMessage($"{result.FirstVideo.NiconicoId.Value}をダウンロードしました。");
                    onMessageShort($"{result.FirstVideo.NiconicoId.Value}をダウンロードしました。");
                }
            }
            else
            {
                onMessage($"ダウンロード出来ませんでした。");
                onMessageShort($"ダウンロード出来ませんでした。");
            }

            return result;
        }

        /// <summary>
        /// ダウンロードをキャンセル
        /// </summary>
        public void Cancel()
        {
            this.parallelTasksHandler.CancellAllTasks();
            this.downloadTasksHandler.DownloadTaskPool.Clear();
            this.cts?.Cancel();
            this.cts = null;
            this.CanDownload.Value = !this.parallelTasksHandler.IsProcessing;
        }

        #endregion

        #region private

        /// <summary>
        /// 動画をダウンロードする
        /// </summary>
        /// <param name="videos"></param>
        /// <param name="setting"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task<INetworkResult?> DownloadVideos()
        {
            this.currentResult = new NetworkResult();

            if (this.parallelTasksHandler.IsProcessing)
            {
                return new NetworkResult();
            }

            if (this.cts is null)
            {
                this.cts = new CancellationTokenSource();
            }

            await this.parallelTasksHandler.ProcessTasksAsync(() => this.CanDownload.Value = !this.parallelTasksHandler.IsProcessing, ct: this.cts.Token);

            this.CanDownload.Value = !this.parallelTasksHandler.IsProcessing;

            return this.currentResult;

        }

        /// <summary>
        /// イベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownloadTaskPoolChangedEventHandler(IDownloadTask task)
        {

            if (this.currentResult is null)
            {
                this.currentResult = new NetworkResult();
            }

            task.SetFuncctions(async (parallelTask, lockObj) =>
            {
                if (!task.IsCanceled.Value && !task.IsDone.Value)
                {

                    var setting = task.DownloadSettings;

                    this.messageHandler.AppendMessage($"{task.NiconicoID}のダウンロード処理を開始しました。");

                    string folderPath = setting.FolderPath;
                    task.IsProcessing.Value = true;

                    IDownloadResult downloadResult = await this.downloadHelper.TryDownloadContentAsync(setting with { NiconicoId = task.NiconicoID, IsEconomy = task.IsEconomyFile, FilePath = task.FilePath }, msg => task.Message.Value = msg, task.CancellationToken);

                    IListVideoInfo video = downloadResult.VideoInfo;

                    if (downloadResult.IsCanceled || !downloadResult.IsSucceeded)
                    {
                        if (downloadResult.IsCanceled)
                        {
                            this.currentResult.FailedCount++;
                            this.messageHandler.AppendMessage($"{task.NiconicoID}のダウンロードがキャンセルされました。");
                            task.Message.Value = "ダウンロードをキャンセル";
                        }
                        else
                        {
                            this.currentResult.FailedCount++;
                            this.messageHandler.AppendMessage($"{task.NiconicoID}のダウンロードに失敗しました。");
                            this.messageHandler.AppendMessage($"詳細: {downloadResult.Message}");
                        }

                        bool isFailedHIstoryDisabled = this.settingHandler.GetBoolSetting(SettingsEnum.DisableDLFailedHistory);
                        if (video is not null && !isFailedHIstoryDisabled)
                        {
                            ITreePlaylistInfo? playlist = this.playlistHandler.GetSpecialPlaylist(SpecialPlaylistTypes.DLFailedHistory);
                            if (playlist is not null)
                            {
                                this.playlistHandler.AddVideo(video, playlist.Id);
                            }
                        }
                    }
                    else
                    {
                        string rMessage = downloadResult.VideoVerticalResolution == 0 ? string.Empty : $"(vertical:{downloadResult.VideoVerticalResolution}px)";
                        this.messageHandler.AppendMessage($"{task.NiconicoID}のダウンロードに成功しました。");

                        if (!downloadResult.VideoFileName.IsNullOrEmpty())
                        {
                            video.FileName.Value = downloadResult.VideoFileName;
                        }
                        this.videoHandler.Update(video);

                        this.lightVideoListinfoHandler.GetLightVideoListInfo(task.NiconicoID, task.PlaylistID).IsSelected.Value = false;
                        this.currentResult.SucceededCount++;

                        task.Message.Value = $"ダウンロード完了{rMessage}";

                        bool isSucceededHIstoryDisabled = this.settingHandler.GetBoolSetting(SettingsEnum.DisableDLSucceededHistory);
                        if (video is not null && !isSucceededHIstoryDisabled)
                        {
                            ITreePlaylistInfo? playlist = this.playlistHandler.GetSpecialPlaylist(SpecialPlaylistTypes.DLSucceedeeHistory);
                            if (playlist is not null)
                            {
                                this.playlistHandler.AddVideo(video, playlist.Id);
                            }
                        }
                    }

                    if (this.currentResult.FirstVideo is null)
                    {
                        this.currentResult.FirstVideo = video;
                    }

                    task.IsProcessing.Value = false;
                    if (!task.IsCanceled.Value)
                    {
                        task.IsDone.Value = true;
                    }

                }
            }, _ =>
            {
                task.Message.Value = "待機中...(15s)";
                this.messageHandler.AppendMessage($"待機中...(15s)");

            });

            this.parallelTasksHandler.AddTaskToQueue(task);

        }


        #endregion
    }


    /// <summary>
    /// DL処理の単位
    /// </summary>
    public class DownloadTaskParallel : IParallelTask<DownloadTaskParallel>
    {
        public DownloadTaskParallel(Func<DownloadTaskParallel, object, Task> taskFunction, Action<int> onwait)
        {
            this.TaskFunction = taskFunction;
            this.OnWait = onwait;
        }

        public Guid TaskId { get; init; }

        public int Index { get; set; }

        public Func<DownloadTaskParallel, object, Task> TaskFunction { get; init; }

        public Action<int> OnWait { get; init; }

    }

}
