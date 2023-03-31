using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Local.State.MessageV2
{
    public interface IMessageHandler
    {
        /// <summary>
        /// メッセージ
        /// </summary>
        ReadOnlyObservableCollection<Message> Messages { get; }

        /// <summary>
        /// メッセージを追加
        /// </summary>
        /// <param name="content"></param>
        /// <param name="dispacer"></param>
        /// <param name="errorLevel"></param>
        void AppendMessage(string content, string dispacer, ErrorLevel errorLevel = ErrorLevel.Log);

        /// <summary>
        /// メッセージを全削除
        /// </summary>
        void ClearMessage();

        /// <summary>
        /// メッセージを文字列として取得
        /// </summary>
        /// <returns></returns>
        string GetAllMessage();
    }


    public class MessageHandler : IMessageHandler
    {
        public MessageHandler()
        {
            this.Messages = new ReadOnlyObservableCollection<Message>(this._messages);
            
        }

        #region field

        private readonly ObservableCollection<Message> _messages = new();

        private readonly object _localObj = new();

        #endregion

        #region Props

        public ReadOnlyObservableCollection<Message> Messages { get; init; }

        #endregion

        #region Method

        public void AppendMessage(string content, string dispacer, ErrorLevel errorLevel = ErrorLevel.Log)
        {
            var message = new Message(content, dispacer, DateTime.Now, errorLevel);
            lock (this._localObj)
            {
                this._messages.Add(message);
            }
        }

        public void ClearMessage()
        {
            this._messages.Clear();
        }

        public string GetAllMessage()
        {
            var list = new List<MessageForJson>();

            foreach (var message in this._messages)
            {
                list.Add(new MessageForJson() { Content = message.Content, Dispacher = message.Dispacer, ErrorLevel = message.ErrorLevel.ToString(), AddedAt = message.AddedAt });
            }

            return JsonParser.Serialize(list);
        }

        #endregion

        class MessageForJson
        {
            public string Content { get; set; } = "";

            public string Dispacher { get; set; } = "";

            public string ErrorLevel { get; set; } = "";

            public DateTime AddedAt { get; set; } 
        }
    }
}
