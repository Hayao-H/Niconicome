using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Niconico.Download.Comment.V2.Core
{
    public interface ICommentCollection
    {

        /// <summary>
        /// コメント数を取得
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 全てのコメントを取得する
        /// </summary>
        IEnumerable<IComment> Comments { get; }

        /// <summary>
        /// コメントを追加
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        IAttemptResult Add(IComment comment);

        /// <summary>
        /// 指定したコメントを取得
        /// </summary>
        /// <param name="thread"></param>
        /// <param name="fork"></param>
        /// <param name="no"></param>
        /// <returns></returns>
        IAttemptResult<IComment> Get(string thread, string fork, int no);

        /// <summary>
        /// コメントの欠け情報を取得する
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<UnFilledRange> GetUnFilledRange();

        /// <summary>
        /// 指定したthread/forkの最初のコメントを取得
        /// </summary>
        /// <param name="trhead"></param>
        /// <param name="fork"></param>
        /// <returns></returns>
        IAttemptResult<IComment> GetFirstComment(string trhead, string fork);
    }

    public class CommentCollection : ICommentCollection
    {
        public CommentCollection(int commentCount, int commentCountPerBlock)
        {
            this._commentCount = commentCount;
            this._commentCountPerBlock = commentCountPerBlock;
        }

        #region field

        private readonly List<IChildCommentCollection> _children = new();

        private readonly int _commentCountPerBlock;

        private readonly int _commentCount;

        #endregion

        #region Props

        public int Count => this._children.Select(c => c.Count).Sum();

        public IEnumerable<IComment> Comments => this._children.Select(c => c.Comments).SelectMany(_ => _).Where(x => x is not null);

        #endregion

        #region Method

        public IAttemptResult Add(IComment comment)
        {
            string thread = comment.Thread;
            string fork = comment.Fork;

            IChildCommentCollection collection = this.GetCollection(thread, fork);

            return collection.Add(comment);
        }

        public IAttemptResult<IComment> Get(string thread, string fork, int no)
        {
            var collection = this.GetCollection(thread, fork);

            return collection.Get(no);
        }


        public IReadOnlyList<UnFilledRange> GetUnFilledRange()
        {
            var list = new List<UnFilledRange>();

            foreach (var collection in this._children)
            {
                IReadOnlyList<(IComment, int)> ranges = collection.GetUnFilledRange();

                if (ranges.Count == 0) continue;
                foreach (var range in ranges)
                {
                    (IComment comment, int count) = range;
                    list.Add(new UnFilledRange(comment, count, collection.Thread, collection.Fork));
                }
            }

            return list;
        }

        public IAttemptResult<IComment> GetFirstComment(string thread, string fork)
        {
            IChildCommentCollection collection = this.GetCollection(thread, fork);

            return collection.GetFirstcomment();
        }


        #endregion

        #region field

        /// <summary>
        /// 指定したThread/Forkのコレクションが存在しない場合追加
        /// </summary>
        /// <param name="thread"></param>
        /// <param name="fork"></param>
        private void AddCollectionIfNotExists(string thread, string fork)
        {
            if (!this._children.Any(c => c.Thread == thread && c.Fork == fork))
            {
                this._children.Add(new ChildCommentCollection(this._commentCount, this._commentCountPerBlock, thread, fork));
            }
        }

        /// <summary>
        /// 指定したThread/Forkのコレクションを取得
        /// </summary>
        /// <param name="thread"></param>
        /// <param name="fork"></param>
        /// <returns></returns>
        private IChildCommentCollection GetCollection(string thread, string fork)
        {
            this.AddCollectionIfNotExists(thread, fork);

            return this._children.First(c => c.Thread == thread && c.Fork == fork);

        }

        #endregion
    }

    /// <summary>
    /// コメントの欠け情報
    /// </summary>
    /// <param name="Start">欠けのスタート位置</param>
    /// <param name="Count">欠けたコメントの数（コメント番号が小さい（=古い）方へ）</param>
    /// <param name="Thread">Thread</param>
    /// <param name="Fork">Fork</param>
    public record UnFilledRange(IComment Start, int Count, string Thread, string Fork);
}
