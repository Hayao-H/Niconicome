using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Niconico.Download;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Local.State;
using Niconicome.Models.Playlist;
using Niconicome.Models.Playlist.Playlist;
using Niconicome.Models.Playlist.VideoList;
using Niconicome.Models.Utils;
using Niconicome.ViewModels;
using Reactive.Bindings;

namespace Niconicome.Models.Network.Download
{
    public interface IContentDownloader
    {
        Task DownloadVideosFriendlyAsync(Action<string> onMessage, Action<string> onMessageShort);
        ReactiveProperty<bool> CanDownload { get; }
        void Cancel();
    }

    /// <summary>
    /// DL実処理
    /// </summary>
    class ContentDownloader : BindableBase, IContentDownloader
    {

        public ContentDownloader(ILogger logger, IMessageHandler messageHandler, IDownloadTasksHandler downloadTasksHandler, IContentDownloadHelper downloadHelper, IPlaylistHandler playlistHandler, ILightVideoListinfoHandler lightVideoListinfoHandler, IVideoInfoContainer videoInfoContainer, IVideoListContainer container, ILocalSettingsContainer settingsContainer)
        {
            this._logger = logger;
            this._messageHandler = messageHandler;
            this._downloadTasksHandler = downloadTasksHandler;
            this._downloadHelper = downloadHelper;
            this._playlistHandler = playlistHandler;
            this._lightVideoListinfoHandler = lightVideoListinfoHandler;
            this._settingsContainer = settingsContainer;
            this._videoInfoContainer = videoInfoContainer;
            this._container = container;

            this.RegisterParallelTasksHandler();
            this._downloadTasksHandler.DownloadTaskPool.RegisterAddHandler(this.DownloadTaskPoolChangedEventHandler);
        }

        #region fieldド
        private readonly ILogger _logger;

        private readonly IMessageHandler _messageHandler;

        private readonly IDownloadTasksHandler _downloadTasksHandler;

        private readonly IContentDownloadHelper _downloadHelper;

        private readonly IPlaylistHandler _playlistHandler;

        private readonly ILightVideoListinfoHandler _lightVideoListinfoHandler;

        private readonly IVideoInfoContainer _videoInfoContainer;

        private readonly IVideoListContainer _container;

        private readonly ILocalSettingsContainer _settingsContainer;

        private ParallelTasksHandler<IDownloadTask>? parallelTasksHandler;

        private ReactiveProperty<int>? sleepInterval;

        private ReactiveProperty<int>? maxParallelDL;

        private CancellationTokenSource? cts;

        private int succeededVideos;

        private IListVideoInfo? lastVideo;
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
        public async Task DownloadVideosFriendlyAsync(Action<string> onMessage, Action<string> onMessageShort)
        {
            var videoCount = this.parallelTasksHandler!.PallarelTasks.Count;

            if (videoCount == 0) return;

            onMessage($"動画のダウンロードを開始します。({videoCount}件)");
            onMessageShort($"動画のダウンロードを開始します。({videoCount}件)");

            IAttemptResult<(IListVideoInfo?, int)>? result;

            try
            {
                result = await this.DownloadVideos();

            }
            catch (Exception e)
            {
                this._logger.Error("ダウンロード中にエラーが発生しました", e);
                onMessage($"ダウンロード中にエラーが発生しました。(詳細: {e.Message})");
                onMessageShort($"ダウンロード中にエラーが発生しました。");
                return;
            }

            if (!result.IsSucceeded)
            {
                onMessage($"ダウンロード出来ませんでした。");
                onMessageShort($"ダウンロード出来ませんでした。");
            }
            else
            {
                var (video, succeededCount) = result.Data;


                if (succeededCount > 1)
                {
                    if (video is not null)
                    {
                        onMessage($"{video.NiconicoId.Value}ほか{succeededCount - 1}件の動画をダウンロードしました。");
                        onMessageShort($"{video.NiconicoId.Value}ほか{succeededCount - 1}件の動画をダウンロードしました。");

                        if (succeededCount < videoCount)
                        {
                            onMessage($"{videoCount - succeededCount}件の動画のダウンロードに失敗しました。");
                        }
                    }
                }
                else if (succeededCount == 1)
                {
                    if (video is not null)
                    {
                        onMessage($"{video.NiconicoId.Value}をダウンロードしました。");
                        onMessageShort($"{video.NiconicoId.Value}をダウンロードしました。");
                    }
                }
            }

        }

        /// <summary>
        /// ダウンロードをキャンセル
        /// </summary>
        public void Cancel()
        {
            this.parallelTasksHandler!.CancellAllTasks();
            this._downloadTasksHandler.DownloadTaskPool.Clear();
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
        private async Task<IAttemptResult<(IListVideoInfo?, int)>> DownloadVideos()
        {
            if (this.parallelTasksHandler!.IsProcessing)
            {
                return AttemptResult<(IListVideoInfo?, int)>.Fail();
            }

            if (this.cts is null)
            {
                this.cts = new CancellationTokenSource();
            }

            await this.parallelTasksHandler.ProcessTasksAsync(() => this.CanDownload.Value = !this.parallelTasksHandler.IsProcessing, ct: this.cts.Token);

            this.CanDownload.Value = !this.parallelTasksHandler.IsProcessing;

            var result = AttemptResult<(IListVideoInfo?, int)>.Succeeded((this.lastVideo, this.succeededVideos));

            this.lastVideo = null;
            this.succeededVideos = 0;

            return result;
        }

        /// <summary>
        /// イベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownloadTaskPoolChangedEventHandler(IDownloadTask task)
        {
            task.SetFuncctions(async (self, lockObj) =>
            {
                if (!self.IsCanceled.Value && !self.IsDone.Value)
                {

                    var setting = self.DownloadSettings;

                    this._messageHandler.AppendMessage($"{self.NiconicoID}のダウンロード処理を開始しました。");

                    string folderPath = setting.FolderPath;
                    self.IsProcessing.Value = true;

                    IListVideoInfo video = this._videoInfoContainer.GetVideo(self.NiconicoID);

                    IAttemptResult<IDownloadContext> result = await this._downloadHelper.TryDownloadContentAsync(video, setting with { NiconicoId = self.NiconicoID, IsEconomy = self.IsEconomyFile, FilePath = self.FilePath }, msg => self.Message.Value = msg, self.CancellationToken);


                    if (!result.IsSucceeded || result.Data is null)
                    {
                        this._messageHandler.AppendMessage($"{self.NiconicoID}のダウンロードに失敗しました。");
                        this._messageHandler.AppendMessage($"詳細: {result.Message}");

                        if (video is not null && self.DownloadSettings.SaveFailedHistory)
                        {
                            IAttemptResult<ITreePlaylistInfo> pResult = this._playlistHandler.GetSpecialPlaylist(SpecialPlaylistTypes.DLFailedHistory);
                            if (pResult.IsSucceeded && pResult.Data is not null)
                            {
                                this._playlistHandler.WireVideoToPlaylist(video.Id.Value, pResult.Data.Id);
                            }
                        }
                    }
                    else
                    {
                        string rMessage = result.Data.ActualVerticalResolution == 0 ? string.Empty : $"(vertical:{result.Data.ActualVerticalResolution}px)";
                        this._messageHandler.AppendMessage($"{self.NiconicoID}のダウンロードに成功しました。");

                        if (!string.IsNullOrEmpty(result.Data.FileName))
                        {
                            video.FileName.Value = result.Data.FileName;
                        }
                        this._container.Update(video, self.PlaylistID);

                        this._lightVideoListinfoHandler.GetLightVideoListInfo(self.NiconicoID, self.PlaylistID).IsSelected.Value = false;

                        lock (lockObj)
                        {
                            this.succeededVideos++;
                            this.lastVideo = video;
                        }

                        self.Message.Value = $"ダウンロード完了{rMessage}";

                        if (video is not null && self.DownloadSettings.SaveSucceededHistory)
                        {
                            IAttemptResult<ITreePlaylistInfo> pResult = this._playlistHandler.GetSpecialPlaylist(SpecialPlaylistTypes.DLSucceedeeHistory);
                            if (pResult.IsSucceeded && pResult.Data is not null)
                            {
                                this._playlistHandler.WireVideoToPlaylist(video.Id.Value, pResult.Data.Id);
                            }
                        }
                    }



                    self.IsProcessing.Value = false;
                    if (!self.IsCanceled.Value)
                    {
                        self.IsDone.Value = true;
                    }

                }
            }, _ =>
            {
                task.Message.Value = "待機中...(15s)";
                this._messageHandler.AppendMessage($"待機中...(15s)");

            });

            this.parallelTasksHandler!.AddTaskToQueue(task);

        }


        #endregion

        #region private

        private void RegisterParallelTasksHandler()
        {
            this.maxParallelDL = this._settingsContainer.GetReactiveIntSetting(SettingsEnum.MaxParallelDL, null, value => value < 0 ? NetConstant.DefaultMaxParallelDownloadCount : value);

            this.sleepInterval = this._settingsContainer.GetReactiveIntSetting(SettingsEnum.FetchSleepInterval, null, value => value < 0 ? NetConstant.DefaultFetchWaitInterval : value);

            this.parallelTasksHandler = new ParallelTasksHandler<IDownloadTask>(this.maxParallelDL.Value, this.sleepInterval.Value, 15, untilEmpty: true);

            Observable.Merge(this.maxParallelDL, this.sleepInterval).Subscribe(_ =>
            {
                if (this.parallelTasksHandler.IsProcessing) return;
                this.parallelTasksHandler = new ParallelTasksHandler<IDownloadTask>(this.maxParallelDL.Value, this.sleepInterval.Value, 15, untilEmpty: true);
            });
        }

        #endregion
    }

}
