using System;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage.Subwindows
{
    class LogWindowViewModel : BindableBase, IDisposable
    {
        public LogWindowViewModel()
        {
            this.disposables = new CompositeDisposable();

            this.Message = WS::Mainpage.Messagehandler.Message.ToReactiveProperty().AddTo(this.disposables);

            this.CopyMessageCommand = new ReactiveCommand()
                .WithSubscribe(() =>
                {
                    try
                    {
                        Clipboard.SetText(this.Message.Value);
                    }
                    catch { }
                }).AddTo(this.disposables);

            this.ClearMessageCommand = new ReactiveCommand()
                .WithSubscribe(() =>
                {
                    try
                    {
                        WS::Mainpage.Messagehandler.ClearMessage();

                    }
                    catch { }
                }).AddTo(this.disposables);
        }

        ~LogWindowViewModel()
        {
            this.Dispose();
        }

        /// <summary>
        /// 出力
        /// </summary>        
        public ReactiveProperty<string?> Message { get; init; }


        /// <summary>
        /// 出力をコピー
        /// </summary>
        public ReactiveCommand CopyMessageCommand { get; init; } = new();


        /// <summary>
        /// 出力をクリア
        /// </summary>
        public ReactiveCommand ClearMessageCommand { get; init; } = new();

        /// <summary>
        /// インスタンスを破棄
        /// </summary>
        public void Dispose()
        {
            if (this.hasDisposed) return;
            this.disposables.Dispose();
            this.hasDisposed = true;
            GC.SuppressFinalize(this);
        }

        #region private

        private readonly CompositeDisposable disposables;

        private bool hasDisposed;
        #endregion

    }

    /// <summary>
    /// デザイナー用のVM
    /// </summary>
    public class LogWindowViewModelD
    {
        public LogWindowViewModelD()
        {
            this.Message = new ReactiveProperty<string?>();
            this.Message.Value = "初期化済み";
        }

        public ReactiveProperty<string?> Message { get; init; }

        public ReactiveCommand CopyMessageCommand { get; init; } = new();

        public ReactiveCommand ClearMessageCommand { get; init; } = new();
    }

    class LogWindowBehavior : Behavior<TextBox>
    {

        protected override void OnAttached()
        {
            base.OnAttached();
            WS::Mainpage.Messagehandler.AddChangeHandler(this.OnMessage);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            WS::Mainpage.Messagehandler.RemoveChangeHandler(this.OnMessage);
        }

        private void OnMessage()
        {
            this.AssociatedObject.ScrollToEnd();
        }
    }
}
