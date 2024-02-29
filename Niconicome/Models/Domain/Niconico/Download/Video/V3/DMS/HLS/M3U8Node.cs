using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V3.DMS.HLS
{
    public interface IM3U8Node
    {
        /// <summary>
        /// ノードの種類
        /// </summary>
        M3U8NodeType Type { get; }

        /// <summary>
        /// 値
        /// </summary>
        string Value { get; }

        /// <summary>
        /// URL
        /// </summary>
        string URL { get; }
    }

    //IM3U8Nodeを実装する
    public class M3U8Node : IM3U8Node
    {
        public M3U8Node(M3U8NodeType type, string value, string url)
        {
            this.Type = type;
            this.Value = value;
            this.URL = url;
        }

        public M3U8NodeType Type { get; set; }

        public string Value { get; set; } = string.Empty;

        public string URL { get; set; } = string.Empty;
    }

    public enum M3U8NodeType
    {
        StreamInfo,
        Audio,
        Key,
        Segment,
    }
}
