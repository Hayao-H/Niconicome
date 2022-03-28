using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Network;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Response = Niconicome.Models.Domain.Niconico.Net.Json.API.Comment.V2.Response;

namespace Niconicome.Models.Domain.Niconico.Download.Comment.V2.Fetch
{
    public interface ICommentDownloader
    {
        /// <summary>
        /// 非同期にコメントをダウンロードする
        /// </summary>
        /// <param name="dmcInfo"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        Task<IAttemptResult<IReadOnlyList<Response::ResponseRoot>>> DownloadCommentAsync(string url, string request);
    }

    public class CommentDownloader : ICommentDownloader
    {
        public CommentDownloader(ILogger logger, INetWorkHelper helper, INicoHttp http)
        {
            this._http = http;
            this._helper = helper;
            this._logger = logger;
        }

        #region field

        private readonly INicoHttp _http;

        private readonly INetWorkHelper _helper;

        private readonly ILogger _logger;

        #endregion

        #region Method

        public async Task<IAttemptResult<IReadOnlyList<Response::ResponseRoot>>> DownloadCommentAsync(string url, string request)
        {
            HttpResponseMessage res = await this._http.PostAsync(new Uri(url), new StringContent(request));

            if (!res.IsSuccessStatusCode)
            {
                string status = this._helper.GetHttpStatusForLog(res);
                this._logger.Error($"コメントサーバーへのリクエストに失敗しました。（{status}）");
                return AttemptResult<IReadOnlyList<Response::ResponseRoot>>.Fail($"コメントサーバーへのリクエストに失敗しました。（{ status}）");
            }

            IReadOnlyList<Response::ResponseRoot> data;

            try
            {
                string content = await res.Content.ReadAsStringAsync();
                data = JsonParser.DeSerialize<IReadOnlyList<Response::ResponseRoot>>(content);
            }
            catch (Exception ex)
            {
                this._logger.Error("コメントの解析に失敗しました。", ex);
                return AttemptResult<IReadOnlyList<Response::ResponseRoot>>.Fail($"コメントの解析に失敗しました。（詳細:{ex.Message}）");
            }

            return AttemptResult<IReadOnlyList<Response::ResponseRoot>>.Succeeded(data);
        }


        #endregion
    }
}
