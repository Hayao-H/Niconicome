using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Xaml.Behaviors;
using Niconicome.Extensions;
using Niconicome.Models.Auth;
using Niconicome.Models.Domain.Niconico;
using Niconicome.Models.Local.Settings;
using Niconicome.ViewModels.Controls;
using Niconicome.Views;
using Niconicome.Views.AddonPage;
using Niconicome.Views.Mainpage.Region;
using Niconicome.Views.Setting;
using Prism.Regions;
using Prism.Services.Dialogs;
using Prism.Unity;
using Prism.Ioc;
using Reactive.Bindings;
using WS = Niconicome.Workspaces;
using Niconicome.ViewModels.Mainpage.BottomTabs;

namespace Niconicome.ViewModels.Mainpage
{
    /// <summary>
    /// メイン画面全体のVM
    /// </summary>
    public class MainWindowViewModel : BindableBase
    {

        public MainWindowViewModel(IRegionManager regionManager, IDialogService dialogService)
        {

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

            this.BottomTabs = WS::Mainpage.TabsContainer.Tabs.ToReadOnlyReactiveCollection(x => new TabViewModel(x));

            #region UI系の設定

            this.TreeWidth = WS::Mainpage.StyleHandler.UserChrome.Select(value => value?.MainPage.Tree.Width ?? 250).ToReadOnlyReactiveProperty();
            this.TabsHeight = WS::Mainpage.StyleHandler.UserChrome.Select(value => value?.MainPage.VideoList.TabsHeight ?? 260).ToReadOnlyReactiveProperty();

            #endregion

            regionManager.RegisterViewWithRegion<VideoList>("VideoListRegion");
            regionManager.RegisterViewWithRegion<DownloadSettings>("DownloadSettingsRegion");
            regionManager.RegisterViewWithRegion<Output>("OutputRegion");
            regionManager.RegisterViewWithRegion<VideoSortSetting>("VideoSortSetting");
            regionManager.RegisterViewWithRegion<VideoListState>("VideolistState");
            regionManager.RegisterViewWithRegion<TimerSettings>("TimerSettings");
        }

        /// <summary>
        /// ユーザー情報(フィールド)
        /// </summary>
        private User? user;

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

        /// <summary>
        /// 下のタブ
        /// </summary>
        public ReadOnlyReactiveCollection<TabViewModel> BottomTabs { get; init; }

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
                    if (cResult.Result !=ButtonResult.Yes)
                    {
                        e.Cancel = true;
                        return;
                    }
                }

                WS::Mainpage.Shutdown.ShutdownApp();
            }
        }
    }
}
