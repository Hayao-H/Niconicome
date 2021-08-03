using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.Addons.Core;
using Niconicome.Models.Helper.Result;
using Niconicome.ViewModels.Controls;
using Niconicome.Views.AddonPage.Install;
using Prism.Regions;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage.Subwindows.AddonManager
{
    public class AddonInfomationViewModel
    {
        public AddonInfomationViewModel(AddonInfomation info)
        {
            this.AddonInfomation = info;
            this.Name = info.Name.ToReadOnlyReactiveProperty();
            this.Version = info.Version.Select(v => v.ToString()).ToReadOnlyReactiveProperty();
            this.Author = info.Author.ToReadOnlyReactiveProperty();
            this.Description = info.Description.ToReadOnlyReactiveProperty();
            this.ID = info.ID.ToReadOnlyReactiveProperty();
            this.UpdateCommand = new ReactiveCommand();
            this.IconPath = info.IconPathRelative.ToReadOnlyReactiveProperty();

            this.UninstallCommand = new AsyncReactiveCommand<int>();
        }

        public AddonInfomationViewModel(AddonInfomation info, IDialogService dialogService)
        {

            this.AddonInfomation = info;
            this.Name = info.Name.ToReadOnlyReactiveProperty();
            this.Version = info.Version.Select(v => v.ToString()).ToReadOnlyReactiveProperty();
            this.Author = info.Author.ToReadOnlyReactiveProperty();
            this.Description = info.Description.ToReadOnlyReactiveProperty();
            this.ID = info.ID.ToReadOnlyReactiveProperty();
            this.IconPath = info.IconPathRelative.Select(p => Path.Combine(AppContext.BaseDirectory, FileFolder.AddonsFolder, info.PackageID.Value, p)).ToReadOnlyReactiveProperty();

            this.UpdateCommand = new ReactiveCommand()
                .WithSubscribe(() =>
                {
                    if (WS::AddonPage.InstallManager.IsInstalling.Value)
                    {
                        WS::AddonPage.Queue.Enqueue("ほかのアドオンをインストール中です。");
                        return;
                    }
                    WS::AddonPage.InstallManager.MarkAsUpdate(this.AddonInfomation);
                    dialogService.Show(nameof(AddonInstallWindow));
                });

            this.UninstallCommand = new AsyncReactiveCommand<int>()
                .WithSubscribe(async id =>
                {
                    MaterialMessageBoxResult messageBoxResult = await MaterialMessageBox.Show("本当にアンインストールしますか？", MessageBoxButtons.Yes | MessageBoxButtons.No | MessageBoxButtons.Cancel, MessageBoxIcons.Caution);

                    if (messageBoxResult != MaterialMessageBoxResult.Yes) return;

                    IAttemptResult result = WS::AddonPage.InstallManager.UnistallAddon(id);
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

        #region Props
        public AddonInfomation AddonInfomation { get; init; }

        public ReadOnlyReactiveProperty<string?> Name { get; init; }

        public ReadOnlyReactiveProperty<string?> Version { get; init; }

        public ReadOnlyReactiveProperty<string?> Author { get; init; }

        public ReadOnlyReactiveProperty<string?> Description { get; init; }

        public ReadOnlyReactiveProperty<int> ID { get; init; }

        public ReadOnlyReactiveProperty<string?> IconPath { get; init; }

        #endregion

        #region Commands
        public AsyncReactiveCommand<int> UninstallCommand { get; init; }

        public ReactiveCommand UpdateCommand { get; init; }


        #endregion

    }
}
