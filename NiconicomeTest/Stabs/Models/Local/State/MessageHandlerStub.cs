using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Local.State;
using Reactive.Bindings;

namespace NiconicomeTest.Stabs.Models.Local.State
{
    internal class MessageHandlerStub : IMessageHandler
    {
        public ReactiveProperty<string> Message { get; init; } = new();

        public void AppendMessage(string message)
        {

        }

        public void ClearMessage()
        {

        }

        public void AddChangeHandler(Action handler)
        {

        }

        public void RemoveChangeHandler(Action handler)
        {

        }

    }
}
