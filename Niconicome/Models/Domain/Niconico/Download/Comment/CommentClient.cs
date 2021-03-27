using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
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
        bool IsReplaceStrictedEnable { get; }
        int CommentOffset { get; }
        int MaxcommentsCount { get; }
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

        public int MaxcommentsCount { get; set; }


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

                if (settings.MaxcommentsCount > 0 && comments.Count > settings.MaxcommentsCount)
                {
                    var rmCount = comments.Count - settings.MaxcommentsCount;
                    comments.RemoveFor(rmCount);
                    break;
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


}
