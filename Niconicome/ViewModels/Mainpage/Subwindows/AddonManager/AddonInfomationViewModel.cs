using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.Core;
using Niconicome.Models.Helper.Result;
using Niconicome.ViewModels.Controls;
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

        public AddonInfomation AddonInfomation { get; init; }

        public ReadOnlyReactiveProperty<string?> Name { get; init; }

        public ReadOnlyReactiveProperty<string?> Version { get; init; }

        public ReadOnlyReactiveProperty<string?> Author { get; init; }

        public ReadOnlyReactiveProperty<string?> Description { get; init; }

        public ReadOnlyReactiveProperty<int> ID { get; init; }

        public AsyncReactiveCommand<int> UninstallCommand { get; init; }

    }
}
