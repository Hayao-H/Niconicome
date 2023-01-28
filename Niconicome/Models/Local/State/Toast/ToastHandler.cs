using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Const;
using Niconicome.Models.Local.Settings;
using MaterialDesign = MaterialDesignThemes.Wpf;

namespace Niconicome.Models.Local.State.Toast
{
    public interface IToastHandler
    {
        /// <summary>
        /// キューに追加する
        /// </summary>
        /// <param name="message"></param>
        void Enqueue(string message);

        /// <summary>
        /// アクション付きでキューに追加する
        /// </summary>
        /// <param name="message"></param>
        /// <param name="action"></param>
        /// <param name="actionFunc"></param>
        void Enqueue(string message, string action, Action actionFunc);

        /// <summary>
        /// ハンドラを登録
        /// </summary>
        /// <param name="handler"></param>
        void RegisterToastHandler(Action<IToastMessage> handler);

        /// <summary>
        /// ハンドラを削除
        /// </summary>
        /// <param name="handler"></param>
        void UnRegisterToastHandler(Action<IToastMessage> handler);

    }


    public class ToastHandler : IToastHandler
    {

        #region field

        private readonly List<Action<IToastMessage>> _handlers = new();

        #endregion


        #region Method

        public void Enqueue(string message)
        {
            this.OnMessage(new ToastMessage(message, "System"));
        }

        public void Enqueue(string message, string action, Action actionFunc)
        {
            this.OnMessage(new ToastMessage(message, "System", action, actionFunc));
        }

        public void RegisterToastHandler(Action<IToastMessage> handler)
        {
            this._handlers.Add(handler);
        }

        public void UnRegisterToastHandler(Action<IToastMessage> handler)
        {
            this._handlers.Remove(handler);
        }

        #endregion

        #region private

        /// <summary>
        /// メッセージがキューされた場合
        /// </summary>
        /// <param name="message"></param>
        private void OnMessage(IToastMessage message)
        {
            foreach (var handler in this._handlers)
            {
                try
                {
                    handler(message);
                }
                catch { }
            }
        }

        #endregion

    }
}
