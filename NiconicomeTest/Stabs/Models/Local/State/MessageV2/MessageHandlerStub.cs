using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Local.State.MessageV2;

namespace NiconicomeTest.Stabs.Models.Local.State.MessageV2
{
    public class MessageHandlerStub : IMessageHandler
    {
        public MessageHandlerStub()
        {
            this.Messages = new ReadOnlyObservableCollection<Message>(new ObservableCollection<Message>());
        }

        public ReadOnlyObservableCollection<Message> Messages { get; init; }

        public void AppendMessage(string content, string dispacer, ErrorLevel errorLevel = ErrorLevel.Log)
        {

        }

        public void ClearMessage()
        {

        }

        public string GetAllMessage()
        {
            return string.Empty;
        }
    }
}
