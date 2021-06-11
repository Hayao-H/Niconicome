using System;
using MaterialDesignThemes.Wpf;
using Niconicome.Models.Domain.Utils;
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
        public ThemeHandler (IEnumSettingsHandler settingsHandler,ILogger logger)
        {
            this.settingsHandler = settingsHandler;
            this.logger = logger;
        }

        #region field

        private readonly IEnumSettingsHandler settingsHandler;

        private readonly ILogger logger;

        #endregion

        /// <summary>
        /// テーマを取得する
        /// </summary>
        /// <returns></returns>
        public ApplicationThemeSettings GetTheme()
        {
            return this.settingsHandler.GetSetting<ApplicationThemeSettings>();
        }

        /// <summary>
        /// テーマを取得する
        /// </summary>
        /// <param name="setting"></param>
        public void SetTheme(ApplicationThemeSettings setting)
        {
            this.settingsHandler.SaveSetting(setting);

            var paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();

            if (setting == ApplicationThemeSettings.Inherit)
            {
                return;
            }
            else if (setting == ApplicationThemeSettings.Light)
            {
                try
                {
                    theme.SetBaseTheme(Theme.Light);
                } catch (Exception e)
                {
                    this.logger.Error($"テーマの変更に失敗しました。（{setting}）", e);
                }
            }
            else
            {
                try
                {
                    theme.SetBaseTheme(Theme.Dark);
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
