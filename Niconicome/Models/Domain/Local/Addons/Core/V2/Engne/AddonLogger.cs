﻿using System;
using System.Collections.ObjectModel;
using System.Text;
using Niconicome.Models.Domain.Utils;
using Reactive.Bindings;

namespace Niconicome.Models.Domain.Local.Addons.Core.V2.Engine
{
    public interface IAddonLogger
    {
        ObservableCollection<AddonLogNode> Messages { get; init; }

        void Error(string message, string addonName, Exception e);

        /// <summary>
        /// エラー出力
        /// </summary>
        /// <param name="message"></param>
        /// <param name="addonName"></param>
        void Error(string message, string addonName);

    }

    public class AddonLogger : IAddonLogger
    {
        #region field

        private readonly ILogger logger;

        #endregion

        public AddonLogger(ILogger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// エラー出力
        /// </summary>
        /// <param name="message"></param>
        /// <param name="addonName"></param>
        public void Error(string message, string addonName, Exception e)
        {
            var node = new AddonLogNode(message, addonName, e);
            this.Messages.Add(node);
            this.logger.Error($"({addonName}){message}", e);
        }

        public void Error(string message, string addonName)
        {
            var node = new AddonLogNode(message, addonName);
            this.Messages.Add(node);
            this.logger.Error($"({addonName}){message}");
        }


        /// <summary>
        /// メッセージ
        /// </summary>
        public ObservableCollection<AddonLogNode> Messages { get; init; } = new();

    }

    public class AddonLogNode
    {
        public AddonLogNode(string message, string addonnName, Exception? e = null)
        {
            this.Message = new ReactiveProperty<string>(message);
            this.AddonName = new ReactiveProperty<string>(addonnName);
            this.Exception = new ReactiveProperty<Exception?>(e);
        }

        public ReactiveProperty<string> Message { get; init; }

        public ReactiveProperty<string> AddonName { get; init; }

        public ReactiveProperty<Exception?> Exception { get; init; }
    }
}
