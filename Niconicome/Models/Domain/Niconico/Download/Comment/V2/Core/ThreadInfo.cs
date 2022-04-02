using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Download.Comment.V2.Core
{
    public interface IThreadInfo
    {
        long ResultCode { get; }

        string Thread { get; }

        string? Ticket { get; }

        long ServerTime { get; }

        long LastRes { get; }

        long Revision { get; }
    }

    public class ThreadInfo : IThreadInfo
    {
        public long ResultCode { get; init; }

        public string Thread { get; init; } = string.Empty;

        public string? Ticket { get; init; }

        public long ServerTime { get; init; }

        public long LastRes { get; init; }

        public long Revision { get; init; }
    }
}
