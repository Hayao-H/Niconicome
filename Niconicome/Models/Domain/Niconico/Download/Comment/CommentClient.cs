using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Domain.Niconico.Video.Infomations;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Network.Download;
using Response = Niconicome.Models.Domain.Niconico.Net.Json.API.Comment.Response;

namespace Niconicome.Models.Domain.Niconico.Download.Comment
{

    public interface ICommentClient
    {
        Task<ICommentCollection> DownloadCommentAsync(IDmcInfo dmcInfo, IDownloadSettings settings, Action<string> onMessage, IDownloadContext context, CancellationToken token);
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
        public async Task<ICommentCollection> DownloadCommentAsync(IDmcInfo dmcInfo, IDownloadSettings settings, Action<string> onMessage, IDownloadContext context, CancellationToken token)
        {
            var (dThread, dFork) = this.GetDefaultPosyTarget(dmcInfo);

            if (dThread == -1 || dFork == -1) throw new InvalidOperationException("DefaultPostTargetが見つかりません。");

            var comments = CommentCollection.GetInstance(settings.CommentOffset, dThread, dFork, settings.EnableUnsafeCommentHandle, settings.EnableExperimentalCommentSafetySystem);
            Response::Chat? first = null;
            int index = 0;
            long lastNo = 0;

            do
            {
                int count = comments.Count;

                if (index > 0)
                {
                    if (settings.CommentFetchWaitSpan > 0)
                    {
                        onMessage($"待機中...({settings.CommentFetchWaitSpan}ms)");
                        try
                        {
                            await Task.Delay(settings.CommentFetchWaitSpan, token);
                        }
                        catch { }
                    }
                    onMessage($"過去ログをダウンロード中({index + 1}件目・{count}コメ)");
                }

                long? when = count == 0 ? 0 : first?.Date - 1;
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

                if (settings.MaxCommentsCount > 0 && comments.Count > settings.MaxCommentsCount)
                {
                    var rmCount = comments.Count - settings.MaxCommentsCount;
                    comments.RemoveFor(rmCount);
                    break;
                }

                if (!settings.DownloadLog)
                {
                    break;
                }
                else if (index == 0)
                {
                    onMessage("過去ログのダウンロードを開始します。");
                }

                ++index;
            } while (first?.No > 1 && !token.IsCancellationRequested);

            if (token.IsCancellationRequested) throw new TaskCanceledException("コメントのダウンロードがキャンセルされました。");

            onMessage($"コメントの正規化処理を開始します。");
            comments.Distinct();
            onMessage($"コメントの正規化処理が完了しました。");

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
        private async Task<List<Response::Comment>> GetCommentsAsync(IDmcInfo dmcInfo, IDownloadSettings settings, long? when = null)
        {
            var option = new CommentOptions()
            {
                NoEasyComment = !settings.DownloadEasy,
                OwnerComment = settings.DownloadOwner,
                When = when ?? 0
            };
            var request = await this.requestBuilder.GetRequestDataAsync(dmcInfo, option);
            string server = dmcInfo.CommentThreads.FirstOrDefault()?.Server ?? @"https://nmsg.nicovideo.jp/api.json";

            if (!server.EndsWith("/api.json"))
            {
                server += "/api.json";
            }

            var res = await this.http.PostAsync(new Uri(server), new StringContent(request));
            if (!res.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"コメントの取得に失敗しました。(status_code:{(int)res.StatusCode}, reason_phrase:{res.ReasonPhrase})");
            }

            string content = await res.Content.ReadAsStringAsync();
            var data = JsonParser.DeSerialize<List<Response::Comment>>(content);

            return data;
        }

        private (long, long) GetDefaultPosyTarget(IDmcInfo dmcInfo)
        {
            foreach (var thread in dmcInfo.CommentThreads)
            {
                if (thread.IsDefaultPostTarget)
                {
                    return (thread.ID, thread.Fork);
                }
            }

            return (-1, -1);
        }
    }


}
