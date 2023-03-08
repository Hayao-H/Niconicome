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
        /// <returns></returns>
        Task<IAttemptResult<IProcessResult>> StartProcessAsync(string filePath, string arg, bool useShell);
    }

    public class ProcessManager : IProcessManager
    {
        public ProcessManager(IErrorHandler errorHandler)
        {
            this._errorHandler = errorHandler;
        }

        #region field

        private readonly IErrorHandler _errorHandler;

        private Dictionary<string, Process> _processes = new();

        #endregion

        #region Method

        public Task<IAttemptResult<IProcessResult>> StartProcessAsync(string filePath, string arg, bool useShell)
        {
            var tcs = new TaskCompletionSource<IAttemptResult<IProcessResult>>();

            string id = Guid.NewGuid().ToString("D");
            var process = new Process();

            //GC防止
            this._processes.Add(id, process);

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

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
            process.Exited += (_, _) =>
            {
                var result = new ProcessResult(process.ExitCode, process.StandardOutput, process.StandardError);
                this._processes.Remove(id);
                tcs.SetResult(AttemptResult<IProcessResult>.Succeeded(result));
            };

            try
            {
                process.Start();
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
