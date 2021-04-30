using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Helper.Result.Generic
{
    public interface IAPIAttemptResult<T> : IAttemptResult<T>
    {
        string Version { get; }
    }

    class APIAttemptResult<T> : AttemptResult<T>, IAPIAttemptResult<T>
    {
        public string Version { get; set; } = string.Empty;
    }
}
