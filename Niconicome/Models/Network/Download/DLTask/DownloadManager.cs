using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Local.State;
using Niconicome.Models.Playlist.V2;
using Niconicome.Models.Playlist.VideoList;
using Niconicome.Models.Utils;
using Niconicome.Models.Utils.ParallelTaskV2;
using Niconicome.Models.Utils.Reactive;
using Niconicome.Models.Utils.Reactive.State;
using Reactive.Bindings;

namespace Niconicome.Models.Network.Download.DLTask
{
    public interface IDownloadManager
    {
        /// <summary>
        /// ダウンロードタスク
        /// </summary>
        IReadOnlyCollection<IDownloadTask> Queue { get; init; }

        /// <summary>
        /// ステージ済みタスク
        /// </summary>
        IReadOnlyCollection<IDownloadTask> Staged { get; init; }

        /// <summary>
        /// キャンセル済みを表示
        /// </summary>
        bool DisplayCanceled { get; set; }

        /// <summary>
        /// 完了済みを表示
        /// </summary>
        bool DisplayCompleted { get; set; }

        /// <summary>
        /// ダウンロード中
        /// </summary>
        IReadonlyBindablePperty<bool> IsProcessing { get; }

        /// <summary>
        /// 変更監視オブジェクト
        /// </summary>
        IStateChangeNotifyer StateChangeNotifyer { get; }

        /// <summary>
        /// 動画をステージ
        /// </summary>
        void StageVIdeo(IVideoInfo video);

        /// <summary>
        /// ステージ済みタスクをクリア
        /// </summary>
        void ClearStaged();

        /// <summary>
        /// 指定したタスクをステージ済みから削除
        /// </summary>
        /// <param name="task"></param>
        void RemoveFromStaged(IDownloadTask task);

        /// <summary>
        /// ダウンロードをキャンセル
        /// </summary>
        void CancelDownload();

        /// <summary>
        /// 完了済みのタスクを削除
        /// </summary>
        void ClearCompleted();

        /// <summary>
        /// ダウンロードを開始する
        /// </summary>
        /// <returns></returns>
        Task StartDownloadAsync(Action<string> onMessage, Action<string> onMessageVerbose);
    }

    public class DownloadManager : IDownloadManager
    {

        public DownloadManager(ISettingsContainer settingsContainer, IPlaylistVideoContainer videoListContainer, IDownloadSettingsHandler settingHandler, ILogger logger)
        {
            this.Queue = this._queuePool.Tasks;
            this.Staged = this._stagedPool.Tasks;
            this.IsProcessing = this._isProcessingSource.AsReadOnly();
            this._settingsContainer = settingsContainer;
            this._videoListContainer = videoListContainer;
            this._settingsHandler = settingHandler;
            this._logger = logger;

            this._queuePool.StateChangeNotifyer.Subscribe(() => this.StateChangeNotifyer.RaiseChange());
            this._stagedPool.StateChangeNotifyer.Subscribe(() => this.StateChangeNotifyer.RaiseChange());

            this.InitializeParallelTasksHandler();

        }

        #region field

        private readonly ILogger _logger;

        private readonly ISettingsContainer _settingsContainer;

        private readonly IDownloadSettingsHandler _settingsHandler;

        private readonly IPlaylistVideoContainer _videoListContainer;

        private readonly List<IDownloadTask> _processingTasks = new();

        private readonly IBindableProperty<bool> _isProcessingSource = new BindableProperty<bool>(false);

        private ISettingInfo<int>? _maxParallelDL;

        private ISettingInfo<int>? _sleepInterval;

        private ParallelTasksHandler? _tasksHandler;

        private readonly IDownloadTaskPool _queuePool = new DownloadTaskPool();

        private readonly IDownloadTaskPool _stagedPool = new DownloadTaskPool();

        private CancellationTokenSource? _cts;

        #endregion

        #region Props

        public IReadOnlyCollection<IDownloadTask> Queue { get; init; }

        public IReadOnlyCollection<IDownloadTask> Staged { get; init; }

        public bool DisplayCanceled
        {
            get => this._queuePool.DisplayCanceled;
            set => this._queuePool.DisplayCanceled = value;
        }

        public bool DisplayCompleted
        {
            get => this._queuePool.DisplayCompleted;
            set => this._queuePool.DisplayCompleted = value;
        }

        public IReadonlyBindablePperty<bool> IsProcessing { get; init; }

        public IStateChangeNotifyer StateChangeNotifyer { get; init; } = new StateChangeNotifyer();


        #endregion

        #region Method


        public void StageVIdeo(IVideoInfo video)
        {
            DownloadSettings settings = this._settingsHandler.CreateDownloadSettings();

            //動画固有の情報を設定
            settings.NiconicoId = video.NiconicoId;
            settings.IsEconomy = video.IsEconomy;
            settings.FilePath = video.FilePath;

            //タスクを作成
            IDownloadTask task = DIFactory.Provider.GetRequiredService<IDownloadTask>();
            task.Initialize(video, settings);

            this._stagedPool.AddTask(task);
        }

        public void ClearStaged()
        {
            this._stagedPool.Clear();
        }

        public void RemoveFromStaged(IDownloadTask task)
        {
            this._stagedPool.RemoveTask(task);
        }

        public void ClearCompleted()
        {
            IDownloadTask[] targets = this._queuePool.Tasks.Where(t => t.IsCompleted.Value).ToArray();
            foreach (var target in targets)
            {
                this._queuePool.RemoveTask(target);
            }
        }

        public async Task StartDownloadAsync(Action<string> onMessage, Action<string> onMessageVerbose)
        {
            void Finalize()
            {
                this._isProcessingSource.Value = false;
                this._cts = null;
                this._processingTasks.Clear();
            }

            //ダウンロード中ならキャンセル
            if (this.IsProcessing.Value) return;

            //初期化
            this._processingTasks.Clear();

            //ステージ済みをキューに移動
            this.MoveStagedToQueue();
            var videoCount = this._tasksHandler!.PallarelTasks.Count;
            this._processingTasks.AddRange(this._queuePool.Tasks);

            //タスクが0なら中止
            if (videoCount == 0) return;

            //DL開始
            this._isProcessingSource.Value = true;
            onMessageVerbose($"動画のダウンロードを開始します。({videoCount}件)");
            onMessage($"動画のダウンロードを開始します。({videoCount}件)");

            //トークン生成
            this._cts = new CancellationTokenSource();

            try
            {
                await this._tasksHandler.ProcessTasksAsync(ct: this._cts.Token);
            }
            catch (Exception e)
            {
                this._logger.Error("ダウンロード中にエラーが発生しました", e);
                onMessageVerbose($"ダウンロード中にエラーが発生しました。(詳細: {e.Message})");
                onMessage($"ダウンロード中にエラーが発生しました。");
                Finalize();
                return;
            }

            //結果判定
            int succeededCount = this._processingTasks.Where(t => t.IsSuceeded).Count();

            if (succeededCount == 0)
            //1件もできなかった
            {
                onMessageVerbose("動画を1件もダウンロード出来ませんでした。");
                onMessage("動画を1件もダウンロード出来ませんでした。");
            }
            else
            {
                string niconicoID = this._processingTasks.First().NiconicoID;

                if (succeededCount > 1)
                //2件以上DLできた
                {
                    onMessageVerbose($"{niconicoID}ほか{succeededCount - 1}件の動画をダウンロードしました。");
                    onMessage($"{niconicoID}ほか{succeededCount - 1}件の動画をダウンロードしました。");

                    if (succeededCount < videoCount)
                    {
                        onMessageVerbose($"{videoCount - succeededCount}件の動画のダウンロードに失敗しました。");
                    }
                }
                else if (succeededCount == 1)
                //1件だけDLできた
                {
                    onMessageVerbose($"{niconicoID}をダウンロードしました。");
                    onMessage($"{niconicoID}をダウンロードしました。");
                }
            }

            Finalize();
        }

        public void CancelDownload()
        {
            //並列タスクハンドラ用のトークンをキャンセル
            this._cts?.Cancel();
            this._cts = null;

            //ダウンロード中のタスクをキャンセル
            foreach (var task in this._processingTasks)
            {
                task.Cancel();
            }

            this._tasksHandler!.CancellAllTasks();
            this._isProcessingSource.Value = false;
        }

        #endregion

        #region private

        /// <summary>
        /// ステージング済みタスクをキューに移動
        /// </summary>
        private void MoveStagedToQueue()
        {
            bool downloadFromAnotherPlaylist = this._settingsContainer.GetSetting(SettingNames.DownloadAllWhenPushDLButton, false).Data?.Value ?? false;
            int playlistID = this._videoListContainer.CurrentSelectedPlaylist?.ID ?? -1;

            if (downloadFromAnotherPlaylist || playlistID == -1)
            {
                foreach (var t in this._stagedPool.Tasks)
                {
                    this._queuePool.AddTask(t);
                    this._tasksHandler!.AddTaskToQueue(t);
                }
                this._stagedPool.Clear();

            }
            else
            {
                foreach (var t in this._stagedPool.Tasks.Where(t => t.PlaylistID == playlistID))
                {
                    this._queuePool.AddTask(t);
                    this._tasksHandler!.AddTaskToQueue(t);
                    this._stagedPool.RemoveTask(t);
                }
            }
        }


        /// <summary>
        /// 並行タスクハンドラを初期化
        /// </summary>
        private void InitializeParallelTasksHandler()
        {
            IAttemptResult<ISettingInfo<int>> parallelResult = this._settingsContainer.GetSetting(SettingNames.MaxParallelDownloadCount, NetConstant.DefaultMaxParallelDownloadCount);

            IAttemptResult<ISettingInfo<int>> sleepResult = this._settingsContainer.GetSetting(SettingNames.FetchSleepInterval, NetConstant.DefaultFetchWaitInterval);

            if (this.CheckWhetherGetSettingSucceededOrNot(parallelResult, sleepResult))
            {

                this.RegisterParallelTasksHandler(parallelResult.Data!.Value, sleepResult.Data!.Value);
                this._sleepInterval = sleepResult.Data.WithSubscribe(x => this.RegisterParallelTasksHandler(this._maxParallelDL!.Value, x));
                this._maxParallelDL = parallelResult.Data.WithSubscribe(x => this.RegisterParallelTasksHandler(x, this._sleepInterval!.Value));
            }
        }

        /// <summary>
        /// 並行タスクハンドラを設定
        /// </summary>
        /// <param name="parallelCount"></param>
        /// <param name="sleepInterval"></param>
        private void RegisterParallelTasksHandler(int parallelCount, int sleepInterval)
        {
            if (this.IsProcessing.Value) return;
            this._tasksHandler = new ParallelTasksHandler(parallelCount, sleepInterval, 15, untilEmpty: true);
        }

        private bool CheckWhetherGetSettingSucceededOrNot(IAttemptResult<ISettingInfo<int>> parallelResult, IAttemptResult<ISettingInfo<int>> sleepResult)
        {
            if (!parallelResult.IsSucceeded)
            {
                return false;
            }

            if (parallelResult.Data is null)
            {
                return false;
            }

            if (!sleepResult.IsSucceeded)
            {
                return false;
            }

            if (sleepResult.Data is null)
            {
                return false;
            }

            return true;
        }

        #endregion


    }
}
