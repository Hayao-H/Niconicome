using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.External.Software.NiconicomeProcess
{

    public interface IProcessManager
    {
        /// <summary>
        /// プロセスを作成して起動
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="arg"></param>
        /// <param name="useShell"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        Task<IAttemptResult<IProcessResult>> StartProcessAsync(string filePath, string arg, bool useShell, int timeoutSecond);
    }

    public class ProcessManager : IProcessManager
    {
        public ProcessManager(IErrorHandler errorHandler)
        {
            this._errorHandler = errorHandler;
        }

        #region field

        private readonly IErrorHandler _errorHandler;

        #endregion

        #region Method

        public Task<IAttemptResult<IProcessResult>> StartProcessAsync(string filePath, string arg, bool useShell, int timeoutSecond)
        {
            var tcs = new TaskCompletionSource<IAttemptResult<IProcessResult>>();

            string id = Guid.NewGuid().ToString("D");
            using var process = new Process();
            var stdout = new StringBuilder();
            var stderr = new StringBuilder();

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.OutputDataReceived += (_, e) => { if (e.Data is not null) stdout.AppendLine(e.Data); };
            process.ErrorDataReceived += (_, e) => { if (e.Data is not null) stderr.AppendLine(e.Data); };

            var commands = new List<string>();
            if (useShell)
            {
                process.StartInfo.FileName = Environment.GetEnvironmentVariable("ComSpec") ?? "cmd.exe";
                commands.Add("/c");
                commands.Add(filePath);
            }
            else
            {
                process.StartInfo.FileName = filePath;
            }
            commands.Add(arg);

            process.StartInfo.Arguments = string.Join(" ", commands);

            try
            {
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.WaitForExit(timeoutSecond * 1000);

                var result = new ProcessResult(process.ExitCode, stdout.ToString(), stderr.ToString());
                tcs.SetResult(AttemptResult<IProcessResult>.Succeeded(result));
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(ProcessManagerError.FailedToStartProcess, ex);
                tcs.SetResult(AttemptResult<IProcessResult>.Fail(this._errorHandler.GetMessageForResult(ProcessManagerError.FailedToStartProcess, ex)));
            }

            return tcs.Task;
        }


        #endregion
    }
}
