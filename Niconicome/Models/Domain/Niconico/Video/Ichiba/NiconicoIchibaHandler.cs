using System;
using System.Threading.Tasks;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Network;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Helper.Result;
using V3 = Niconicome.Models.Domain.Niconico.Net.Json.CrossOrigin.V3;

namespace Niconicome.Models.Domain.Niconico.Video.Ichiba
{
    public interface INiconicoIchibaHandler
    {
        Task<IAttemptResult<INiconicoIchibaInfo>> GetIchibaInfo(string id);
    }

    public class NiconicoIchibaHandler : INiconicoIchibaHandler
    {
        public NiconicoIchibaHandler(INicoHttp http, INetWorkHelper netWorkHelper, IIchibaHtmlParser ichibaHtmlParser)
        {
            this.http = http;
            this.netWorkHelper = netWorkHelper;
            this.ichibaHtmlParser = ichibaHtmlParser;
        }

        #region DI
        private readonly INicoHttp http;

        private readonly INetWorkHelper netWorkHelper;

        private readonly IIchibaHtmlParser ichibaHtmlParser;
        #endregion

        /// <summary>
        /// ニコニコ市場APIにアクセスる
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IAttemptResult<INiconicoIchibaInfo>> GetIchibaInfo(string id)
        {
            var res = await this.http.GetAsync(new Uri(APIConstant.IchibaAPIV3 + id));

            if (!res.IsSuccessStatusCode)
            {
                var message = this.netWorkHelper.GetHttpStatusForLog(res);
                return new AttemptResult<INiconicoIchibaInfo>() { Message = $"市場PAIへのアクセスに失敗しました。(詳細:{message})" };
            }

            V3::IchibaEmbedApiResponse ichiba;
            try
            {
                ichiba = JsonParser.DeSerialize<V3::IchibaEmbedApiResponse>(await res.Content.ReadAsStringAsync());
            }
            catch (Exception e)
            {
                return new AttemptResult<INiconicoIchibaInfo>() { Message = $"市場PAIからのレスポンスの解析に失敗しました。(詳細:{e.Message})", Exception = e };
            }

            var result = this.ichibaHtmlParser.ParseHtml(ichiba.Main);

            return result;

        }
    }
}
