using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Domain.Niconico.Net.Json.API.Comment.V2;
using Niconicome.Models.Domain.Niconico.Video.Infomations;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Request = Niconicome.Models.Domain.Niconico.Net.Json.API.Comment.V2.Request;

namespace Niconicome.Models.Domain.Niconico.Download.Comment.V2.Fetch
{
    public interface ICommentRequestBuilder
    {
        /// <summary>
        /// コメントリクエストを非同期に構築する
        /// </summary>
        /// <param name="dmcInfo"></param>
        /// <param name="option"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<IAttemptResult<string>> BuildRequestAsync(IDmcInfo dmcInfo, CommentFetchOption option, string key);

        /// <summary>
        /// 状態をリセットする
        /// </summary>
        /// <returns>リクエストビルダーとリクエスト元を１対１で結びつけるためのキー</returns>
        string ResetState();
    }

    public class CommentRequestBuilder : ICommentRequestBuilder
    {
        public CommentRequestBuilder(IOfficialCommentHandler officialCommentHandler, ILogger logger)
        {
            this._officialCommentHandler = officialCommentHandler;
            this._logger = logger;
        }

        #region field

        private readonly IOfficialCommentHandler _officialCommentHandler;

        private readonly ILogger _logger;

        private string secret = string.Empty;

        private int requestIndex = 0;

        private int commandIndex = 0;

        #endregion

        #region Method

        public async Task<IAttemptResult<string>> BuildRequestAsync(IDmcInfo dmcInfo, CommentFetchOption option, string key)
        {
            IAttemptResult<List<Request::RequestRoot>> result;

            try
            {
                result = await this.BuildRequestAsyncInternal(dmcInfo, option, key);
            }
            catch (Exception ex)
            {
                this._logger.Error("リクエストの構築中にエラーが発生しました。", ex);
                return AttemptResult<string>.Fail($"リクエストの構築中にエラーが発生しました。（詳細:{ex.Message}）");
            }

            if (!result.IsSucceeded || result.Data is null)
            {
                return AttemptResult<string>.Fail(result.Message);
            }

            string json;

            try
            {
                json = JsonParser.Serialize(result.Data);
            }
            catch (Exception ex)
            {

                this._logger.Error("リクエストのシリアライズ中にエラーが発生しました。", ex);
                return AttemptResult<string>.Fail($"リクエストのシリアライズ中にエラーが発生しました。（詳細:{ex.Message}）");
            }

            return AttemptResult<string>.Succeeded(json);

        }

        public string ResetState()
        {
            this.requestIndex = 0;
            this.commandIndex = 0;

            string key = Guid.NewGuid().ToString("D");
            this.secret = key;
            return key;
        }

        #endregion

        #region private

        /// <summary>
        /// 内部的にリクエストを構築
        /// </summary>
        /// <param name="dmcInfo"></param>
        /// <param name="option"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private async Task<IAttemptResult<List<Request::RequestRoot>>> BuildRequestAsyncInternal(IDmcInfo dmcInfo, ICommentFetchOption option, string key)
        {

            if (!this.CheckKey(key))
            {
                return AttemptResult<List<Request::RequestRoot>>.Fail("Keyが違います。");
            }

            var request = new List<Request::RequestRoot>();

            //リクエスト開始Ping要求を挿入
            request.Add(new Request::RequestRoot() { Ping = this.GetPingContent(PingType.StartRequest, this.requestIndex) });

            //ループ
            foreach (IThread thread in dmcInfo.CommentThreads)
            {

                //かんたんコメント判定
                if (!option.DownloadEasy && thread.IsEasyCommentPostTarget)
                {
                    continue;
                }

                //投コメ判定
                if (!option.DownloadOwner && thread.IsOwnerThread)
                {
                    continue;
                }

                //Thread要求を挿入
                IAttemptResult<Request::Thread> tResult = await this.GetThreadAsync(thread, dmcInfo, option);
                if (!tResult.IsSucceeded || tResult.Data is null)
                {
                    return AttemptResult<List<Request::RequestRoot>>.Fail(tResult.Message);
                }

                request.Add(new Request::RequestRoot() { Ping = this.GetPingContent(PingType.StartCommand, this.commandIndex) });
                request.Add(new Request::RequestRoot() { Thread = tResult.Data });
                request.Add(new Request::RequestRoot() { Ping = this.GetPingContent(PingType.StartCommand, this.commandIndex) });
                this.commandIndex++;

                //ThreadLeaves要求を挿入
                if (thread.IsLeafRequired)
                {
                    IAttemptResult<Request::ThreadLeaves> lResult = this.GetThreadLeaves(thread, dmcInfo, option, tResult.Data.WayBackKey);
                    if (!lResult.IsSucceeded || lResult.Data is null)
                    {
                        return AttemptResult<List<Request::RequestRoot>>.Fail(lResult.Message);
                    }

                    request.Add(new Request::RequestRoot() { Ping = this.GetPingContent(PingType.StartCommand, this.commandIndex) });
                    request.Add(new Request::RequestRoot() { ThreadLeaves = lResult.Data });
                    request.Add(new Request::RequestRoot() { Ping = this.GetPingContent(PingType.EndCommand, this.commandIndex) });

                    this.commandIndex++;
                }

            }

            //リクエスト終了Ping要求を挿入
            request.Add(new Request::RequestRoot() { Ping = this.GetPingContent(PingType.EndRequest, this.requestIndex) });

            this.requestIndex++;

            return AttemptResult<List<Request::RequestRoot>>.Succeeded(request);
        }

        /// <summary>
        /// Ping要求を取得する
        /// </summary>
        /// <param name="pingType"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private Request::Ping GetPingContent(PingType pingType, int index)
        {
            string content = pingType switch
            {
                PingType.StartRequest => $"rs:{index}",
                PingType.EndRequest => $"rf:{index}",
                PingType.StartCommand => $"ps:{index}",
                _ => $"pf:{index}",
            };

            return new Request::Ping() { Content = content };
        }

        /// <summary>
        /// Thread要求を構築する
        /// </summary>
        /// <param name="threadInfo"></param>
        /// <param name="dmcInfo"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        private async Task<IAttemptResult<Request::Thread>> GetThreadAsync(IThread threadInfo, IDmcInfo dmcInfo, ICommentFetchOption option)
        {
            var thread = new Request::Thread()
            {
                ThreadNo = threadInfo.ID.ToString(),
                UserID = dmcInfo.UserId,
                Fork = threadInfo.Fork,
                Language = 0,
                WithGlobal = 1,
                Scores = 1,
                Nicoru = 3,
                Force184 = threadInfo.Is184Forced ? "1" : null,
            };

            //投コメを判定
            if (threadInfo.IsOwnerThread)
            {
                thread.ResFrom = -1000;
                thread.Version = "20061206";
            }
            else
            {
                thread.Version = "20090904";
            }

            //Force184?
            if (threadInfo.Is184Forced)
            {
                thread.Force184 = "1";
            }


            //公式動画を判別
            if (threadInfo.IsThreadkeyRequired)
            {
                thread.ThreadKey = threadInfo.Threadkey;
            }
            else
            {
                thread.UserKey = dmcInfo.Userkey;
            }

            //過去ログ
            if (option.DownloadLog)
            {
                IAttemptResult<WayBackKey> wResult = await this._officialCommentHandler.GetWayBackKeyAsync(threadInfo.ID.ToString());

                if (!wResult.IsSucceeded || wResult.Data is null)
                {
                    return AttemptResult<Request::Thread>.Fail(wResult.Message);
                }

                thread.When = option.When;
                thread.WayBackKey = wResult.Data.Key;
            }

            return AttemptResult<Request::Thread>.Succeeded(thread);
        }

        /// <summary>
        /// ThreadLeaves要求を取得
        /// </summary>
        /// <param name="thread"></param>
        /// <param name="dmcInfo"></param>
        /// <param name="option"></param>
        /// <param name="wayBackKey"></param>
        /// <returns></returns>
        private IAttemptResult<Request::ThreadLeaves> GetThreadLeaves(IThread thread, IDmcInfo dmcInfo, ICommentFetchOption option, string? wayBackKey)
        {
            var leaves = new Request::ThreadLeaves()
            {
                ThreadNo = thread.ID.ToString(),
                UserID = dmcInfo.UserId,
                Fork = thread.Fork,
                Language = 0,
                Scores = 1,
                Nicoru = 3,
            };

            //force184?
            if (thread.Is184Forced)
            {
                leaves.Force184 = "1";
            }

            //公式動画を判別
            if (thread.IsThreadkeyRequired)
            {
                leaves.ThreadKey = thread.Threadkey;
            }
            else
            {
                leaves.UserKey = dmcInfo.Userkey;
            }

            //過去ログ
            if (option.DownloadLog)
            {
                leaves.When = option.When;
                leaves.WayBackKey = wayBackKey;
            }

            int flooredDuration;

            try
            {
                flooredDuration = (int)Math.Floor(dmcInfo.Duration / 60f);
            }
            catch (Exception ex)
            {
                this._logger.Error($"動画再生時間の計算に失敗しました。", ex);
                return AttemptResult<Request::ThreadLeaves>.Fail($"動画再生時間の計算に失敗しました。（詳細:{ex.Message}）");
            }

            leaves.Content = $"0-{flooredDuration}:100,1000:nicoru:100";

            return AttemptResult<Request::ThreadLeaves>.Succeeded(leaves);
        }

        /// <summary>
        /// キーの一致を確認する
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool CheckKey(string key)
        {
            return this.secret == key;
        }

        /// <summary>
        /// Ping要求の種類
        /// </summary>
        private enum PingType
        {
            StartRequest,
            EndRequest,
            StartCommand,
            EndCommand,
        }

        #endregion

        #region Test

        public async Task<IAttemptResult<List<Request::RequestRoot>>> BuildRequestAsyncInternalForTest(IDmcInfo dmcInfo, ICommentFetchOption option, string key)
        {
            return await this.BuildRequestAsyncInternal(dmcInfo, option, key);
        }

        public bool CheckKeyForTest(string key)
        {
            return this.CheckKey(key);
        }

        #endregion
    }

}
