using System;
using System.Collections.ObjectModel;
using Niconicome.Models.Domain.Local.Addons.Core;
using Prism.Services.Dialogs;
using Reactive.Bindings;
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
        }

        #region field

        private readonly IDialogService dialogService;

        #endregion

        #region Props

        public ReadOnlyReactiveCollection<AddonInfomationViewModel> Addons { get; init; }

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

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {

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
    }
}
