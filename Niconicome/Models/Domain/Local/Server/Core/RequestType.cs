using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.Server.Core
{
    public enum RequestType
    {
        None,
        Video,
        M3U8,
        TS,
        UserChrome,
        WatchAPI,
        CommentAPI,
        RegacyHLSAPI,
    }
}
