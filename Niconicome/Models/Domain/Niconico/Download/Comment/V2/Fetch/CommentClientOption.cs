using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Download.Comment.V2.Fetch
{
    public interface ICommentClientOption
    {
        bool IsOriginationSpecified { get; }

        DateTime Origination { get; }
    }

    public record CommentClientOption(bool IsOriginationSpecified, DateTime Origination) : ICommentClientOption;
}
