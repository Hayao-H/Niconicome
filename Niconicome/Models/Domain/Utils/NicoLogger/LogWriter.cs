using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Utils.NicoLogger
{
    public interface ILogWriter
    {
        /// <summary>
        /// ログファイルに書き込む
        /// </summary>
        /// <param name="message"></param>
        void Write(string message);

        /// <summary>
        /// ログファイルのパス
        /// </summary>
        string LogFilePath { get; }
    }
}
