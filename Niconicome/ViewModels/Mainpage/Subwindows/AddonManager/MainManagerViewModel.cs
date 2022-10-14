using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Niconicome.Models.Const;
using Niconicome.ViewModels.Mainpage.BottomTabs;
using Niconicome.ViewModels.Mainpage.Tabs;
using Prism.Regions;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage.Subwindows.AddonManager
{
    public class MainManagerViewModel : TabViewModelBase
    {
        public MainManagerViewModel(IRegionManager regionManager) : base("アドオンマネージャー", LocalConstant.AddonManagerTabID, true)
        {

            this._regionManager = regionManager;
            this.CloseCommand.Subscribe(s =>
            {
                IRegion region = this._regionManager.Regions[LocalConstant.TopTabRegionName];
                IEnumerable<object> viewsToRemove = region.Views.Where(v =>
                {
                    if (v is not UserControl control) return false;
                    if (control.DataContext is not TabViewModelBase vm) return false;
                    return vm.ID == LocalConstant.AddonManagerTabID;
                });

                foreach (var view in viewsToRemove)
                {
                    region.Remove(view);
                    if (view is IDisposable disposableView)
                    {
                        disposableView.Dispose();
                    }
                }

                WS::Mainpage.LocalState.IsAddonManagerOpen = false;
            });
        }

        #region field

        private readonly IRegionManager _regionManager;

        #endregion
    }
}
