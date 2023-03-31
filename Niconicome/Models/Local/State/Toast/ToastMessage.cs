using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Local.State.Toast
{
    public interface IToastMessage
    {
        /// <summary>
        /// メッセージ
        /// </summary>
        string Message { get; }

        /// <summary>
        /// 発行者
        /// </summary>
        string Dispatcher { get; }

        /// <summary>
        /// メッセージ発行日時
        /// </summary>
        DateTime AddedAt { get; }

        /// <summary>
        /// アクション文字列
        /// </summary>
        string ActionString { get; }

        /// <summary>
        /// アクションが存在するかどうか
        /// </summary>
        bool HasAction { get; }

        /// <summary>
        /// アクション
        /// </summary>
        Action ActionObject { get; }
    }

    public class ToastMessage : IToastMessage
    {
        public ToastMessage(string message, string dispatcher, string actionString, Action actionObject)
        {
            this.Message = message;
            this.Dispatcher = dispatcher;
            this.AddedAt = DateTime.Now;
            this.ActionString = actionString;
            this.HasAction = true;
            this.ActionObject = actionObject;
        }

        public ToastMessage(string message, string dispatcher)
        {
            this.Message = message;
            this.Dispatcher = dispatcher;
            this.AddedAt = DateTime.Now;
            this.ActionString = string.Empty;
            this.HasAction = false;
            this.ActionObject = () => { };
        }


        #region Props

        public string Message { get; init; }

        public string Dispatcher { get; init; }

        public DateTime AddedAt { get; init; }

        public string ActionString { get; init; }

        public bool HasAction { get; init; }

        public Action ActionObject { get; init; }

        #endregion
    }
}
