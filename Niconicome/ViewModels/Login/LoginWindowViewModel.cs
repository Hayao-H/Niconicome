using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;
using Niconicome.Workspaces;
using Niconicome.Models.Auth;
using System.Diagnostics;
using Niconicome.Views;
using Utils = Niconicome.Models.Domain.Utils;
using Niconicome.Models.Domain.Niconico;
using Niconicome.ViewModels.Mainpage.Subwindows;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Login
{
    class LoginWindowViewModel : BindableBase
    {
        public LoginWindowViewModel()
        {
            this.LoginCommand = new CommandBase<Window>(
                    (object? arg) => !this.isLoginAttempting && !this.IsLogin&&this.LoginAttemptCount<=this.MaxLoginAttempts,
                    async (Window? window) =>
                    {

                        //ログイン回数確認
                        if (this.LoginAttemptCount > this.MaxLoginAttempts)
                        {
                            this.Message = $"{this.MaxLoginAttempts}回以上ログインを試行しています。連続ログインはアカウントのロックを引き起こす可能性がありますので、時間をおいてアプリケーションを終了してから、もう一度お試し下さい。";
                            this.LoginCommand?.RaiseCanExecutechanged();
                            return;
                        }

                        if (this.userCredential != null)
                        {
                            this.Message = "ログイン試行中です...";
                            this.isLoginAttempting = true;
                            this.LoginCommand?.RaiseCanExecutechanged();

                            ISession session = LoginPage.Session;
                            bool result = await session.Login(this.userCredential);

                            if (result)
                            {

                                if (this.isUserCredentialChanged && this.IsStoringCredentialEnable)
                                {
                                    LoginPage.AccountManager.Save(this.UserCredentialName, this.UserCredentialPassword);
                                }

                                this.Message = $"ログインに成功しました。({session.User.Value?.Nickname}さん)";
                                this.IsLogin = true;
                                this.RaiseLoginSucceeded();
                                window?.Close();
                            }
                            else
                            {
                                this.Message = "ログインに失敗しました。ユーザー名・パスワードは合っていますか？";
                            }

                            this.isLoginAttempting = false;
                            this.LoginCommand?.RaiseCanExecutechanged();
                            ++this.LoginAttemptCount;
                        }
                    }
                );

            var manager = LoginPage.AccountManager;

            if (manager.IsPasswordSaved)
            {
                this.userCredential = manager.GetUserCredential();
                this.OnPropertyChanged();
                this.isStoringCredentialEnableField = true;
            }
        }

        /// <summary>
        /// イベントを発行
        /// </summary>
        public void RaiseLoginSucceeded()
        {
            this.LoginSucceeded?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// ログインコマンド
        /// </summary>
        public CommandBase<Window> LoginCommand { get; private set; }

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
        /// 認証情報(プロパティー)
        /// </summary>
        private IUserCredential? userCredential;

        /// <summary>
        /// 認証情報(ユーザー名)
        /// </summary>
        public string UserCredentialName
        {
            get
            {
                return this.userCredential?.Username ?? string.Empty;
            }
            set
            {
                this.isUserCredentialChanged = true;
                this.SetProperty(ref this.userCredential, AccountManager.GetUserCredential(value, this.userCredential?.Password ?? string.Empty));
            }
        }

        /// <summary>
        /// 認証情報(パスワード)
        /// </summary>
        public string UserCredentialPassword
        {
            get
            {
                return this.userCredential?.Password ?? string.Empty;
            }
            set
            {
                this.isUserCredentialChanged = true;
                this.SetProperty(ref this.userCredential, AccountManager.GetUserCredential(this.userCredential?.Username ?? string.Empty, value));
            }
        }

        /// <summary>
        /// ログイン試行中(フィールド)
        /// </summary>
        private bool isLoginAttempting;

        private bool isStoringCredentialEnableField;

        /// <summary>
        /// 資格情報を保存するかどうか
        /// </summary>
        public bool IsStoringCredentialEnable { get => this.isStoringCredentialEnableField; set => this.SetProperty(ref this.isStoringCredentialEnableField, value); }

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
        /// ログイン試行回数
        /// </summary>
        private int LoginAttemptCount { get; set; } = 0;

        /// <summary>
        /// ログイン最大試行回数
        /// </summary>
        private readonly int MaxLoginAttempts = 5;

        /// <summary>
        /// 資格情報が変更されたかどうか
        /// </summary>
        private bool isUserCredentialChanged;

        public event EventHandler? LoginSucceeded;
    }

    class OpenLoginBrowserBehavior : Behavior<Label>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.MouseDown += this.Onclick;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.MouseDown -= this.Onclick;
        }

        public Window? Window
        {
            get
            {
                return (Window)this.GetValue(WindowProperty);
            }
            set
            {
                this.SetValue(WindowProperty, value);
            }
        }

        public static readonly DependencyProperty WindowProperty = DependencyProperty.Register("Window", typeof(Window), typeof(OpenLoginBrowserBehavior), new PropertyMetadata(null));

        /// <summary>
        /// クリック時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Onclick(object sender, EventArgs e)
        {
            Debug.WriteLine(sender.GetType());

            var context = new LoginBrowserViewModel();
            context.LoginSucceeded += (_, _) => { (this.AssociatedObject.DataContext as LoginWindowViewModel)?.RaiseLoginSucceeded(); };

            WS::Mainpage.WindowsHelper.OpenWindow(() => new LoginBrowser()
            {
                Owner = Application.Current.MainWindow,
                DataContext = context
            });

            this.Window?.Close();
        }
    }
}
