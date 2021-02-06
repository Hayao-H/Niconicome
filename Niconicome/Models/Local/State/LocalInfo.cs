using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Utils = Niconicome.Models.Domain.Utils;

namespace Niconicome.Models.Local.State
{
    public interface ILocalInfo
    {
        public string LogFileName { get; }
        public string ApplicationVersion { get; }
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

    }
}
