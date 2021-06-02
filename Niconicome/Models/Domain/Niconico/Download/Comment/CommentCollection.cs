using System;
using System.Collections.Generic;
using System.Linq;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Domain.Niconico.Net.Json.WatchPage.V2;
using Response = Niconicome.Models.Domain.Niconico.Net.Json.API.Comment.Response;

namespace Niconicome.Models.Domain.Niconico.Download.Comment
{


    public interface ICommentCollection
    {
        LinkedList<Response::Comment> Comments { get; }
        Response::Thread? ThreadInfo { get; }
        int Count { get; }
        long Thread { get; }
        int Fork { get; }
        bool IsDefaultPostTarget { get; }
        void Add(Response::Comment comment, bool sort = true);
        void Add(List<Response::Comment> comments, bool addSafe = true);
        void Clear();
        void Distinct();
        void Sort();
        void Where(Func<Response::Comment, bool> predicate);
        int RemoveFor(int count);
        ICommentCollection Clone();
        IEnumerable<Response::Chat> GetAllComments();
        Response::Chat? GetFirstComment(bool includeOwnerComment = true);
    }


    /// <summary>
    /// コメントのコレクション
    /// コメ番昇順
    /// </summary>
    public class CommentCollection : ICommentCollection
    {
        public CommentCollection(ICommentCollection original)
        {
            this.commentsfield = new LinkedList<Response.Comment>(original.Comments);
            this.ThreadInfo = original.ThreadInfo;
        }

        public CommentCollection(int cOffset, long defaultThread, int defaultFork, bool unsafeHandle)
        {
            this.comThroughSetting = cOffset;
            this.isRoot = true;
            this.defaultTread = defaultThread;
            this.defaultFork = defaultFork;
            this.unsafeHandle = unsafeHandle;
        }

        public CommentCollection(long thread, int fork, bool isDefault, bool unsafeHandle)
        {
            this.Thread = thread;
            this.Fork = fork;
            this.IsDefaultPostTarget = isDefault;
            this.unsafeHandle = unsafeHandle;
        }

        public static ICommentCollection GetInstance(int cOffset, long defaultThread, int defaultFork, bool unsafeHandle)
        {
            return new CommentCollection(cOffset, defaultThread, defaultFork, unsafeHandle);
        }

        /// <summary>
        /// コメントの実体
        /// 降順で管理
        /// </summary>
        private readonly LinkedList<Response::Comment> commentsfield = new();

        private readonly List<ICommentCollection> childCollections = new();

        public const int NumberToThrough = 40;

        private readonly int comThroughSetting;

        private readonly bool isRoot;

        private readonly bool unsafeHandle;

        private readonly long defaultTread;

        private readonly int defaultFork;

        private bool isSorted;

        /// <summary>
        /// スレッド情報
        /// </summary>
        private Response::Thread? threadInfoField;

        /// <summary>
        /// スレッド番号
        /// </summary>
        public long Thread { get; init; }

        /// <summary>
        /// Fork
        /// </summary>
        public int Fork { get; init; }

        /// <summary>
        /// デフォルトフラグ
        /// </summary>
        public bool IsDefaultPostTarget { get; init; }

        /// <summary>
        /// コメント
        /// </summary>
        public LinkedList<Response::Comment> Comments
        {
            get
            {
                if (this.isRoot)
                {

                    LinkedList<Response::Comment> comments = new();
                    foreach (var child in this.childCollections)
                    {
                        foreach (var comment in child.Comments)
                        {
                            comments.AddFirst(comment);
                        }
                    }

                    return comments;
                }
                else
                {
                    if (!this.unsafeHandle && !this.isSorted)
                    {
                        this.Sort();
                    }
                    return this.commentsfield;
                }

            }
        }

        /// <summary>
        /// 要素数
        /// </summary>
        public int Count
        {
            get
            {
                if (this.isRoot)
                {
                    var count = 0;
                    foreach (var child in this.childCollections)
                    {
                        count += child.Count;
                    }
                    return count;
                }
                else
                {
                    return this.commentsfield.Count;
                }
            }
        }

        /// <summary>
        /// スレッド
        /// </summary>
        public Response::Thread? ThreadInfo
        {
            get
            {
                if (this.isRoot)
                {
                    return this.GetDefaultThread()?.ThreadInfo;
                }
                else
                {
                    return this.threadInfoField;
                }
            }
            private set => this.threadInfoField = value;
        }

        #region 操作系メソッド
        /// <summary>
        /// コメントを追加
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="sort"></param>
        public void Add(Response::Comment comment, bool sort = true)
        {
            if (this.isRoot)
            {
                long thread;
                int fork;

                if (comment.Thread is not null)
                {
                    thread = long.Parse(comment.Thread.ThreadThread);
                    fork = (int)(comment.Thread.Fork ?? 0);
                }
                else if (comment.Chat is not null)
                {
                    thread = long.Parse(comment.Chat.Thread);
                    fork = (int)(comment.Chat.Fork ?? 0);
                }
                else
                {
                    return;
                }


                if (!this.HasChild(thread, fork)) this.CreateChild(thread, fork);

                var child = this.GetChild(thread, fork);

                child.Add(comment);

            }
            else
            {
                if (comment.Thread is not null)
                {
                    if ((this.ThreadInfo?.LastRes ?? 0) < comment.Thread.LastRes)
                    {
                        this.ThreadInfo = comment.Thread;
                    }
                }
                else if (comment.Chat is not null)
                {
                    this.commentsfield.AddFirst(comment);
                    this.isSorted = false;
                    if (!this.unsafeHandle)
                    {
                        this.Distinct();
                    }
                }
                else
                {
                    return;
                }

            }
        }

        /// <summary>
        /// 複数のコメントを追加する
        /// </summary>
        /// <param name="comments"></param>
        public void Add(List<Response::Comment> comments, bool addSafe = true)
        {
            bool nicoruEnded = false;

            var chats = new LinkedList<Response::Comment>();

            //チャットを抽出する
            //コレクションの実装はAddFirst(降順)なのでこれは昇順になるようにする
            foreach (var item in comments)
            {
                if (item.Chat is null) continue;

                if (!nicoruEnded && item.Chat!.Nicoru is not null)
                {
                    continue;
                }
                else if (!nicoruEnded && item.Chat!.Nicoru is null)
                {
                    nicoruEnded = true;
                }

                chats.AddLast(item);
            }

            //最初の方にある古いコメントを除去する
            //この時点ではすでに昇順
            if (addSafe && chats.Count > this.comThroughSetting + 20)
            {
                var first = this.GetFirstComment();
                bool isFirstIsOldEnough = first is null ? false : first.No < this.comThroughSetting + 10;

                foreach (var _ in Enumerable.Range(0, this.comThroughSetting))
                {
                    chats.RemoveFirst();
                }
            }

            foreach (var item in chats)
            {
                this.Add(item, false);
            }

            if (!this.unsafeHandle)
            {
                this.Distinct();
            }

            var thread = comments.FirstOrDefault(c => c.Thread is not null);
            if (thread is not null)
            {
                this.Add(thread, false);
            }
        }

        /// <summary>
        /// 全てのコメントを削除
        /// </summary>
        public void Clear()
        {
            if (this.isRoot)
            {
                foreach (var child in this.childCollections)
                {
                    child.Clear();
                }
            }
            else
            {
                this.commentsfield.Clear();
            }
        }

        /// <summary>
        /// ユニーク化する
        /// </summary>
        public void Distinct()
        {
            if (this.isRoot)
            {
                foreach (var child in this.childCollections)
                {
                    child.Distinct();
                }
            } else
            {
                var copy = this.Clone();
                this.commentsfield.Clear();
                this.AddRange(copy.Comments.Distinct(c => c.Chat!.No));
            }
        }

        /// <summary>
        /// フィルターする
        /// </summary>
        /// <param name="predicate"></param>
        public void Where(Func<Response::Comment, bool> predicate)
        {
            if (this.isRoot)
            {
                foreach (var child in this.childCollections)
                {
                    child.Where(predicate);
                }
            }
            else
            {
                var copy = this.Comments.Copy();
                this.commentsfield.Clear();
                this.AddRange(copy.Where(predicate));
            }
        }

        /// <summary>
        /// 複製する
        /// </summary>
        /// <returns></returns>
        public ICommentCollection Clone()
        {
            return new CommentCollection(this);
        }

        /// <summary>
        /// 全てのコメントを取得
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Response::Chat> GetAllComments()
        {
            if (this.isRoot)
            {

                LinkedList<Response::Chat> comments = new();
                foreach (var child in this.childCollections)
                {
                    this.AddRange(comments, child.GetAllComments());
                }

                return comments;
            }
            else
            {
                return this.Comments.Select(c => c.Chat ?? new Response::Chat() { No = -1 }).Where(c => c.No != -1);
            }
        }

        /// <summary>
        /// 最初のコメントを取得する
        /// </summary>
        /// <returns></returns>
        public Response::Chat? GetFirstComment(bool includeOwnerComment = true)
        {
            if (this.isRoot)
            {

                var defaultThread = this.GetDefaultThread();

                return defaultThread?.GetFirstComment();
            }
            else
            {
                return this.Comments.LastOrDefault(c => includeOwnerComment || (c.Chat?.UserId != null || c.Chat?.Deleted == null))?.Chat;
            }
        }

        /// <summary>
        /// 並び替える
        /// </summary>
        public void Sort()
        {
            if (this.isRoot) return;

            var copy = this.commentsfield.ToList();
            copy.Sort((a, b) => (int)(b.Chat!.No - a.Chat!.No));
            this.commentsfield.Clear();
            this.AddRange(copy);
            this.isSorted = true;
        }

        /// <summary>
        /// 指定した数のコメント削除する
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public int RemoveFor(int count)
        {
            if (this.isRoot)
            {
                var defaultThread = this.GetDefaultThread()?.RemoveFor(count) ?? 0;
                return defaultThread;
            }
            else
            {
                if (count > this.commentsfield.Count)
                {
                    count = this.commentsfield.Count;
                }

                foreach (var _ in Enumerable.Range(0, count))
                {
                    this.commentsfield.RemoveFirst();
                }

                return count;
            }
        }

        #endregion

        #region private

        /// <summary>
        /// 子コレクションを確認
        /// </summary>
        /// <param name="thread"></param>
        /// <param name="fork"></param>
        /// <returns></returns>
        private bool HasChild(long thread, int fork)
        {
            return this.childCollections.Any(c => c.Thread == thread && c.Fork == fork);
        }

        /// <summary>
        /// 子コレクションを追加
        /// </summary>
        /// <param name="thread"></param>
        /// <param name="fork"></param>
        private void CreateChild(long thread, int fork)
        {
            if (this.HasChild(thread, fork)) return;

            var isDefault = thread == this.defaultTread && fork == this.defaultFork;

            this.childCollections.Add(new CommentCollection(thread, fork, isDefault, this.unsafeHandle));
        }

        /// <summary>
        /// 子コレクションを取得
        /// </summary>
        /// <param name="thread"></param>
        /// <param name="fork"></param>
        /// <returns></returns>
        private ICommentCollection GetChild(long thread, int fork)
        {
            return this.childCollections.First(c => c.Thread == thread && c.Fork == fork);
        }

        /// <summary>
        /// デフォルトスレッドを取得する
        /// </summary>
        /// <returns></returns>
        private ICommentCollection? GetDefaultThread()
        {
            if (!this.isRoot) throw new InvalidOperationException("このコレクションはルートではないためデフォルトスレッドを取得できません。");

            return this.childCollections.FirstOrDefault(c => c.IsDefaultPostTarget);
        }

        /// <summary>
        /// 複数のコメントを追加する
        /// </summary>
        /// <param name="comments"></param>
        private void AddRange(IEnumerable<Response::Comment> comments)
        {
            if (this.isRoot) throw new InvalidOperationException("ルートコレクションでは'Addrange'が許可されません。");

            this.AddRange(this.commentsfield, comments);
        }

        /// <summary>
        /// AddRangeの実装
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="comments"></param>
        private void AddRange<T>(LinkedList<T> list, IEnumerable<T> comments)
        {
            foreach (var comment in comments)
            {
                list.AddFirst(comment);
            }
        }

        #endregion
    }

}
