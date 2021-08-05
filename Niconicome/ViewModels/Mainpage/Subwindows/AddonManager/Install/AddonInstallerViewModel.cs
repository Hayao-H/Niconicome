using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Niconicome.ViewModels.Controls;
using Niconicome.Views.AddonPage.Install;
using Niconicome.Views.Mainpage.Region;
using Prism.Regions;
using Prism.Services.Dialogs;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage.Subwindows.AddonManager.Install
{
    class AddonInstallerViewModel : IDialogAware
    {
        public AddonInstallerViewModel(IRegionManager regionManager, IDialogService service)
        {
            this.RequestClose += _ => { };
            this.RegionManager = new ReactiveProperty<IRegionManager>(regionManager.CreateRegionManager());
            this.service = service;

            this.ToNext = new[]
            {
                this.currentPage.Select(_=>this.CanNavigate()),
                WS::AddonPage.InstallManager.IsSelected.Select(value=>value&&this.currentPage.Value==1),
                WS::AddonPage.InstallManager.IsLoaded.Select(value=>value&&this.currentPage.Value==2)
            }.CombineLatest(x => x.Any(v => v))
            .ToReactiveCommand()
                .WithSubscribe(() =>
                {
                    if (!this.CanNavigate()) return;
                    string page = this.GetNextPage();
                    this.RegionManager.Value.RequestNavigate(AddonRegionName.Name, page);
                    this.currentPage.Value++;
                });

            if (WS::AddonPage.InstallManager.IsInstalling.Value)
            {
                this.CloseWindow();
                return;
            }
        }

        #region field

        private ReactiveProperty<int> currentPage = new(0);

        private bool isUpdate;

        private readonly IDialogService service;

        #endregion

        #region methods

        private void CloseWindow()
        {
            this.RequestClose?.Invoke(new DialogResult());
        }

        #endregion

        #region Props

        public ReactiveProperty<IRegionManager> RegionManager { get; init; }

        public ReactiveCommand ToNext { get; init; }

        #endregion

        #region privae

        private bool CanNavigate()
        {
            return this.currentPage.Value switch
            {
                0 => true,
                1 => WS::AddonPage.InstallManager.IsSelected.Value,
                2 => WS::AddonPage.InstallManager.IsLoaded.Value,
                3 => false,
                _ => false
            };
        }

        private string GetNextPage()
        {
            return this.currentPage.Value switch
            {
                0 => nameof(FileOpenPage),
                1 => nameof(AddonLoadPage),
                2 => nameof(AddonInstallPage),
                _ => nameof(FileOpenPage)
            };
        }

        #endregion

        #region IDialogAware

        /// <summary>
        /// タイトル
        /// </summary>
        public string Title { get; init; } = "アドオンインストーラー";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            if (WS::AddonPage.InstallManager.IsInstalling.Value)
            {
                IDialogResult result = CommonMessageBoxAPI.Show(this.service, "インストールを中止しますか？", CommonMessageBoxAPI.MessageType.Warinng, CommonMessageBoxButtons.Yes | CommonMessageBoxButtons.No | CommonMessageBoxButtons.Cancel);

                if (result.Result == ButtonResult.Yes){
                    WS::AddonPage.InstallManager.Cancel();
                    return true;
                } else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            string page = this.GetNextPage();
            this.RegionManager.Value.RequestNavigate(AddonRegionName.Name, page);
            this.currentPage.Value++;

            this.isUpdate = parameters.GetValue<bool>("isupdate");
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
