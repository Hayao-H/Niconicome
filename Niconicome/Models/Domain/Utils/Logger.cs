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
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Domain.Utils.NicoLogger;

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

        /// <summary>
        /// 結果をもとにログ出力する
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="result">結果</param>
        void Error(string message, IAttemptResult result);

        bool IsDebugMode { get; set; }
    }

    public interface IErrorHandler
    {
        string GetErrorInfo(string message, Exception e);
    }

    public class Logger : ILogger
    {

        private readonly ILogWriter logStream;

        private readonly IErrorHandler errorHandler;

        public Logger(ILogWriter logStream, IErrorHandler errorHandler)
        {
            this.logStream = logStream;
            this.errorHandler = errorHandler;
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

        public void Error(string message, IAttemptResult result)
        {
            if (result.Exception is not null)
            {
                this.Error(message, result.Exception);
            }
            else
            {
                this.Error(message);
            }
        }

        public bool IsDebugMode { get; set; }


        /// <summary>
        /// ログファイル・出力に書き込む
        /// </summary>
        /// <param name="source"></param>
        private void Write(string source, bool force = false)
        {
            if (!force && !this.IsDebugMode) return;

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


}
