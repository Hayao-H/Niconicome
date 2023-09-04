using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using AngleSharp.Dom;
using Niconicome.Models.Domain.Niconico.Download.Video.V2.HLS.M3U8;
using Niconicome.Models.Domain.Niconico.Video.Infomations;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V2.HLS.Stream
{

    public interface IStreamhandler
    {
        IStreamInfo GetStreamInfo(string playlist, string baseUrl, IResolution resolution, long bandWidth);

        IEnumerable<IPlaylistInfo> GetPlaylistInfos(string playlist, string baseUrl);
    }

    public class StreamHandler : IStreamhandler
    {
        public StreamHandler(IM3U8Handler handler)
        {
            this._handler = handler;
        }

        #region field

        private readonly IM3U8Handler _handler;

        #endregion

        #region Method

        public IStreamInfo GetStreamInfo(string playlist, string baseUrl, IResolution resolution, long bandWidth)
        {
            IEnumerable<IPlaylistNode> nodes = this._handler.Parse(playlist);
            var urls = new List<ISegmentURL>();
            var index = 0;
            var iv = "";
            var key = "";

            foreach (var node in nodes)
            {
                if (node.NodeType == PlayListNodeType.Uri)
                {
                    ISegmentURL sUrl = new SegmentURL(node.Content, baseUrl, index);
                    urls.Add(sUrl);
                    index++;
                }
                else if (node.NodeType == PlayListNodeType.Key)
                {
                    var info = new Dictionary<string, string>();

                    foreach (var content in node.Content.Split(","))
                    {
                        if (!content.Contains("=")) continue;
                        int equalIndex = content.IndexOf("=");
                        info.Add(content[0..equalIndex], content[(equalIndex + 1)..]);
                    }

                    if (info.TryGetValue("IV", out string? x))
                    {
                        iv = x;
                    }

                    if (info.TryGetValue("URI", out string? y))
                    {
                        key = y[1..^1];
                    }
                }
            }

            return new StreamInfo(urls, resolution, bandWidth, iv, key);

        }

        public IEnumerable<IPlaylistInfo> GetPlaylistInfos(string playlist, string baseUrl)
        {
            IEnumerable<IPlaylistNode> nodes = this._handler.Parse(playlist);
            var playlists = new List<IPlaylistInfo>();

            IResolution resolution = Resolution.Default;
            long bandWidth = 0;

            foreach (var node in nodes)
            {
                if (node.NodeType == PlayListNodeType.StreamInfo)
                {
                    resolution = node.AsResolution();
                    bandWidth = node.AsBandWidth();
                }
                else if (node.NodeType == PlayListNodeType.PlaylistUri)
                {
                    playlists.Add(new PlaylistInfo(baseUrl + node.Content, resolution, bandWidth));
                }
            }

            return playlists;
        }

        #endregion

    }
}