using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Utils;

namespace Niconicome.Models.Domain.Niconico.Download.Video
{

    public interface IParallelDownloadTask : IParallelTask<IParallelDownloadTask>
    {
        /// <summary>
        /// コンテクスト
        /// </summary>
        IDownloadContext Context { get; }

        /// <summary>
        /// ファイル名
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// ストリームのURL
        /// </summary>
        string Url { get; }

        /// <summary>
        /// Index
        /// </summary>
        int SequenceZero { get; }

        /// <summary>
        /// 結果
        /// </summary>
        IAttemptResult? Result { get; }

        /// <summary>
        /// 結果をセットする
        /// </summary>
        /// <param name="result"></param>
        void SetResult(IAttemptResult result);
    }

    /// <summary>
    /// 並列DLタスク
    /// </summary>
    public class ParallelDownloadTask : IParallelDownloadTask
    {
        public ParallelDownloadTask(Func<IParallelDownloadTask, object, Task> taskFunc, IDownloadContext context, string url, int sequenceZero, string filename)
        {
            this.TaskFunction = taskFunc;
            this.OnWait += (_) => { };
            this.Context = context;
            this.Url = url;
            this.SequenceZero = sequenceZero;
            this.FileName = filename;
        }

        #region Props

        public IDownloadContext Context { get; init; }

        public string Url { get; init; }

        public string FileName { get; init; }

        public int SequenceZero { get; init; }

        public int Index { get; set; }

        public IAttemptResult? Result { get; private set; }

        #endregion

        #region IParallelTask

        public Func<IParallelDownloadTask, object, Task> TaskFunction { get; init; }

        public Action<int> OnWait { get; init; }

        #endregion

        #region Method

        public void SetResult(IAttemptResult result)
        {
            this.Result = result;
        }

        #endregion
    }

}
