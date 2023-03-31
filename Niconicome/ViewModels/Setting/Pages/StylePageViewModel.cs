using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Reactive.Linq;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.Settings.EnumSettingsValue;
using Niconicome.Models.Utils.Reactive;
using Niconicome.Models.Utils.Reactive.Command;
using Niconicome.ViewModels.Mainpage.Utils;
using Niconicome.ViewModels.Setting.Pages.String;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using WS = Niconicome.Workspaces;
using SC = Niconicome.ViewModels.Setting.Pages.String.StylePageVMStringContent;

namespace Niconicome.ViewModels.Setting.Pages
{
    class StylePageViewModel : BindableBase
    {
        public StylePageViewModel()
        {
            #region テーマ

            var light = new ComboboxItem<ApplicationThemeSettings>(ApplicationThemeSettings.Light, WS::SettingPage.StringHandler.GetContent(SC.Light));
            var dark = new ComboboxItem<ApplicationThemeSettings>(ApplicationThemeSettings.Dark, WS::SettingPage.StringHandler.GetContent(SC.Dark));
            var inherit = new ComboboxItem<ApplicationThemeSettings>(ApplicationThemeSettings.Inherit, WS::SettingPage.StringHandler.GetContent(SC.Inherit));
            this.SelectableThemes = new List<ComboboxItem<ApplicationThemeSettings>>() { inherit, light, dark };

            this.SelectedTheme = new BindableProperty<ComboboxItem<ApplicationThemeSettings>>(WS::SettingPage.Themehandler.GetTheme() switch {
                ApplicationThemeSettings.Light=>light,
                ApplicationThemeSettings.Dark =>dark,
                _=>inherit
            }).Subscribe(value =>
            {
                WS::SettingPage.Themehandler.SetTheme(value.Value);
                if (value.Value == ApplicationThemeSettings.Inherit)
                {
                    string message = WS::SettingPage.StringHandler.GetContent(SC.NeedRestart);
                    string restart = WS::SettingPage.StringHandler.GetContent(SC.Restart);
                    WS::SettingPage.SnackbarMessageQueue.Enqueue(message, restart, () => WS::SettingPage.PowerManager.Restart());
                }
            });


            this.SaveStyleCommand = new BindableCommand(() =>
                {
                    IAttemptResult result = WS::SettingPage.UserChromeHandler.SaveStyle();
                    if (!result.IsSucceeded)
                    {
                        string message = WS::SettingPage.StringHandler.GetContent(SC.FailedToWriteUserChrome);
                        string messageD = WS::SettingPage.StringHandler.GetContent(SC.FailedToWriteUserChromeDetail, result.Message ?? string.Empty);
                        WS::SettingPage.NewMessageHandler.AppendMessage(messageD, LocalConstant.SystemMessageDispacher, ErrorLevel.Log);
                        WS::SettingPage.SnackbarMessageQueue.Enqueue(message);
                    }
                    else
                    {
                        string message = WS::SettingPage.StringHandler.GetContent(SC.WritingUserChromeHasCompleted);
                        WS::SettingPage.NewMessageHandler.AppendMessage(message, LocalConstant.SystemMessageDispacher, ErrorLevel.Log);
                        WS::SettingPage.SnackbarMessageQueue.Enqueue(message);
                    }
                }, new BindableProperty<bool>(true));

            #endregion
        }

        /// <summary>
        /// 選択可能なテーマ
        /// </summary>
        public List<ComboboxItem<ApplicationThemeSettings>> SelectableThemes { get; init; }

        /// <summary>
        /// 選択されたテーマ
        /// </summary>
        public IBindableProperty<ComboboxItem<ApplicationThemeSettings>> SelectedTheme { get; init; }

        /// <summary>
        /// スタイルを書き出す
        /// </summary>
        public IBindableCommand SaveStyleCommand { get; init; }

    }

    class StylePageViewModelD
    {
        public StylePageViewModelD()
        {
            #region テーマ

            var light = new ComboboxItem<ApplicationThemeSettings>(ApplicationThemeSettings.Light, "ライト");
            var dark = new ComboboxItem<ApplicationThemeSettings>(ApplicationThemeSettings.Dark, "ダーク");
            var inherit = new ComboboxItem<ApplicationThemeSettings>(ApplicationThemeSettings.Inherit, "システム設定に従う");
            this.SelectableThemes = new List<ComboboxItem<ApplicationThemeSettings>>() { inherit, light, dark };
            this.SelectedTheme = new BindableProperty<ComboboxItem<ApplicationThemeSettings>>(light);

            #endregion
        }

        public List<ComboboxItem<ApplicationThemeSettings>> SelectableThemes { get; init; }

        public IBindableProperty<ComboboxItem<ApplicationThemeSettings>> SelectedTheme { get; init; }

        public IBindableCommand SaveStyleCommand { get; init; } = BindableCommand.Empty;
    }
}
