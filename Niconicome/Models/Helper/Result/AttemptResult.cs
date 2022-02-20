using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Helper.Result
{
    public interface IAttemptResult
    {
        bool IsSucceeded { get; }
        string? Message { get; }
        Exception? Exception { get; }
        string ExceptionMessage { get; }
    }

    public class AttemptResult : IAttemptResult
    {
        public bool IsSucceeded { get; set; }

        public string? Message { get; set; }

        public Exception? Exception { get; set; }

        public string ExceptionMessage => this.Exception?.Message ?? "None";

        /// <summary>
        /// インスタンスを作成
        /// </summary>
        /// <returns></returns>
        public static IAttemptResult Fail(string? message = null, Exception? ex = null)
        {
            return new AttemptResult() { Message = message, Exception = ex };
        }

        /// <summary>
        /// インスタンスを作成
        /// </summary>
        /// <param name="data"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static IAttemptResult Succeeded(string? message = null)
        {
            return new AttemptResult() { IsSucceeded = true, Message = message };
        }
    }
}
