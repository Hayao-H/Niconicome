using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Reactive.Linq;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.Settings.EnumSettingsValue;
using Niconicome.ViewModels.Mainpage.Utils;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Setting.Pages
{
    class StylePageViewModel : BindableBase
    {
        public StylePageViewModel()
        {
            #region テーマ

            var light = new ComboboxItem<ApplicationThemeSettings>(ApplicationThemeSettings.Light, "ライト");
            var dark = new ComboboxItem<ApplicationThemeSettings>(ApplicationThemeSettings.Dark, "ダーク");
            var inherit = new ComboboxItem<ApplicationThemeSettings>(ApplicationThemeSettings.Inherit, "システム設定に従う");
            this.SelectableThemes = new List<ComboboxItem<ApplicationThemeSettings>>() { inherit, light, dark };
            this.SelectedTheme = WS::SettingPage.Themehandler.GetTheme() switch
            {
                ApplicationThemeSettings.Light => new ReactiveProperty<ComboboxItem<ApplicationThemeSettings>>(light),
                ApplicationThemeSettings.Dark => new ReactiveProperty<ComboboxItem<ApplicationThemeSettings>>(dark),
                _ => new ReactiveProperty<ComboboxItem<ApplicationThemeSettings>>(inherit)
            };

            this.SelectedTheme.Skip(1).Subscribe(value =>
            {
                WS::SettingPage.Themehandler.SetTheme(value.Value);
                if (value.Value == ApplicationThemeSettings.Inherit)
                {
                    WS::SettingPage.SnackbarMessageQueue.Enqueue("システムテーマを適用するには、再起動する必要があります。", "再起動", () => WS::SettingPage.PowerManager.Restart());
                }
            }).AddTo(this.disposables);

            this.SaveStyleCommand = new ReactiveCommand()
                .WithSubscribe(() =>
                {
                    IAttemptResult result = WS::SettingPage.StyleHandler.SaveUserChrome();
                    if (!result.IsSucceeded)
                    {
                        WS::SettingPage.MessageHandler.AppendMessage($"スタイルファイルの書き出しに失敗しました。(詳細:{result.Exception?.Message ?? "None"})");
                        WS::SettingPage.SnackbarMessageQueue.Enqueue("スタイルファイルの書き出しに失敗しました。");
                    }
                    else
                    {
                        WS::SettingPage.MessageHandler.AppendMessage("スタイルファイルを書き出しにました");
                        WS::SettingPage.SnackbarMessageQueue.Enqueue("スタイルファイルを書き出しました。");
                    }
                });

            #endregion
        }

        /// <summary>
        /// 選択可能なテーマ
        /// </summary>
        public List<ComboboxItem<ApplicationThemeSettings>> SelectableThemes { get; init; }

        /// <summary>
        /// 選択されたテーマ
        /// </summary>
        public ReactiveProperty<ComboboxItem<ApplicationThemeSettings>> SelectedTheme { get; init; }

        /// <summary>
        /// スタイルを書き出す
        /// </summary>
        public ReactiveCommand SaveStyleCommand { get; init; }

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
            this.SelectedTheme = new ReactiveProperty<ComboboxItem<ApplicationThemeSettings>>(light);

            #endregion
        }

        public List<ComboboxItem<ApplicationThemeSettings>> SelectableThemes { get; init; }

        public ReactiveProperty<ComboboxItem<ApplicationThemeSettings>> SelectedTheme { get; init; }

        public ReactiveCommand SaveStyleCommand { get; init; } = new();
    }
}
