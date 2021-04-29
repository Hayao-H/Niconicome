using System.Collections.Generic;
using System;
using System.Linq;

namespace Niconicome.Models.Domain.Niconico.Dmc
{
    public interface IStreamsCollection
    {
        IEnumerable<IStreamInfo> GetAllStreams();
        IStreamInfo GetTheBestStream();
        IStreamInfo GetTheWorstStream();
        IStreamInfo GetStream(uint verticalResolution);
        void Add(IStreamInfo stream);
        void AddRange(IEnumerable<IStreamInfo> streams);
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