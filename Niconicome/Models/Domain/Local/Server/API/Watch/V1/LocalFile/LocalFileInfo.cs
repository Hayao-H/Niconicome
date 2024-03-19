using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Local.Server.API.Watch.V1.LocalFile
{
    public interface ILocalFileInfo
    {
        Dictionary<int, IStreamInfo> Streams { get; }
    }

    public interface IStreamInfo
    {
        string VideoKey { get; }

        string AudioKey { get; }

        string VideoIV { get; }

        string AudioIV { get; }

        string VideoMapFileName { get; }

        string AudioMapFileName { get; }

        int VideoBandWidth { get; }

        IEnumerable<ISegmentInfo> VideoSegments { get; }

        IEnumerable<ISegmentInfo> AudioSegments { get; }
    }

    public interface ISegmentInfo
    {
        string FileName { get; }

        string Duration { get; }
    }

}
