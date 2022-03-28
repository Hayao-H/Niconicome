using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Video.Infomations;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Network.Download;
using Converter = Niconicome.Models.Domain.Niconico.Download.Comment.V2.Core.Converter;
using Core = Niconicome.Models.Domain.Niconico.Download.Comment.V2.Core;
using Fetch = Niconicome.Models.Domain.Niconico.Download.Comment.V2.Fetch;
using Response = Niconicome.Models.Domain.Niconico.Net.Json.API.Comment.V2.Response;

namespace Niconicome.Models.Domain.Niconico.Download.Comment.V2.Integrate
{
    public interface ICommentClient
    {
        /// <summary>
        /// 非同期にコメントをダウンロードする
        /// </summary>
        /// <param name="dmcInfo"></param>
        /// <param name="settings"></param>
        /// <param name="getCommetUntil">指定した日付より新しいコメントを取得</param>
        /// <returns></returns>
        Task<IAttemptResult<IEnumerable<Core::IComment>>> DownloadCommentAsync(IDmcInfo dmcInfo, IDownloadSettings settings, DateTime getCommetUntil = default);
    }

    public class CommentClient : ICommentClient
    {
        public CommentClient(Fetch::ICommentRequestBuilder requestBuilder, Fetch::ICommentDownloader downloader, Converter::INetCommentConverter converter, ILogger logger)
        {
            this._requestBuilder = requestBuilder;
            this._downloader = downloader;
            this._converter = converter;
            this._logger = logger;
        }

        #region field

        /// <summary>
        /// key:スレッドID val:{ index:fork item1: 開始コメ番 item2: コメント数 }
        /// </summary>
        private readonly Dictionary<string, (int, int)[]> _lastFetchedInfo = new();

        private readonly Fetch::ICommentRequestBuilder _requestBuilder;

        private readonly Fetch::ICommentDownloader _downloader;

        private readonly Converter::INetCommentConverter _converter;

        private readonly ILogger _logger;

        #endregion

        #region Method

        public async Task<IAttemptResult<IEnumerable<Core::IComment>>> DownloadCommentAsync(IDmcInfo dmcInfo, IDownloadSettings settings, DateTime getCommetUntil = default)
        {
            IAttemptResult<IEnumerable<Core::IComment>> result;

            try
            {
                result = await this.DownloadCommentAsyncInternal(dmcInfo, settings, getCommetUntil);
            }
            catch (Exception ex)
            {
                this._logger.Error("コメント取得でエラーが発生しました。", ex);
                return AttemptResult<IEnumerable<Core::IComment>>.Fail($"コメント取得でエラーが発生しました。（詳細:{ex.Message}）");
            }

            return result;
        }


        #endregion

        #region private

        /// <summary>
        /// 非同期にコメントを取得
        /// </summary>
        /// <param name="dmcInfo"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        private async Task<IAttemptResult<IEnumerable<Core::IComment>>> DownloadCommentAsyncInternal(IDmcInfo dmcInfo, IDownloadSettings settings, DateTime getCommetUntil = default)
        {
            //コレクションを作成
            var collection = new Core::CommentCollection(dmcInfo.CommentCount, settings.CommentCountPerBlock);

            //リクエストビルダーをリセット
            var key = this._requestBuilder.ResetState();

            //変数定義
            var fetchedCommentCountOfDefaultThread = new List<int>();
            Core::IComment? firstComment = null;
            IThread? defaultThread = dmcInfo.CommentThreads.FirstOrDefault(t => t.IsDefaultPostTarget);

            async Task<IAttemptResult<IEnumerable<Core::IComment>>> Fetch(Fetch::ICommentFetchOption option)
            {
                //リクエストを構築
                IAttemptResult<string> rResult = await this._requestBuilder.BuildRequestAsync(dmcInfo, option, key);
                if (!rResult.IsSucceeded || rResult.Data is null)
                {
                    return AttemptResult<IEnumerable<Core::IComment>>.Fail(rResult.Message);
                }

                //コメントをダウンロード
                IAttemptResult<IReadOnlyList<Response::ResponseRoot>> dResult = await this._downloader.DownloadCommentAsync(dmcInfo.CommentServer, rResult.Data);

                if (!dResult.IsSucceeded || dResult.Data is null)
                {
                    return AttemptResult<IEnumerable<Core::IComment>>.Fail(dResult.Message);
                }

                //ダウンロードしたコメントを変換
                var converted = dResult.Data.Where(c => c.Chat is not null).Select(c => this._converter.ConvertNetCommentToCoreComment(c.Chat!));

                return AttemptResult<IEnumerable<Core::IComment>>.Succeeded(converted);
            }

            //デフォルトスレッドが存在しない場合はエラー
            if (defaultThread is null)
            {
                return AttemptResult<IEnumerable<Core::IComment>>.Fail("デフォルトスレッドを取得できませんでした。");
            }

            string defaultThreadID = defaultThread.ID.ToString();
            int defaultThreadFork = defaultThread.Fork;

            while (firstComment?.No is null or > 1)
            {
                //過去ログの起点を取得
                long when = firstComment is null ? 0 : firstComment.Date - 1;

                //コメント取得の起点に達した場合、ループを終了
                if (getCommetUntil != default && when < new DateTimeOffset(getCommetUntil).ToUnixTimeSeconds()) break;

                //リクエストのオプションを定義
                var option = new Fetch::CommentFetchOption(settings.DownloadOwner, settings.DownloadEasy, settings.DownloadLog, when);

                //コメントを取得
                IAttemptResult<IEnumerable<Core::IComment>> fResult = await Fetch(option);

                if (!fResult.IsSucceeded || fResult.Data is null)
                {
                    return fResult;
                }

                var converted = fResult.Data!;

                //取得したコメントのうち、デフォルトスレッドに投稿されたものの数を記録
                fetchedCommentCountOfDefaultThread.Add(this.GetDefaultThreadCommentCount(defaultThreadID, defaultThreadFork, converted));


                //コメントをコレクションに追加
                foreach (var c in converted) collection.Add(c);

                //過去ログをDLしない場合、ループを終了
                if (!settings.DownloadLog && getCommetUntil != default) break;

                //最初のコメントを取得
                IAttemptResult<Core::IComment> fiResult = collection.GetFirstComment(defaultThreadID, defaultThreadFork);
                if (!fiResult.IsSucceeded || fiResult.Data is null)
                {
                    return AttemptResult<IEnumerable<Core::IComment>>.Fail(fResult.Message);
                }
                firstComment = fiResult.Data;

            }

            //過去ログをダウンロードしない場合は終了
            if (!settings.DownloadLog && getCommetUntil != default)
            {
                return AttemptResult<IEnumerable<Core::IComment>>.Succeeded(collection.Comments);
            }

            //取得できなかったコメントを再取得
            var averageFetchCount = (int)Math.Floor(fetchedCommentCountOfDefaultThread.Average());
            var unfetchedRange = collection.GetUnFilledRange().OrderByDescending(r => r.Start.No).ToList();

            //取得情報を初期化
            this.SetupLastFetchedInfo(dmcInfo);


            while (true)
            {
                //取得できなかったコメントのうち、一番新しいものの情報（すでに取得したものは除外）
                var unfetched = collection.GetUnFilledRange().OrderByDescending(r => r.Start.No).FirstOrDefault(r => r.Start.No < this._lastFetchedInfo[r.Thread][r.Fork].Item1 - this._lastFetchedInfo[r.Thread][r.Fork].Item2);

                //取得できなかったコメントが存在しないので終了
                if (unfetched is null)
                {
                    return AttemptResult<IEnumerable<Core::IComment>>.Succeeded(collection.Comments);
                }

                //取得
                var option = new Fetch::CommentFetchOption(settings.DownloadOwner, settings.DownloadEasy, true, unfetched.Start.Date - 1);
                IAttemptResult<IEnumerable<Core::IComment>> fResult = await Fetch(option);

                if (!fResult.IsSucceeded || fResult.Data is null)
                {
                    return fResult;
                }

                //コメントを追加
                foreach (var c in fResult.Data!) collection.Add(c);

                //変数を更新
                this._lastFetchedInfo[unfetched.Thread][unfetched.Fork] = new(unfetched.Start.No - 1, this.GetDefaultThreadCommentCount(defaultThreadID, defaultThreadFork, fResult.Data));
            }



        }

        /// <summary>
        /// 与えられたコメントのうち、デフォルトスレッドに投稿された数を取得
        /// </summary>
        /// <param name="threadID"></param>
        /// <param name="fork"></param>
        /// <param name="comments"></param>
        /// <returns></returns>
        private int GetDefaultThreadCommentCount(string threadID, int fork, IEnumerable<Core::IComment> comments)
        {
            return comments.Where(c => c.Thread == threadID && c.Fork == fork).Count();
        }

        /// <summary>
        /// コメント取得情報を初期化
        /// </summary>
        /// <param name="dmcInfo"></param>
        private void SetupLastFetchedInfo(IDmcInfo dmcInfo)
        {
            if (dmcInfo.CommentThreads.Count == 0) return;

            //forkの最大値を求める
            var max = dmcInfo.CommentThreads.Select(t => t.Fork).Max();

            //リセット
            this._lastFetchedInfo.Clear();

            //threadを追加
            foreach (var threadID in dmcInfo.CommentThreads.Select(t => t.ID))
            {
                this._lastFetchedInfo.Add(threadID.ToString(), new (int, int)[max + 1]);
            }

        }

        #endregion
    }
}
