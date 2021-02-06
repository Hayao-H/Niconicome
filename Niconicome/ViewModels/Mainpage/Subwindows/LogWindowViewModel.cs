using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage.Subwindows
{
    class LogWindowViewModel:BindableBase
    {
        public LogWindowViewModel()
        {
            WS::Mainpage.Messagehandler.AddChangeHandler(() => this.OnPropertyChanged(nameof(this.Message)));

            this.CopyMessageCommand = new CommandBase<object>(_ => true, _ =>
            {
                try
                {
                    Clipboard.SetText(this.Message);
                }
                catch { }
            });

            this.ClearMessageCommand = new CommandBase<object>(_ => true, _ =>
              {
                  WS::Mainpage.Messagehandler.ClearMessage();
              });
        }

        /// <summary>
        /// 出力
        /// </summary>
        public string Message { get => WS::Mainpage.Messagehandler.Message; }

        /// <summary>
        /// 出力をコピー
        /// </summary>
        public CommandBase<object> CopyMessageCommand { get; init; }

        /// <summary>
        /// 出力をクリア
        /// </summary>
        public CommandBase<object> ClearMessageCommand { get; init; }
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
