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
using Niconicome.Views.Setting;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage
{
    /// <summary>
    /// メイン画面全体のVM
    /// </summary>
    class MainWindowViewModel : BindableBase
    {

        public MainWindowViewModel()
        {
            if (WS::Mainpage.Session.IsLogin)
            {
                this.OnLogin(this, EventArgs.Empty);
            } else
            {
                WS::Mainpage.StartUp.AutoLoginSucceeded += this.OnLogin;
            }


            this.LoginCommand = new CommandBase<object>(
                    (object? arg) => true,
                    async (object? arg) =>
                    {

                        if (!this.IsLogin)
                        {
                            Window loginpage = new Loginxaml
                            {
                                Owner = Application.Current.MainWindow,
                                ShowInTaskbar = true
                            };
#pragma warning disable CS8602
                            (loginpage.DataContext as Login.LoginWindowViewModel).LoginSucceeded += this.OnLogin;
                            loginpage.Show();
                        }
                        else
                        {
                            ISession session = WS::Mainpage.Session;
                            await session.Logout();
                            this.IsLogin = false;
                            this.LoginBtnVal = "ログイン";
                            this.OnPropertyChanged(nameof(this.LoginBtnTooltip));
                            this.user = null;
                            this.OnPropertyChanged(nameof(this.Username));
                            this.OnPropertyChanged(nameof(this.UserImage));
                        }

                    }
                );
            this.OpenSettingCommand = new CommandBase<object>(_ => true, _ =>
            {
                var window = new SettingWindow()
                {
                    Owner = Application.Current.MainWindow,
                };
                window.Show();
            });

            this.OpenDownloadTaskWindowsCommand = new CommandBase<object>(_ => true,_=>
            {
                var windows = new DownloadTasksWindows();
                windows.Show();
            });
        }

        /// <summary>
        /// ユーザー情報(フィールド)
        /// </summary>
        private User? user;

        /// <summary>
        /// ユーザー名
        /// </summary>
        public string Username
        {
            get
            {
                return this.user?.Nickname ?? "未ログイン";
            }
        }

        /// <summary>
        /// ユーザー画像
        /// </summary>
        public Uri UserImage
        {
            get
            {
                return this.user?.UserImage ?? new Uri("https://secure-dcdn.cdn.nimg.jp/nicoaccount/usericon/defaults/blank.jpg");
            }
        }

        /// <summary>
        /// ログインボタンのツールチップ
        /// </summary>
        public string LoginBtnTooltip
        {
            get
            {
                return this.IsLogin ? "ログアウトする" : "ログイン画面を表示する";
            }
        }

        /// <summary>
        /// ログインボタンの表示文字(フィールド)
        /// </summary>
        private string loginBtnVal = "ログイン";

        /// <summary>
        /// ログインボタンの表示文字
        /// </summary>
        public string LoginBtnVal
        {
            get { return this.loginBtnVal; }
            set { this.SetProperty(ref this.loginBtnVal, value, nameof(this.LoginBtnVal)); }
        }

        /// <summary>
        /// ログインコマンド
        /// </summary>
        public CommandBase<object> LoginCommand { get; private set; }

        public CommandBase<object> OpenSettingCommand { get; init; }

        public CommandBase<object> OpenDownloadTaskWindowsCommand { get; init; }

        /// <summary>
        /// ログイン状態(フィールド)
        /// </summary>
        private bool isLogin;

        /// <summary>
        /// ログイン状態を取得
        /// </summary>
        public bool IsLogin
        {
            get
            {
                return this.isLogin;
            }
            set
            {
                this.SetProperty(ref this.isLogin, value, nameof(this.IsLogin));
            }
        }

        /// <summary>
        /// メッセージ(フィールド)
        /// </summary>
        private readonly StringBuilder message = new StringBuilder();

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
        private void OnLogin(object? sender,EventArgs e)
        {
            this.IsLogin = true;
            this.user = NiconicoContext.User;
            this.LoginBtnVal = "ログアウト";
            this.OnPropertyChanged(nameof(this.LoginBtnTooltip));
            this.OnPropertyChanged(nameof(this.Username));
            this.OnPropertyChanged(nameof(this.UserImage));
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

            WS::Mainpage.Shutdown.ShutdownApp();
        }
    }
}
