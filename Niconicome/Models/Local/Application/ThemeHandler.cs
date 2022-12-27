using System;
using MaterialDesignThemes.Wpf;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Local.Settings.EnumSettingsValue;

namespace Niconicome.Models.Local.Application
{
    public interface IThemehandler
    {
        ApplicationThemeSettings GetTheme();
        void SetTheme(ApplicationThemeSettings theme);
        void Initialize();
    }

    public class ThemeHandler : IThemehandler
    {
        public ThemeHandler(ISettingsConainer settingsConainer, ILogger logger)
        {
            this.logger = logger;
            this._settingsConainer = settingsConainer;
        }

        #region field

        private readonly ISettingsConainer _settingsConainer;

        private readonly ILogger logger;

        #endregion

        /// <summary>
        /// テーマを取得する
        /// </summary>
        /// <returns></returns>
        public ApplicationThemeSettings GetTheme()
        {
            IAttemptResult<ISettingInfo<ApplicationThemeSettings>> result = this._settingsConainer.GetSetting(SettingNames.ApplicationTheme, ApplicationThemeSettings.Inherit);
            if (!result.IsSucceeded || result.Data is null)
            {
                return ApplicationThemeSettings.Inherit;
            }
            else
            {
                return result.Data.Value;
            }
        }

        /// <summary>
        /// テーマを取得する
        /// </summary>
        /// <param name="setting"></param>
        public void SetTheme(ApplicationThemeSettings setting)
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
                this.logger.Error($"IThemeの取得に失敗しました。（{setting}）", e);
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
                    this.logger.Error($"テーマの変更に失敗しました。（{setting}）", e);
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
                    this.logger.Error($"テーマの変更に失敗しました。（{setting}）", e);
                }
            }

            this.logger.Log($"テーマを変更しました。（{setting}）");

        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            var theme = this.GetTheme();
            this.SetTheme(theme);
        }

    }
}
