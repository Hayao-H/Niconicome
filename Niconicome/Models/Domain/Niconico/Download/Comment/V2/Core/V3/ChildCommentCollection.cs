using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Niconico.Download.Comment.V2.Core.V3
{
    public interface IChildCommentCollection
    {
        /// <summary>
        /// Fork
        /// </summary>
        string Fork { get; init; }

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
        /// 指定したコメントを取得
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        IAttemptResult<IComment> Get(int no);

        /// <summary>
        /// 欠けているコメントの幅を取得
        /// > コメントX（IComment）からコメント番号が小さい（古い）方にY（int）コメント
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<(IComment, int)> GetUnFilledRange(int originationNo = 0);

        /// <summary>
        /// 最初のコメントを取得
        /// </summary>
        /// <returns></returns>
        IAttemptResult<IComment> GetFirstcomment();
    }

    public class ChildCommentCollection : IChildCommentCollection
    {
        public ChildCommentCollection(int commentCount, string thread, string fork)
        {
            this.Fork = fork;
            this.Thread = thread;
            this._comment = new IComment[commentCount];
        }

        #region field

        private readonly IComment[] _comment;

        #endregion

        #region Props

        public string Fork { get; init; }

        public string Thread { get; init; }

        public int Count => this._comment.Count(c => c is not null);

        public IEnumerable<IComment> Comments => this._comment.ToList().AsReadOnly();

        #endregion

        #region Method

        public IAttemptResult Add(IComment comment)
        {
            if (comment.Thread != this.Thread || comment.Fork != this.Fork)
            {
                return AttemptResult.Fail("コメントのThreadまたはForkがコレクションのThreadまたはForkと一致しません。");
            }


            this._comment[comment.No - 1] = comment;

            return AttemptResult.Succeeded();
        }

        public IAttemptResult<IComment> Get(int no)
        {

            var comment = this._comment[no - 1];

            if (comment is null)
            {
                return AttemptResult<IComment>.Fail("指定されたコメントが存在しません");
            }
            else
            {
                return AttemptResult<IComment>.Succeeded(comment);
            }
        }


        public IReadOnlyList<(IComment, int)> GetUnFilledRange(int originationNo = 0)
        {
            var range = new List<(IComment, int)>();


            bool requreEmptyCheck = true;
            IComment? lastStartComment = null;
            int jumpCount = 0;

            foreach (var i in Enumerable.Range(0, this._comment.Length - 1).Reverse())
            {
                IComment? c = this._comment[i];

                if (requreEmptyCheck)
                {
                    if (c is null)
                    {
                        continue;
                    }
                    else
                    {
                        requreEmptyCheck = false;
                        lastStartComment = c;
                    }
                }

                if (originationNo > 0 && i + 1 > originationNo)
                {
                    lastStartComment = c;
                    continue;
                }

                if (c is not null && jumpCount < 10)
                {
                    lastStartComment = c;
                    jumpCount = 0;
                    continue;
                }
                else if (c is not null && jumpCount >= 10)
                {
                    if (lastStartComment is not null)
                    {
                        range.Add((lastStartComment, jumpCount));
                    }

                    lastStartComment = c;
                    jumpCount = 0;
                }
                else if (c is null)
                {
                    jumpCount++;
                }

            }

            return range;

        }

        public IAttemptResult<IComment> GetFirstcomment()
        {
            IComment? first = this.Comments.FirstOrDefault(c => c is not null);

            if (first is null)
            {
                return AttemptResult<IComment>.Fail("最初のコメントを取得できませんでした。コメント画からの可能性があります。");
            }
            else
            {
                return AttemptResult<IComment>.Succeeded(first);
            }
        }


        #endregion
    }
}
