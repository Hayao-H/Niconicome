using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Download.Comment.V2.Fetch
{
    public interface ICommentFetchOption
    {
        bool DownloadOwner { get; }

        bool DownloadEasy { get; }

        bool DownloadLog { get; }

        long When { get; }
    }

    public record CommentFetchOption(bool DownloadOwner, bool DownloadEasy, bool DownloadLog, long When = 0) : ICommentFetchOption;
}
