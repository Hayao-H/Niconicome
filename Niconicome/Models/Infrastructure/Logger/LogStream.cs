using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Utils.AppEnvironment;
using Niconicome.Models.Domain.Utils.NicoLogger;
using Const = Niconicome.Models.Const;

namespace Niconicome.Models.Infrastructure.Log
{
    public class LogStream : ILogWriter
    {
        public LogStream(IOSInfomationHandler osInfomationHandler, IAppInfomationHandler appInfomationHandler)
        {
            this._osInfomationHandler = osInfomationHandler;
            this._appInfomationHandler = appInfomationHandler;
        }

        #region private

        private bool _isInitialized;

        private bool _headerHasWritten;

        private readonly IOSInfomationHandler _osInfomationHandler;

        private readonly IAppInfomationHandler _appInfomationHandler;

        #endregion

        #region Method

        public void Write(string message)
        {
            if (!this._isInitialized)
            {
                try
                {
                    this.Initialize();
                }
                catch { }
            }

            if (!this._headerHasWritten)
            {
                try
                {
                    this.WriteHeader();
                }
                catch { }
            }

            this.WriteInternal(message);
        }

        #endregion

        #region Props

        public string LogFilePath { get; private set; } = "ログファイルの作成に失敗しました。";

        #endregion

        #region private

        /// <summary>
        /// 初期化
        /// </summary>
        private void Initialize()
        {
            if (this._isInitialized) return;

            string logFolder = Path.Combine(AppContext.BaseDirectory, Const::FileFolder.LogFolderName);
            if (!Directory.Exists(logFolder))
            {
                Directory.CreateDirectory(logFolder);
            }

            this.LogFilePath = Path.Combine(logFolder, $"niconicome-log-{DateTime.Now:yy-MM-dd-HH-mm-ss}.log");

            this._isInitialized = true;
        }

        /// <summary>
        /// ヘッダーを書き込む
        /// </summary>
        private void WriteHeader()
        {
            if (this._headerHasWritten) return;

            var systemInfo = new StringBuilder();
            string is64bit = this._osInfomationHandler.Is64BitOperatingSYstem ? "64" : "32";
            string is64bitProcess = this._appInfomationHandler.Is64BitProcess ? "64" : "32";

            systemInfo.AppendLine("Niconicomeログ");
            systemInfo.AppendLine($"App Version : {Assembly.GetExecutingAssembly().GetName().Version}");
            systemInfo.AppendLine($"App Path : {Process.GetCurrentProcess().MainModule?.FileName}");
            systemInfo.AppendLine($"OS : {this._osInfomationHandler.OSversionString}");
            systemInfo.AppendLine($"OS Version : {this._osInfomationHandler.OSversion}");
            systemInfo.AppendLine($"OS Architecture : {is64bit}bit Operating System");
            systemInfo.AppendLine($"Process : {is64bitProcess}bit Process");
            systemInfo.AppendLine($"Date : {DateTime.Now:yy/MM/dd H:mm:ss}");
            systemInfo.AppendLine("-".Repeat(50));

            this.WriteInternal(systemInfo.ToString());
            this._headerHasWritten = true;
        }

        /// <summary>
        /// 書き込みの実処理
        /// </summary>
        /// <param name="message"></param>
        private void WriteInternal(string message)
        {
            if (!this._isInitialized) return;

            try
            {
                using var writer = new StreamWriter(this.LogFilePath, true);
                writer.WriteLine(message);
            }
            catch { }
        }

        #endregion
    }
}
