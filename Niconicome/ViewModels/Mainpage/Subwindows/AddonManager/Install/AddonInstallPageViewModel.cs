﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Helper.Result;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage.Subwindows.AddonManager.Install
{
    class AddonInstallPageViewModel : BindableBase
    {
        public AddonInstallPageViewModel()
        {
            this.Install = this.IsInstalling
                .Select(value => !value)
                .ToReactiveCommand()
                .WithSubscribe(() =>
                {
                    this.IsInstalling.Value = true;
                    this.Clear();
                    this.AppendLine("インストールを開始します。");

                    IAttemptResult result = WS::AddonPage.InstallManager.InstallAddon();

                    if (!result.IsSucceeded)
                    {
                        this.AppendLine("インストールに失敗しました。");
                        this.AppendLine($"詳細：{result.Message}");
                    } else
                    {
                        this.AppendLine("インストールが完了しました。");
                    }

                    this.IsInstalling.Value = false;

                }).AddTo(this.disposables);
        }

        #region fields

        private readonly StringBuilder builder = new();

        #endregion

        #region Props

        public ReactiveProperty<string> Message { get; init; } = new("準備完了。");

        public ReactiveProperty<bool> IsInstalling { get; init; } = new();

        #endregion

        #region Command

        public ReactiveCommand Install { get; init; }

        #endregion

        #region private

        /// <summary>
        /// メッセージ追記
        /// </summary>
        /// <param name="message"></param>
        private void AppendLine(string message)
        {
            this.builder.AppendLine(message);
            this.Message.Value = this.builder.ToString();
        }

        /// <summary>
        /// メッセージクリア
        /// </summary>
        private void Clear()
        {
            this.builder.Clear();
            this.Message.Value = string.Empty;
        }

        #endregion
    }

    class AddonInstallPageViewModelD
    {
        public ReactiveProperty<string> Message { get; init; } = new("準備完了。");

        public ReactiveProperty<bool> IsInstalling { get; init; } = new(true);

        public ReactiveCommand Install { get; init; } = new();
    }
}
