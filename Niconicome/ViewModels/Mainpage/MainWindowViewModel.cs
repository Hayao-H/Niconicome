using System;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Xaml.Behaviors;
using Niconicome.Extensions;
using Niconicome.Models.Auth;
using Niconicome.Models.Domain.Niconico;
using Niconicome.Views;
using Niconicome.Views.Mainpage.Region;
using Niconicome.Views.Setting;
using Prism.Regions;
using Reactive.Bindings;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage
{
    /// <summary>
    /// メイン画面全体のVM
    /// </summary>
    class MainWindowViewModel : BindableBase
    {

        public MainWindowViewModel(IRegionManager regionManager)
        {

            WS::Mainpage.Session.IsLogin.Subscribe(_ => this.OnLogin());
            this.LoginBtnVal= new ReactiveProperty<string>("ログイン");
            this.Username = new ReactiveProperty<string>("未ログイン");
            this.LoginBtnTooltip = new ReactiveProperty<string>("ログイン画面を表示する");
            this.UserImage = new ReactiveProperty<Uri>(new Uri("https://secure-dcdn.cdn.nimg.jp/nicoaccount/usericon/defaults/blank.jpg"));


            this.LoginCommand = new ReactiveCommand()
                .WithSubscribe(async () =>
                {

                    if (!WS::Mainpage.Session.IsLogin.Value)
                    {
                        Window loginpage = new Loginxaml
                        {
                            Owner = Application.Current.MainWindow,
                            ShowInTaskbar = true
                        };
                        loginpage.Show();
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
                var window = new SettingWindow()
                {
                    Owner = Application.Current.MainWindow,
                };
                window.Show();
            });

            this.OpenDownloadTaskWindowsCommand = new ReactiveCommand()
                .WithSubscribe(()=>
             {
                 var windows = new DownloadTasksWindows();
                 windows.Show();
             });

            regionManager.RegisterViewWithRegion("VideoListRegion", typeof(VideoList));
            regionManager.RegisterViewWithRegion("DownloadSettingsRegion", typeof(DownloadSettings));
            regionManager.RegisterViewWithRegion("OutputRegion", typeof(Output));
            regionManager.RegisterViewWithRegion("VideoSortSetting", typeof(VideoSortSetting));
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
        /// メッセージ(フィールド)
        /// </summary>
        private readonly StringBuilder message = new();

        /// <summary>
        /// メッセージ
        /// </summary>
        public string Message
        {
            get
            {
                return this.message.ToString();
            }
            set
            {
                this.message.AppendLine(value);
                this.OnPropertyChanged(nameof(this.message));
            }
        }

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
            if (sender is null || sender.AsNullable<Window>() is not Window window) return;
            if (window != Application.Current.MainWindow) return;
            if (window is not MainWindow mw) return;
            //if (mw.videolist.DataContext is not VideoListViewModel listVM) return;
            //
            //listVM.SaveColumnWidth();
            WS::Mainpage.Shutdown.ShutdownApp();
        }
    }
}
