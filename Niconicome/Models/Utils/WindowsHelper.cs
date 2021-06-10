using System;
using System.Linq;
using System.Windows;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result.Generic;
using Niconicome.Models.Local.Settings;

namespace Niconicome.Models.Utils
{
    public interface IWindowsHelper
    {
        IAttemptResult<bool> OpenWindow<T>() where T : Window, new();
        IAttemptResult<bool> OpenWindow<T>(Func<T> factory) where T : Window;
    }

    class WindowsHelper : IWindowsHelper
    {
        public WindowsHelper(ILocalSettingHandler settingHandler,ILogger logger)
        {
            this.settingHandler = settingHandler;
            this.logger = logger;
        }

        #region field

        private readonly ILocalSettingHandler settingHandler;

        private readonly ILogger logger;

        #endregion

        /// <summary>
        /// ウィンドウを起動する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IAttemptResult<bool> OpenWindow<T>() where T : Window, new()
        {
            return this.OpenWindow(() => new T());
        }

        /// <summary>
        /// ファクトリーメソッドを与えて起動
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="factory"></param>
        /// <returns></returns>
        public IAttemptResult<bool> OpenWindow<T>(Func<T> factory) where T : Window
        {
            if (this.settingHandler.GetBoolSetting(SettingsEnum.SingletonWindows))
            {
                T? window = Application.Current.Windows.OfType<T>().FirstOrDefault();
                if (window is not null)
                {
                    try
                    {
                        window.Activate();
                    }
                    catch (Exception e)
                    {
                        this.logger.Error($"ウィンドウの全面化に失敗しました。(type:{typeof(T).Name})", e);
                        return new AttemptResult<bool>() { Message = "ウィンドウの全面化に失敗しました。", Exception = e };
                    }

                    this.logger.Log($"ウィンドウ（{typeof(T).Name}）を全面化しました。");
                    return new AttemptResult<bool>() { Data = true };
                }
            }

            var newWindows = factory();
            try
            {
                newWindows.Show();
            }
            catch (Exception e)
            {
                this.logger.Error($"ウィンドウの起動に失敗しました。(type:{typeof(T).Name})", e);
                return new AttemptResult<bool>() { Message = "ウィンドウの起動に失敗しました。", Exception = e };
            }

            this.logger.Log($"ウィンドウ（{typeof(T).Name}）を起動しました。");
            return new AttemptResult<bool>() { Data = true };
        }

    }
}
