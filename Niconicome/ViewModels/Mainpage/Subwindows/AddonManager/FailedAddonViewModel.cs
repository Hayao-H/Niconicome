using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.Addon.Result;
using Niconicome.ViewModels.Controls;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage.Subwindows.AddonManager
{
    class FailedAddonViewModel
    {
        public FailedAddonViewModel(FailedAddonResult result) : this(result, null) { }
        public FailedAddonViewModel(FailedAddonResult result, IDialogService? service)
        {
            this.PackageID = new ReactiveProperty<string>(result.PackageID);
            this.Details = new ReactiveProperty<string>(result.Message);
            this.CanUninstall = new ReactiveProperty<bool>(result.CanUninstall);
            this.service = service;
            this.UninstallCommand = new AsyncReactiveCommand()
                .WithSubscribe(async () =>
                {
                    if (this.service is null)
                    {
                        return;
                    }

                    if (!this.CanUninstall.Value)
                    {
                        WS::AddonPage.Queue.Enqueue("このアドオンはアンインストールできません。");
                        return;
                    }

                    IDialogResult messageBoxResult = CommonMessageBoxAPI.Show(this.service!, "本当にアンインストールしますか？", CommonMessageBoxAPI.MessageType.Warinng, CommonMessageBoxButtons.Yes | CommonMessageBoxButtons.No | CommonMessageBoxButtons.Cancel);

                    if (messageBoxResult.Result != ButtonResult.Yes) return;

                    IAttemptResult result = await WS::AddonPage.InstallManager.UnistallAddonAsync(this.PackageID.Value);
                    if (!result.IsSucceeded)
                    {
                        WS::AddonPage.Queue.Enqueue("アンインストールに失敗しました。");
                    }
                    else
                    {
                        WS::AddonPage.Queue.Enqueue("アンインストールしました。");
                    }
                });
        }

        #region field

        private readonly IDialogService? service;

        #endregion

        #region Props

        public ReactiveProperty<string> PackageID { get; init; }

        public ReactiveProperty<string> Details { get; init; }

        public ReactiveProperty<bool> CanUninstall { get; init; }

        #endregion

        #region Command

        public AsyncReactiveCommand UninstallCommand { get; init; }

        #endregion
    }
}
