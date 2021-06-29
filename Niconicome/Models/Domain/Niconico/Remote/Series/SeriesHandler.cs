using System;
using System.Net.Http;
using System.Threading.Tasks;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Network;
using Niconicome.Models.Helper.Result.Generic;

namespace Niconicome.Models.Domain.Niconico.Remote.Series
{
    public interface ISeriesHandler
    {
        Task<IAttemptResult<RemotePlaylistInfo>> GetSeries(string id);
    }

    public class SeriesHandler : ISeriesHandler
    {
        public SeriesHandler(INicoHttp http, INetWorkHelper netWorkHelper, ISeriesPageHtmlParser seriesPageHtmlParser)
        {
            this.http = http;
            this.netWorkHelper = netWorkHelper;
            this.seriesPageHtmlParser = seriesPageHtmlParser;
        }

        #region field

        private readonly INicoHttp http;

        private readonly INetWorkHelper netWorkHelper;

        private readonly ISeriesPageHtmlParser seriesPageHtmlParser;

        #endregion

        /// <summary>
        /// シリーズを取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IAttemptResult<RemotePlaylistInfo>> GetSeries(string id)
        {
            IAttemptResult<string> netresult;

            try
            {
                netresult = await this.GetSeriesPage(id);
            }
            catch (Exception e)
            {
                return new AttemptResult<RemotePlaylistInfo>() { Message = $"シリーズページの取得に失敗しました。(詳細:{e.Message})" };
            }

            if (!netresult.IsSucceeded || netresult.Data is null)
            {
                return new AttemptResult<RemotePlaylistInfo>() { Message = netresult.Message, Exception = netresult.Exception };
            }

            IAttemptResult<RemotePlaylistInfo> infoResult;

            try
            {
                infoResult = this.seriesPageHtmlParser.GetSeriesInfo(netresult.Data);
            }
            catch (Exception e)
            {
                return new AttemptResult<RemotePlaylistInfo>() { Message = $"シリーズページの解析に失敗しました。(詳細:{e.Message})" };
            }

            return infoResult;
        }

        /// <summary>
        /// シリーズページを取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<IAttemptResult<string>> GetSeriesPage(string id)
        {
            var url = new Uri(API.SeriesUrl + id);

            HttpResponseMessage response = await this.http.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return new AttemptResult<string>() { Message = $"シリーズページの取得に失敗しました。({this.netWorkHelper.GetHttpStatusForLog(response)})" };
            }

            string content = await response.Content.ReadAsStringAsync();

            return new AttemptResult<string>() { IsSucceeded = true, Data = content };
        }
    }
}
