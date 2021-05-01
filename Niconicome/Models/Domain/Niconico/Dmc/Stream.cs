using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Niconicome.Models.Domain.Niconico.Video.Infomations;

namespace Niconicome.Models.Domain.Niconico.Dmc
{

    public interface IStreamhandler
    {
        IStreamInfo GetStreamInfo(string playlist, string baseUrl);
    }
    /// <summary>
    /// 動画ストリームハンドラー
    /// </summary>
    public class StreamHandler : IStreamhandler
    {

        public StreamHandler(IM3U8Handler handler)
        {
            this.handler = handler;
        }

        private readonly IM3U8Handler handler;

        /// <summary>
        /// 動画ストリーム情報を文字列を指定して取得する
        /// </summary>
        /// <param name="playlist"></param>
        /// <returns></returns>
        public IStreamInfo GetStreamInfo(string playlist, string baseUrl)
        {
            this.handler.Parse(playlist);

            uint segmentDuration = this.handler.SegmentDuration;

            return this.Parse(segmentDuration, this.handler.PlayListNodes, baseUrl);

        }

        /// <summary>
        /// ノードリストを解析する
        /// </summary>
        /// <param name="segmentDuration"></param>
        /// <param name="playListNodes"></param>
        /// <returns></returns>
        private IStreamInfo Parse(uint segmentDuration, List<IPlaylistNode> playListNodes, string baseUrl)
        {
            var stream = new StreamInfo(segmentDuration);
            IResolution? resolution = default;
            long bandWidth = 0;
            int streamIndex = 0;
            foreach (int i in Enumerable.Range(0, playListNodes.Count))
            {
                IPlaylistNode node = playListNodes[i];
                switch (node.NodeType)
                {
                    case PlayListNodeType.Uri:
                        IStreamUrl sUrl = new StreamUrl(node.Content, baseUrl, streamIndex);
                        stream.StreamUrls.Add(sUrl);
                        streamIndex++;
                        break;
                    case PlayListNodeType.PlaylistUri:
                        if (resolution is null || bandWidth == 0) throw new Exception($"ストリームの解像度・帯域が不明です。({i + 1}行目)");
                        IPlaylistUrl pUrl = new PlaylistUrl(node.Content, baseUrl, resolution, bandWidth);
                        stream.PlaylistUrls.Add(pUrl);
                        resolution = default;
                        bandWidth = 0;
                        break;
                    case PlayListNodeType.StreamInfo:
                        resolution = node.PlaylistResolution;
                        bandWidth = node.PlaylistBandWidth;
                        break;
                }
            }

            return stream;
        }

    }
}