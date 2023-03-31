using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.Infomation;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Permisson;
using Niconicome.Models.Domain.Network;
using Niconicome.Models.Domain.Niconico;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.Addons.Core.V2.Update
{
    public interface IAddonUpdator
    {
        /// <summary>
        /// アップデートをダウンロードして情報を取得
        /// </summary>
        /// <param name="infomation"></param>
        /// <returns></returns>
        Task<IAttemptResult<UpdateInfomation>> DownloadAndLoadUpdate(IAddonInfomation infomation);

        /// <summary>
        /// アドオンインストールファイルをダウンロードする
        /// ・主に必須アドオンの自動インストールを想定
        /// </summary>
        /// <param name="updateJSON"></param>
        /// <returns></returns>
        Task<IAttemptResult<string>> DownloadAddonAsync(string updateJSON);
    }

    public class AddonUpdator : IAddonUpdator
    {
        public AddonUpdator(INicoHttp http, ILogger logger, INetWorkHelper netWorkHelper, IManifestLoader loader, IPermissionsHandler permissionsHandler)
        {
            this._http = http;
            this._logger = logger;
            this._netWorkHelper = netWorkHelper;
            this._loader = loader;
            this._permissionsHandler = permissionsHandler;
        }

        #region field

        private readonly INicoHttp _http;

        private readonly ILogger _logger;

        private readonly INetWorkHelper _netWorkHelper;

        private readonly IManifestLoader _loader;

        private readonly IPermissionsHandler _permissionsHandler;

        #endregion

        #region Method

        public async Task<IAttemptResult<UpdateInfomation>> DownloadAndLoadUpdate(IAddonInfomation infomation)
        {
            IAttemptResult<UpdateJSON> infoResult = await this.DownloadInfomation(infomation.UpdateJsonURL);
            if (!infoResult.IsSucceeded || infoResult.Data is null)
            {
                return AttemptResult<UpdateInfomation>.Fail(infoResult.Message);
            }

            IAttemptResult<string> dlResult = await this.DownloadUpdate(infoResult.Data.ApplicationFile);
            if (!dlResult.IsSucceeded || dlResult.Data is null)
            {
                return AttemptResult<UpdateInfomation>.Fail(dlResult.Message);
            }

            IAttemptResult<IAddonInfomation> loadResult = this._loader.LoadManifestFromArchive(dlResult.Data, "");
            if (!loadResult.IsSucceeded || loadResult.Data is null)
            {
                return AttemptResult<UpdateInfomation>.Fail(loadResult.Message);
            }

            IAddonInfomation newInfomation = loadResult.Data;
            List<Permission> newPermissions = this.GetNewPermissions(infomation.Permissions, newInfomation.Permissions);

            UpdateJSON json = infoResult.Data;

            return AttemptResult<UpdateInfomation>.Succeeded(new UpdateInfomation(json.Version, newPermissions, newPermissions.Count > 0, newInfomation, json.Changelog, dlResult.Data));
        }

        public async Task<IAttemptResult<string>> DownloadAddonAsync(string updateJSON)
        {
            IAttemptResult<UpdateJSON> infoResult = await this.DownloadInfomation(updateJSON);
            if (!infoResult.IsSucceeded || infoResult.Data is null)
            {
                return AttemptResult<string>.Fail(infoResult.Message);
            }

            IAttemptResult<string> dlResult = await this.DownloadUpdate(infoResult.Data.ApplicationFile);
            return dlResult;
        }


        #endregion

        #region private

        private List<Permission> GetNewPermissions(IReadOnlyList<Permission> oldPermissions, IReadOnlyList<Permission> newPermissions)
        {
            var result = new List<Permission>();

            foreach (var p in newPermissions)
            {
                if (oldPermissions.Contains(p)) continue;
                result.Add(p);
            }

            return result;
        }

        /// <summary>
        /// アップデート情報をダウンロード
        /// </summary>
        /// <param name="jsonURL"></param>
        /// <returns></returns>
        private async Task<IAttemptResult<UpdateJSON>> DownloadInfomation(string jsonURL)
        {
            var res = await this._http.GetAsync(new Uri(jsonURL));
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
                    return AttemptResult<UpdateJSON>.Fail($"アップデート情報の解析に失敗しました。(詳細：{ex.Message})");
                }
            }
            else
            {
                this._logger.Error($"アップデート情報ダウンロードに失敗しました。(詳細：{this._netWorkHelper.GetHttpStatusForLog(res)} URL：{jsonURL})");
                return AttemptResult<UpdateJSON>.Fail($"アップデート情報ダウンロードに失敗しました。");
            }

            UpdateJSON data;

            try
            {
                data = JsonParser.DeSerialize<UpdateJSON>(content);
            }
            catch (Exception ex)
            {
                this._logger.Error($"アップデート情報の解析に失敗しました。", ex);
                return AttemptResult<UpdateJSON>.Fail($"アップデート情報の解析に失敗しました。(詳細：{ex.Message})");
            }

            return AttemptResult<UpdateJSON>.Succeeded(data);
        }

        /// <summary>
        /// アップデート本体をダウンロードする
        /// </summary>
        /// <param name="applicationURL"></param>
        /// <returns>保存先のパス</returns>
        private async Task<IAttemptResult<string>> DownloadUpdate(string applicationURL)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, applicationURL);
            var res = await this._http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            if (!res.IsSuccessStatusCode)
            {
                this._logger.Error($"更新ファイルのダウンロードに失敗しました。(詳細：{this._netWorkHelper.GetHttpStatusForLog(res)}　UR：{applicationURL})");
                return AttemptResult<string>.Fail("更新ファイルのダウンロードに失敗しました。");
            }

            string path = Path.Combine(AppContext.BaseDirectory, FileFolder.AddonsTmpDirectory, Guid.NewGuid().ToString("D") + FileFolder.DefaultAddonExtension);

            var info = new FileInfo(path);
            if (info.Directory is not null && !info.Directory.Exists)
            {
                info.Directory.Create();
            }

            try
            {
                using var stream = await res.Content.ReadAsStreamAsync();
                using var file = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                stream.CopyTo(file);
            }
            catch (Exception ex)
            {
                this._logger.Error("アドオンファイの書き込みに失敗しました。", ex);
                return AttemptResult<string>.Fail($"アドオンファイの書き込みに失敗しました。(詳細：{ex.Message})");
            }

            return AttemptResult<string>.Succeeded(path);
        }

        #endregion
    }
}
