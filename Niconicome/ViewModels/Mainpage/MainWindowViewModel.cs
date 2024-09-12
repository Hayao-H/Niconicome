using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Xaml.Behaviors;
using Niconicome.Extensions;
using Niconicome.Models.Auth;
using Niconicome.Models.Domain.Niconico;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Utils.Reactive.Command;
using Niconicome.ViewModels.Controls;
using Niconicome.Views;
using Prism.Ioc;
using Prism.Regions;
using Prism.Services.Dialogs;
using Prism.Unity;
using Reactive.Bindings;
using Style = Niconicome.Models.Local.State.Style;
using Tree = Niconicome.Views.Mainpage.Region.PlaylistTree;
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
            this.RegionManager = regionManager;
            this.dialogService = dialogService;

            this.RegionManager.RegisterViewWithRegion<Tree::PlaylistTree>("PlaylistTree");

            this.LoginBtnVal = new ReactiveProperty<string>("ログイン");
            this.Username = new ReactiveProperty<string>("未ログイン");
            this.LoginBtnTooltip = new ReactiveProperty<string>("ログイン画面を表示する");
            this.UserImage = new ReactiveProperty<Uri>(new Uri("https://secure-dcdn.cdn.nimg.jp/nicoaccount/usericon/defaults/blank.jpg"));

            WS::Mainpage.Themehandler.Initialize();
            WS::Mainpage.NiconicoContext.IsLogin.Subscribe(x => { if (x) this.OnLogin(); });

            this.LoginCommand = new BindableCommand(async () =>
            {

                if (!WS::Mainpage.NiconicoContext.IsLogin.Value)
                {
                    WS::Mainpage.WindowsHelper.OpenWindow(() => new LoginBrowser()
                    {
                        Owner = Application.Current.MainWindow,
                        ShowInTaskbar = true
                    });
                }
                else
                {

                    await WS::Mainpage.NiconicoContext.LogoutAsync();
                    this.LoginBtnVal.Value = "ログイン";
                    this.Username.Value = "未ログイン";
                    this.LoginBtnTooltip.Value = "ログイン画面を表示する";
                    this.UserImage.Value = new Uri("https://secure-dcdn.cdn.nimg.jp/nicoaccount/usericon/defaults/blank.jpg");
                }

            });

            this.OpenSettingCommand = new BindableCommand(() =>
            {
                WS::Mainpage.TabControler.Open(Models.Local.State.Tab.V1.TabType.Settings);
                WS::Mainpage.BlazorPageManager.RequestBlazorToNavigate("/settings/general");
            });

            this.OpenDownloadTaskWindowsCommand = new BindableCommand(
              () =>
             {
                 WS::Mainpage.TabControler.Open(Models.Local.State.Tab.V1.TabType.Download);
                 WS::Mainpage.BlazorPageManager.RequestBlazorToNavigate("/downloadtask/download");
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
                    WS::Mainpage.TabControler.Open(Models.Local.State.Tab.V1.TabType.Addon);
                    WS::Mainpage.BlazorPageManager.RequestBlazorToNavigate("/addons");
                });



            #region UI系の設定

            this.TreeWidth = new ReadOnlyReactiveProperty<int>(new ReactiveProperty<int>(250));
            this.TabsHeight = new ReadOnlyReactiveProperty<int>(new ReactiveProperty<int>(260));

            #endregion

        }

        #region field

        private User? user;

        private readonly SynchronizationContext? ctx;

        private readonly IDialogService dialogService;

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
        public BindableCommand LoginCommand { get; private set; }

        /// <summary>
        /// 設定を開く
        /// </summary>
        public BindableCommand OpenSettingCommand { get; init; }

        public BindableCommand OpenDownloadTaskWindowsCommand { get; init; }

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
            if (WS::Mainpage.NiconicoContext.User is null) return;
            this.user = WS::Mainpage.NiconicoContext.User;
            this.LoginBtnVal.Value = "ログアウト";
            this.LoginBtnTooltip.Value = "ログアウトする";
            this.Username.Value = this.user.Nickname;
            this.UserImage.Value = this.user.UserImage;
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
            this.AssociatedObject.Loaded += (_, _) =>
            {
                this.SetWindowPosition();
            };
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.Closing -= this.OnClosing;
        }

        private void OnClosing(object? sender, CancelEventArgs e)
        {

            Window window = this.AssociatedObject;
            WS.Mainpage.WindowStyleManager.SaveTyle(new Style::WindowStyle((int)window.Top, (int)window.Left, (int)window.Height, (int)window.Width));

            if (!WS::Mainpage.Shutdown.IsShutdowned)
            {
                IDialogService service = Application.Current.As<PrismApplication>().Container.Resolve<IDialogService>();
                bool confirm = WS::Mainpage.SettingHandler.GetBoolSetting(SettingsEnum.ConfirmIfDownloading);
                if (WS::Mainpage.DownloadManager.IsProcessing.Value && confirm)
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

        private void SetWindowPosition()
        {
            IAttemptResult<Style::WindowStyle> result = WS.Mainpage.WindowStyleManager.GetStyle();
            if (!result.IsSucceeded || result.Data is null)
            {
                return;
            }

            Style::WindowStyle style = result.Data;

            if (style.Top >= 0)
            {
                this.AssociatedObject.Top = style.Top;
            }

            if (style.Left >= 0)
            {
                this.AssociatedObject.Left = style.Left;
            }

            if (style.Width >= 0)
            {
                this.AssociatedObject.Width = style.Width;
            }

            if (style.Height >= 0)
            {
                this.AssociatedObject.Height = style.Height;
            }
        }
    }
}
