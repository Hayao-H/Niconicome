using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoInfo = Niconicome.Models.Domain.Niconico.Video.Infomations;

namespace Niconicome.Models.Domain.Niconico.Download.Thumbnail
{
    public interface IThumbDownloadSettings
    {
        string NiconicoId { get; }
        string FolderName { get; }
        string FileNameFormat { get; }
        string Extension { get; }
        string Suffix { get; }
        bool IsOverwriteEnable { get; }
        bool IsReplaceStrictedEnable { get; }
        VideoInfo::ThumbSize ThumbSize { get; }
    }

    /// <summary>
    /// ダウンロード設定
    /// </summary>
    public class ThumbDownloadSettings : IThumbDownloadSettings
    {
        public string NiconicoId { get; set; } = string.Empty;

        public string FolderName { get; set; } = string.Empty;

        public string FileNameFormat { get; set; } = string.Empty;

        public string Extension { get; set; } = string.Empty;

        public string Suffix { get; set; } = string.Empty;

        public bool IsOverwriteEnable { get; set; }

        public bool IsReplaceStrictedEnable { get; set; }

        public VideoInfo::ThumbSize ThumbSize { get; set; }

    }
}
