using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Domain.Niconico.Net.Json.API.Search;
using Niconicome.Models.Domain.Niconico.Watch;
using Niconicome.Models.Domain.Utils;
using Response = Niconicome.Models.Domain.Niconico.Net.Json.API.Comment.Response;

namespace Niconicome.Models.Domain.Niconico.Download.Comment
{

    public interface ICommentDownloadSettings
    {
        string NiconicoId { get; }
        string FolderName { get; }
        string FileNameFormat { get; }
        bool IsOverwriteEnable { get; }
        bool IsAutoDisposingEnable { get; }
        bool IsDownloadingLogEnable { get; }
        bool IsDownloadingOwnerCommentEnable { get; }
        bool IsDownloadingEasyCommentEnable { get; }
        bool IsReplaceStrictedEnable { get; }
        int CommentOffset { get; }
    }

    public interface ICommentCollection
    {
        List<Response::Comment> Comments { get; }
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
        ICommentCollection Clone();
        IEnumerable<Response::Chat> GetAllComments();
        Response::Chat? GetFirstComment(bool includeOwnerComment = true);
    }

    public interface ICommentClient
    {
        Task<ICommentCollection> DownloadCommentAsync(IDmcInfo dmcInfo, ICommentDownloadSettings settings, IDownloadMessenger messenger, IDownloadContext context, CancellationToken token);
    }

    public class CommentDownloadSettings : ICommentDownloadSettings
    {
        public string NiconicoId { get; set; } = string.Empty;

        public string FolderName { get; set; } = string.Empty;

        public string FileNameFormat { get; set; } = string.Empty;

        public bool IsOverwriteEnable { get; set; }

        public bool IsAutoDisposingEnable { get; set; }

        public bool IsDownloadingLogEnable { get; set; }

        public bool IsDownloadingOwnerCommentEnable { get; set; }

        public bool IsDownloadingEasyCommentEnable { get; set; }
        
        public bool IsReplaceStrictedEnable { get; set; }

        public int CommentOffset { get; set; }

    }

    /// <summary>
    /// コメントクライアント
    /// </summary>
    public class CommentClient : ICommentClient
    {

        public CommentClient(INicoHttp http, ICommentRequestBuilder requestBuilder, ILogger logger)
        {
            this.http = http;
            this.requestBuilder = requestBuilder;
            this.logger = logger;
        }

        private readonly INicoHttp http;

        private readonly ICommentRequestBuilder requestBuilder;

        private readonly ILogger logger;

        /// <summary>
        /// コメントをダウンロードする
        /// </summary>
        /// <param name="dmcInfo"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public async Task<ICommentCollection> DownloadCommentAsync(IDmcInfo dmcInfo, ICommentDownloadSettings settings, IDownloadMessenger messenger, IDownloadContext context, CancellationToken token)
        {
            var (dThread, dFork) = this.GetDefaultPosyTarget(dmcInfo);

            if (dThread == -1 || dFork == -1) throw new InvalidOperationException("DefaultPostTargetが見つかりません。");

            var comments = CommentCollection.GetInstance(settings.CommentOffset, dThread, dFork);
            Response::Chat? first;
            int index = 0;
            long lastNo = 0;

            do
            {
                if (index > 0) messenger.SendMessage($"過去ログをダウンロード中({index + 1}件目)");
                first = comments.GetFirstComment(false);
                long? when = comments.Count == 0 ? 0 : first?.Date - 1;
                List<Response::Comment> retlieved = await this.GetCommentsAsync(dmcInfo, settings, when);

                comments.Add(retlieved);

                if (index > 0)
                {
                    this.logger.Log($"過去ログをダウンロードしました。({index}個目, {retlieved.Count}コメント, {context.GetLogContent()})");
                }
                else
                {
                    this.logger.Log($"現行スレッドをダウンロードしました。({retlieved.Count}コメント, {context.GetLogContent()})");
                }

                first = comments.GetFirstComment(false);
                if ((first?.No ?? 0) == lastNo)
                {
                    break;
                }
                else
                {
                    lastNo = first?.No ?? 0;
                }

                if (!settings.IsDownloadingLogEnable)
                {
                    break;
                }
                else if (index == 0)
                {
                    messenger.SendMessage("過去ログのダウンロードを開始します。");
                }

                ++index;
            } while (first?.No > 1 && !token.IsCancellationRequested);

            if (token.IsCancellationRequested) throw new TaskCanceledException("コメントのダウンロードがキャンセルされました。");

            this.logger.Log($"コメントのダウンロードが完了しました。({comments.Count}コメント, {context.GetLogContent()})");

            return comments;

        }

        /// <summary>
        /// コメントを取得する
        /// </summary>
        /// <param name="dmcInfo"></param>
        /// <param name="settings"></param>
        /// <param name="when"></param>
        /// <returns></returns>
        private async Task<List<Response::Comment>> GetCommentsAsync(IDmcInfo dmcInfo, ICommentDownloadSettings settings, long? when = null)
        {
            var option = new CommentOptions()
            {
                NoEasyComment = !settings.IsDownloadingEasyCommentEnable,
                OwnerComment = settings.IsDownloadingOwnerCommentEnable,
                When = when ?? 0
            };
            var request = await this.requestBuilder.GetRequestDataAsync(dmcInfo, option);

            var res = await this.http.PostAsync(new Uri("https://nmsg.nicovideo.jp/api.json/"), new StringContent(request));
            if (!res.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"コメントの取得に失敗しました。(status_code:{(int)res.StatusCode}, reason_phrase:{res.ReasonPhrase})");
            }

            string content = await res.Content.ReadAsStringAsync();
            var data = JsonParser.DeSerialize<List<Response::Comment>>(content);

            return data;
        }

        private (long, int) GetDefaultPosyTarget(IDmcInfo dmcInfo)
        {
            foreach (var thread in dmcInfo.CommentThreads)
            {
                if (thread.IsDefaultPostTarget)
                {
                    return (thread.Id, thread.Fork);
                }
            }

            return (-1, -1);
        }
    }

    /// <summary>
    /// コメントのコレクション
    /// </summary>
    public class CommentCollection : ICommentCollection
    {
        public CommentCollection(ICommentCollection original)
        {
            this.commentsfield = new List<Response.Comment>();
            this.commentsfield.AddRange(original.Comments);
            this.ThreadInfo = original.ThreadInfo;
        }

        public CommentCollection(int cOffset, long defaultThread, int defaultFork)
        {
            this.comThroughSetting = cOffset;
            this.isRoot = true;
            this.defaultTread = defaultThread;
            this.defaultFork = defaultFork;
        }

        public CommentCollection(long thread, int fork, bool isDefault)
        {
            this.Thread = thread;
            this.Fork = fork;
            this.IsDefaultPostTarget = isDefault;

        }

        public static ICommentCollection GetInstance(int cOffset, long defaultThread, int defaultFork)
        {
            return new CommentCollection(cOffset, defaultThread, defaultFork);
        }

        private readonly List<Response::Comment> commentsfield = new();

        private readonly List<ICommentCollection> childCollections = new();

        public const int NumberToThrough = 40;

        private readonly int comThroughSetting;

        private readonly bool isRoot;

        private readonly long defaultTread;

        private readonly int defaultFork;

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
        public List<Response::Comment> Comments
        {
            get
            {
                if (this.isRoot)
                {

                    List<Response::Comment> comments = new();
                    foreach (var child in this.childCollections)
                    {
                        comments.AddRange(child.Comments);
                    }

                    return comments;
                }
                else
                {
                    this.commentsfield.Sort((a, b) => (int)(a.Chat!.No - b.Chat!.No));
                    return this.commentsfield.Distinct(c => c.Chat!.No).ToList();
                }

            }
        }


        /// <summary>
        /// 要素数
        /// </summary>
        public int Count { get => this.Comments.Count; }

        /// <summary>
        /// スレッド
        /// </summary>
        public Response::Thread? ThreadInfo
        {
            get
            {
                if (this.isRoot)
                {
                    return this.childCollections.FirstOrDefault()?.ThreadInfo;
                }
                else
                {
                    return this.threadInfoField;
                }
            }
            private set => this.threadInfoField = value;
        }

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
                    //if (!this.commentsfield.Any(c => c.Chat!.No == comment.Chat.No))
                    //{
                    this.commentsfield.Add(comment);
                    //}
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

            ///if (addSafe && this.comThroughSetting > 0 && comments.Count > this.comThroughSetting)
            ///{
            ///    var filtered = comments.Copy().Where(c => c.Chat is not null).ToList();
            ///    if (filtered.Count > this.comThroughSetting)
            ///    {
            ///        filtered.RemoveRange(0, this.comThroughSetting);
            ///        comments.Clear();
            ///        comments.AddRange(filtered);
            ///    }
            ///}

            //bool chatStarted = false;
            bool nicoruEnded = false;
            int skipped = 0;
            int cCounts = comments.Where(c => c.Chat is not null).Count();
            var first = this.GetFirstComment();
            bool isFirstIsOldEnough = first is null ? false : first.No < this.comThroughSetting + 10;

            foreach (var item in comments)
            {
                if (item.Chat is not null)
                {
                    if (!nicoruEnded && item.Chat.Nicoru is not null)
                    {
                        continue;
                    }
                    else if (!nicoruEnded && item.Chat.Nicoru is null)
                    {
                        nicoruEnded = true;
                    }
                }

                if (addSafe && !isFirstIsOldEnough && this.comThroughSetting > 0 && cCounts > this.comThroughSetting + 20 && item.Chat is not null)
                {
                    if (skipped < this.comThroughSetting)
                    {
                        ++skipped;
                        continue;
                    }
                }
                this.Add(item, false);
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
            var copy = this.Clone();
            this.commentsfield.Clear();
            this.commentsfield.AddRange(copy.Comments.Distinct(c => c.Chat!.No));
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
                this.commentsfield.AddRange(copy.Where(predicate));
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

                List<Response::Chat> comments = new();
                foreach (var child in this.childCollections)
                {
                    comments.AddRange(child.GetAllComments());
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
                //IEnumerable<Response::Chat?> GetFirstComents(IEnumerable<ICommentCollection> collection)
                //{
                //    return collection.Select(c => c.GetFirstComment()).Where(c => c is not null);
                //}

                var defaultThread = this.childCollections.FirstOrDefault(c => c.IsDefaultPostTarget);

                return defaultThread?.Comments.FirstOrDefault()?.Chat;

                ////デフォルトスレッドに1がある場合はリターン
                //if (GetFirstComents(defaultThread).Any(c => c!.No == 1))
                //{
                //    return GetFirstComents(defaultThread).FirstOrDefault(c => c!.No == 1);
                //}
                //
                //var children = GetFirstComents(this.childCollections);
                //if (!children.Any()) return null;
                //var max = children.Max(c => c!.No);
                //return children.FirstOrDefault(c => c!.No == max);
            }
            else
            {
                return this.commentsfield.FirstOrDefault(c => includeOwnerComment || (c.Chat?.UserId != null || c.Chat?.Deleted != null))?.Chat;
            }
        }

        /// <summary>
        /// 並び替える
        /// </summary>
        public void Sort()
        {
            this.commentsfield.Sort((a, b) => (int)(a.Chat!.No - b.Chat!.No));
        }

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

            this.childCollections.Add(new CommentCollection(thread, fork, isDefault));
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
    }

}
