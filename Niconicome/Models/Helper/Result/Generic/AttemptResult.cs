using System;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Helper.Result
{

    public interface IAttemptResult<T> : IAttemptResult
    {
        /// <summary>
        /// 情報
        /// </summary>
        T? Data { get; }
    }

    public class AttemptResult<T> : AttemptResult, IAttemptResult<T>
    {
        public T? Data { get; set; }

        /// <summary>
        /// インスタンスを作成
        /// </summary>
        /// <returns></returns>
        public new static IAttemptResult<T> Fail(string? message = null, Exception? ex = null, ErrorLevel errorLevel = ErrorLevel.Error)
        {
            return new AttemptResult<T>() { Message = message, Exception = ex, ErrorLevel = ErrorLevel.Error };
        }

        /// <summary>
        /// インスタンスを作成
        /// </summary>
        /// <param name="data"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static IAttemptResult<T> Succeeded(T? data, string? message = null)
        {
            return new AttemptResult<T>() { IsSucceeded = true, Message = message, Data = data };
        }
    }
}
