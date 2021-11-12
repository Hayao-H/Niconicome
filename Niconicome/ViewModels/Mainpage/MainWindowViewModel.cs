using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Xaml.Behaviors;
using Niconicome.Extensions;
using Niconicome.Models.Auth;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Niconico;
using Niconicome.Models.Local.Settings;
using Niconicome.ViewModels.Controls;
using Niconicome.ViewModels.Mainpage.BottomTabs;
using Niconicome.ViewModels.Mainpage.Tabs;
using Niconicome.Views;
using Niconicome.Views.AddonPage;
using Niconicome.Views.Mainpage.Region;
using Niconicome.Views.Setting;
using Prism.Ioc;
using Prism.Regions;
using Prism.Services.Dialogs;
using Prism.Unity;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage
{
    /// <summary>
    /// メイン画面全体のVM
    /// </summary>
    public class MainWindowViewModel : BindableBase
    {

        public MainWindowViewModel(IRegionManager regionManager, IDialogService dialogService, IContainerProvider containerProvider)
        {

            this.ctx = SynchronizationContext.Current;

            this.LoginBtnVal = new ReactiveProperty<string>("ログイン");
            this.Username = new ReactiveProperty<string>("未ログイン");
            this.LoginBtnTooltip = new ReactiveProperty<string>("ログイン画面を表示する");
            this.UserImage = new ReactiveProperty<Uri>(new Uri("https://secure-dcdn.cdn.nimg.jp/nicoaccount/usericon/defaults/blank.jpg"));

            WS::Mainpage.Themehandler.Initialize();
            WS::Mainpage.Session.IsLogin.Subscribe(_ => this.OnLogin());

            this.LoginCommand = new ReactiveCommand()
                .WithSubscribe(async () =>
                {

                    if (!WS::Mainpage.Session.IsLogin.Value)
                    {
                        WS::Mainpage.WindowsHelper.OpenWindow(() => new Loginxaml
                        {
                            Owner = Application.Current.MainWindow,
                            ShowInTaskbar = true
                        });
                    }
                    else
                    {
                        ISession session = WS::Mainpage.Session;
                        await session.Logout();
                        this.LoginBtnVal.Value = "ログイン";
                        this.Username.Value = "未ログイン";
                        this.LoginBtnTooltip.Value = "ログイン画面を表示する";
                        this.UserImage.Value = new Uri("https://secure-dcdn.cdn.nimg.jp/nicoaccount/usericon/defaults/blank.jpg");
                    }

                });

            this.OpenSettingCommand = new ReactiveCommand()
                .WithSubscribe(() =>
            {
                WS::Mainpage.WindowsHelper.OpenWindow(() => new SettingWindow()
                {
                    Owner = Application.Current.MainWindow,
                });
            });

            this.OpenDownloadTaskWindowsCommand = new ReactiveCommand()
                .WithSubscribe(() =>
             {
                 WS::Mainpage.WindowsHelper.OpenWindow<DownloadTasksWindows>();
             });

            this.Restart = new ReactiveCommand()
                .WithSubscribe(async () =>
                {
                    var cResult = await MaterialMessageBox.Show("再起動しますか？", MessageBoxButtons.Yes | MessageBoxButtons.No, MessageBoxIcons.Question);

                    if (cResult != MaterialMessageBoxResult.Yes) return;

                    var result = WS::Mainpage.ApplicationPower.Restart();
                    if (!result.IsSucceeded)
                    {
                        WS::Mainpage.Messagehandler.AppendMessage("再起動に失敗しました。");
                        WS::Mainpage.SnackbarHandler.Enqueue("再起動できませんでした。");
                    }
                });

            this.ShutDown = new ReactiveCommand()
                .WithSubscribe(async () =>
                {
                    var cResult = await MaterialMessageBox.Show("終了しますか？", MessageBoxButtons.Yes | MessageBoxButtons.No, MessageBoxIcons.Question);

                    if (cResult != MaterialMessageBoxResult.Yes) return;

                    WS::Mainpage.ApplicationPower.ShutDown();
                });

            this.OpenAddonManagerCommand = new ReactiveCommand()
                .WithSubscribe(() =>
                {
                    if (WS::Mainpage.LocalInfo.IsAddonManagerOpen && !WS::Mainpage.LocalInfo.IsMultiWindowsAllowed)
                    {
                        return;
                    }
                    dialogService.Show(nameof(AddonManagerWindow));
                });


            #region UI系の設定

            this.TreeWidth = WS::Mainpage.StyleHandler.UserChrome.Select(value => value?.MainPage.Tree.Width ?? 250).ToReadOnlyReactiveProperty();
            this.TabsHeight = WS::Mainpage.StyleHandler.UserChrome.Select(value => value?.MainPage.VideoList.TabsHeight ?? 260).ToReadOnlyReactiveProperty();

            #endregion

            regionManager.RegisterViewWithRegion<VideoList>("VideoListRegion");

            this.RegionManager = regionManager;

            this.RegisterTabHandlers();
        }

        #region field

        private User? user;

        private readonly SynchronizationContext ctx;

        #endregion

        #region Props
        public IRegionManager RegionManager { get; init; }

        /// <summary>
        /// ユーザー名
        /// </summary>
        public ReactiveProperty<string> Username { get; init; }

        /// <summary>
        /// ユーザー画像
        /// </summary>
        public ReactiveProperty<Uri> UserImage { get; init; }

        /// <summary>
        /// ログインボタンのツールチップ
        /// </summary>
        public ReactiveProperty<string> LoginBtnTooltip { get; init; }

        /// <summary>
        /// ログインボタンの表示文字
        /// </summary>
        public ReactiveProperty<string> LoginBtnVal { get; init; }

        #endregion

        #region Command

        /// <summary>
        /// ログインコマンド
        /// </summary>
        public ReactiveCommand LoginCommand { get; private set; }

        public ReactiveCommand OpenSettingCommand { get; init; }

        public ReactiveCommand OpenDownloadTaskWindowsCommand { get; init; }

        /// <summary>
        /// アドオンマネージャーを開く
        /// </summary>
        public ReactiveCommand OpenAddonManagerCommand { get; init; }

        /// <summary>
        /// シャットダウン
        /// </summary>
        public ReactiveCommand ShutDown { get; init; }

        /// <summary>
        /// 再起動
        /// </summary>
        public ReactiveCommand Restart { get; init; }

        #endregion

        #region private


        /// <summary>
        /// ログイン成功時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLogin()
        {
            if (WS::Mainpage.Session.User.Value is null) return;
            this.user = WS::Mainpage.Session.User.Value;
            this.LoginBtnVal.Value = "ログアウト";
            this.LoginBtnTooltip.Value = "ログアウトする";
            this.Username.Value = this.user.Nickname;
            this.UserImage.Value = this.user.UserImage;
        }

        /// <summary>
        /// タブハンドラを登録
        /// </summary>
        private void RegisterTabHandlers()
        {
            WS::Mainpage.TabHandler.RegisterAddHandler(tab =>
            {
                this.ctx.Post(_ =>
                {
                    var vm = new BottomTabViewModel(tab);
                    var control = new BottomTab(vm);
                    this.RegionManager.Regions[LocalConstant.TabRegionName].Add(control);
                }, null);
            });

            WS::Mainpage.TabHandler.RegisterRemoveHandler(id =>
            {
                IEnumerable<object> viewToRemove = this.RegionManager.Regions[LocalConstant.TabRegionName].Views.Where(v =>
                {
                    if (v is not UserControl control) return false; ;
                    if (control.DataContext is not TabViewModelBase vm) return false;
                    return vm.ID == id;
                });

                foreach (var view in viewToRemove)
                {
                    this.RegionManager.Regions[LocalConstant.TabRegionName].Remove(view);
                }

            });
        }

        #endregion


        #region UI系

        /// <summary>
        /// ツリーの幅
        /// </summary>
        public ReadOnlyReactiveProperty<int> TreeWidth { get; init; }

        /// <summary>
        /// タブの高さ
        /// </summary>
        public ReadOnlyReactiveProperty<int> TabsHeight { get; init; }

        #endregion


    }

    public record TabInfo(string Title, string RegionName, IRegionManager RegionManager);

    /// <summary>
    /// ユーザー画像の取得に失敗したときなど
    /// </summary>
    class UserImageBehavior : Behavior<Image>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.ImageFailed += this.DefaultUrl;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.ImageFailed -= this.DefaultUrl;
        }

        private void DefaultUrl(object? sender, ExceptionRoutedEventArgs e)
        {
            this.AssociatedObject.Source = new BitmapImage(new Uri("https://secure-dcdn.cdn.nimg.jp/nicoaccount/usericon/defaults/blank.jpg", UriKind.Absolute));
        }
    }

    class MainWindowBehavior : Behavior<Window>
    {

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Closing += this.OnClosing;
            this.AssociatedObject.Loaded += (_, _) => this.CreateTabs();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.Closing -= this.OnClosing;
        }

        private void OnClosing(object? sender, CancelEventArgs e)
        {
            if (!WS::Mainpage.Shutdown.IsShutdowned)
            {
                IDialogService service = Application.Current.As<PrismApplication>().Container.Resolve<IDialogService>();
                bool confirm = WS::Mainpage.SettingHandler.GetBoolSetting(SettingsEnum.ConfirmIfDownloading);
                if (!WS::Mainpage.Videodownloader.CanDownload.Value && confirm)
                {
                    var cResult = CommonMessageBoxAPI.Show(service, "ダウンロードが進行中ですが、本当に終了しますか？", CommonMessageBoxAPI.MessageType.Warinng, CommonMessageBoxButtons.Yes | CommonMessageBoxButtons.No);
                    if (cResult.Result != ButtonResult.Yes)
                    {
                        e.Cancel = true;
                        return;
                    }
                }

                WS::Mainpage.Shutdown.ShutdownApp();
            }
        }

        private void CreateTabs()
        {
            if (this.AssociatedObject.DataContext is not MainWindowViewModel vm) return;
            if (Application.Current is not PrismApplication application) return;

            IContainerProvider containerProvider = application.Container;
            IRegionManager regionManager = vm.RegionManager;

            IRegion tabRegion = regionManager.Regions[LocalConstant.TabRegionName];
            tabRegion.Add(containerProvider.Resolve<DownloadSettings>());
            tabRegion.Add(containerProvider.Resolve<Output>());
            tabRegion.Add(containerProvider.Resolve<VideoSortSetting>());
            tabRegion.Add(containerProvider.Resolve<VideoListState>());
            tabRegion.Add(containerProvider.Resolve<TimerSettings>());
        }
    }
}
