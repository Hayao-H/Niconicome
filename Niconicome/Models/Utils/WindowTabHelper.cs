using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Niconicome.Models.Const;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Local.State;
using Niconicome.Views;
using Niconicome.Views.AddonPage.V2;
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
        /// <param name="dialogService"></param>
        void OpenDownloadTaskWindow(IRegionManager regionManager, IDialogService dialogService);

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
        public void OpenDownloadTaskWindow(IRegionManager regionManager, IDialogService dialogService)
        {

            if (Application.Current is not PrismApplication app) return;

            IContainerProvider container = app.Container;

            if (this._localState.IsTaskWindowOpen && this._settingHandler.GetBoolSetting(SettingsEnum.SingletonWindows))
            {
                return;
            }

            if (this._settingHandler.GetBoolSetting(SettingsEnum.ShowTasksAsTab))
            {
                IRegion region = regionManager.Regions[LocalConstant.TopTabRegionName];
                var view = container.Resolve<DownloadTasksWindows>();
                region.Add(view);
                region.Activate(view);
                this._localState.IsTaskWindowOpen = true;
            }
            else
            {
                dialogService.Show(nameof(DownloadTasksWindows));
            }
        }

        public void OpenAddonManager(IRegionManager regionManager)
        {

            if (Application.Current is not PrismApplication app) return;

            IContainerProvider container = app.Container;

            if (this._localState.IsAddonManagerOpen && this._settingHandler.GetBoolSetting(SettingsEnum.SingletonWindows))
            {
                return;
            }

            IRegion region = regionManager.Regions[LocalConstant.TopTabRegionName];
            var view = container.Resolve<MainManager>();
            region.Add(view);
            region.Activate(view);
            this._localState.IsAddonManagerOpen = true;
        }

        public void OpenSettingsTab(IRegionManager regionManager)
        {

            if (Application.Current is not PrismApplication app) return;

            IContainerProvider container = app.Container;

            if (this._localState.IsSettingTabOpen && this._settingHandler.GetBoolSetting(SettingsEnum.SingletonWindows))
            {
                return;
            }

            IRegion region = regionManager.Regions[LocalConstant.TopTabRegionName];
            var view = container.Resolve<SettingPage>();
            region.Add(view);
            region.Activate(view);
            this._localState.IsSettingTabOpen = true;
        }


        #endregion
    }
}
