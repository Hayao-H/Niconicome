using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.StringHandler;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V2.Fetch.Segment.StringContent
{
    public enum SegmentDownloaderSC
    {
        [StringEnum("完了: {0}/{1} {2}px")]
        CompletedMessage,
    }
}
