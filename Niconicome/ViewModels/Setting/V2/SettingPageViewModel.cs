﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Niconicome.Models.Const;
using Niconicome.ViewModels.Mainpage.Tabs;
using Prism.Regions;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Setting.V2
{
    public class SettingPageViewModel : TabViewModelBase
    {
        public SettingPageViewModel(IRegionManager regionManager) : base("設定", LocalConstant.SettingTabID, true)
        {
            this._regionManager = regionManager;
            this.CloseCommand.Subscribe(s =>
            {
                IRegion region = this._regionManager.Regions[LocalConstant.TopTabRegionName];
                IEnumerable<object> viewsToRemove = region.Views.Where(v =>
                {
                    if (v is not UserControl control) return false;
                    if (control.DataContext is not TabViewModelBase vm) return false;
                    return vm.ID == LocalConstant.SettingTabID;
                });

                foreach (var view in viewsToRemove)
                {
                    region.Remove(view);
                    if (view is IDisposable disposableView)
                    {
                        disposableView.Dispose();
                    }
                }

                WS::Mainpage.LocalState.IsSettingTabOpen = false;
            });
        }

        private readonly IRegionManager _regionManager;
    }
}
