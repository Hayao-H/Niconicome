using System;
using System.Collections.Generic;
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

            this.SelectedTheme.Subscribe(value => WS::SettingPage.Themehandler.SetTheme(value.Value)).AddTo(this.disposables);

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
    }
}
