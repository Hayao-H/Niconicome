using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Windows;
using Niconicome.Models.Domain.Local.Addons.Core;
using Niconicome.Views.AddonPage.Install;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage.Subwindows.AddonManager
{
    class AddonManagerViewModel : BindableBase, IDialogAware
    {
        public AddonManagerViewModel(IDialogService dialogService)
        {
            this.RequestClose += _ => { };
            this.dialogService = dialogService;
            this.Addons = WS::AddonPage.AddonHandler.Addons.ToReadOnlyReactiveCollection(value => new AddonInfomationViewModel(value));

            this.InstallCommand = WS::AddonPage.AddonHandler.IsInstalling
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

        #endregion

        #region Command

        public ReactiveCommand InstallCommand { get; init; }

        #endregion

        #region IDialogAware

        /// <summary>
        /// タイトル
        /// </summary>
        public string Title { get; init; } = "アドオン管理ウィンドウ";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            WS::AddonPage.LocalInfo.IsAddonManagerOpen = false;
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            WS::AddonPage.LocalInfo.IsAddonManagerOpen = true;
        }

        #endregion
    }

    class AddonManagerViewModelD
    {
        public AddonManagerViewModelD()
        {
            var addon1 = new AddonInfomation();
            addon1.Name.Value = "テスト1";
            addon1.Description.Value = "View用のアドオンです。何もできません。";
            var addon2 = new AddonInfomation();
            addon2.Name.Value = "テスト2";
            addon2.Description.Value = "View用のアドオンです。(ry";
            this.Addons = new ObservableCollection<AddonInfomationViewModel>() { new AddonInfomationViewModel(addon1), new AddonInfomationViewModel(addon2) };
        }

        public ObservableCollection<AddonInfomationViewModel> Addons { get; init; }

        public ReactiveCommand InstallCommand { get; init; } = new();

    }
}
