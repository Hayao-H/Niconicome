using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Playlist;

namespace Niconicome.Models.Domain.Local.External
{
    public interface ICommandExecuter
    {
        IAttemptResult Execute(string path, string? arg = null);
    }

    class CommandExecuter : ICommandExecuter
    {
        public CommandExecuter(ILogger logger)
        {
            this.logger = logger;
        }

        #region フィールド
        private readonly ILogger logger;
        #endregion

        /// <summary>
        /// コマンドを実行して結果を返す
        /// </summary>
        /// <param name="path"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public IAttemptResult Execute(string path, string? arg = null)
        {
            var process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = path;
            process.StartInfo.Arguments = arg ?? string.Empty;

            try
            {
                process.Start();
            } catch (Exception e)
            {
                this.logger.Error($"コマンドの実行に失敗しました。(path:{path}, arg: {arg})",e);
                return new AttemptResult()
                {
                    Message= $"コマンドの実行に失敗しました。(path:{path}, arg: {arg})",
                    Exception = e,
                };
            }

            return new AttemptResult()
            {
                IsSucceeded = true
            };

        }

    }
}
