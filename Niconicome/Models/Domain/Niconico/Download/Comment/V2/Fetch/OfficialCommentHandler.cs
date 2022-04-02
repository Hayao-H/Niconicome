using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Network;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Niconico.Download.Comment.V2.Fetch
{
    public interface IOfficialCommentHandler
    {
        /// <summary>
        /// スレッド番号からWayBackKeyを非同期で取得する
        /// </summary>
        /// <param name="thread"></param>
        /// <returns></returns>
        Task<IAttemptResult<WayBackKey>> GetWayBackKeyAsync(string thread);
    }

    /// <summary>
    /// 公式動画の過去ログ取得に必要なキー情報
    /// </summary>
    /// <param name="WayBackKey"></param>
    public record WayBackKey(string Key);

    public class OfficialCommentHandler : IOfficialCommentHandler
    {
        public OfficialCommentHandler(INicoHttp http, ILogger logger, INetWorkHelper netHelper)
        {
            this._http = http;
            this._logger = logger;
            this._netHelper = netHelper;
        }

        #region field

        private readonly INicoHttp _http;

        private readonly ILogger _logger;

        private readonly INetWorkHelper _netHelper;

        #endregion

        #region Method

        public async Task<IAttemptResult<WayBackKey>> GetWayBackKeyAsync(string thread)
        {
            HttpResponseMessage res = await this._http.GetAsync(new Uri($"https://flapi.nicovideo.jp/api/getwaybackkey?thread={thread}"));

            if (!res.IsSuccessStatusCode)
            {
                this._logger.Error($"WaybackKey取得サーバーへのリクエストに失敗しました。（{this._netHelper.GetHttpStatusForLog(res)}）");
                return AttemptResult<WayBackKey>.Fail("WaybackKey取得サーバーへのリクエストに失敗しました。");
            }

            string content = await res.Content.ReadAsStringAsync();

            int index = content.IndexOf("=");
            if (index < 0 || index + 1 >= content.Length)
            {
                return AttemptResult<WayBackKey>.Fail("Waybackkeyサーバーから不正な文字列が返却されました。");
            }
            else
            {
                return AttemptResult<WayBackKey>.Succeeded(new WayBackKey(content[(index + 1)..]));
            }
        }

        #endregion
    }
}
