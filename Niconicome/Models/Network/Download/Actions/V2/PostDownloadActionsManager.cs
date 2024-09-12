using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Machine;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Domain.Utils.NicoLogger;
using Niconicome.Models.Utils.Reactive;
using Niconicome.ViewModels.Setting.Utils;
using Err = Niconicome.Models.Network.Download.Actions.V2.PostDownloadActionsManagerError;

namespace Niconicome.Models.Network.Download.Actions.V2
{
    interface IPostDownloadActionssManager
    {
        /// <summary>
        /// DL完了後のアクション
        /// </summary>
        IBindableProperty<PostDownloadActions> PostDownloadAction { get; }

        /// <summary>
        /// 設定されたアクションを実行する
        /// </summary>
        void HandleAction();
    }

    public class PostDownloadActionsManager : IPostDownloadActionssManager
    {
        public PostDownloadActionsManager(IComPowerManager comPowerManager, ISettingsContainer conainer, IErrorHandler errorHandler)
        {
            this._comPower = comPowerManager;
            this._settingsContainer = conainer;
            this._errorHandler = errorHandler;
            this.PostDownloadAction = new BindableProperty<PostDownloadActions>(this._settingsContainer.GetOnlyValue(SettingNames.PostDownloadAction, PostDownloadActions.None).Data)
                .Subscribe(value =>
                {
                    if (this._postDownloadAction is null)
                    {
                        var result = this._settingsContainer.GetSetting(SettingNames.PostDownloadAction, PostDownloadActions.None);
                        if (!result.IsSucceeded || result.Data is null) return;
                        this._postDownloadAction = result.Data;
                    }

                    if (this._postDownloadAction.Value == value) return;
                    this._postDownloadAction.Value = value;
                    this.PostDownloadAction!.Value = value;

                });
        }

        private readonly IComPowerManager _comPower;

        private readonly ISettingsContainer _settingsContainer;

        private readonly IErrorHandler _errorHandler;

        private ISettingInfo<PostDownloadActions>? _postDownloadAction;

        public IBindableProperty<PostDownloadActions> PostDownloadAction { get; init; }

        public void HandleAction()
        {
            if (this.PostDownloadAction is null) return;
            PostDownloadActions action = this.PostDownloadAction.Value;

            if (action == PostDownloadActions.Shutdown)
            {
                this._errorHandler.HandleError(Err.ShutDown);
                this._comPower.Shutdown();
            }
            else if (action == PostDownloadActions.Restart)
            {
                this._errorHandler.HandleError(Err.Restart);
                this._comPower.Restart();
            }
            else if (action == PostDownloadActions.LogOff)
            {
                this._errorHandler.HandleError(Err.LogOff);
                this._comPower.LogOff();
            }
            else if (action == PostDownloadActions.Sleep)
            {
                this._errorHandler.HandleError(Err.Sleep);
                this._comPower.Sleep();
            }
        }


    }
}
