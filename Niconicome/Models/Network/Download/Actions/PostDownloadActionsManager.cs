using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Auth;
using Niconicome.Models.Domain.Local.Machine;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.Settings;
using Niconicome.ViewModels.Setting.Utils;

namespace Niconicome.Models.Network.Download.Actions
{
    interface IPostDownloadActionssManager
    {
        /// <summary>
        /// DL完了後のアクション
        /// </summary>
        SettingInfoViewModel<PostDownloadActions> PostDownloadAction { get; }

        /// <summary>
        /// 設定されたアクションを実行する
        /// </summary>
        void HandleAction();
    }

    class PostDownloadActionsManager : IPostDownloadActionssManager
    {
        public PostDownloadActionsManager(IComPowerManager comPowerManager, ILogger logger,ISettingsConainer conainer)
        {
            this.comPowerManager = comPowerManager;
            this.logger = logger;
            this.PostDownloadAction = new SettingInfoViewModel<PostDownloadActions>(conainer.GetSetting(SettingNames.PostDownloadAction, PostDownloadActions.None),PostDownloadActions.None);
        }

        #region field

        private readonly IComPowerManager comPowerManager;

        private readonly ILogger logger;

        #endregion

        #region Props

        public SettingInfoViewModel<PostDownloadActions> PostDownloadAction { get; init; }

        #endregion

        #region Methods

        public void HandleAction()
        {
            PostDownloadActions action = this.PostDownloadAction.Value;
            DateTime now = DateTime.Now;

            if (action == PostDownloadActions.Shutdown)
            {
                this.logger.Log($"コンピューターをシャットダウンします。({now})");
                this.comPowerManager.Shutdown();
            }
            else if (action == PostDownloadActions.Restart)
            {
                this.logger.Log($"コンピューターを再起動します。({now})");
                this.comPowerManager.Restart();
            }
            else if (action == PostDownloadActions.LogOff)
            {
                this.logger.Log($"Windowsからログオフします。({now})");
                this.comPowerManager.LogOff();
            }
            else if (action == PostDownloadActions.Sleep)
            {
                this.logger.Log($"コンピューターを休止状態に移行します。({now})");
                this.comPowerManager.Sleep();
            }
        }

        #endregion

    }
}
