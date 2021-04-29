using System;
using System.Collections.Generic;
using System.Linq;
using Niconicome.Models.Domain.Niconico.Video.Infomations;

namespace Niconicome.Models.Domain.Niconico.Dmc
{

    public interface IStreamInfo
    {
        uint SegmentDuration { get; }
        List<IStreamUrl> StreamUrls { get; }
        List<IPlaylistUrl> PlaylistUrls { get; }
        IResolution? Resolution { get; set; }
        long BandWidth { get; set; }
        bool IsMasterPlaylist { get; }
        IPlaylistUrl GetTheBestStream();
        IPlaylistUrl GetTheWorstStream();
        IPlaylistUrl GetStream(uint verticalResolution);
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
        /// BANDWIDTH
        /// </summary>
        public long BandWidth { get; set; }

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
}
