using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Niconicome.Extensions;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Local.State;
using Niconicome.Models.Network.Download;
using Niconicome.ViewModels.Mainpage.Utils;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Material = MaterialDesignThemes.Wpf;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage.Subwindows
{
    class DownloadTasksWindowViewModel : BindableBase, IDisposable
    {
        public DownloadTasksWindowViewModel()
        {
            this.StagedTasks = new ObservableCollection<DownloadTaskViewModel>();
            this.Tasks = new ObservableCollection<DownloadTaskViewModel>();

            WS::Mainpage.DownloadTasksHandler.StagedDownloadTaskPool.TaskPoolChange += this.OnStagedTaskChanged;
            WS::Mainpage.DownloadTasksHandler.DownloadTaskPool.TaskPoolChange += this.OnTaskChanged;

            this.RefreshTask(TaskPoolType.Download);
            this.RefreshTask(TaskPoolType.Staged);

            this.disposables = new CompositeDisposable();

            this.RefreshStagedTaskCommand = new CommandBase<object>(_ => true, _ => this.RefreshTask(TaskPoolType.Staged));
            this.RefreshTaskCommand = new CommandBase<object>(_ => true, _ => this.RefreshTask(TaskPoolType.Download));

            this.StartDownloadCommand = WS::Mainpage.Videodownloader.CanDownload
            .ToReactiveCommand()
            .WithSubscribe(async () =>
           {
               await WS::Mainpage.Videodownloader.DownloadVideosFriendly(m => WS::Mainpage.Messagehandler.AppendMessage(m), m => this.Queue.Enqueue(m));
               WS::Mainpage.PostDownloadTasksManager.HandleAction();
           })
            .AddTo(this.disposables);

            this.CancelDownloadCommand = WS::Mainpage.Videodownloader.CanDownload
                .Select(f => !f)
                .ToReactiveCommand()
                .WithSubscribe(() =>
                {
                    WS::Mainpage.DownloadTasksHandler.DownloadTaskPool.CancelAllTasks();
                    this.Queue.Enqueue("ユーザーによってダウンロードがキャンセルされました。");
                    WS::Mainpage.Messagehandler.AppendMessage("ユーザーによってダウンロードがキャンセルされました。");
                })
            .AddTo(this.disposables);

            this.ClearStagedCommand = new CommandBase<object>(_ => true, _ =>
            {
                WS::Mainpage.DownloadTasksHandler.StagedDownloadTaskPool.Clear();
                this.StagedTasks.Clear();
            });

            this.RemoveStagedTaskCommand = new CommandBase<object>(_ => true, _ =>
             {
                 var tasks = this.StagedTasks.Where(t => t.IsChecked);
                 foreach (var task in tasks)
                 {
                     WS::Mainpage.DownloadTasksHandler.StagedDownloadTaskPool.RemoveTask(task.Task);
                 }

                 this.StagedTasks.RemoveAll(t => t.IsChecked);
             });

            this.MoveTasksToQueue = new CommandBase<object>(_ => true, _ =>
            {
                WS::Mainpage.DownloadTasksHandler.MoveStagedToQueue();
                this.StagedTasks.Clear();
            });
        }

        ~DownloadTasksWindowViewModel()
        {
            this.Dispose();
        }

        private bool displayCanceledField;

        private bool displayCompletedField;

        /// <summary>
        /// ステージング済みタスク
        /// </summary>
        public ObservableCollection<DownloadTaskViewModel> StagedTasks { get; init; }

        /// <summary>
        /// タスク
        /// </summary>
        public ObservableCollection<DownloadTaskViewModel> Tasks { get; init; }

        /// <summary>
        /// ステージング済みタスク追加時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStagedTaskChanged(object? sender, TaskPoolChangeEventargs e)
        {
            if (e.ChangeType == TaskPoolChangeType.Add)
            {
                var tvm = new DownloadTaskViewModel(e.Task);
                this.StagedTasks.Add(tvm);
            }
        }

        /// <summary>
        /// タスク追加時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTaskChanged(object? sender, TaskPoolChangeEventargs e)
        {
            if (e.ChangeType == TaskPoolChangeType.Add)
            {
                var tvm = new DownloadTaskViewModel(e.Task);
                this.Tasks.Add(tvm);
            }
        }

        /// <summary>
        /// キャンセル済みを表示
        /// </summary>
        public bool DisplayCanceled { get => this.displayCanceledField; set => this.SetProperty(ref this.displayCanceledField, value); }

        /// <summary>
        /// 完了済みを表示
        /// </summary>
        public bool DisplayCompleted { get => this.displayCompletedField; set => this.SetProperty(ref this.displayCompletedField, value); }

        /// <summary>
        /// タスクを更新する
        /// </summary>
        public CommandBase<object> RefreshTaskCommand { get; init; }

        /// <summary>
        /// ステージング済みタスクを更新
        /// </summary>
        public CommandBase<object> RefreshStagedTaskCommand { get; init; }

        /// <summary>
        /// DLを開始
        /// </summary>
        public ReactiveCommand StartDownloadCommand { get; init; }

        /// <summary>
        /// DLを中止
        /// </summary>
        public ReactiveCommand CancelDownloadCommand { get; init; }

        /// <summary>
        /// ステージング済みをクリア
        /// </summary>
        public CommandBase<object> ClearStagedCommand { get; init; }

        /// <summary>
        /// 削除する
        /// </summary>
        public CommandBase<object> RemoveStagedTaskCommand { get; init; }

        /// <summary>
        /// ステージング済みタスクをキューに追加
        /// </summary>
        public CommandBase<object> MoveTasksToQueue { get; init; }

        /// <summary>
        /// タスクを更新
        /// </summary>
        private void RefreshTask(TaskPoolType type)
        {
            var tasks = type switch
            {
                TaskPoolType.Download => WS::Mainpage.DownloadTasksHandler.DownloadTaskPool.GetAllTasks(),
                _ => WS::Mainpage.DownloadTasksHandler.StagedDownloadTaskPool.GetAllTasks(),
            };

            if (!this.DisplayCanceled)
            {
                tasks = tasks.Where(t => !t.IsCanceled.Value);
            }

            if (!this.DisplayCompleted)
            {
                tasks = tasks.Where(t => !t.IsDone.Value);
            }

            if (type == TaskPoolType.Download)
            {
                this.Tasks.Clear();
            }
            else
            {
                this.StagedTasks.Clear();
            }

            foreach (var task in tasks)
            {
                if (type == TaskPoolType.Download)
                {
                    this.Tasks.Add(new DownloadTaskViewModel(task));
                }
                else
                {
                    this.StagedTasks.Add(new DownloadTaskViewModel(task));
                }
            }
        }

        /// <summary>
        /// メッセージキュー
        /// </summary>
        public ISnackbarHandler Queue { get; init; } = WS::Mainpage.SnackbarHandler.CreateNewHandler();

        /// <summary>
        /// インスタンスを破棄する
        /// </summary>
        public override void Dispose()
        {
            WS::Mainpage.DownloadTasksHandler.StagedDownloadTaskPool.TaskPoolChange -= this.OnStagedTaskChanged;
            WS::Mainpage.DownloadTasksHandler.DownloadTaskPool.TaskPoolChange -= this.OnTaskChanged;
            base.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    class DownloadTasksWindowViewModelD
    {
        public DownloadTasksWindowViewModelD()
        {
            this.StagedTasks = new ObservableCollection<DownloadTaskViewModel>();
            this.Tasks = new ObservableCollection<DownloadTaskViewModel>();
            this.StagedTasks.Add(new DownloadTaskViewModel(new DownloadTask("sm9", "陰陽師", 1, new DownloadSettings()) { IsProcessing = new ReactiveProperty<bool>(true), Message = new ReactiveProperty<string>("初期化完了") }) { IsChecked = true });
            this.Tasks.Add(new DownloadTaskViewModel(new DownloadTask("sm9", "陰陽師", 1, new DownloadSettings()) { IsProcessing = new ReactiveProperty<bool>(true), Message = new ReactiveProperty<string>("初期化完了") }) { IsChecked = true });
        }

        public ObservableCollection<DownloadTaskViewModel> StagedTasks { get; init; }

        public ObservableCollection<DownloadTaskViewModel> Tasks { get; init; }

        public bool DisplayCanceled { get; set; } = true;

        public bool DisplayCompleted { get; set; } = true;

        public CommandBase<object> RefreshTaskCommand { get; init; } = new CommandBase<object>(_ => true, _ => { });

        public CommandBase<object> RefreshStagedTaskCommand { get; init; } = new CommandBase<object>(_ => true, _ => { });

        public ReactiveCommand StartDownloadCommand { get; init; } = new();

        public ReactiveCommand CancelDownloadCommand { get; init; } = new();

        public CommandBase<object> ClearStagedCommand { get; init; } = new CommandBase<object>(_ => true, _ => { });

        public CommandBase<object> RemoveStagedTaskCommand { get; init; } = new CommandBase<object>(_ => true, _ => { });

        public CommandBase<object> MoveTasksToQueue { get; init; } = new CommandBase<object>(_ => true, _ => { });

        public ISnackbarHandler? Queue { get; init; } = null;
    }

    enum TaskPoolType
    {
        Staged,
        Download
    }

    class DownloadTaskViewModel : BindableBase
    {
        public DownloadTaskViewModel(IDownloadTask task)
        {
            this.associatedTask = task;
            this.Message = task.Message.ToReadOnlyReactiveProperty<string>();
            this.IsProcessing = task.IsProcessing.ToReadOnlyReactiveProperty();
            this.IsCancel = task.IsCanceled.ToReadOnlyReactiveProperty();
            this.IsEnd = task.IsDone.ToReadOnlyReactiveProperty();

            var r1 = new ComboboxItem<int>(1080, "1080p");
            var r2 = new ComboboxItem<int>(720, "720p");
            var r3 = new ComboboxItem<int>(480, "480p");
            var r4 = new ComboboxItem<int>(360, "360p");
            var r5 = new ComboboxItem<int>(240, "240p");

            this.SelectableResolutions = new List<ComboboxItem<int>>() { r1, r2, r3, r4, r5 };
            this.selectedResolutionField = task.DownloadSettings.VerticalResolution switch
            {
                1080 => r1,
                720 => r2,
                480 => r3,
                240 => r4,
                _ => r1
            };


            this.CancelCommand = new[] {
                    this.IsCancel,
                    this.IsEnd,
                }
                .CombineLatestValuesAreAllFalse()
                .ToReactiveCommand()
                .WithSubscribe(() =>
                {
                    this.associatedTask.Cancel();
                }).AddTo(this.disposables);

        }

        private readonly IDownloadTask associatedTask;

        private bool isCheckedField;

        private ComboboxItem<int> selectedResolutionField;

        public List<ComboboxItem<int>> SelectableResolutions { get; init; }

        /// <summary>
        /// タスク
        /// </summary>
        public IDownloadTask Task { get => this.associatedTask; }

        /// <summary>
        /// 選択された解像度
        /// </summary>
        public ComboboxItem<int> SelectedResolution
        {
            get => this.selectedResolutionField;
            set
            {
                this.SetProperty(ref this.selectedResolutionField, value);
                this.associatedTask.DownloadSettings.VerticalResolution = (uint)value.Value;
            }
        }

        /// <summary>
        /// 動画タイトル
        /// </summary>
        public string Title { get => this.associatedTask.Title; }

        /// <summary>
        /// ニコニコのID
        /// </summary>
        public string NiconicoID { get => this.associatedTask.NiconicoID; }

        /// <summary>
        /// 処理中フラグ
        /// </summary>
        public ReadOnlyReactiveProperty<bool> IsProcessing { get; }

        /// <summary>
        /// 終了フラグ
        /// </summary>
        public ReadOnlyReactiveProperty<bool> IsEnd { get; init; }

        /// <summary>
        /// キャンセルフラグ
        /// </summary>
        public ReadOnlyReactiveProperty<bool> IsCancel { get; }

        /// <summary>
        /// 状態
        /// </summary>
        public ReadOnlyReactiveProperty<string> Message { get; }

        /// <summary>
        /// 選択フラグ
        /// </summary>
        public bool IsChecked { get => this.isCheckedField; set => this.SetProperty(ref this.isCheckedField, value); }

        /// <summary>
        /// 処理をキャンセル
        /// </summary>
        public ReactiveCommand CancelCommand { get; init; }


    }
}
