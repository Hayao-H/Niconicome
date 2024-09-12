using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Server.API.Watch.V1.LocalFile;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V3.Local.StreamJson
{
    public class StreamType
    {
        public List<Stream> Streams { get; set; } = new();
    }

    public class Stream
    {
        public int Resolution { get; set; }

        public string VideoKey { get; set; } = string.Empty;

        public string AudioKey { get; set; } = string.Empty;

        public string VideoIV { get; set; } = string.Empty;

        public string AudioIV { get; set; } = string.Empty;

        public string VideoMapFileName { get; set; } = string.Empty;

        public string AudioMapFileName { get; set; } = string.Empty;

        public int VideoBandWidth { get; set; }

        public List<Segment> VideoSegments { get; set; } = new List<Segment>();

        public List<Segment> AudioSegments { get; set; } = new List<Segment>();

    }

    public class Segment : ISegmentInfo
    {
        public string FileName { get; set; } = string.Empty;

        public string Duration { get; set; } = string.Empty;
    }
}
