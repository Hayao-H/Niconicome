using System;
using System.Collections.Generic;
using System.Linq;
using Niconicome.Models.Domain.Niconico.Video.Infomations;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V2.HLS.Stream
{

    public interface IStreamInfo
    {
        /// <summary>
        /// URL
        /// </summary>
        IEnumerable<ISegmentURL> SegmentUrls { get; }

        /// <summary>
        /// 解像度
        /// </summary>
        IResolution VideoResolution { get; }

        /// <summary>
        /// 帯域
        /// </summary>
        long BandWidth { get; }

        /// <summary>
        /// IV
        /// </summary>
        string IV { get; }
    }

    /// <summary>
    /// 動画ストリーム情報
    /// </summary>
    public class StreamInfo : IStreamInfo
    {

        public StreamInfo(IEnumerable<ISegmentURL> segmentURLs, IResolution resolution, long bandWidth, string IV)
        {
            this.SegmentUrls = segmentURLs;
            this.VideoResolution = resolution;
            this.BandWidth = bandWidth;
            this.IV = IV;
        }

        public IEnumerable<ISegmentURL> SegmentUrls { get; init; }

        public IResolution VideoResolution { get; init; }

        public long BandWidth { get; init; }

        public string IV { get; init; }

    }
}
