using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Playlist;

namespace Niconicome.Models.Domain.Niconico.Remote.V2.Channel
{
    public record ChannelPageInfo(IReadOnlyList<VideoInfo> Videos, string ChannnelName, bool HasNext, string NextPageQuery);
}
