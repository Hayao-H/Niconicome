using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Dmc;
using Niconicome.Models.Domain.Niconico.Video.Infomations;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V2.HLS.M3U8
{
    public interface IPlaylistNode
    {
        /// <summary>
        /// ノードの種類
        /// </summary>
        PlayListNodeType NodeType { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        string Content { get; }

        /// <summary>
        /// 解像度
        /// </summary>
        /// <returns></returns>
        IResolution AsResolution();

        /// <summary>
        /// BandWidth
        /// </summary>
        /// <returns></returns>
        long AsBandWidth();
    }

    public class PlaylistNode : IPlaylistNode
    {
        public PlaylistNode(string line)
        {
            this.NodeType = this.GetType(line);
            this.Content = this.GetContent(line);
        }

        #region Props

        public PlayListNodeType NodeType { get; set; }

        public string Content { get; private set; }

        #endregion

        #region Method

        public IResolution AsResolution()
        {
            var resolution = Resolution.Default;

            IEnumerable<KeyValuePair<string, string>> infoLists = this.GetInfoList();

            foreach (var sInfo in infoLists)
            {
                if (sInfo.Key == "RESOLUTION")
                {
                    resolution = new Resolution(sInfo.Value);
                }
            }

            return resolution;
        }

        public long AsBandWidth()
        {
            long bandWidth = 0;

            IEnumerable<KeyValuePair<string, string>> infoLists = this.GetInfoList();

            foreach (var sInfo in infoLists)
            {
                if (sInfo.Key == "BANDWIDTH")
                {
                    bandWidth = long.Parse(sInfo.Value);
                }
            }

            return bandWidth;
        }


        #endregion

        #region private

        /// <summary>
        /// ノードタイプを取得する
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private PlayListNodeType GetType(string line)
        {
            if (line.StartsWith("#"))
            {
                if (line.StartsWith("#EXTM3U"))
                {
                    return PlayListNodeType.Flag;
                }
                else if (line.StartsWith("#EXT-X-VERSION"))
                {
                    return PlayListNodeType.Version;
                }
                else if (line.StartsWith("#EXT-X-TARGETDURATION"))
                {
                    return PlayListNodeType.OverallDuration;
                }
                else if (line.StartsWith("#EXT-X-MEDIA-SEQUENCE"))
                {
                    return PlayListNodeType.Sequence;
                }
                else if (line.StartsWith("#EXT-X-PLAYLIST-TYPE"))
                {
                    return PlayListNodeType.Type;
                }
                else if (line.StartsWith("#EXTINF"))
                {
                    return PlayListNodeType.TsFileDuration;
                }
                else if (line.StartsWith("#EXT-X-STREAM-INF"))
                {
                    return PlayListNodeType.StreamInfo;
                }
                else if (line.StartsWith("#EXT-X-ENDLIST"))
                {
                    return PlayListNodeType.EndOfList;
                } else if (line.StartsWith("EXT-X-KEY"))
                {
                    return PlayListNodeType.Key;
                }
                else
                {
                    return PlayListNodeType.Unknown;
                }
            }
            else
            {
                string? extension;
                if (line.Contains('?'))
                {
                    line = line.Substring(0, line.IndexOf('?'));
                }
                try
                {
                    extension = Path.GetExtension(line);
                }
                catch
                {
                    return PlayListNodeType.Unknown;
                }

                if (extension.Contains(".ts"))
                {
                    return PlayListNodeType.Uri;
                }
                else
                {
                    return PlayListNodeType.PlaylistUri;
                }

            }
        }


        /// <summary>
        /// 内容を取得する
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private string GetContent(string line)
        {
            static string removeSymbol(string source) => Regex.Replace(source, @"[\r\n\t\s]", "");

            if (line.StartsWith("#"))
            {

                int index = line.IndexOf(":");
                if (index != -1)
                {
                    return removeSymbol(line[(line.IndexOf(":") + 1)..]);
                }
                else
                {
                    return removeSymbol(line[1..]);
                }
            }
            else
            {
                return removeSymbol(line);
            }
        }

        /// <summary>
        /// Stream情報を取得
        /// </summary>
        /// <returns></returns>
        private IEnumerable<KeyValuePair<string, string>> GetInfoList()
        {
            string[] infoes = this.Content.Split(',');
            var infoLists = new List<KeyValuePair<string, string>>();

            if (this.NodeType != PlayListNodeType.StreamInfo)
            {
                return infoLists;
            }

            foreach (var sInfo in infoes)
            {
                string[] splitedInfo = sInfo.Split('=');
                infoLists.Add(new KeyValuePair<string, string>(splitedInfo[0], splitedInfo.Length > 1 ? splitedInfo[1] : string.Empty));
            }

            return infoLists;
        }

        #endregion

    }
}
