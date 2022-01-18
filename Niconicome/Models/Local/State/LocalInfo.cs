using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Models.Local.Settings;
using Reactive.Bindings;
using Utils = Niconicome.Models.Domain.Utils;

namespace Niconicome.Models.Local.State
{
    public interface ILocalInfo
    {
        string LogFileName { get; }
        string ApplicationVersion { get; }
        bool IsMultiWindowsAllowed { get; }
    }

    public class LocalInfo : ILocalInfo
    {
        public LocalInfo(ILocalSettingsContainer container)
        {
            this.settingsContainer = container;
        }

        #region field

        private ILocalSettingsContainer settingsContainer;

        #endregion

        /// <summary>
        /// ログファイル名
        /// </summary>
        public string LogFileName
        {
            get
            {
                var logStraem = Utils::DIFactory.Provider.GetRequiredService<Utils::ILogStream>();

                return Path.Combine(AppContext.BaseDirectory, logStraem.FileName);
            }
        }

        /// <summary>
        /// アプリバージョン
        /// </summary>
        public string ApplicationVersion
        {
            get
            {
                var v = Assembly.GetExecutingAssembly().GetName().Version;
                return v?.ToString() ?? "Nan";
            }
        }

        /// <summary>
        /// マルチウィンドウ
        /// </summary>
        public bool IsMultiWindowsAllowed => !this.settingsContainer.GetReactiveBoolSetting(SettingsEnum.SingletonWindows).Value;


    }
}
