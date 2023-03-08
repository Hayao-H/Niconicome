using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.External.Software.NiconicomeProcess
{
    public interface IProcessResult
    {
        /// <summary>
        /// ExitCode
        /// </summary>
        int ExitCode { get; }

        /// <summary>
        /// 標準出力
        /// </summary>
        string StandardOutput { get; }

        /// <summary>
        /// 例外
        /// </summary>
        string ErrorOutput { get; }
    }

    public record ProcessResult(int ExitCode, string StandardOutput, string ErrorOutput) : IProcessResult;
}
