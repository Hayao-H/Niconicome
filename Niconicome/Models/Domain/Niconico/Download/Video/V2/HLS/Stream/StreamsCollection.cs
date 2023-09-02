using System.Collections.Generic;
using System;
using System.Linq;
using Niconicome.Extensions;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V2.HLS.Stream
{
    public interface IStreamsCollection
    {
        /// <summary>
        /// 最高画質のストリームを取得
        /// </summary>
        /// <returns></returns>
        IStreamInfo GetTheBestStream();

        /// <summary>
        /// 指定した画質のストリームを取得
        /// </summary>
        /// <param name="verticalResolution"></param>
        /// <returns></returns>
        IStreamInfo GetStream(uint verticalResolution);

    }


    public class StreamsCollection : IStreamsCollection
    {
        public StreamsCollection(IEnumerable<IStreamInfo> streams)
        {
            this._innerList = streams;
        }

        private readonly IEnumerable<IStreamInfo> _innerList;

        public IStreamInfo GetTheBestStream()
        {
            this.CheckStreamsListAndThrowError();
            var last = this._innerList.OrderBy(s => s.VideoResolution.Vertical).Last();
            var bests = this._innerList.Where(p => p.VideoResolution.Vertical == last.VideoResolution.Vertical).OrderByDescending(p => p.BandWidth);

            return bests.First();

        }

        public IStreamInfo GetStream(uint verticalResolution)
        {
            this.CheckStreamsListAndThrowError();
            var first = this._innerList.OrderByDescending(s => s.VideoResolution.Vertical).SkipWhile(s => s.VideoResolution.Vertical > verticalResolution).FirstOrDefault();

            if (first is null) return this.GetTheBestStream();

            var bests = this._innerList.Where(p => p.VideoResolution.Vertical == first.VideoResolution.Vertical).OrderByDescending(p => p.BandWidth);

            return bests.First();
        }

        private void CheckStreamsListAndThrowError()
        {
            if (this._innerList.Count() < 1) throw new InvalidOperationException();
        }
    }
}