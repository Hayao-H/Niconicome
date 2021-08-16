using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using Niconicome.Models.Domain.Local.Addons.Core;
using Niconicome.Models.Helper.Result.Generic;
using Niconicome.Models.Local.Addon.Result;
using Niconicome.Models.Local.Settings;
using Niconicome.Views.AddonPage.Install;
using Prism.Regions;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage.Subwindows.AddonManager.AddonManagerPages
{
    class MainPageViewModel : BindableBase
    {
        public MainPageViewModel(IRegionManager regionManager, IDialogService dialogService)
        {
            this.dialogService = dialogService;
            this.Addons = WS::AddonPage.AddonHandler.Addons.ToReadOnlyReactiveCollection(value => new AddonInfomationViewModel(value, dialogService));
            this.FailedAddons = WS::AddonPage.AddonHandler.LoadFailedAddons.ToReadOnlyReactiveCollection(value => new FailedAddonViewModel(value, dialogService));
            this.IsAddonDebuggingEnable = WS::AddonPage.SettingsContainer.GetReactiveBoolSetting(SettingsEnum.IsAddonDebugEnable);

            this.InstallCommand = WS::AddonPage.InstallManager.IsInstalling
                .Select(value => !value)
                .ToReactiveCommand()
                .WithSubscribe(() =>
                {
                    dialogService.Show(nameof(AddonInstallWindow));
                }).AddTo(this.disposables);
        }

        #region field

        private readonly IDialogService dialogService;

        #endregion

        #region Props

        public ReadOnlyReactiveCollection<AddonInfomationViewModel> Addons { get; init; }

        public ReadOnlyReactiveCollection<FailedAddonViewModel> FailedAddons { get; init; }

        public ReactiveProperty<bool> IsAddonDebuggingEnable { get; init; }

        #endregion

        #region Command

        public ReactiveCommand InstallCommand { get; init; }


        #endregion
    }

    class MainPageViewModelD
    {

        public MainPageViewModelD()
        {
            var addon1 = new AddonInfomation();
            addon1.Name.Value = "テスト1";
            addon1.Description.Value = "View用のアドオンです。何もできません。";
            var addon2 = new AddonInfomation();
            addon2.Name.Value = "テスト2";
            addon2.Description.Value = "View用のアドオンです。(ry";
            this.Addons = new ObservableCollection<AddonInfomationViewModel>() { new AddonInfomationViewModel(addon1), new AddonInfomationViewModel(addon2) };

            var result = new FailedAddonResult(Guid.NewGuid().ToString("D"), "てすと", true);
            this.FailedAddons = new ObservableCollection<FailedAddonViewModel>() { new FailedAddonViewModel(result) };
        }

        public ObservableCollection<AddonInfomationViewModel> Addons { get; init; }

        public ObservableCollection<FailedAddonViewModel> FailedAddons { get; init; }

        public ReactiveCommand<int> UninstallCommand { get; init; } = new();

        public ReactiveProperty<bool> IsAddonDebuggingEnable { get; init; } = new();

        public ReactiveCommand InstallCommand { get; init; } = new();


    }
}
