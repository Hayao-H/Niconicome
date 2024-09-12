using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Local.Addons.API.Hooks;
using Niconicome.Models.Domain.Niconico.Net.Html;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Domain.Niconico.Video.Infomations;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using DmcRequest = Niconicome.Models.Domain.Niconico.Net.Json.WatchPage.DMC.Request;
using WatchJson = Niconicome.Models.Domain.Niconico.Net.Json.WatchPage.V2;

namespace Niconicome.Models.Domain.Niconico.Watch
{
    public interface IWatchInfohandler
    {
        Task<IAttemptResult<IDomainVideoInfo>> GetVideoInfoAsync(string id);
    }

    /// <summary>
    /// 動画情報を取得して解析する
    /// </summary>
    public class WatchInfohandler : IWatchInfohandler
    {
        public WatchInfohandler(INicoHttp http, ILogger logger, IHooksManager hooksManager)
        {
            this.http = http;
            this.logger = logger;
            this.hooksManager = hooksManager;
        }

        #region field

        private readonly INicoHttp http;

        private readonly ILogger logger;

        private readonly IHooksManager hooksManager;

        #endregion


        /// <summary>
        /// 動画情報を取得する
        /// </summary>
        /// <returns></returns>
        public async Task<IAttemptResult<IDomainVideoInfo>> GetVideoInfoAsync(string id)
        {
            string source;
            Uri url = new Uri("https://nicovideo.jp");

            try
            {
                source = await this.http.GetStringAsync(url);
            }
            catch (Exception e)
            {
                this.logger.Error($"動画情報の取得に失敗しました(url: {url.AbsoluteUri})", e);
                return new AttemptResult<IDomainVideoInfo>() { Message = "httpリクエストに失敗しました。(サーバーエラー・IDの指定間違い)", Exception = e };
            }

            bool isRegistered = this.hooksManager.IsRegistered(HookType.WatchPageParser);
            if (!isRegistered)
            {
                this.logger.Error($"ページ解析プラグイン、またはそれに相当するアドオンがインストールされていないか、初期化が完了していません。");
                return new AttemptResult<IDomainVideoInfo>() { Message = "視聴ページの解析に失敗しました。(ページ解析プラグイン未インストール)" };
            }

            IAttemptResult<dynamic> result;

            result = this.hooksManager.ParseWatchPage(source);

            if (!result.IsSucceeded || result.Data is null)
            {

                if (result.Exception is null)
                {
                    this.logger.Error($"視聴ページの解析に失敗しました。(id:{id}, 詳細:{result.Message})");
                }
                else
                {
                    this.logger.Error($"視聴ページの解析に失敗しました。(id:{id}, 詳細:{result.Message})", result.Exception);
                }

                return new AttemptResult<IDomainVideoInfo>() { Message = "視聴ページの解析に失敗しました。(サーバーエラー・プラグインでエラー)" };
            }

            var info = new DomainVideoInfo()
            {
                RawDmcInfo = result.Data,
            };

            return new AttemptResult<IDomainVideoInfo>() { IsSucceeded = true, Data = info };
        }
    }


    /// <summary>
    /// オプション
    /// </summary>
    [Flags]
    public enum WatchInfoOptions
    {
        Default,
        NoDmcData,
    }
}
