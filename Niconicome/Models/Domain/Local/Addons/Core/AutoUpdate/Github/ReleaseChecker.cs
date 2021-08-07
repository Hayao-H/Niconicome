using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Network;
using Niconicome.Models.Domain.Niconico;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result.Generic;

namespace Niconicome.Models.Domain.Local.Addons.Core.AutoUpdate.Github
{
    public interface IReleaseChecker
    {
        IAttemptResult<string> GetAssetUrl(string namePattern, Release release);
        Task<IAttemptResult<Release>> GetTheLatestAsync(string owner, string repoName);
        IAttemptResult<Version> GetVersion(Release release);
    }

    public class ReleaseChecker : IReleaseChecker
    {
        public ReleaseChecker(INicoHttp http, INetWorkHelper netWorkHelper, ILogger logger)
        {
            this.http = http;
            this.netWorkHelper = netWorkHelper;
            this.logger = logger;
        }

        #region field

        private readonly INicoHttp http;

        private readonly INetWorkHelper netWorkHelper;

        private readonly ILogger logger;

        #endregion

        /// <summary>
        /// 最新バージョンを取得する
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="repoName"></param>
        /// <returns></returns>
        public async Task<IAttemptResult<Release>> GetTheLatestAsync(string owner, string repoName)
        {
            string url = this.GetUrl(owner, repoName);

            HttpResponseMessage res = await this.http.GetAsync(new Uri(url));

            if (!res.IsSuccessStatusCode)
            {
                string message = this.netWorkHelper.GetHttpStatusForLog(res);
                this.logger.Error($"Release APIへのアクセスに失敗しました。(詳細:{message})");
                return new AttemptResult<Release>() { Message = $"Release APIへのアクセスに失敗しました。(詳細:{message})" };
            }

            var data = new List<Release>();
            string content = await res.Content.ReadAsStringAsync();

            try
            {
                data = JsonParser.DeSerialize<List<Release>>(content);
            }
            catch (Exception e)
            {

                this.logger.Error($"リリース情報の解析に失敗しました。", e);
                return new AttemptResult<Release>() { Message = $"リリース情報の解析に失敗しました。", Exception = e };
            }

            if (data.Count == 0)
            {
                return new AttemptResult<Release>() { Message = $"リリース存在しません。" };
            }

            Release latest = data.First();

            return new AttemptResult<Release>() { IsSucceeded = true, Data = latest };
        }

        /// <summary>
        /// バージョンを取得する
        /// </summary>
        /// <param name="release"></param>
        /// <returns></returns>
        public IAttemptResult<Version> GetVersion(Release release)
        {
            if (!Regex.IsMatch(release.Name, @"^v\d\.\d(\.\d)?$"))
            {
                return new AttemptResult<Version>() { Message = $"バージョンの形式が不正です。({release.Name})" };
            }
            else
            {
                Version version;
                try
                {
                    version = Version.Parse(release.Name[1..]);
                }
                catch (Exception e)
                {
                    this.logger.Error("バージョンの解析に失敗しました。", e);
                    return new AttemptResult<Version>() { Message = $"バージョンの解析に失敗しました。", Exception = e };
                }

                return new AttemptResult<Version>() { IsSucceeded = true, Data = version };

            }
        }

        /// <summary>
        /// URLを取得する
        /// </summary>
        /// <param name="namePattern"></param>
        /// <param name="release"></param>
        /// <returns></returns>
        public IAttemptResult<string> GetAssetUrl(string namePattern, Release release)
        {
            Asset? first = release.Assets.FirstOrDefault(a => Regex.IsMatch(a.Name, namePattern));

            if (first is null)
            {
                return new AttemptResult<string>() { Message = "条件に一致するファイルを発見できませんでした。" };
            }
            else
            {
                return new AttemptResult<string>() { IsSucceeded = true, Data = first.Url };
            }
        }

        #region private

        /// <summary>
        /// APIのURLを取得
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="repoName"></param>
        /// <returns></returns>
        private string GetUrl(string owner, string repoName)
        {
            return $"https://api.github.com/repos/{owner}/{repoName}/releases";
        }

        #endregion
    }
}
