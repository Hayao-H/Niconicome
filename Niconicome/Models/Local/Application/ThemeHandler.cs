using System;
using MaterialDesignThemes.Wpf;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.Settings.EnumSettingsValue;
using Niconicome.Models.Utils;
using Niconicome.Models.Utils.Reactive;
using Windows.UI.ViewManagement;
using Err = Niconicome.Models.Local.Application.Error.ThemeHandlerError;

namespace Niconicome.Models.Local.Application
{
    public interface IThemehandler
    {
        /// <summary>
        /// テーマモード
        /// </summary>
        /// <returns></returns>
        IBindableProperty<ApplicationThemeSettings> ApplicationTheme { get; }

        /// <summary>
        /// ダークモードならdarkを返す
        /// それ以外なら空の文字列
        /// </summary>
        IBindableProperty<string> DarkMode { get; }

        /// <summary>
        /// システム設定に従えるかどうか」
        /// </summary>
        bool CanUseInheritOption { get; }

        /// <summary>
        /// 初期化
        /// </summary>
        void Initialize();
    }

    public class ThemeHandler : IThemehandler
    {
        public ThemeHandler(ISettingsContainer settingsConainer, IErrorHandler errorHandler, IOSThemeHandler oSThemeHandler)
        {
            this._settingsConainer = settingsConainer;
            this._errorHandler = errorHandler;

            this.ApplicationTheme.Subscribe((theme) =>
            {
                this.SetTheme(theme);
            });
            this._oSThemeHandler = oSThemeHandler;
        }

        ~ThemeHandler()
        {
            this._oSThemeHandler.ThemeChanged -= this.OnThemeChange;
        }

        #region field

        private readonly ISettingsContainer _settingsConainer;

        private readonly IErrorHandler _errorHandler;

        private readonly IOSThemeHandler _oSThemeHandler;

        #endregion

        public IBindableProperty<ApplicationThemeSettings> ApplicationTheme { get; init; } = new BindableProperty<ApplicationThemeSettings>(ApplicationThemeSettings.Inherit);

        public IBindableProperty<string> DarkMode { get; init; } = new BindableProperty<string>(string.Empty);

        public bool CanUseInheritOption => this._oSThemeHandler.FunctionHasCompatibility;

        public void Initialize()
        {
            this._oSThemeHandler.ThemeChanged += this.OnThemeChange;

            IAttemptResult<ISettingInfo<ApplicationThemeSettings>> result = this._settingsConainer.GetSetting(SettingNames.ApplicationTheme, ApplicationThemeSettings.Inherit);
            if (!result.IsSucceeded || result.Data is null)
            {
                return;
            }

            var theme = result.Data.Value;
            this.ApplicationTheme.Value = theme;

        }

        /// <summary>
        /// テーマを取得する
        /// </summary>
        /// <param name="setting"></param>
        private void SetTheme(ApplicationThemeSettings setting)
        {
            IAttemptResult<ISettingInfo<ApplicationThemeSettings>> result = this._settingsConainer.GetSetting(SettingNames.ApplicationTheme, ApplicationThemeSettings.Inherit);
            if (!result.IsSucceeded || result.Data is null)
            {
                return;
            }
            else
            {
                result.Data.Value = setting;
            }



            if (setting == ApplicationThemeSettings.Inherit)
            {
                setting = this._oSThemeHandler.IsDarkMode ? ApplicationThemeSettings.Dark : ApplicationThemeSettings.Light;
            }

            this.SetThemeInternal(setting);

        }

        /// <summary>
        /// テーマ変更処理
        /// </summary>
        /// <param name="setting"></param>
        private void SetThemeInternal(ApplicationThemeSettings setting)
        {
            var paletteHelper = new PaletteHelper();
            ITheme theme;
            try
            {
                theme = paletteHelper.GetTheme();
            }
            catch (Exception e)
            {
                this._errorHandler.HandleError(Err.FailedToGetITheme, e);
                return;
            }

            if (setting == ApplicationThemeSettings.Light)
            {
                try
                {
                    theme.SetBaseTheme(Theme.Light);
                    paletteHelper.SetTheme(theme);
                }
                catch (Exception e)
                {
                    this._errorHandler.HandleError(Err.FailedToSetITheme, e);
                }
            }
            else
            {
                try
                {
                    theme.SetBaseTheme(Theme.Dark);
                    paletteHelper.SetTheme(theme);
                }
                catch (Exception e)
                {
                    this._errorHandler.HandleError(Err.FailedToSetITheme, e);
                    return;
                }
                this.DarkMode.Value = "dark";
            }

            this._errorHandler.HandleError(Err.ThemeChanged, setting.ToString());
        }

        /// <summary>
        /// テーマ変更時
        /// </summary>
        private void OnThemeChange(object? sender, EventArgs e)
        {
            IAttemptResult<ApplicationThemeSettings> result = this._settingsConainer.GetOnlyValue(SettingNames.ApplicationTheme, ApplicationThemeSettings.Inherit);
            if (!result.IsSucceeded)
            {
                return;
            }

            if (result.Data != ApplicationThemeSettings.Inherit) return;

            this.SetThemeInternal(this._oSThemeHandler.IsDarkMode ? ApplicationThemeSettings.Dark : ApplicationThemeSettings.Light);
        }

    }
}
