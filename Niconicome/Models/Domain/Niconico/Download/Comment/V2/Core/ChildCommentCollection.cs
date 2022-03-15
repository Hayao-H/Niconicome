using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Niconico.Download.Comment.V2.Core
{
    public interface IChildCommentCollection
    {
        /// <summary>
        /// Fork
        /// </summary>
        int Fork { get; init; }

        /// <summary>
        /// Thread
        /// </summary>
        string Thread { get; init; }

        /// <summary>
        /// コメント数を取得
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 全てのコメントを取得する
        /// </summary>
        IEnumerable<IComment> Comments { get; }

        /// <summary>
        /// コメントを追加する
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        IAttemptResult Add(IComment comment);

        /// <summary>
        /// 欠けているコメントの幅を取得
        /// > コメントX（IComment）からコメント番号が小さい（古い）方にY（int）コメント
        /// </summary>
        /// <returns></returns>
        List<(IComment, int)> GetUnFilledRange();
    }

    public class ChildCommentCollection : IChildCommentCollection
    {
        public ChildCommentCollection(long commentCount, int commentCountPerBlock, string thread, int fork)
        {
            this.Fork = fork;
            this.Thread = thread;
            this._comment = new IComment[(int)Math.Ceiling((double)(commentCount / commentCountPerBlock))][];
            this._commentCountPerBlock = commentCountPerBlock;

            for (var i = 0; i < this._comment.Length; i++)
                this._comment[i] = new IComment[commentCountPerBlock];
        }

        #region field

        private readonly IComment[][] _comment;

        private readonly int _commentCountPerBlock;

        #endregion

        #region Props

        public int Fork { get; init; }

        public string Thread { get; init; }

        public int Count => this._comment.SelectMany(_ => _).Count(c => c is not null);

        public IEnumerable<IComment> Comments => this._comment.SelectMany(_ => _);

        #endregion

        #region Method

        public IAttemptResult Add(IComment comment)
        {
            if (comment.Thread != this.Thread || comment.Fork != this.Fork)
            {
                return AttemptResult.Fail("コメントのThreadまたはForkがコレクションのThreadまたはForkと一致しません。");
            }

            int cIndex = (int)Math.Ceiling((double)comment.No / (double)this._commentCountPerBlock) - 1;
            int cInnerIndex = comment.No - cIndex * this._commentCountPerBlock - 1;

            if (cIndex < 0 || cIndex + 1 > this._comment.GetLength(0) || cInnerIndex < 0 || cInnerIndex + 1 > this._commentCountPerBlock)
            {
                return AttemptResult.Fail("コメントインデックスの算出に失敗しました。");
            }

            this._comment[cIndex][cInnerIndex] = comment;

            return AttemptResult.Succeeded();
        }

        public List<(IComment, int)> GetUnFilledRange()
        {
            bool requreEmptyCheck = true;
            var range = new List<(IComment, int)>();

            for (var i = this._comment.Length - 1; i >= 0; i--)
            {
                if (requreEmptyCheck)
                {
                    if (!this._comment[i].Any())
                    {
                        continue;
                    }
                    else
                    {
                        requreEmptyCheck = false;
                    }
                }

                int commentscount = this._comment[i].Where(c => c is not null).Count();

                if (commentscount < this._commentCountPerBlock)
                {
                    if (i == this._comment.Length - 1)
                    {
                        continue;
                    }
                    else
                    {
                        IComment? first = this._comment[i + 1].FirstOrDefault();
                        if (first is not null)
                        {
                            range.Add((first, this._commentCountPerBlock));
                        }
                        else if (range.Count > 0)
                        {
                            var (c, r) = range[range.Count - 1];
                            range[range.Count - 1] = (c, r + this._commentCountPerBlock);
                        }
                    }
                }

            }

            return range;

        }

        #endregion

        #region Test

        public IComment[][] CommentsForTest => this._comment;

        #endregion
    }
}
