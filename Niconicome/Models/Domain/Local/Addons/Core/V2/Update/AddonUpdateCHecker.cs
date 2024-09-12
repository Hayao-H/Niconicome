using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.Infomation;
using Niconicome.Models.Domain.Network;
using Niconicome.Models.Domain.Niconico;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.Addons.Core.V2.Update
{
    public interface IAddonUpdateChecker
    {
        /// <summary>
        /// 更新を確認する
        /// </summary>
        /// <param name="infomation"></param>
        /// <returns></returns>
        Task<IAttemptResult<UpdateCheckInfomation>> CheckForUpdate(IAddonInfomation infomation, int retry = 0);
    }

    public class AddonUpdateChecker : IAddonUpdateChecker
    {
        public AddonUpdateChecker(INicoHttp http,ILogger logger,INetWorkHelper netWorkHelper)
        {
            this._http = http;
            this._logger = logger;
            this._netWorkHelper = netWorkHelper;
        }

        #region field

        private readonly INicoHttp _http;

        private readonly ILogger _logger;

        private readonly INetWorkHelper _netWorkHelper;

        #endregion

        #region Method
        public async Task<IAttemptResult<UpdateCheckInfomation>> CheckForUpdate(IAddonInfomation infomation, int retry = 0)
        {
            if (!Uri.IsWellFormedUriString(infomation.UpdateJsonURL, UriKind.Absolute))
            {
                return AttemptResult<UpdateCheckInfomation>.Fail("URLが不正です。");
            }

            HttpResponseMessage res;
            try
            {
                res = await this._http.GetAsync(new Uri(infomation.UpdateJsonURL));
            }
            catch
            {
                if (retry <= 3)
                {
                    return await this.CheckForUpdate(infomation, retry + 1);
                } else
                {
                    return AttemptResult<UpdateCheckInfomation>.Fail("アップデート情報の取得に失敗しました。");
                }
            }
            var content = "";

            if (res.IsSuccessStatusCode)
            {
                try
                {
                    content = await res.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    this._logger.Error("アップデート情報の解析に失敗しました。", ex);
                    return AttemptResult<UpdateCheckInfomation>.Fail($"アップデート情報の解析に失敗しました。(詳細：{ex.Message})");
                }
            }
            else
            {
                this._logger.Error($"アップデート情報ダウンロードに失敗しました。(詳細：{this._netWorkHelper.GetHttpStatusForLog(res)} URL：{infomation.UpdateJsonURL})");
                return AttemptResult<UpdateCheckInfomation>.Fail($"アップデート情報ダウンロードに失敗しました。");
            }

            UpdateJSON data;

            try
            {
                data = JsonParser.DeSerialize<UpdateJSON>(content);
            }
            catch (Exception ex)
            {
                this._logger.Error($"アップデート情報の解析に失敗しました。", ex);
                return AttemptResult<UpdateCheckInfomation>.Fail($"アップデート情報の解析に失敗しました。(詳細：{ex.Message})");
            }

            Version.TryParse(data.Version, out Version? updateVersion);
            if (updateVersion is null)
            {
                this._logger.Error($"バージョン文字列の解析に失敗しました。(content：{data.Version})");
                return AttemptResult<UpdateCheckInfomation>.Fail("バージョン文字列の解析に失敗しました。");
            }

            return AttemptResult<UpdateCheckInfomation>.Succeeded(new UpdateCheckInfomation(updateVersion.CompareTo(infomation.Version) > 0,infomation,updateVersion,data.Changelog));
        }

        #endregion
    }
}
