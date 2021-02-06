using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Niconicome.Extensions.System;
using Niconicome.Extensions;
using Niconicome.Models.Local.State;

namespace Niconicome.Models.Domain.Utils
{
    public interface ILogger
    {
        void Log(string message);
        void Log(object message);
        void Caution(string message);
        void Caution(object message);
        void Error(string message, Exception exception);
        void Error(string message);
    }

    public interface ILogStream
    {
        void WriteAsync(string source);
        void Write(string source);
        string FileName { get; }
    }

    public interface IErrorHandler
    {
        string GetErrorInfo(string message, Exception e);
    }

    public class Logger : ILogger
    {

        private readonly ILogStream logStream;

        private readonly IErrorHandler errorHandler;

        private readonly ILocalState localState;

        public Logger(ILogStream logStream, IErrorHandler errorHandler, ILocalState localState)
        {
            this.logStream = logStream;
            this.errorHandler = errorHandler;
            this.localState = localState;
        }

        /// <summary>
        /// ロガーを取得する
        /// </summary>
        /// <param name="isDebug"></param>
        /// <returns></returns>
        public static ILogger GetLogger()
        {
            return DIFactory.Provider.GetRequiredService<ILogger>();
        }

        /// <summary>
        /// ログ
        /// </summary>
        /// <param name="source"></param>
        public void Log(string source)
        {
            this.Write($"[Log]{source}");
        }

        /// <summary>
        /// ログ
        /// </summary>
        /// <param name="source"></param>
        public void Log(object source)
        {
            this.Write($"[Log]{source}");
        }

        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="source"></param>
        public void Caution(string source)
        {
            this.Write($"[Caution]{source}");
        }

        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="source"></param>
        public void Caution(object source)
        {
            this.Write($"[Caution]{source}");
        }

        /// <summary>
        /// エラー
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void Error(string source, Exception e)
        {
            string error = this.errorHandler.GetErrorInfo(source, e);
            this.WriteError(error);
        }

        /// <summary>
        /// エラー
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void Error(string source)
        {
            this.Write($"[Error]{source}", true);
        }

        /// <summary>
        /// ログファイル・出力に書き込む
        /// </summary>
        /// <param name="source"></param>
        private void Write(string source, bool force = false)
        {
            if (!force && !this.localState.IsDebugMode) return;

            string dt = DateTime.Now.ToString("HH:mm.ss");

            Debug.WriteLine($"[{dt}]{source}");
            this.logStream.Write($"[{dt}]{source}");
        }

        /// <summary>
        /// エラーを書き込む
        /// </summary>
        /// <param name="source"></param>
        private void WriteError(string source)
        {
            string dt = DateTime.Now.ToString("HH:mm.ss");

            Debug.WriteLine($"[{dt}]{source}");
            this.logStream.Write($"[{dt}]{source}");
        }
    }

    public class ErrorHandler : IErrorHandler
    {
        /// <summary>
        /// エラー情報を取得する
        /// </summary>
        /// <param name="message"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public string GetErrorInfo(string message, Exception e)
        {
            var result = new StringBuilder();

            result.AppendLine($"[Error] {message}");
            result.AppendLine("Niconicomeエラー情報");
            result.AppendLine("-".Repeat(50));
            result.AppendLine($"Exception Message : {e.Message}");
            result.AppendLine($"Exception Type : {e.GetType().Name}");
            result.AppendLine($"Stack Trace :\n{e.StackTrace}");

            while (e.InnerException != null)
            {
                e = e.InnerException;
                result.AppendLine("-".Repeat(20));
                result.AppendLine("InnerException");
                result.AppendLine($"Exception Message : {e.Message}");
                result.AppendLine($"Exception Type : {e.GetType().Name}");
                result.AppendLine($"Stack Trace :\n{e.StackTrace}");
                result.AppendLine("-".Repeat(20));
            }
            result.AppendLine("-".Repeat(50));

            return result.ToString();
        }
    }

    public class LogStream : ILogStream
    {
        public LogStream()
        {
            this.FileName = $"log\\niconicome-log-{DateTime.Now:yy-MM-dd-HH-mm-ss}.log";
            if (!Directory.Exists("log"))
            {
                Directory.CreateDirectory("log");
            }
        }

        public string FileName { get; init; }

        private bool isInitialized;

        /// <summary>
        /// ログファイルに書き込む
        /// </summary>
        /// <param name="source"></param>
        public async void WriteAsync(string source)
        {
            if (!this.isInitialized) this.Initialize();
            try
            {
                using var fs = new StreamWriter(this.FileName, true);
                await fs.WriteLineAsync(source);
            }
            catch { return; }
        }

        /// <summary>
        /// ログファイルに同期的に書き込む
        /// </summary>
        /// <param name="source"></param>
        public void Write(string source)
        {
            if (!this.isInitialized) this.Initialize();
            try
            {
                using var fs = new StreamWriter(this.FileName, true);
                fs.WriteLine(source);
            }
            catch { return; }
        }

        /// <summary>
        /// システム情報を書き込む
        /// </summary>
        private void Initialize()
        {
            var systemInfo = new StringBuilder();

            systemInfo.AppendLine("Niconicomeログ");
            systemInfo.AppendLine($"App Version : {Assembly.GetExecutingAssembly().GetName().Version}");
            systemInfo.AppendLine($"App Path : {Process.GetCurrentProcess().MainModule?.FileName}");
            systemInfo.AppendLine($"OS : {this.GetOs()}");
            systemInfo.AppendLine($"OS Version : {this.GetOsVer()}");
            systemInfo.AppendLine($"Date : {DateTime.Now:yy/MM/dd H:mm:ss}");
            systemInfo.AppendLine("-".Repeat(50));

            this.isInitialized = true;
            this.Write(systemInfo.ToString());
        }

        /// <summary>
        /// OSのバージョンを取得する
        /// </summary>
        /// <returns></returns>
        private string GetOsVer()
        {
            string v = Environment.OSVersion.VersionString;
            return v;
        }

        /// <summary>
        /// OS名を取得する
        /// </summary>
        /// <returns></returns>
        private string GetOs()
        {
            Version v = Environment.OSVersion.Version;
            PlatformID platform = Environment.OSVersion.Platform;
            string osversion = $"{v.Major}.{v.Minor}";

            if (platform == PlatformID.Win32NT)
            {
                if (osversion == "10.0")
                {
                    return "Windows 10";
                }
                else if (osversion == "6.3")
                {
                    return "Windows 8.1";
                }
                else if (osversion == "6.2")
                {
                    return "Windows 8";
                }
                else if (osversion == "6.1")
                {
                    return "Windows 7";
                }
                else if (osversion == "6.0")
                {
                    return "Windows Vista";
                }
                else if (osversion == "5.1")
                {
                    return "Windows XP";
                }
                else
                {
                    return "unknown OS (Windows)";
                }
            }
            else if (platform == PlatformID.Unix)
            {
                return "Unix";
            }
            else
            {
                return "unknown OS";
            }
        }

    }
}
