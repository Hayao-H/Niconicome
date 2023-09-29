using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Niconicome.Models.Const;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Local.State;
using Niconicome.ViewModels.Mainpage.Tabs;
using Niconicome.Views;
using Niconicome.Views.AddonPage.V2;
using Niconicome.Views.DownloadTask;
using Niconicome.Views.Setting.V2;
using Prism.Ioc;
using Prism.Regions;
using Prism.Services.Dialogs;
using Prism.Unity;

namespace Niconicome.Models.Utils
{
    public interface IWindowTabHelper
    {
        /// <summary>
        /// ダウンロードタスク一覧を開く
        /// </summary>
        /// <param name="regionManager"></param>
        void OpenDownloadTaskWindow(IRegionManager regionManager);

        /// <summary>
        /// アドオンマネージャーを新しいタブで開く
        /// </summary>
        /// <param name="regionManager"></param>
        void OpenAddonManager(IRegionManager regionManager);

        /// <summary>
        /// 設定タブを開く
        /// </summary>
        void OpenSettingsTab(IRegionManager regionManager);
    }

    public class WindowTabHelper : IWindowTabHelper
    {

        public WindowTabHelper(ILocalState localState, ILocalSettingHandler settingHandler)
        {
            this._localState = localState;
            this._settingHandler = settingHandler;
        }

        #region field

        private readonly ILocalState _localState;

        private readonly ILocalSettingHandler _settingHandler;

        #endregion

        #region Methods
        public void OpenDownloadTaskWindow(IRegionManager regionManager)
        {

            if (Application.Current is not PrismApplication app) return;

            IContainerProvider container = app.Container;

            IRegion region = regionManager.Regions[LocalConstant.TopTabRegionName];


            if (this._localState.IsTaskWindowOpen && this._settingHandler.GetBoolSetting(SettingsEnum.SingletonWindows))
            {
                foreach (var view in region.Views)
                {
                    if (view is not UserControl c) continue;
                    if (c.DataContext is not TabViewModelBase vm) continue;
                    if (vm.ID != LocalConstant.TaskTabID) continue;

                    region.Activate(view);
                }
            }
            else
            {
                var view = container.Resolve<DownloadTask>();
                region.Add(view);
                region.Activate(view);
                this._localState.IsTaskWindowOpen = true;
            }

        }

        public void OpenAddonManager(IRegionManager regionManager)
        {

            if (Application.Current is not PrismApplication app) return;

            IContainerProvider container = app.Container;

            IRegion region = regionManager.Regions[LocalConstant.TopTabRegionName];

            if (this._localState.IsAddonManagerOpen && this._settingHandler.GetBoolSetting(SettingsEnum.SingletonWindows))
            {
                foreach (var view in region.Views)
                {
                    if (view is not UserControl c) continue;
                    if (c.DataContext is not TabViewModelBase vm) continue;
                    if (vm.ID != LocalConstant.AddonManagerTabID) continue;

                    region.Activate(view);
                }
            }
            else
            {
                var view = container.Resolve<MainManager>();
                region.Add(view);
                region.Activate(view);
                this._localState.IsAddonManagerOpen = true;
            }
        }

        public void OpenSettingsTab(IRegionManager regionManager)
        {

            if (Application.Current is not PrismApplication app) return;

            IContainerProvider container = app.Container;

            IRegion region = regionManager.Regions[LocalConstant.TopTabRegionName];


            if (this._localState.IsSettingTabOpen && this._settingHandler.GetBoolSetting(SettingsEnum.SingletonWindows))
            {
                foreach (var view in region.Views)
                {
                    if (view is not UserControl c) continue;
                    if (c.DataContext is not TabViewModelBase vm) continue;
                    if (vm.ID != LocalConstant.SettingTabID) continue;

                    region.Activate(view);
                }
            }
            else
            {
                var view = container.Resolve<SettingPage>();
                region.Add(view);
                region.Activate(view);
                this._localState.IsSettingTabOpen = true;
            }
        }


        #endregion
    }
}
