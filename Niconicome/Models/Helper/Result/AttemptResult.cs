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
    }

    public class AttemptResult : IAttemptResult
    {
        public bool IsSucceeded { get; set; }

        public string? Message { get; set; }

        public Exception? Exception { get; set; }
    }
}
