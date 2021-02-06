using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using Niconicome.Models.Domain.Niconico.Video.Infomations;
using System.Diagnostics;

namespace Niconicome.Models.Domain.Niconico.Dmc
{

    public interface IM3U8Handler
    {
        string IndexFileName { get; }
        uint SegmentDuration { get; }
        List<IPlaylistNode> PlayListNodes { get; }
        void Parse(List<string> playlist);
        void Parse(string playlist);
    }

    public interface IPlaylistNode
    {
        PlayListNodeType NodeType { get; set; }
        string Content { get; }
        IResolution PlaylistResolution { get; set; }
    }

    public interface IStreamUrl
    {
        string Uri { get; }
        string AbsoluteUrl { get; }
        string FileName { get; }
        int SequenceZero { get; }
        int Sequence { get; }
    }

    public interface IPlaylistUrl
    {
        string Uri { get; }
        string AbsoluteUri { get; }

        IResolution Resolution { get; }
    }

    /// <summary>
    /// プレイリストハンドラー
    /// </summary>
    public class M3U8Handler : IM3U8Handler
    {

        /// <summary>
        /// インデックスファイル名
        /// </summary>
        public string IndexFileName { get; private set; } = string.Empty;

        /// <summary>
        /// セグメントファイル秒数
        /// </summary>
        private uint segmentDuration;

        /// <summary>
        /// セグメントファイル秒数(プロパティ)
        /// </summary>
        public uint SegmentDuration
        {
            get
            {
                if (this.segmentDuration == 0)
                {
                    this.segmentDuration = this.GetSegmentDuration();
                }
                return this.segmentDuration;
            }
        }

        /// <summary>
        /// ノードリスト
        /// </summary>
        public List<IPlaylistNode> PlayListNodes { get; private set; } = new List<IPlaylistNode>();

        /// <summary>
        /// 文字列のリストを解析(実装)
        /// </summary>
        /// <param name="playlist"></param>
        public void Parse(List<string> playlist)
        {
            bool isPrevStreamInfo = false;
            bool isPrevTsFileDuration = false;

            this.PlayListNodes.Clear();

            foreach (var i in Enumerable.Range(0, playlist.Count))
            {
                var line = playlist[i];

                if (line == string.Empty) continue;

                var node = new PlayListNode(line);

                if (isPrevStreamInfo)
                {
                    if (node.NodeType != PlayListNodeType.PlaylistUri)
                    {
                        throw new InvalidDataException($"プレイリストファイルのURIが指定されていません。(行:{i + 1})");
                    }

                    node.PlaylistResolution = this.PlayListNodes[i - 1].PlaylistResolution;
                    isPrevStreamInfo = false;

                }
                else if (isPrevTsFileDuration)
                {
                    if (node.NodeType != PlayListNodeType.Uri) throw new InvalidDataException($"セグメントファイルのURIが指定されていません。(行:{i + 1})");
                    isPrevTsFileDuration = false;
                }

                switch (node.NodeType)
                {
                    case PlayListNodeType.StreamInfo:
                        node.PlaylistResolution = this.GetResolution(node.Content);
                        isPrevStreamInfo = true;
                        break;
                }

                this.PlayListNodes.Add(node);
            }
        }

        /// <summary>
        /// 文字列を解析
        /// </summary>
        /// <param name="playlist"></param>
        public void Parse(string playlist)
        {
            var playlistAsList = new List<string>();
            playlistAsList.AddRange(playlist.Split('\n'));
            playlistAsList = playlistAsList.Where(p => !Regex.IsMatch(p, @"^[\r\n]?$")).ToList();
            this.Parse(playlistAsList);
        }

        /// <summary>
        /// セグメントファイル秒数を取得
        /// </summary>
        /// <returns></returns>
        private uint GetSegmentDuration()
        {
            foreach (var node in this.PlayListNodes)
            {
                if (node.NodeType == PlayListNodeType.OverallDuration)
                {
                    return uint.Parse(node.Content);
                }
            }

            return 0;
        }

        /// <summary>
        /// 解像度を解析
        /// </summary>
        /// <param name="origin"></param>
        private IResolution GetResolution(string origin)
        {
            string[] infoes = origin.Split(',');
            var infoLists = new List<KeyValuePair<string, string>>();

            foreach (var sInfo in infoes)
            {
                string[] splitedInfo = sInfo.Split('=');
                infoLists.Add(new KeyValuePair<string, string>(splitedInfo[0], splitedInfo.Length > 1 ? splitedInfo[1] : string.Empty));
            }

            foreach (var sInfo in infoLists)
            {
                if (sInfo.Key == "RESOLUTION")
                {
                    return new Resolution(sInfo.Value);
                }
            }

            return Resolution.Default;

        }

    }

    /// <summary>
    /// プレイリストノード
    /// </summary>
    public class PlayListNode : IPlaylistNode
    {
        public PlayListNodeType NodeType { get; set; }

        public string Content { get; private set; }


        /// <summary>
        /// 解像度(プロパティ)
        /// </summary>
        public IResolution PlaylistResolution { get; set; } = Resolution.Default;

        public PlayListNode(string line)
        {
            this.NodeType = this.GetType(line);
            this.Content = this.GetContent(line);
        }

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
                catch (Exception e)
                {
                    throw new InvalidDataException($"セグメントファイル、またはプレイリストファイルのURIが不正です。(指定されたURI:{line} 詳細:{e.Message})");
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
    }

    /// <summary>
    /// セグメントファイルURL
    /// </summary>
    public class StreamUrl : IStreamUrl
    {
        /// <summary>
        /// Uri
        /// </summary>
        public string Uri { get; init; } = string.Empty;

        /// <summary>
        /// 絶対URL
        /// </summary>
        public string AbsoluteUrl { get; init; } = string.Empty;

        /// <summary>
        /// インデックス(zero index)
        /// </summary>
        public int SequenceZero { get; init; }

        /// <summary>
        /// インデックス
        /// </summary>
        public int Sequence { get; init; }

        /// <summary>
        /// ファイル名
        /// </summary>
        public string FileName { get; init; }



        public StreamUrl(string uri, string UrlBase, int SequenceZero)
        {
            this.Uri = uri;
            this.AbsoluteUrl = string.Concat(UrlBase, uri);
            this.SequenceZero = SequenceZero;
            this.Sequence = SequenceZero + 1;
            this.FileName = uri.Substring(0, uri.Contains('?') ? uri.IndexOf('?'):uri.Length);
        }
    }

    /// <summary>
    /// プレイリストURL
    /// </summary>
    public class PlaylistUrl : IPlaylistUrl
    {
        /// <summary>
        /// 相対URL
        /// </summary>
        public string Uri { get; init; }

        /// <summary>
        /// 絶対URL
        /// </summary>
        public string AbsoluteUri { get; init; }

        /// <summary>
        /// 解像度
        /// </summary>
        public IResolution Resolution { get; init; }

        public PlaylistUrl(string uri, string baseUrl, IResolution resolution)
        {
            this.Uri = uri;
            this.AbsoluteUri = string.Concat(baseUrl, uri);
            this.Resolution = resolution;
        }

    }

    /// <summary>
    /// ノードのタイプ
    /// </summary>
    public enum PlayListNodeType
    {
        Flag,
        Version,
        OverallDuration,
        TsFileDuration,
        Sequence,
        Type,
        StreamInfo,
        PlaylistUri,
        Uri,
        EndOfList,
        Unknown
    }

}
