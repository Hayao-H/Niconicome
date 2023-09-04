using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Dmc;
using Niconicome.Models.Domain.Niconico.Video.Infomations;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V2.HLS.M3U8
{
    public interface IM3U8Handler
    {
        /// <summary>
        /// プレイリストを解析
        /// </summary>
        /// <param name="playlist"></param>
        /// <returns></returns>
        IEnumerable<IPlaylistNode> Parse(string playlist);
    }


    /// <summary>
    /// プレイリストハンドラー
    /// </summary>
    public class M3U8Handler : IM3U8Handler
    {
        /// <summary>
        /// 文字列を解析
        /// </summary>
        /// <param name="playlist"></param>
        public IEnumerable<IPlaylistNode> Parse(string playlist)
        {
            var playlistAsList = new List<string>();
            var nodes = new List<PlaylistNode>();

            playlistAsList.AddRange(playlist.Split('\n').Where(p => !Regex.IsMatch(p, @"^[\r\n]?$")));

            foreach (var line in playlistAsList)
            {

                if (line == string.Empty) continue;

                var node = new PlaylistNode(line);

                nodes.Add(node);
            }

            return nodes;
        }


        /// <summary>
        /// 解像度を解析
        /// </summary>
        /// <param name="origin"></param>
        private (IResolution, long) GetResolution(string origin)
        {
            string[] infoes = origin.Split(',');
            var infoLists = new List<KeyValuePair<string, string>>();

            foreach (var sInfo in infoes)
            {
                string[] splitedInfo = sInfo.Split('=');
                infoLists.Add(new KeyValuePair<string, string>(splitedInfo[0], splitedInfo.Length > 1 ? splitedInfo[1] : string.Empty));
            }

            var resolution = Resolution.Default;
            long bandWidth = 0;

            foreach (var sInfo in infoLists)
            {
                if (sInfo.Key == "RESOLUTION")
                {
                    resolution = new Resolution(sInfo.Value);
                }
                else if (sInfo.Key == "BANDWIDTH")
                {
                    bandWidth = long.Parse(sInfo.Value);
                }
            }

            return (resolution, bandWidth);

        }

    }

}
