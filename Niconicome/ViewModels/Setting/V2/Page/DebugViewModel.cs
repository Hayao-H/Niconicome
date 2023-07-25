using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Utils.Reactive;
using Niconicome.ViewModels.Setting.Utils;
using Niconicome.ViewModels.Shared;
using WS = Niconicome.Workspaces.SettingPageV2;
using SC = Niconicome.ViewModels.Setting.V2.Page.StringContent.DebugVMSC;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Const;

namespace Niconicome.ViewModels.Setting.V2.Page
{
    public class DebugViewModel : AlertViewModel
    {
        public DebugViewModel()
        {

            this.Bindables.Add(this._alertBindables);

            this.IsDebugMode = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.IsDebugMode, false), false).AddTo(this.Bindables);

            this.IsDebugMode.Subscribe(value =>
            {
                WS.State.IsDebugMode = value;
                string isEnable = WS.StringHandler.GetContent(value ? SC.Enable : SC.Disable);
                string message = WS.StringHandler.GetContent(SC.Debug, isEnable);

                this.ShowAlert(message, AlertType.Info);
            });

            this.IsDevMode = new BindableSettingInfo<bool>(WS.SettingsContainer.GetSetting(SettingNames.IsDeveloppersMode, false), false).AddTo(this.Bindables);
        }

        /// <summary>
        /// 変更監視オブジェクト
        /// </summary>
        public Bindables Bindables { get; init; } = new();

        /// <summary>
        /// デバッグフラグ
        /// </summary>
        public IBindableSettingInfo<bool> IsDebugMode { get; init; }

        /// <summary>
        /// 開発者モード
        /// </summary>
        public IBindableSettingInfo<bool> IsDevMode { get; init; }

        /// <summary>
        /// ログファイルパス
        /// </summary>
        public string LogFilePath => WS.LocalInfo.LogFileName;

        /// <summary>
        /// ログファールのパスをコピー
        /// </summary>
        public void CopyLogFIlePath()
        {
            IAttemptResult result = WS.ClipbordManager.SetToClipBoard(this.LogFilePath);
            if (result.IsSucceeded)
            {
                string message = WS.StringHandler.GetContent(SC.LogFilePathCopied);
                this.ShowAlert(message, AlertType.Info);
            } else
            {
                string message = WS.StringHandler.GetContent(SC.LogFilePathCopyFailed);
                string messageD = WS.StringHandler.GetContent(SC.LogFilePathCopyFailedDetail,result.Message);

                this.ShowAlert(message, AlertType.Error);
                WS.MessageHandler.AppendMessage(messageD, LocalConstant.SystemMessageDispacher);
            }
        }
    }
}
