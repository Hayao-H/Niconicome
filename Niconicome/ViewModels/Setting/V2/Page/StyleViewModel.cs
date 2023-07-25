using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Local.Settings.EnumSettingsValue;
using Niconicome.Models.Utils.Reactive.Command;
using Niconicome.Models.Utils.Reactive;
using Niconicome.ViewModels.Mainpage.Utils;
using Niconicome.ViewModels.Setting.V2.Utils;
using SC = Niconicome.ViewModels.Setting.V2.Page.StringContent.StyleVMStringContent;
using WS = Niconicome.Workspaces.SettingPageV2;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Const;
using Niconicome.ViewModels.Shared;

namespace Niconicome.ViewModels.Setting.V2.Page
{
    public class StyleViewModel : AlertViewModel
    {
        public StyleViewModel()
        {
            this.Bindables.Add(this._alertBindables);

            var light = new SelectBoxItem<ApplicationThemeSettings>(WS.StringHandler.GetContent(SC.Light), ApplicationThemeSettings.Light);
            var dark = new SelectBoxItem<ApplicationThemeSettings>(WS.StringHandler.GetContent(SC.Dark), ApplicationThemeSettings.Dark);
            var inherit = new SelectBoxItem<ApplicationThemeSettings>(WS.StringHandler.GetContent(SC.Inherit), ApplicationThemeSettings.Inherit);
            this.SelectableThemes = new() { inherit, light, dark };

            this.SelectedTheme = new BindableProperty<ApplicationThemeSettings>(WS.Themehandler.GetTheme()).Subscribe(value =>
            {
                WS.Themehandler.SetTheme(value);
                if (value == ApplicationThemeSettings.Inherit)
                {
                    string message = WS.StringHandler.GetContent(SC.NeedRestart);
                    string restart = WS.StringHandler.GetContent(SC.Restart);
                    this.ShowAlert(message, AlertType.Info, () => WS.PowerManager.Restart(), restart);
                }
            }).AddTo(this.Bindables);
        }

        /// <summary>
        /// 変更監視オブジェクト
        /// </summary>
        public Bindables Bindables { get; init; } = new();

        /// <summary>
        /// 選択可能なテーマ
        /// </summary>
        public List<SelectBoxItem<ApplicationThemeSettings>> SelectableThemes { get; init; }

        /// <summary>
        /// 選択されたテーマ
        /// </summary>        
        public IBindableProperty<ApplicationThemeSettings> SelectedTheme { get; init; }


        /// <summary>
        /// スタイルを書き出す
        /// </summary>
        public void SaveStyleCommand()
        {
            IAttemptResult result = WS.UserChromeHandler.SaveStyle();
            if (!result.IsSucceeded)
            {
                string message = WS.StringHandler.GetContent(SC.FailedToWriteUserChrome);
                string messageD = WS.StringHandler.GetContent(SC.FailedToWriteUserChromeDetail, result.Message ?? string.Empty);
                WS.MessageHandler.AppendMessage(messageD, LocalConstant.SystemMessageDispacher, ErrorLevel.Log);
                this.ShowAlert(message, AlertType.Error);
            }
            else
            {
                string message = WS.StringHandler.GetContent(SC.WritingUserChromeHasCompleted);
                WS.MessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher, ErrorLevel.Log);
                this.ShowAlert(message, AlertType.Info);
            }
        }
    }
}
