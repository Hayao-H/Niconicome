using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Domain.Utils;
using Err = Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Domain.Utils.StringHandler;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Playlist.V2;
using Niconicome.Models.Utils.ParallelTaskV2;
using Niconicome.Models.Utils.Reactive;
using Niconicome.Models.Utils.Reactive.State;
using SC = Niconicome.Models.Network.Download.DLTask.StringContent.DownloadManagerSC;
using Niconicome.Models.Network.Download.DLTask.Error;
using Niconicome.Models.Domain.Local.IO.Media.Audio;
using Niconicome.Models.Local.State.MessageV2;
using Niconicome.Models.Local.State.Toast;

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

        /// <summary>
        /// ダウンロードを開始する
        /// </summary>
        /// <returns></returns>
        Task StartDownloadAsync();

    }

    public enum EventType
    {
        DBClick,
        MiddleClick,
    }

    public class DownloadManager : IDownloadManager
    {

        public DownloadManager(ISettingsContainer settingsContainer, IPlaylistVideoContainer videoListContainer, IDownloadSettingsHandler settingHandler, IStringHandler stringHandler, Err::IErrorHandler errorHandler, IAudioPlayer audioPlayer, IMessageHandler messageHandler, IToastHandler toastHandler)
        {
            this.Queue = this._queuePool.Tasks;
            this.Staged = this._stagedPool.Tasks;
            this.IsProcessing = this._isProcessingSource.AsReadOnly();
            this._settingsContainer = settingsContainer;
            this._videoListContainer = videoListContainer;
            this._settingsHandler = settingHandler;
            this._stringHandler = stringHandler;
            this._errorHandler = errorHandler;
            this._audioPlayer = audioPlayer;
            this._messageHandler = messageHandler;
            this._toastHandler = toastHandler;

            this._queuePool.StateChangeNotifyer.Subscribe(() => this.StateChangeNotifyer.RaiseChange());
            this._stagedPool.StateChangeNotifyer.Subscribe(() => this.StateChangeNotifyer.RaiseChange());

            this.InitializeParallelTasksHandler();
        }

        #region field

        private readonly Err::IErrorHandler _errorHandler;

        private readonly IStringHandler _stringHandler;

        private readonly ISettingsContainer _settingsContainer;

        private readonly IDownloadSettingsHandler _settingsHandler;

        private readonly IPlaylistVideoContainer _videoListContainer;

        private readonly IMessageHandler _messageHandler;

        private readonly IToastHandler _toastHandler;

        private readonly List<IDownloadTask> _processingTasks = new();

        private readonly IBindableProperty<bool> _isProcessingSource = new BindableProperty<bool>(false);

        private readonly IAudioPlayer _audioPlayer;

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
                this.PlaySound();
            }

            //ダウンロード中ならキャンセル
            if (this.IsProcessing.Value) return;

            //初期化
            this._processingTasks.Clear();

            //ステージ済みをキューに移動
            this.MoveStagedToQueue();
            var videoCount = this._tasksHandler!.PallarelTasks.Count;
            this._processingTasks.AddRange(this._queuePool.Tasks.Where(t => !t.IsCompleted.Value));

            //タスクが0なら中止
            if (videoCount == 0) return;

            //DL開始
            this._isProcessingSource.Value = true;

            string startM = this._stringHandler.GetContent(SC.DownloadHasStarted, videoCount);
            onMessageVerbose(startM);
            onMessage(startM);

            //トークン生成
            this._cts = new CancellationTokenSource();

            try
            {
                await this._tasksHandler.ProcessTasksAsync(ct: this._cts.Token);
            }
            catch (Exception e)
            {
                this._errorHandler.HandleError(DownloadManagerError.Error, e);
                onMessageVerbose(this._stringHandler.GetContent(SC.ErrorD, e.Message));
                onMessage(this._stringHandler.GetContent(SC.Error));
                Finalize();
                return;
            }

            //結果判定
            int succeededCount = this._processingTasks.Where(t => t.IsSuceeded).Count();

            if (succeededCount == 0)
            //1件もできなかった
            {
                string cannotDLM = this._stringHandler.GetContent(SC.CannotDownloadAny);
                onMessageVerbose(cannotDLM);
                onMessage(cannotDLM);
            }
            else
            {

                if (succeededCount > 1)
                //2件以上DLできた
                {
                    string succeededMany = this._stringHandler.GetContent(SC.DownloadedMany, succeededCount);
                    onMessageVerbose(succeededMany);
                    onMessage(succeededMany);

                    if (succeededCount < videoCount)
                    {
                        onMessageVerbose(this._stringHandler.GetContent(SC.SomeDownloadHasFailed, videoCount - succeededCount));
                    }
                }
                else if (succeededCount == 1)
                //1件だけDLできた
                {
                    string niconicoID = this._processingTasks.First().NiconicoID;
                    string succeededOne = this._stringHandler.GetContent(SC.DownloadedOne, niconicoID);
                    onMessageVerbose(succeededOne);
                    onMessage(succeededOne);
                }
            }

            Finalize();
        }

        public async Task StartDownloadAsync()
        {
            await this.StartDownloadAsync(m => this._messageHandler.AppendMessage(m, LocalConstant.SystemMessageDispacher), m => this._toastHandler.Enqueue(m));
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

        /// <summary>
        /// 完了時にサウンドを再生
        /// </summary>
        private void PlaySound()
        {
            IAttemptResult<bool> result = this._settingsContainer.GetOnlyValue(SettingNames.PlaySoundAfterDownload, false);
            if (!result.IsSucceeded || !result.Data) return;

            IAttemptResult<string> path = this._settingsContainer.GetOnlyValue(SettingNames.DownloadCompletionAudioPath, string.Empty);
            if (!path.IsSucceeded || string.IsNullOrEmpty(path.Data)) return;

            this._audioPlayer.Play(path.Data);
        }

        #endregion


    }
}
