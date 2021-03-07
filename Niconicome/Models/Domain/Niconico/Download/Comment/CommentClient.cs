using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Domain.Niconico.Net.Json;
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
        int CommentOffset { get; }
    }

    public interface ICommentCollection
    {
        List<Response::Comment> Comments { get; }
        Response::Thread? Thread { get; }
        int Count { get; }
        void Add(Response::Comment comment, bool sort = true);
        void Add(List<Response::Comment> comments, bool addSafe = true);
        void Clear();
        void Distinct();
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
            var comments = CommentCollection.GetInstance(settings.CommentOffset);
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

            comments.Distinct();

            long prev = 0;
            var skipped = new List<long>();

            foreach (var comment in comments.Comments)
            {
                if (prev == 0)
                {
                    prev = comment.Chat!.No;
                    continue;
                }

                if (prev + 1 != comment.Chat!.No)
                {
                    for (var i = 0; i < comment.Chat!.No - prev - 1; i++)
                    {
                        skipped.Add(prev + 1);
                        prev++;
                    }
                } else
                {
                    prev++;
                }
            }

            var skippedS = string.Join(Environment.NewLine, skipped);

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
    }

    /// <summary>
    /// コメントのコレクション
    /// </summary>
    public class CommentCollection : ICommentCollection
    {
        public CommentCollection(ICommentCollection original)
        {
            this.comments = new List<Response.Comment>();
            this.comments.AddRange(original.Comments);
            this.Thread = original.Thread;
        }

        public CommentCollection(int cOffset)
        {
            this.comments = new List<Response.Comment>();
            this.comThroughSetting = cOffset;
        }

        public static ICommentCollection GetInstance(int cOffset = CommentCollection.NumberToThrough)
        {
            return new CommentCollection(cOffset);
        }

        private readonly List<Response::Comment> comments;

        public const int NumberToThrough = 40;

        private readonly int comThroughSetting;

        /// <summary>
        /// コメント
        /// </summary>
        public List<Response::Comment> Comments { get => this.comments; }


        /// <summary>
        /// 要素数
        /// </summary>
        public int Count { get => this.comments.Count; }

        /// <summary>
        /// スレッド
        /// </summary>
        public Response::Thread? Thread { get; private set; }

        /// <summary>
        /// コメントを追加
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="sort"></param>
        public void Add(Response::Comment comment, bool sort = true)
        {
            if (comment.Thread is not null)
            {
                if ((this.Thread?.LastRes ?? 0) < comment.Thread.LastRes)
                {
                    this.Thread = comment.Thread;
                }
            }
            else if (comment.Chat is not null)
            {
                if (!this.comments.Any(c => c.Chat!.No == comment.Chat.No))
                {
                    this.comments.Add(comment);
                }
            }
            else
            {
                return;
            }

            if (sort) this.Sort();
        }

        /// <summary>
        /// 複数のコメントを追加する
        /// </summary>
        /// <param name="comments"></param>
        public void Add(List<Response::Comment> comments, bool addSafe = true)
        {

            if (addSafe)
            {
                var newCollection = CommentCollection.GetInstance();
                newCollection.Add(comments, false);
                if (newCollection.Count > this.comThroughSetting)
                {
                    newCollection.Comments.RemoveRange(0, this.comThroughSetting);
                    comments.Clear();
                    comments.AddRange(newCollection.Comments);
                }
            }

            foreach (var comment in comments)
            {
                this.Add(comment, false);
            }
            this.Sort();
        }

        /// <summary>
        /// 全てのコメントを削除
        /// </summary>
        public void Clear()
        {
            this.comments.Clear();
        }

        /// <summary>
        /// ユニーク化する
        /// </summary>
        public void Distinct()
        {
            var copy = this.Clone();
            this.comments.Clear();
            this.comments.AddRange(copy.Comments.Distinct(c => c.Chat!.No));
        }

        /// <summary>
        /// フィルターする
        /// </summary>
        /// <param name="predicate"></param>
        public void Where(Func<Response::Comment, bool> predicate)
        {
            var copy = this.comments.Copy();
            this.comments.Clear();
            this.comments.AddRange(copy.Where(predicate));
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
            return this.comments.Select(c => c.Chat ?? new Response::Chat() { No = -1 }).Where(c => c.No != -1);
        }

        /// <summary>
        /// 最初のコメントを取得する
        /// </summary>
        /// <returns></returns>
        public Response::Chat? GetFirstComment(bool includeOwnerComment = true)
        {
            return this.comments.FirstOrDefault(c => includeOwnerComment || (c.Chat?.UserId != null || c.Chat?.Deleted != null))?.Chat;
        }

        /// <summary>
        /// 並び替える
        /// </summary>
        private void Sort()
        {
            this.comments.Sort((a, b) => (int)(a.Chat!.No - b.Chat!.No));
        }
    }

}
