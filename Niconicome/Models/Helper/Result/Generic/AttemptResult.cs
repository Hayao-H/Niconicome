using Result = Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Helper.Result.Generic
{

    public interface IAttemptResult<T> : Result::IAttemptResult
    {
        T? Data { get; }
    }

    public class AttemptResult<T> : Result::AttemptResult, IAttemptResult<T>
    {
        public T? Data { get; set; }
    }
}
