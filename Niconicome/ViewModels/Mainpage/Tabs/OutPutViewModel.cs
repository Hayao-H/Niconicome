using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Views;
using System.Windows;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using WS = Niconicome.Workspaces;
using Niconicome.ViewModels.Mainpage.Tabs;

namespace Niconicome.ViewModels.Mainpage
{
    class OutPutViewModel : TabViewModelBase
    {
        public OutPutViewModel() : base("出力")
        {
            this.Message = WS::Mainpage.Messagehandler.Message.ToReactiveProperty();

            this.ClearMessageCommand = new ReactiveCommand()
                .WithSubscribe(
                () =>
                {
                    WS::Mainpage.Messagehandler.ClearMessage();
                });

            this.CopyMessageCommand = new ReactiveCommand()
                .WithSubscribe(() =>
                {
                    Clipboard.SetText(this.Message.Value);
                    WS::Mainpage.Messagehandler.AppendMessage("出力をクリップボードにコピーしました。");
                    WS::Mainpage.SnackbarHandler.Enqueue("出力をクリップボードにコピーしました。");
                });

            this.OpenLogWindowCommand = new ReactiveCommand()
                .WithSubscribe(() =>
                {
                    WS::Mainpage.WindowsHelper.OpenWindow(() => new LogWindow()
                    {
                        Owner = Application.Current.MainWindow,
                    });
                });

        }

        /// <summary>
        /// メッセージ
        /// </summary>
        public ReactiveProperty<string?> Message { get; init; }

        /// <summary>
        /// メッセージを削除
        /// </summary>
        public ReactiveCommand ClearMessageCommand { get; init; }

        /// <summary>
        /// メッセージをコピー
        /// </summary>
        public ReactiveCommand CopyMessageCommand { get; init; }

        /// <summary>
        /// メッセージを別ウィンドウで開く
        /// </summary>
        public ReactiveCommand OpenLogWindowCommand { get; init; }

    }

    class OutPutViewModelD
    {
        public ReactiveProperty<string?> Message { get; init; } = new();

        public ReactiveCommand ClearMessageCommand { get; init; } = new();

        public ReactiveCommand CopyMessageCommand { get; init; } = new();

        public ReactiveCommand OpenLogWindowCommand { get; init; } = new();
    }
}
