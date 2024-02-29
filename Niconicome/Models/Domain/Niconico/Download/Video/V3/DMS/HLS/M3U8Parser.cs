using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V3.DMS.HLS
{
    public interface IM3U8Parser
    {
        /// <summary>
        /// プレイリストを解析する
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        IEnumerable<IM3U8Node> Parse(string text);
    }

    public class M3U8Parser : IM3U8Parser
    {
        public IEnumerable<IM3U8Node> Parse(string text)
        {
            var nodes = new List<IM3U8Node>();
            var lines = text.Split(Environment.NewLine);
            if (lines.Length == 1)
            {
                lines = text.Split("\n");
            }

            var index = 0;

            while (index < lines.Length)
            {
                var line = lines[index];
                if (line.StartsWith("#EXT-X-STREAM-INF:"))
                {
                    var streamInfo = new M3U8Node(M3U8NodeType.StreamInfo, line[(line.IndexOf(":") + 1)..], lines[index + 1]);
                    nodes.Add(streamInfo);
                    index += 2;
                }
                else if (line.StartsWith("#EXT-X-KEY:"))
                {
                    var streamInfo = new M3U8Node(M3U8NodeType.Key, line[(line.IndexOf(":") + 1)..], string.Empty);
                    nodes.Add(streamInfo);
                    index += 1;
                }
                else if (line.StartsWith("#EXT-X-MEDIA:"))
                {
                    var streamInfo = new M3U8Node(M3U8NodeType.Audio, line[(line.IndexOf(":") + 1)..], string.Empty);
                    nodes.Add(streamInfo);
                    index += 1;
                }
                else if (line.StartsWith("#EXTINF:"))
                {

                    var streamInfo = new M3U8Node(M3U8NodeType.Segment, line[(line.IndexOf(":") + 1)..], lines[index + 1]);
                    nodes.Add(streamInfo);
                    index += 2;
                }
                else
                {
                    index += 1;
                }
            }

            return nodes;
        }
    }
}
