using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V2.Fetch.Segment
{
    public interface ISegmentDLResultContainer
    {
        /// <summary>
        /// 結果をセット
        /// </summary>
        /// <param name="result"></param>
        /// <param name="index"></param>
        void SetResult(bool result, int index);

        /// <summary>
        /// 結果を確認
        /// </summary>
        bool IsAllSucceeded { get; }

        /// <summary>
        /// 失敗を確認
        /// </summary>
        bool IsFailedInAny { get; }

        /// <summary>
        /// セグメント数
        /// </summary>
        int Length { get; }

        /// <summary>
        /// 完了数
        /// </summary>
        int CompletedCount { get; }

    }

    public class SegmentDLResultContainer : ISegmentDLResultContainer
    {
        public SegmentDLResultContainer(int length)
        {
            this._result = Enumerable.Range(0, length).Select(_ => true).ToArray();
        }

        #region field

        private readonly object _lock = new();

        private readonly bool[] _result;

        #endregion

        #region Props

        public bool IsAllSucceeded => this._result.All(r => r);

        public bool IsFailedInAny => this._result.Any(r => !r);

        public int Length => this._result.Length;

        public int CompletedCount { get; private set; } = 0;

        #endregion

        #region Method

        public void SetResult(bool result, int index)
        {
            lock (this._lock)
            {
                this.CompletedCount++;
            }

            this._result[index] = result;
        }


        #endregion
    }
}
