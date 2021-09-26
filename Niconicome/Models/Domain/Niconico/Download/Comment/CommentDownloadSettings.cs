using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Download.Comment
{
    public interface ICommentDownloadSettings
    {
        string NiconicoId { get; }
        string FolderName { get; }
        string FileNameFormat { get; }
        string OwnerSuffix { get; }
        bool IsOverwriteEnable { get; }
        bool IsAutoDisposingEnable { get; }
        bool IsDownloadingLogEnable { get; }
        bool IsDownloadingOwnerCommentEnable { get; }
        bool IsDownloadingEasyCommentEnable { get; }
        bool IsReplaceStrictedEnable { get; }
        bool IsUnsafeHandleEnable { get; }

        bool IsExperimentalSafetySystemEnable { get; }
        int CommentOffset { get; }
        int MaxcommentsCount { get; }
    }

    public class CommentDownloadSettings : ICommentDownloadSettings
    {
        public string NiconicoId { get; set; } = string.Empty;

        public string FolderName { get; set; } = string.Empty;

        public string FileNameFormat { get; set; } = string.Empty;

        public string OwnerSuffix { get; set; } = string.Empty;

        public bool IsOverwriteEnable { get; set; }

        public bool IsAutoDisposingEnable { get; set; }

        public bool IsDownloadingLogEnable { get; set; }

        public bool IsDownloadingOwnerCommentEnable { get; set; }

        public bool IsDownloadingEasyCommentEnable { get; set; }

        public bool IsReplaceStrictedEnable { get; set; }

        public bool IsUnsafeHandleEnable { get; set; }

        public bool IsExperimentalSafetySystemEnable { get; set; }

        public int CommentOffset { get; set; }

        public int MaxcommentsCount { get; set; }


    }

}
