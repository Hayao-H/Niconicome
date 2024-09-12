using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Net.Json.API.Mylist;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V3.DMS
{
    public interface IStreamCollection
    {
        /// <summary>
        /// 指定したresolutionのストリームを取得する
        /// 存在しない場合、resolution以下の最大のストリームを取得する
        /// </summary>
        /// <param name="resolution"></param>
        /// <returns></returns>
        public IStreamInfo GetStream(int resolution);

        /// <summary>
        /// ストリームの数
        /// </summary>
        int Count { get; }
    }

    public class StreamCollection : IStreamCollection
    {
        public StreamCollection(IEnumerable<IStreamInfo> streams)
        {
            this.streams = streams;
        }

        private readonly IEnumerable<IStreamInfo> streams;

        public IStreamInfo GetStream(int resolution)
        {
            var stream = this.streams.Where(s => !s.IsLowest).Where(s => s.VerticalResolution == resolution).FirstOrDefault();
            if (stream is not null) return stream;
            return this.streams.OrderByDescending(s => s.VerticalResolution).First(s => s.VerticalResolution <= resolution);
        }

        public int Count => this.streams.Count();
    }
}
