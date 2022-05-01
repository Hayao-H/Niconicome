using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Local.State;
using Niconicome.Models.Playlist.VideoList;
using Niconicome.Models.Utils;
using Reactive.Bindings;

namespace Niconicome.Models.Network.Download.DLTask
{
    public interface IDownloadManager
    {
        /// <summary>
        /// ダウンロードタスク
        /// </summary>
        ReadOnlyObservableCollection<IDownloadTask> Queue { get; init; }

        /// <summary>
        /// ステージ済みタスク
        /// </summary>
        ReadOnlyObservableCollection<IDownloadTask> Staged { get; init; }

        /// <summary>
        /// キャンセル済みを表示
        /// </summary>
        ReactiveProperty<bool> DisplayCanceled { get; }

        /// <summary>
        /// 完了済みを表示
        /// </summary>
        ReactiveProperty<bool> DisplayCompleted { get; }

        /// <summary>
        /// ダウンロード中
        /// </summary>
        ReadOnlyReactiveProperty<bool> IsProcessing { get; }

        /// <summary>
        /// 動画をステージ
        /// </summary>
        void StageVIdeo();

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
        /// ダウンロードを開始する
        /// </summary>
        /// <returns></returns>
        Task StartDownloadAsync(Action<string> onMessage, Action<string> onMessageVerbose);
    }

    public class DownloadManager : IDownloadManager
    {

        public DownloadManager(ILocalSettingsContainer settingsContainer, IVideoListContainer videoListContainer, IDownloadSettingsHandler settingHandler, ILogger logger, ICurrent current)
        {
            this.Queue = this._queuePool.Tasks;
            this.Staged = this._stagedPool.Tasks;
            this.DisplayCanceled = this._queuePool.DisplayCanceled;
            this.DisplayCompleted = this._queuePool.DisplayCompleted;
            this.IsProcessing = this._isProcessingSource.ToReadOnlyReactiveProperty();
            this._settingsContainer = settingsContainer;
            this._videoListContainer = videoListContainer;
            this._settingsHandler = settingHandler;
            this._logger = logger;
            this._current = current;

            this.RegisterParallelTasksHandler();

        }

        #region field

        private readonly ILogger _logger;

        private readonly ILocalSettingsContainer _settingsContainer;

        private readonly ICurrent _current;

        private readonly IDownloadSettingsHandler _settingsHandler;

        private readonly IVideoListContainer _videoListContainer;

        private readonly ReactiveProperty<bool> _isProcessingSource = new();

        private ReactiveProperty<int>? _maxParallelDL;

        private ReactiveProperty<int>? _sleepInterval;

        private ParallelTasksHandler<IDownloadTask>? _tasksHandler;

        private readonly IDownloadTaskPool _queuePool = new DownloadTaskPool();

        private readonly IDownloadTaskPool _stagedPool = new DownloadTaskPool();

        #endregion

        #region Props

        public ReadOnlyObservableCollection<IDownloadTask> Queue { get; init; }

        public ReadOnlyObservableCollection<IDownloadTask> Staged { get; init; }

        public ReactiveProperty<bool> DisplayCanceled { get; init; }

        public ReactiveProperty<bool> DisplayCompleted { get; init; }

        public ReadOnlyReactiveProperty<bool> IsProcessing { get; init; }

        #endregion

        #region Method


        public void StageVIdeo()
        {
            DownloadSettings settings = this._settingsHandler.CreateDownloadSettings();

            foreach (var video in this._videoListContainer.Videos.Where(v => v.IsSelected.Value).ToList())
            {
                //動画固有の情報を設定
                settings.NiconicoId = video.NiconicoId.Value;
                settings.IsEconomy = video.IsEconomy.Value;
                settings.FilePath = video.FileName.Value;

                //タスクを作成
                IDownloadTask task = DIFactory.Provider.GetRequiredService<IDownloadTask>();
                task.Initialize(video, settings);

                this._stagedPool.AddTask(task);
            }
        }

        public　void ClearStaged()
        {
            this._stagedPool.Clear();
        }

        public　void RemoveFromStaged(IDownloadTask task)
        {
            this._stagedPool.RemoveTask(task);
        }



        public async Task StartDownloadAsync(Action<string> onMessage, Action<string> onMessageVerbose)
        {
            //ダウンロード中ならキャンセル
            if (this.IsProcessing.Value) return;

            //ステージ済みをキューに移動
            this.MoveStagedToQueue();
            var videoCount = this._tasksHandler!.PallarelTasks.Count;
            IDownloadTask[]? tmp = this._tasksHandler.PallarelTasks.ToArray();

            //タスクが0なら中止
            if (videoCount == 0) return;

            //DL開始
            this._isProcessingSource.Value = true;
            onMessageVerbose($"動画のダウンロードを開始します。({videoCount}件)");
            onMessage($"動画のダウンロードを開始します。({videoCount}件)");


            try
            {
                await this._tasksHandler.ProcessTasksAsync();
            }
            catch (Exception e)
            {
                this._logger.Error("ダウンロード中にエラーが発生しました", e);
                onMessageVerbose($"ダウンロード中にエラーが発生しました。(詳細: {e.Message})");
                onMessage($"ダウンロード中にエラーが発生しました。");
                this._isProcessingSource.Value = false;
                return;
            }

            //結果判定
            int succeededCount = tmp.Select(t => t.IsSuceeded).Count();

            if (succeededCount == 0)
            //1件もできなかった
            {
                onMessageVerbose("動画を1件もダウンロード出来ませんでした。");
                onMessage("動画を1件もダウンロード出来ませんでした。");
            }
            else
            {
                string niconicoID = tmp.First().NiconicoID;

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

            this._isProcessingSource.Value = false;
            tmp = null;
        }

        public void CancelDownload()
        {
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
            bool downloadFromAnotherPlaylist = this._settingsContainer.GetReactiveBoolSetting(SettingsEnum.DLAllFromQueue).Value;
            int playlistID = this._current.SelectedPlaylist.Value?.Id ?? -1;

            if (downloadFromAnotherPlaylist || playlistID == -1)
            {
                foreach (var t in this._stagedPool.Tasks)
                {
                    this._queuePool.AddTask(t);
                    this._tasksHandler!.AddTaskToQueue(t);
                }
                this._queuePool.Clear();

            }
            else
            {
                foreach (var t in this._stagedPool.Tasks.Where(t => t.PlaylistID == playlistID).ToList())
                {
                    this._queuePool.AddTask(t);
                    this._tasksHandler!.AddTaskToQueue(t);
                    this._stagedPool.RemoveTask(t);
                }
            }
        }


        /// <summary>
        /// 並行タスクハンドラを設定
        /// </summary>
        private void RegisterParallelTasksHandler()
        {
            this._maxParallelDL = this._settingsContainer.GetReactiveIntSetting(SettingsEnum.MaxParallelDL, null, value => value < 0 ? NetConstant.DefaultMaxParallelDownloadCount : value);

            this._sleepInterval = this._settingsContainer.GetReactiveIntSetting(SettingsEnum.FetchSleepInterval, null, value => value < 0 ? NetConstant.DefaultFetchWaitInterval : value);

            this._tasksHandler = new ParallelTasksHandler<IDownloadTask>(this._maxParallelDL.Value, this._sleepInterval.Value, 15, untilEmpty: true);

            Observable.Merge(this._maxParallelDL, this._sleepInterval).Subscribe(_ =>
            {
                if (this._tasksHandler.IsProcessing) return;
                this._tasksHandler = new ParallelTasksHandler<IDownloadTask>(this._maxParallelDL.Value, this._sleepInterval.Value, 15, untilEmpty: true);
            });
        }

        #endregion


    }
}
