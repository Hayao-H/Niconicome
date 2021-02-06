using System;

namespace Niconicome.Models.Domain.Niconico.Download
{
    public interface IDownloadMessenger
    {
        void SendMessage(string message);
        void AddHandler(Action<string> handler);
        void RemoveHandler(Action<string> handler);
        string Message { get; }
    }


    /// <summary>
    /// メッセンジャー
    /// </summary>
    public class DownloadMessanger : IDownloadMessenger
    {

        /// <summary>
        /// メッセージを登録する
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(string message)
        {
            this.Message = message;
            this.RaiseOnMessage();
        }

        public string Message { get; private set; } = string.Empty;


        /// <summary>
        /// イベントハンドラを追加する
        /// </summary>
        /// <param name="newHandler"></param>
        public void AddHandler(Action<string> newHandler)
        {
            this.handler += newHandler;
        }

        /// <summary>
        /// イベントハンドラを削除する
        /// </summary>
        /// <param name="handlerToRemove"></param>
        public void RemoveHandler(Action<string> handlerToRemove)
        {
            this.handler -= handlerToRemove;
        }


        private Action<string>? handler;

        private void RaiseOnMessage()
        {
            this.handler?.Invoke(this.Message);
        }
    }

}
