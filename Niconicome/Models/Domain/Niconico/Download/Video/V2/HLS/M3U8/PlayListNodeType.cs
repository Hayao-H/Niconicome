using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using Niconicome.Models.Domain.Niconico.Video.Infomations;
using System.Diagnostics;
using System.Security.Permissions;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V2.HLS.M3U8
{
    public enum PlayListNodeType
    {
        Flag,
        Version,
        OverallDuration,
        TsFileDuration,
        Sequence,
        Type,
        StreamInfo,
        PlaylistUri,
        Uri,
        EndOfList,
        Unknown
    }

}
