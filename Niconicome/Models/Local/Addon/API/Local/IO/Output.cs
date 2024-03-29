﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Local.State.MessageV2;

namespace Niconicome.Models.Local.Addon.API.Local.IO
{
    public interface IOutput : IAPIBase
    {
        /// <summary>
        /// 書き込む
        /// </summary>
        /// <param name="content"></param>
        void write(string content);

        /// <summary>
        /// オブジェクトを書き込む
        /// </summary>
        /// <param name="content"></param>
        void write(object content);
    }

    public class Output : APIBase, IOutput
    {
        public Output(IMessageHandler messageHandler)
        {
            this.messageHandler = messageHandler;
        }

        #region field

        private readonly IMessageHandler messageHandler;

        #endregion

        #region methods

        public void write(string content)
        {
            this.messageHandler.AppendMessage($"{content}", "Addon", ErrorLevel.Log);
        }

        public void write(object content)
        {
            this.write(content.ToString() ?? string.Empty);
        }

        #endregion
    }
}
