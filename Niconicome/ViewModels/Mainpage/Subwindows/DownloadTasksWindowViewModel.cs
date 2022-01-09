using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Controls;
using Niconicome.Extensions;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Const;
using Niconicome.Models.Local.State;
using Niconicome.Models.Network.Download;
using Niconicome.Models.Playlist;
using Niconicome.ViewModels.Mainpage.Tabs;
using Niconicome.ViewModels.Mainpage.Utils;
using Prism.Regions;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Helpers;
using Material = MaterialDesignThemes.Wpf;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage.Subwindows
{
    class DownloadTasksWindowViewModel : TabViewModelBase, IDisposable, IDialogAware
    {
        public DownloadTasksWindowViewModel(IRegionManager regionManager) : base("ダウンロード", LocalConstant.TaskTabID, true)
        {
            this.StagedTasks = WS::Mainpage.DownloadTasksHandler.StagedDownloadTaskPool.Tasks.ToReadOnlyReactiveCollection(t => new DownloadTaskViewModel(t));
            this.Tasks = WS::Mainpage.DownloadTasksHandler.DownloadTaskPool.Tasks.ToReadOnlyReactiveCollection(t => new DownloadTaskViewModel(t));
            this.DisplayCanceled = WS::Mainpage.DownloadTasksHandler.DisplayCanceled.ToReactivePropertyAsSynchronized(x => x.Value);
            this.DisplayCompleted = WS::Mainpage.DownloadTasksHandler.DisplayCompleted.ToReactivePropertyAsSynchronized(x => x.Value);

            this.regionManager = regionManager;
            this.RequestClose += _ => { };

            this.StartDownloadCommand = WS::Mainpage.Videodownloader.CanDownload
            .ToReactiveCommand()
            .WithSubscribe(async () =>
           {
               await WS::Mainpage.Videodownloader.DownloadVideosFriendlyAsync(m => WS::Mainpage.Messagehandler.AppendMessage(m), m => this.Queue.Enqueue(m));
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

            this.ClearStagedCommand = new ReactiveCommand().WithSubscribe(() =>
            {
                WS::Mainpage.DownloadTasksHandler.StagedDownloadTaskPool.Clear();
            });

            this.RemoveStagedTaskCommand = new ReactiveCommand().WithSubscribe(() =>
             {
                 var tasks = this.StagedTasks.Where(t => t.IsChecked);
                 foreach (var task in tasks)
                 {
                     WS::Mainpage.DownloadTasksHandler.StagedDownloadTaskPool.RemoveTask(task.Task);
                 }

             });

            this.MoveTasksToQueue = new ReactiveCommand().WithSubscribe(() =>
            {
                WS::Mainpage.DownloadTasksHandler.MoveStagedToQueue();
            });


            this.CloseCommand.Subscribe(() =>
            {
                IRegion region = this.regionManager.Regions[LocalConstant.TopTabRegionName];
                IEnumerable<object> viewsToRemove = region.Views.Where(v =>
                {
                    if (v is not UserControl control) return false;
                    if (control.DataContext is not TabViewModelBase vm) return false;
                    return vm.ID == LocalConstant.TaskTabID;
                });

                foreach (var view in viewsToRemove)
                {
                    region.Remove(view);
                }
            });
        }

        ~DownloadTasksWindowViewModel()
        {
            this.Dispose();
        }

        private readonly IRegionManager regionManager;

        /// <summary>
        /// ステージング済みタスク
        /// </summary>
        public ReadOnlyReactiveCollection<DownloadTaskViewModel> StagedTasks { get; init; }

        /// <summary>
        /// タスク
        /// </summary>
        public ReadOnlyReactiveCollection<DownloadTaskViewModel> Tasks { get; init; }


        /// <summary>
        /// キャンセル済みを表示
        /// </summary>
        public ReactiveProperty<bool> DisplayCanceled { get; init; } 

        /// <summary>
        /// 完了済みを表示
        /// </summary>
        public ReactiveProperty<bool> DisplayCompleted { get; init; } 

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
        public ReactiveCommand ClearStagedCommand { get; init; }

        /// <summary>
        /// 削除する
        /// </summary>
        public ReactiveCommand RemoveStagedTaskCommand { get; init; }

        /// <summary>
        /// ステージング済みタスクをキューに追加
        /// </summary>
        public ReactiveCommand MoveTasksToQueue { get; init; }

        /// <summary>
        /// メッセージキュー
        /// </summary>
        public ISnackbarHandler Queue { get; init; } = WS::Mainpage.SnackbarHandler.CreateNewHandler();

        #region IDIalogAware

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            this.Dispose();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {

        }

        public event Action<IDialogResult> RequestClose;

        #endregion
    }

    class DownloadTasksWindowViewModelD
    {
        public DownloadTasksWindowViewModelD()
        {
            this.StagedTasks = new ObservableCollection<DownloadTaskViewModel>();
            this.Tasks = new ObservableCollection<DownloadTaskViewModel>();
            this.StagedTasks.Add(new DownloadTaskViewModel(new DownloadTask(new NonBindableListVideoInfo() { Title = new ReactiveProperty<string>("陰陽師"), NiconicoId = new ReactiveProperty<string>("sm9") }, new DownloadSettings()) { IsProcessing = new ReactiveProperty<bool>(true), Message = new ReactiveProperty<string>("初期化完了") }) { IsChecked = true });
            this.Tasks.Add(new DownloadTaskViewModel(new DownloadTask(new NonBindableListVideoInfo() { Title = new ReactiveProperty<string>("陰陽師"), NiconicoId = new ReactiveProperty<string>("sm9") }, new DownloadSettings()) { IsProcessing = new ReactiveProperty<bool>(true), Message = new ReactiveProperty<string>("初期化完了") }) { IsChecked = true });
        }

        public ObservableCollection<DownloadTaskViewModel> StagedTasks { get; init; }

        public ObservableCollection<DownloadTaskViewModel> Tasks { get; init; }

        public ReactiveProperty<bool> DisplayCanceled { get; set; } = new(true);

        public ReactiveProperty<bool> DisplayCompleted { get; set; } = new(true);

        public ReactiveCommand RefreshTaskCommand { get; init; } = new();

        public ReactiveCommand RefreshStagedTaskCommand { get; init; } = new();

        public ReactiveCommand StartDownloadCommand { get; init; } = new();

        public ReactiveCommand CancelDownloadCommand { get; init; } = new();

        public ReactiveCommand ClearStagedCommand { get; init; } = new();

        public ReactiveCommand RemoveStagedTaskCommand { get; init; } = new();

        public ReactiveCommand MoveTasksToQueue { get; init; } = new();

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
