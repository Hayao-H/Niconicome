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

    public interface IStreamInfo
    {
        uint SegmentDuration { get; }
        List<IStreamUrl> StreamUrls { get; }
        List<IPlaylistUrl> PlaylistUrls { get; }
        IResolution? Resolution { get; set; }
        bool IsMasterPlaylist { get; }
        IPlaylistUrl GetTheBestStream();
        IPlaylistUrl GetTheWorstStream();
        IPlaylistUrl GetStream(uint verticalResolution);
    }

    public interface IStreamsCollection
    {
        IEnumerable<IStreamInfo> GetAllStreams();
        IStreamInfo GetTheBestStream();
        IStreamInfo GetTheWorstStream();
        IStreamInfo GetStream(uint verticalResolution);
        void Add(IStreamInfo stream);
        void AddRange(IEnumerable<IStreamInfo> streams);
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
                        if (resolution == null) throw new Exception($"ストリームの解像度が不明です。({i + 1}行目)");
                        IPlaylistUrl pUrl = new PlaylistUrl(node.Content, baseUrl, resolution);
                        stream.PlaylistUrls.Add(pUrl);
                        break;
                    case PlayListNodeType.StreamInfo:
                        resolution = node.PlaylistResolution;
                        break;
                }
            }

            return stream;
        }

    }

    /// <summary>
    /// 動画ストリーム情報
    /// </summary>
    public class StreamInfo : IStreamInfo
    {

        public StreamInfo(uint segmentDuraion)
        {
            this.SegmentDuration = segmentDuraion;
        }

        public StreamInfo() : this(0) { }

        /// <summary>
        /// セグメントファイルの秒数
        /// </summary>
        public uint SegmentDuration { get; private set; }

        /// <summary>
        /// セグメントファイルのURL
        /// </summary>
        public List<IStreamUrl> StreamUrls { get; private set; } = new List<IStreamUrl>();

        /// <summary>
        /// プレイリストのURL
        /// </summary>
        public List<IPlaylistUrl> PlaylistUrls { get; private set; } = new List<IPlaylistUrl>();

        /// <summary>
        /// 解像度
        /// </summary>
        public IResolution? Resolution { get; set; }

        /// <summary>
        /// マスター・プレイリストフラグ
        /// </summary>
        public bool IsMasterPlaylist
        {
            get
            {
                return this.PlaylistUrls.Count > 0;
            }
        }

        /// <summary>
        /// 最高解像度のストリームを返す
        /// </summary>
        /// <returns></returns>
        public IPlaylistUrl GetTheBestStream()
        {
            this.CheckIfMasterAndThrowException();
            return this.PlaylistUrls.OrderByDescending(s => s.Resolution.Vertical).First();
        }

        /// <summary>
        /// 最低解像度のストリームを返す
        /// </summary>
        /// <returns></returns>
        public IPlaylistUrl GetTheWorstStream()
        {
            this.CheckIfMasterAndThrowException();
            return this.PlaylistUrls.OrderByDescending(s => s.Resolution.Vertical).Last();
        }

        /// <summary>
        /// 指定した解像度以下の最高解像度のストリームを返す
        /// </summary>
        /// <param name="verticalResolution"></param>
        /// <returns></returns>
        public IPlaylistUrl GetStream(uint verticalResolution)
        {
            this.CheckIfMasterAndThrowException();
            return this.PlaylistUrls.OrderByDescending(p => p.Resolution.Vertical).SkipWhile(p => p.Resolution.Vertical > verticalResolution).First() ?? this.GetTheWorstStream();
        }

        /// <summary>
        /// マスタープレイリストで無かった場合に例外を投げる
        /// </summary>
        private void CheckIfMasterAndThrowException()
        {
            if (!this.IsMasterPlaylist) throw new InvalidOperationException("マスタープレイリストでは無いため、子プレイリストを取得することは出来ません。");
        }

    }

    public class StreamsCollection : IStreamsCollection
    {
        /// <summary>
        /// 内部でデータを保持する
        /// </summary>
        private readonly List<IStreamInfo> innerList = new();

        /// <summary>
        /// ストリームを追加する
        /// </summary>
        /// <param name="stream"></param>
        public void Add(IStreamInfo stream)
        {
            if (stream.Resolution is null) throw new InvalidOperationException("IStreamsCollectionはResolutionがnullであるIStreamInfoを保持できません。");
            this.innerList.Add(stream);
        }

        /// <summary>
        /// 複数のストリームを追加する
        /// </summary>
        /// <param name="streams"></param>
        public void AddRange(IEnumerable<IStreamInfo> streams)
        {
            foreach (var stream in streams)
            {
                this.Add(stream);
            }
        }

        /// <summary>
        /// 全てのストリームを取得する
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IStreamInfo> GetAllStreams()
        {
            return this.innerList;
        }

        /// <summary>
        /// 最高解像度のストリームを取得する
        /// </summary>
        /// <returns></returns>
        public IStreamInfo GetTheBestStream()
        {
            this.CheckStreamsListAndThrowError();
            return this.innerList.OrderBy(s => s.Resolution!.Vertical).Last();
        }

        /// <summary>
        /// 最低解像度のストリームを取得する
        /// </summary>
        /// <returns></returns>
        public IStreamInfo GetTheWorstStream()
        {
            this.CheckStreamsListAndThrowError();
            return this.innerList.OrderBy(s => s.Resolution!.Vertical).First();
        }

        /// <summary>
        /// 指定した解像度以下で最高解像度のストリームを取得する
        /// </summary>
        /// <param name="verticalResolution"></param>
        /// <returns></returns>
        public IStreamInfo GetStream(uint verticalResolution)
        {
            this.CheckStreamsListAndThrowError();
            return this.innerList.OrderByDescending(s => s.Resolution!.Vertical).SkipWhile(s => s.Resolution!.Vertical > verticalResolution).FirstOrDefault() ?? this.GetTheWorstStream();
        }

        private void CheckStreamsListAndThrowError(int least = 1)
        {
            if (this.innerList.Count < least) throw new InvalidOperationException($"登録されているストリーム数が{least}個未満のため、取得できません。");
        }
    }
}
