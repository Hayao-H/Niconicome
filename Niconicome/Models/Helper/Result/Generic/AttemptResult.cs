using System;
using Result = Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Helper.Result
{

    public interface IAttemptResult<T> : Result::IAttemptResult
    {
        /// <summary>
        /// 情報
        /// </summary>
        T? Data { get; }
    }

    public class AttemptResult<T> : Result::AttemptResult, IAttemptResult<T>
    {
        public T? Data { get; set; }

        /// <summary>
        /// インスタンスを作成
        /// </summary>
        /// <returns></returns>
        public static IAttemptResult<T> Fail(string? message = null, Exception? ex = null)
        {
            return new AttemptResult<T>() { Message = message, Exception = ex};
        }

        /// <summary>
        /// インスタンスを作成
        /// </summary>
        /// <returns></returns>
        public static IAttemptResult<T> Succeeded()
        {
            return new AttemptResult<T>() { IsSucceeded = true };
        }
    }
}
