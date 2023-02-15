using System;
using System.Collections.Generic;
using System.Text;
using V2 = Niconicome.Models.Local.State.MessageV2;
using Reactive.Bindings;
using System.Collections.Specialized;
using Niconicome.Models.Const;

namespace Niconicome.Models.Local.State
{
    public interface IMessageHandler
    {
        ReactiveProperty<string> Message { get; }
        void AppendMessage(string message);
        void ClearMessage();
        void AddChangeHandler(Action handler);
        void RemoveChangeHandler(Action handler);
    }

    public class MessageHandler : IMessageHandler
    {
        public MessageHandler(V2::IMessageHandler messageHandler)
        {
            this._messageHandler = messageHandler;
        }


        private readonly StringBuilder messagefield = new();

        private readonly List<Action> changeEventHandlers = new();

        private readonly V2::IMessageHandler _messageHandler;

        /// <summary>
        /// イベントを発火する
        /// </summary>
        private void OnMessageChanged()
        {
            this.Message.Value = this.messagefield.ToString();

            foreach (var handler in this.changeEventHandlers)
            {
                handler();
            }
        }

        /// <summary>
        /// ハンドラーを追加する
        /// </summary>
        /// <param name="handler"></param>
        public void AddChangeHandler(Action handler)
        {
            if (this.changeEventHandlers.Contains(handler)) return;
            this.changeEventHandlers.Add(handler);
        }

        /// <summary>
        /// ハンドラーを削除する
        /// </summary>
        /// <param name="handler"></param>
        public void RemoveChangeHandler(Action handler)
        {
            this.changeEventHandlers.RemoveAll(h => h.Equals(handler));
        }

        /// <summary>
        /// メッセージ
        /// </summary>
        public ReactiveProperty<string> Message { get; init; } = new();

        /// <summary>
        /// メッセージを追加する
        /// </summary>
        /// <param name="message"></param>
        public void AppendMessage(string message)
        {
            this._messageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher, Domain.Utils.Error.ErrorLevel.Log);
        }

        /// <summary>
        /// メッセージを全て削除する
        /// </summary>
        public void ClearMessage()
        {
            this.messagefield.Clear();
            this.OnMessageChanged();
        }
    }
}
