using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Extensions.System;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Local.State.Toast;
using Niconicome.Models.Network.Download;
using Niconicome.Models.Network.Download.DLTask;
using Niconicome.Models.Playlist;
using Niconicome.Models.Utils.Reactive;
using Niconicome.Models.Utils.Reactive.Command;
using Niconicome.ViewModels.Mainpage.Tabs;
using Niconicome.ViewModels.Mainpage.Utils;
using Prism.Regions;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Helpers;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage.Subwindows
{
    class DownloadTasksWindowViewModel : TabViewModelBase, IDisposable, IDialogAware
    {
        public DownloadTasksWindowViewModel(IRegionManager regionManager) : base("ダウンロード", LocalConstant.TaskTabID, true)
        {
            this.StagedTasks = WS::Mainpage.DownloadManager.Staged.ToReadOnlyReactiveCollection(t => new DownloadTaskViewModel(t));
            this.Tasks = WS::Mainpage.DownloadManager.Queue.ToReadOnlyReactiveCollection(t => new DownloadTaskViewModel(t));
            this.DisplayCanceled = WS::Mainpage.DownloadManager.DisplayCanceled;
            this.DisplayCompleted = WS::Mainpage.DownloadManager.DisplayCompleted;

            this.regionManager = regionManager;
            this.RequestClose += _ => { };

            this.StartDownloadCommand = new BindableCommand(async () =>
            {
                await WS::Mainpage.DownloadManager.StartDownloadAsync(m => WS::Mainpage.Messagehandler.AppendMessage(m), m => this.Queue.Enqueue(m));
                WS::Mainpage.PostDownloadTasksManager.HandleAction();
            }, WS::Mainpage.DownloadManager.IsProcessing.Select(x => !x));

            this.CancelDownloadCommand = new BindableCommand(
               () =>
                {
                    WS::Mainpage.DownloadManager.CancelDownload();
                    this.Queue.Enqueue("ユーザーによってダウンロードがキャンセルされました。");
                    WS::Mainpage.Messagehandler.AppendMessage("ユーザーによってダウンロードがキャンセルされました。");
                }, WS::Mainpage.DownloadManager.IsProcessing);

            this.ClearStagedCommand = new BindableCommand(() =>
            {
                WS::Mainpage.DownloadManager.ClearStaged();
            }, new BindableProperty<bool>(true));

            this.RemoveStagedTaskCommand = new BindableCommand(() =>
             {
                 var tasks = this.StagedTasks.Where(t => t.IsChecked.Value);
                 foreach (var task in tasks)
                 {
                     WS::Mainpage.DownloadManager.RemoveFromStaged(task.Task);
                 }

             }, new BindableProperty<bool>(true));

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

                WS::Mainpage.LocalState.IsTaskWindowOpen = false;
            });
        }

        ~DownloadTasksWindowViewModel()
        {
            this.Dispose();
        }

        private readonly IRegionManager regionManager;

        #region Props

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
        public IBindableProperty<bool> DisplayCanceled { get; init; }

        /// <summary>
        /// 完了済みを表示
        /// </summary>
        public IBindableProperty<bool> DisplayCompleted { get; init; }

        /// <summary>
        /// DLを開始
        /// </summary>
        public IBindableCommand StartDownloadCommand { get; init; }

        /// <summary>
        /// DLを中止
        /// </summary>
        public IBindableCommand CancelDownloadCommand { get; init; }

        /// <summary>
        /// ステージング済みをクリア
        /// </summary>
        public IBindableCommand ClearStagedCommand { get; init; }

        /// <summary>
        /// 削除する
        /// </summary>
        public IBindableCommand RemoveStagedTaskCommand { get; init; }

        /// <summary>
        /// メッセージキュー
        /// </summary>
        public IToastHandler Queue { get; init; } = DIFactory.Provider.GetRequiredService<IToastHandler>();

        /// <summary>
        /// 幅
        /// </summary>
        public ReactivePropertySlim<double> Width { get; private set; } = new(double.NaN);

        /// <summary>
        /// 高さ
        /// </summary>
        public ReactivePropertySlim<double> Height { get; private set; } = new(double.NaN);

        #endregion

        #region IDIalogAware

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            WS::Mainpage.LocalState.IsTaskWindowOpen = false;
            this.Dispose();
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            this.Height.Value = 600;
            this.Width.Value = 1200;
            WS::Mainpage.LocalState.IsTaskWindowOpen = true;
        }

        public event Action<IDialogResult> RequestClose;

        #endregion
    }

    class DownloadTasksWindowViewModelD
    {
        public DownloadTasksWindowViewModelD()
        {
            this.StagedTasks = new ObservableCollection<DownloadTaskViewModelD>();
            this.Tasks = new ObservableCollection<DownloadTaskViewModelD>();
            this.StagedTasks.Add(new DownloadTaskViewModelD("陰陽師", "sm9", true, false, false, true));
            this.StagedTasks.Add(new DownloadTaskViewModelD("陰陽師", "sm9", false, false, true, true));
            this.StagedTasks.Add(new DownloadTaskViewModelD("陰陽師", "sm9", false, true, false, true));
        }

        public ObservableCollection<DownloadTaskViewModelD> StagedTasks { get; init; }

        public ObservableCollection<DownloadTaskViewModelD> Tasks { get; init; }

        public ReactiveProperty<bool> DisplayCanceled { get; set; } = new(true);

        public ReactiveProperty<bool> DisplayCompleted { get; set; } = new(true);

        public ReactivePropertySlim<double> Width { get; private set; } = new(600);

        public ReactivePropertySlim<double> Height { get; private set; } = new(1200);

        public IBindableCommand StartDownloadCommand { get; init; } = BindableCommand.Empty;

        public IBindableCommand CancelDownloadCommand { get; init; } = BindableCommand.Empty;

        public IBindableCommand ClearStagedCommand { get; init; } = BindableCommand.Empty;

        public IBindableCommand RemoveStagedTaskCommand { get; init; } = BindableCommand.Empty;

        public IToastHandler? Queue { get; init; } = null;
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
            this.Message = task.Message;
            this.IsProcessing = task.IsProcessing;
            this.IsCanceled = task.IsCanceled;
            this.IsCompleted = task.IsCompleted;

            var r1 = new ComboboxItem<int>(1080, "1080p");
            var r2 = new ComboboxItem<int>(720, "720p");
            var r3 = new ComboboxItem<int>(480, "480p");
            var r4 = new ComboboxItem<int>(360, "360p");
            var r5 = new ComboboxItem<int>(240, "240p");

            this.SelectableResolutions = new List<ComboboxItem<int>>() { r1, r2, r3, r4, r5 };
            this.SelectedResolution = new ReactiveProperty<ComboboxItem<int>>(task.Resolution.Value switch
            {
                1080 => r1,
                720 => r2,
                480 => r3,
                240 => r4,
                _ => r1
            });


            this.CancelCommand = new[] {
                    this.IsCanceled,
                    this.IsCompleted,
                }
                .CombineLatestAreAllTrue(() =>
                {
                    this.associatedTask.Cancel();
                });

        }
        #region field

        private readonly IDownloadTask associatedTask;

        #endregion

        #region Props

        /// <summary>
        /// タスク
        /// </summary>
        public IDownloadTask Task { get => this.associatedTask; }

        /// <summary>
        /// 動画タイトル
        /// </summary>
        public string Title { get => this.associatedTask.Title; }

        /// <summary>
        /// ニコニコのID
        /// </summary>
        public string NiconicoID { get => this.associatedTask.NiconicoID; }

        /// <summary>
        /// 選択可能な解像度
        /// </summary>
        public List<ComboboxItem<int>> SelectableResolutions { get; init; }

        /// <summary>
        /// 処理中フラグ
        /// </summary>
        public IBindableProperty<bool> IsProcessing { get; }

        /// <summary>
        /// 終了フラグ
        /// </summary>
        public IBindableProperty<bool> IsCompleted { get; init; }

        /// <summary>
        /// キャンセルフラグ
        /// </summary>
        public IBindableProperty<bool> IsCanceled { get; }

        /// <summary>
        /// 状態
        /// </summary>
        public IBindableProperty<string> Message { get; }

        /// <summary>
        /// 選択フラグ
        /// </summary>
        public ReactiveProperty<bool> IsChecked { get; init; } = new();

        /// <summary>
        /// 選択された解像度
        /// </summary>
        public ReactiveProperty<ComboboxItem<int>> SelectedResolution { get; init; }

        #endregion

        /// <summary>
        /// 処理をキャンセル
        /// </summary>
        public IBindableCommand CancelCommand { get; init; }


    }

    class DownloadTaskViewModelD : BindableBase
    {
        public DownloadTaskViewModelD(string title, string niconicoID, bool isProcessing, bool isCompleted, bool isCanceled, bool isChecked)
        {
            this.Title = title;
            this.NiconicoID = niconicoID;
            this.IsProcessing = new ReactivePropertySlim<bool>(isProcessing);
            this.IsCompleted = new ReactivePropertySlim<bool>(isCompleted);
            this.IsCanceled = new ReactivePropertySlim<bool>(isCanceled);
            this.IsChecked = new ReactivePropertySlim<bool>(isChecked);

            var r1 = new ComboboxItem<int>(1080, "1080p");
            var r2 = new ComboboxItem<int>(720, "720p");
            var r3 = new ComboboxItem<int>(480, "480p");
            var r4 = new ComboboxItem<int>(360, "360p");
            var r5 = new ComboboxItem<int>(240, "240p");

            this.SelectableResolutions = new List<ComboboxItem<int>>() { r1, r2, r3, r4, r5 };
            this.SelectedResolution = new ReactiveProperty<ComboboxItem<int>>(r1);

        }

        public string Title { get; init; }

        public string NiconicoID { get; init; }

        public List<ComboboxItem<int>> SelectableResolutions { get; init; }

        public ReactivePropertySlim<bool> IsProcessing { get; init; }

        public ReactivePropertySlim<bool> IsCompleted { get; init; }

        public ReactivePropertySlim<bool> IsCanceled { get; init; }

        public ReactivePropertySlim<string> Message { get; init; } = new("テスト");

        public ReactivePropertySlim<bool> IsChecked { get; init; }

        public IBindableCommand CancelCommand { get; init; } = BindableCommand.Empty;

        public ReactiveProperty<ComboboxItem<int>> SelectedResolution { get; init; }

    }
}
