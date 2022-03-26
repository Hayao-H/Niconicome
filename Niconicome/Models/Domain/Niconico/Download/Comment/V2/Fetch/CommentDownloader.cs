using System;
using System.Net.Http;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Network;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Response = Niconicome.Models.Domain.Niconico.Net.Json.API.Comment.Response;

namespace Niconicome.Models.Domain.Niconico.Download.Comment.V2.Fetch
{
    public interface ICommentDownloadder
    {
        /// <summary>
        /// 非同期にコメントをダウンロードする
        /// </summary>
        /// <param name="dmcInfo"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        Task<IAttemptResult<Response::Comment>> DownloadCommentAsync(string url, string request);
    }

    public class CommentDownloader : ICommentDownloadder
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

        public async Task<IAttemptResult<Response::Comment>> DownloadCommentAsync(string url, string request)
        {
            HttpResponseMessage res = await this._http.PostAsync(new Uri(url), new StringContent(request));

            if (!res.IsSuccessStatusCode)
            {
                string status = this._helper.GetHttpStatusForLog(res);
                this._logger.Error($"コメントサーバーへのリクエストに失敗しました。（{status}）");
                return AttemptResult<Response::Comment>.Fail($"コメントサーバーへのリクエストに失敗しました。（{ status}）");
            }

            Response::Comment data;

            try
            {
                string content = await res.Content.ReadAsStringAsync();
                data = JsonParser.DeSerialize<Response::Comment>(content);
            }
            catch (Exception ex)
            {
                this._logger.Error("コメントの解析に失敗しました。", ex);
                return AttemptResult<Response::Comment>.Fail($"コメントの解析に失敗しました。（詳細:{ex.Message}）");
            }

            return AttemptResult<Response::Comment>.Succeeded(data);
        }


        #endregion
    }
}
