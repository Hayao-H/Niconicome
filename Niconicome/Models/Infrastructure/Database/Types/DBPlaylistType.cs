using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Infrastructure.Database.Types
{
    public enum DBPlaylistType
    {
        Local,
        Mylist,
        Series,
        WatchLater,
        UserVideos,
        Channel,
        Root,
        Temporary,
        DownloadSucceededHistory,
        DownloadFailedHistory,
        PlaybackHistory,
    }
}
