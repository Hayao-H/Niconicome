using System;
using System.Diagnostics;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using OsApplication = System.Windows.Application;

namespace Niconicome.Models.Local.Application
{
    public interface IApplicationPowerManager
    {
        void ShutDown();
        IAttemptResult Restart();
    }

    class ApplicationPowerManager : IApplicationPowerManager
    {
        public ApplicationPowerManager(IShutdown shutdown, ILogger logger)
        {
            this.shutdown = shutdown;
            this.logger = logger;
        }

        #region field

        private readonly IShutdown shutdown;

        private readonly ILogger logger;

        #endregion

        /// <summary>
        /// シャットダウンする
        /// </summary>
        public void ShutDown()
        {
            OsApplication.Current.Shutdown();
        }

        /// <summary>
        /// 再起動する
        /// </summary>
        public IAttemptResult Restart()
        {
            string? filename = Process.GetCurrentProcess().MainModule?.FileName;
            if (filename is not null)
            {
                try
                {
                    Process.Start(filename);
                }
                catch (Exception e)
                {
                    this.logger.Error("新規プロセスの軌道に失敗しました。", e);
                    return new AttemptResult();
                }
            }

            OsApplication.Current.Shutdown();

            return new AttemptResult() { IsSucceeded = true };
        }
    }
}
