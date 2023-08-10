using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Download.Comment.V2.Fetch.V3
{
    public interface ICommentFetchOption
    {
        bool DownloadOwner { get; }

        bool DownloadEasy { get; }

        bool DownloadLog { get; }

        long When { get; }

        string TargetFork { get; }

        string ThreadKey { get; }
    }

    public record CommentFetchOption(string ThreadKey, bool DownloadOwner, bool DownloadEasy, bool DownloadLog, long When = 0, string TargetFork = "") : ICommentFetchOption;
}
