using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Views.AddonPage.Install;
using Niconicome.Views.Mainpage.Region;
using Prism.Regions;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage.Subwindows.AddonManager.Install
{
    class AddonInstallerViewModel : IDialogAware
    {
        public AddonInstallerViewModel(IRegionManager regionManager)
        {
            this.RequestClose += _ => { };
            this.RegionManager = new ReactiveProperty<IRegionManager>(regionManager.CreateRegionManager());

            this.TestCommand = new ReactiveCommand()
                .WithSubscribe(() =>
                {
                    this.RegionManager.Value.RequestNavigate(AddonRegionName.Name, nameof(FileOpenPage), result =>
                     {
                         result.ToString();
                     });
                });

            if (WS::AddonPage.InstallManager.IsInstalling.Value)
            {
                this.CloseWindow();
                return;
            }
        }

        #region field


        #endregion

        #region methods

        private void CloseWindow()
        {
            this.RequestClose?.Invoke(new DialogResult());
        }

        #endregion

        #region Props

        public ReactiveProperty<IRegionManager> RegionManager { get; init; }

        public ReactiveCommand TestCommand { get; init; }

        #endregion

        #region IDialogAware

        /// <summary>
        /// タイトル
        /// </summary>
        public string Title { get; init; } = "アドオンインストーラー";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return !WS::AddonPage.InstallManager.IsInstalling.Value;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }

        #endregion
    }

    class AddonInstallerViewModelD
    {
        public ReactiveCommand ToNext { get; init; } = new();

        public ReactiveProperty<IRegionManager> RegionManager { get; init; } = new();

    }

    public class AddonRegionName
    {
        public const string Name = "AddonInstallRegion";
    }
}
