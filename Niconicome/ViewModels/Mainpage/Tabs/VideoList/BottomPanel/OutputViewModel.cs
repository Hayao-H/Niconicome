using System;
using System.Collections.Specialized;
using System.Text;
using System.Windows;
using Niconicome.Extensions;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.State.MessageV2;
using Niconicome.Models.Utils.Reactive;
using Niconicome.ViewModels.Mainpage.Tabs.VideoList.Pages.StringContent;
using Niconicome.Views;
using LocalConstant = Niconicome.Models.Const.LocalConstant;
using WS = Niconicome.Workspaces.Mainpage;

namespace Niconicome.ViewModels.Mainpage.Tabs.VideoList.BottomPanel
{
    public class OutputViewModel : IDisposable
    {
        public OutputViewModel()
        {
            this.Messages = new BindableCollection<string, Message>(WS.MessageHandler.Messages, x => $"[{x.Dispacer}]{x.Content}");
            this.Bindables.Add(this.Messages);
        }

        public Bindables Bindables { get; init; } = new();

        public BindableCollection<string, Message> Messages { get; init; }

        public void OnClearButtonClick()
        {
            WS.MessageHandler.ClearMessage();
        }

        public void OnCopyButtonClick()
        {
            var builder = new StringBuilder();

            foreach (var x in WS.MessageHandler.Messages)
            {
                builder.AppendLine($"[{x.Dispacer}]{x.Content}");
            }

            IAttemptResult result = WS.ClipbordManager.SetToClipBoard(builder.ToString());

            OutputViewModelStringContent type;
            ErrorLevel errorLevel;

            if (result.IsSucceeded)
            {
                type = OutputViewModelStringContent.SettingOutputToClipboardSucceeded;
                errorLevel = ErrorLevel.Log;
            }
            else
            {
                type = OutputViewModelStringContent.SettingOutputToClipboardFailed;
                errorLevel = ErrorLevel.Error;
            }

            string message = WS.StringHandler.GetContent(type);

            WS.MessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher, errorLevel);
            WS.SnackbarHandler.Enqueue(message);
        }

        public void OnOpenLogWindowButtonClick()
        {
            WS.WindowsHelper.OpenWindow(() => new LogWindow()
            {
                Owner = Application.Current.MainWindow,
            });
        }

        public void Dispose()
        {
            this.Bindables.Dispose();
        }
    }
}
