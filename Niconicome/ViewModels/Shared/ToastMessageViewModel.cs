using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Local.State.Toast;

namespace Niconicome.ViewModels.Shared
{
    public class ToastMessageViewModel
    {
        public ToastMessageViewModel(IToastMessage message)
        {
            this.Message = message.Message;
            this.Dispatcher = message.Dispatcher;
            this.AddedAt = message.AddedAt.ToString("HH:mm");
            this.HasAction = message.HasAction;
            this.ActionString = message.ActionString;
            this.ActionObject = message.ActionObject;
        }

        #region Props

        /// <summary>
        /// メッセージ
        /// </summary>
        public string Message { get; init; }

        /// <summary>
        /// 発行者
        /// </summary>
        public string Dispatcher { get; init; }

        /// <summary>
        /// メッセージ発行日時
        /// </summary>
        public string AddedAt { get; init; }

        /// <summary>
        /// アクション文字列
        /// </summary>
        public string ActionString { get; init; }

        /// <summary>
        /// アクションが存在するかどうか
        /// </summary>
        public bool HasAction { get; init; }

        /// <summary>
        /// アクション
        /// </summary>
        public Action ActionObject { get; init; }

        #endregion
    }
}
