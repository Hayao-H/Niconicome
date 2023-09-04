using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V2.HLS.Stream
{
    public interface ISegmentURL
    {
        string AbsoluteUrl { get; }
        string FileName { get; }
        int SequenceZero { get; }
    }

    public class SegmentURL : ISegmentURL
    {

        /// <summary>
        /// 絶対URL
        /// </summary>
        public string AbsoluteUrl { get; init; } = string.Empty;

        /// <summary>
        /// インデックス(zero index)
        /// </summary>
        public int SequenceZero { get; init; }

        /// <summary>
        /// ファイル名
        /// </summary>
        public string FileName { get; init; }



        public SegmentURL(string uri, string UrlBase, int SequenceZero)
        {
            this.AbsoluteUrl = string.Concat(UrlBase, uri);
            this.SequenceZero = SequenceZero;
            this.FileName = uri.Substring(0, uri.Contains('?') ? uri.IndexOf('?') : uri.Length);
        }
    }

}
