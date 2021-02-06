using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Extensions.System.List;
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
    }

    public interface ICommentCollection
    {
        IEnumerable<Response::Comment> Comments { get; }
        Response::Thread? Thread { get; }
        int Count { get; }
        void Add(Response::Comment comment, bool sort = true);
        void Add(IEnumerable<Response::Comment> comments);
        void Clear();
        void Distinct();
        void Where(Func<Response::Comment, bool> predicate);
        ICommentCollection Clone();
        IEnumerable<Response::Chat> GetAllComments();
        Response::Chat? GetFirstComment(bool includeOwnerComment = true);
    }

    public interface ICommentClient
    {
        Task<ICommentCollection> DownloadCommentInternalAsync(IDmcInfo dmcInfo, ICommentDownloadSettings settings, IDownloadMessenger messenger, CancellationToken token);
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
        public async Task<ICommentCollection> DownloadCommentInternalAsync(IDmcInfo dmcInfo, ICommentDownloadSettings settings, IDownloadMessenger messenger, CancellationToken token)
        {
            var comments = CommentCollection.GetInstance();
            Response::Chat? first;
            int index = 0;
            long lastNo = 0;

            do
            {
                if (index > 0) messenger.SendMessage($"過去ログをダウンロード中({index + 1}件目)");
                long? when = comments.Count == 0 ? 0 : comments.GetFirstComment(false)?.Date - 1;
                List<Response::Comment> retlieved = await this.GetCommentsAsync(dmcInfo, settings, when) ;

                comments.Add(retlieved);

                if (index > 0)
                {
                    this.logger.Log($"過去ログをダウンロードしました。({index}個目、{retlieved.Count}コメント)");
                }
                else
                {
                    this.logger.Log($"現行スレッドをダウンロードしました。({retlieved.Count}コメント)");
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

            this.logger.Log($"コメントのダウンロードが完了しました。({comments.Count}コメント)");

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

        public CommentCollection()
        {
            this.comments = new List<Response.Comment>();
        }

        public static ICommentCollection GetInstance()
        {
            return new CommentCollection();
        }

        private readonly List<Response::Comment> comments;

        /// <summary>
        /// コメント
        /// </summary>
        public IEnumerable<Response::Comment> Comments { get => this.comments; }


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
        public void Add(IEnumerable<Response::Comment> comments)
        {
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
