using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Utils.AppEnvironment;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Local.State.Style
{
    public interface IWindowStyleManager
    {
        /// <summary>
        /// スタイル情報を取得
        /// </summary>
        /// <returns></returns>
        IAttemptResult<WindowStyle> GetStyle();

        /// <summary>
        /// スタイル情報を保存
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        IAttemptResult SaveTyle(WindowStyle style);
    }

    public class WindowStyleManager : IWindowStyleManager
    {
        public WindowStyleManager(IOSInfomationHandler oSInfomationHandler, ISettingsContainer settingsContainer)
        {
            this._osInfomation = oSInfomationHandler;
            this._settingsContainer = settingsContainer;
        }

        #region field

        private readonly IOSInfomationHandler _osInfomation;

        private readonly ISettingsContainer _settingsContainer;

        #endregion

        #region Method

        public IAttemptResult<WindowStyle> GetStyle()
        {

            IAttemptResult<StyleSettingsContainer> result = this.GetSettings();
            if (!result.IsSucceeded || result.Data is null)
            {
                return AttemptResult<WindowStyle>.Fail(result.Message);
            }

            StyleSettingsContainer settings = result.Data;
            int top = settings.Top.Value;
            int left = settings.Left.Value;
            int width = settings.Width.Value;
            int height = settings.Height.Value;

            int workAreaHeight = (int)this._osInfomation.WorkAreHeight;
            int workAreaWidth = (int)this._osInfomation.WorkAreWidth;

            if (height > workAreaHeight)
            {
                height = workAreaHeight;
            }

            if (width > workAreaWidth)
            {
                width = workAreaWidth;
            }

            if (left + width > workAreaWidth)
            {
                left = 0;
            }

            if (top + height > workAreaHeight)
            {
                top = 0;
            }

            return AttemptResult<WindowStyle>.Succeeded(new WindowStyle(top, left, height, width));
        }


        public IAttemptResult SaveTyle(WindowStyle style)
        {
            IAttemptResult<StyleSettingsContainer> result = this.GetSettings();
            if (!result.IsSucceeded || result.Data is null)
            {
                return AttemptResult.Fail(result.Message);
            }

            StyleSettingsContainer settings = result.Data;
            settings.Top.Value = style.Top;
            settings.Left.Value = style.Left;
            settings.Width.Value = style.Width;
            settings.Height.Value = style.Height;

            return AttemptResult.Succeeded();
        }


        #endregion

        private IAttemptResult<StyleSettingsContainer> GetSettings()
        {

            IAttemptResult<ISettingInfo<int>> topResult = this._settingsContainer.GetSetting(SettingNames.WindowTop, -1);
            if (!topResult.IsSucceeded || topResult.Data is null)
            {
                return AttemptResult<StyleSettingsContainer>.Fail(topResult.Message);
            }

            IAttemptResult<ISettingInfo<int>> leftResult = this._settingsContainer.GetSetting(SettingNames.WindowLeft, -1);
            if (!leftResult.IsSucceeded || leftResult.Data is null)
            {
                return AttemptResult<StyleSettingsContainer>.Fail(leftResult.Message);
            }

            IAttemptResult<ISettingInfo<int>> widthResult = this._settingsContainer.GetSetting(SettingNames.WindowWidth, -1);
            if (!widthResult.IsSucceeded || widthResult.Data is null)
            {
                return AttemptResult<StyleSettingsContainer>.Fail(widthResult.Message);
            }

            IAttemptResult<ISettingInfo<int>> heightResult = this._settingsContainer.GetSetting(SettingNames.WindowHeight, -1);
            if (!heightResult.IsSucceeded || heightResult.Data is null)
            {
                return AttemptResult<StyleSettingsContainer>.Fail(heightResult.Message);
            }

            return AttemptResult<StyleSettingsContainer>.Succeeded(new StyleSettingsContainer(topResult.Data, leftResult.Data, widthResult.Data, heightResult.Data));
        }

        private record StyleSettingsContainer(ISettingInfo<int> Top, ISettingInfo<int> Left, ISettingInfo<int> Width, ISettingInfo<int> Height);
    }

    public record WindowStyle(int Top, int Left, int Height, int Width);
}
