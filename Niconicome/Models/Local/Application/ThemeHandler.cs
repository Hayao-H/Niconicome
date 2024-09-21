using System;
using MaterialDesignThemes.Wpf;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Local.Settings.EnumSettingsValue;
using Niconicome.Models.Utils.Reactive;
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
        /// 初期化
        /// </summary>
        void Initialize();
    }

    public class ThemeHandler : IThemehandler
    {
        public ThemeHandler(ISettingsContainer settingsConainer, IErrorHandler errorHandler)
        {
            this._settingsConainer = settingsConainer;
            this._errorHandler = errorHandler;

            this.ApplicationTheme.Subscribe((theme) =>
            {
                this.SetTheme(theme);
            });
        }

        #region field

        private readonly ISettingsContainer _settingsConainer;

        private readonly IErrorHandler _errorHandler;

        #endregion

        public IBindableProperty<ApplicationThemeSettings> ApplicationTheme { get; init; } = new BindableProperty<ApplicationThemeSettings>(ApplicationThemeSettings.Inherit);

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

            if (setting == ApplicationThemeSettings.Inherit)
            {
                return;
            }
            else if (setting == ApplicationThemeSettings.Light)
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
            }

            this._errorHandler.HandleError(Err.ThemeChanged, setting.ToString());

        }

        public void Initialize()
        {
            IAttemptResult<ISettingInfo<ApplicationThemeSettings>> result = this._settingsConainer.GetSetting(SettingNames.ApplicationTheme, ApplicationThemeSettings.Inherit);
            if (!result.IsSucceeded || result.Data is null)
            {
                return;
            }

            var theme = result.Data.Value;
            this.ApplicationTheme.Value = theme;
        }

    }
}
