using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using MaterialDesignThemes.Wpf;
using Niconicome.Models.Domain.Local.Addons.Core;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Local.State;
using Niconicome.Views.AddonPage.Install;
using Niconicome.Views.AddonPage.Pages;
using Prism.Regions;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage.Subwindows.AddonManager
{
    class AddonManagerViewModel : BindableBase, IDialogAware
    {
        public AddonManagerViewModel(IRegionManager regionManager)
        {
            this.RegionManager = new ReactiveProperty<IRegionManager>(regionManager.CreateRegionManager());
            this.RequestClose += _ => { };
            this.Queue = WS::AddonPage.Queue;
        }

        #region field

        #endregion

        #region Props

        public ISnackbarHandler Queue { get; init; }

        public ReactiveProperty<IRegionManager> RegionManager { get; init; }

        #endregion

        #region Command



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
            this.RegionManager.Value.RegisterViewWithRegion(AddonManagerRegionName.Name, typeof(MainPage));
        }

        #endregion
    }

    class AddonManagerViewModelD
    {

        public ISnackbarHandler? Queue { get; init; } = null;

        public ReactiveProperty<IRegionManager> RegionManager { get; init; } = new();
    }

    static class AddonManagerRegionName
    {
        public static string Name => "addonPageRegion";
    }
}
