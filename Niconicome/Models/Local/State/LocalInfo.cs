using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Models.Domain.Utils.NicoLogger;
using Niconicome.Models.Local.Settings;
using Reactive.Bindings;
using Utils = Niconicome.Models.Domain.Utils;

namespace Niconicome.Models.Local.State
{
    public interface ILocalInfo
    {
        /// <summary>
        /// ログファイル名
        /// </summary>
        string LogFileName { get; }

        /// <summary>
        /// アプリケーションのバージョン
        /// </summary>
        string ApplicationVersion { get; }
    }

    public class LocalInfo : ILocalInfo
    {

        /// <summary>
        /// ログファイル名
        /// </summary>
        public string LogFileName
        {
            get
            {
                var logStraem = Utils::DIFactory.Resolve<ILogWriter>();

                return logStraem.LogFilePath;
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

    }
}
