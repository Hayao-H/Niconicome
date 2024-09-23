using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Domain.Niconico.Net.Json.API.Comment.V2;
using Niconicome.Models.Domain.Niconico.Video.Infomations;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Request = Niconicome.Models.Domain.Niconico.Net.Json.API.Comment.V3.Request;

namespace Niconicome.Models.Domain.Niconico.Download.Comment.V2.Fetch.V3
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
        IAttemptResult<string> BuildRequest(IDmcInfo dmcInfo, ICommentFetchOption option, string key);

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

        public IAttemptResult<string> BuildRequest(IDmcInfo dmcInfo, ICommentFetchOption option, string key)
        {
            IAttemptResult<Request::RequestRoot> result;

            try
            {
                result = this.BuildRequestAsyncInternal(dmcInfo, option, key);
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
        private IAttemptResult<Request::RequestRoot> BuildRequestAsyncInternal(IDmcInfo dmcInfo, ICommentFetchOption option, string key)
        {

            if (!this.CheckKey(key))
            {
                return AttemptResult<Request::RequestRoot>.Fail("Keyが違います。");
            }

            var request = new Request::RequestRoot();
            request.Params.Language = dmcInfo.CommentLanguage;
            request.ThreadKey = option.ThreadKey;

            if (option.DownloadLog)
            {
                request.Additionals = new Request::Additionals()
                {
                    When = option.When,
                };
            }

            //ループ
            foreach (ITarget target in dmcInfo.CommentTargets)
            {
                if (!string.IsNullOrEmpty(option.TargetFork) && target.Fork != option.TargetFork)
                {
                    continue;
                }

                //かんたんコメント判定
                if (!option.DownloadEasy && target.Fork == "easy")
                {
                    continue;
                }

                //投コメ判定
                if (!option.DownloadOwner && target.Fork == "owner")
                {
                    continue;
                }


                //Thread要求を挿入
                request.Params.Targets.Add(new Request::Target() { Id = target.Id, Fork = target.Fork });
            }

            //リクエスト終了Ping要求を挿入

            this.requestIndex++;

            return AttemptResult<Request::RequestRoot>.Succeeded(request);
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

        #endregion

        #region Test

        public IAttemptResult<Request::RequestRoot> BuildRequestInternalForTest(IDmcInfo dmcInfo, ICommentFetchOption option, string key)
        {
            return this.BuildRequestAsyncInternal(dmcInfo, option, key);
        }

        public bool CheckKeyForTest(string key)
        {
            return this.CheckKey(key);
        }

        #endregion
    }

}
