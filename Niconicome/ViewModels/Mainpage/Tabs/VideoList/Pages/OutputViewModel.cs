using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Error = Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Local.State.MessageV2;
using WS = Niconicome.Workspaces;
using Niconicome.Models.Utils.Reactive;
using Niconicome.Models.Helper.Result;
using Niconicome.ViewModels.Mainpage.Tabs.VideoList.Pages.StringContent;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Const;

namespace Niconicome.ViewModels.Mainpage.Tabs.VideoList.Pages
{
    public class OutputViewModel
    {
        public OutputViewModel()
        {
            this.Messages = new BindableCollection<MessageViewModel, Message>(WS::Mainpage.MessageHandler.Messages, m => new MessageViewModel(m));
            this.IsEnabled = new BindableProperty<bool>(false).AddTo(this.Bindables);
            this.Bindables.Add(this.Messages);
        }

        #region Props

        public Bindables Bindables { get; init; } = new();

        public IBindableProperty<bool> IsEnabled { get; init; }

        public BindableCollection<MessageViewModel, Message> Messages { get; init; }

        #endregion

        #region Method

        public void OnClearButtonClicked()
        {
            WS::Mainpage.MessageHandler.ClearMessage();
        }

        public void OnCopyButtonClicked()
        {
            string content = WS::Mainpage.MessageHandler.GetAllMessage();
            IAttemptResult result = WS::Mainpage.ClipbordManager.SetToClipBoard(content);

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

            string message = WS::Mainpage.StringHandler.GetContent(type);

            WS::Mainpage.MessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher, errorLevel);
            WS::Mainpage.SnackbarHandler.Enqueue(message);

        }


        public void OnOutputButtonClicked()
        {
            this.IsEnabled.Value = !this.IsEnabled.Value;
        }

        #endregion

    }

    public class MessageViewModel
    {
        public MessageViewModel(Message message)
        {
            this.Content = message.Content;
            this.Dispacher = message.Dispacer;
            this.AddedAt = message.AddedAt.ToString("HH:mm.ss");
            this.ErrorIcon = message.ErrorLevel switch
            {
                Error::ErrorLevel.Error => "fa-circle-exclamation",
                Error::ErrorLevel.Warning => "fa-triangle-exclamation",
                _ => "fa-circle-info",
            };
            this.ColorClass = message.ErrorLevel switch
            {
                Error::ErrorLevel.Error => "Red",
                Error::ErrorLevel.Warning => "Yellow",
                _ => ""
            };

        }

        public string Content { get; init; }

        public string Dispacher { get; init; }

        public string AddedAt { get; init; }

        public string ErrorIcon { get; init; }

        public string ColorClass { get; init; }
    }
}
