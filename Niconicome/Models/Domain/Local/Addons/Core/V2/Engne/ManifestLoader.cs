using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Niconicome.Extensions;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Permisson;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using V1 = Niconicome.Models.Domain.Local.Addons.Manifest.V1;

namespace Niconicome.Models.Domain.Local.Addons.Core.V2.Engne
{
    public interface IManifestLoader
    {
        /// <summary>
        /// マニフェストファイルを読み込む
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        IAttemptResult<IAddonInfomation> LoadManifest(string path);
    }

    public class ManifestLoader : IManifestLoader
    {
        public ManifestLoader(INicoFileIO fileIO, ILogger logger, IPermissionsHandler permissionsHandler)
        {
            this._fileIO = fileIO;
            this._logger = logger;
            this._permissionsHandler = permissionsHandler;
        }

        #region field

        private readonly INicoFileIO _fileIO;

        private readonly ILogger _logger;

        private readonly IPermissionsHandler _permissionsHandler;

        #endregion

        public IAttemptResult<IAddonInfomation> LoadManifest(string path)
        {
            if (!this._fileIO.Exists(path))
            {
                return AttemptResult<IAddonInfomation>.Fail($"指定されたマニフェストファイル({path})が存在しません。");
            }

            string content;

            try
            {
                content = this._fileIO.OpenRead(path);
            }
            catch (Exception e)
            {
                this._logger.Error($"マニフェストファイルの読み込みに失敗しました。({path})", e);
                return AttemptResult<IAddonInfomation>.Fail("マニフェストファイルの読み込みに失敗しました。", e);
            }

            V1::Manifest info;
            try
            {
                info = JsonParser.DeSerialize<V1::Manifest>(content);
            }
            catch (Exception e)
            {
                this._logger.Error($"マニフェストファイルの解析に失敗しました。({path})", e);
                return AttemptResult<IAddonInfomation>.Fail("マニフェストファイルの解析に失敗しました。", e);
            }

            IAttemptResult manifestResult = this.CheckManifest(info);
            if (!manifestResult.IsSucceeded)
            {
                this._logger.Error($"不正なマニフェストファイルです。({path},{manifestResult.Message})");
                return AttemptResult<IAddonInfomation>.Fail(manifestResult.Message);
            }

            AddonInfomation addon = this.ConvertToAddonInfo(info);

            return AttemptResult<IAddonInfomation>.Succeeded(addon);
        }

        #region private

        /// <summary>
        /// マニフェストをチェックする
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns></returns>
        private IAttemptResult CheckManifest(V1::Manifest manifest)
        {
            if (string.IsNullOrEmpty(manifest.Name))
            {
                return new AttemptResult() { Message = "アドオン名が空白です。" };
            }
            else if (string.IsNullOrEmpty(manifest.Author))
            {
                return new AttemptResult() { Message = "アドオン作者名が空白です。" };
            }
            else if (string.IsNullOrEmpty(manifest.Version))
            {
                return new AttemptResult() { Message = "アドオンバージョンが空白です。" };
            }
            else if (string.IsNullOrEmpty(manifest.ManifestVersion))
            {
                return new AttemptResult() { Message = "マニフェストバージョンが空白です。" };
            }

            if (manifest.ManifestVersion != "1.0")
            {
                return new AttemptResult() { Message = $"対応していないマニフェストバージョンです。({manifest.ManifestVersion})" };
            }

            bool result = Version.TryParse(manifest.Version, out Version _);
            if (!result)
            {
                return new AttemptResult() { Message = $"バージョンの形式が不正です。({manifest.Version})" };
            }

            bool aResult = Version.TryParse(manifest.TargetAPIVersion, out Version _);
            if (!aResult)
            {
                return new AttemptResult() { Message = $"APIバージョンの形式が不正です。({manifest.TargetAPIVersion})" };
            }

            foreach (string permission in manifest.Permissions)
            {
                if (!this._permissionsHandler.IsKnownPermission(permission))
                {
                    return new AttemptResult() { Message = $"不明な権限です。({permission})" };
                }
            }

            foreach (string host in manifest.HostPermissions)
            {
                if (!this._permissionsHandler.IsValidUrlPattern(host))
                {
                    return new AttemptResult() { Message = $"不正なホストです。({host})" };
                }
            }

            return new AttemptResult() { IsSucceeded = true };
        }

        /// <summary>
        /// ローカル情報に変換する
        /// </summary>
        /// <param name="manifest"></param>
        /// <returns></returns>
        private AddonInfomation ConvertToAddonInfo(V1::Manifest manifest)
        {

            bool vResult = Version.TryParse(manifest.Version, out Version? version);
            bool avResult = Version.TryParse(manifest.TargetAPIVersion, out Version? apiVerison);

            string GetIconPath()
            {
                if (manifest.Icons.ContainsKey("32"))
                {
                    return manifest.Icons["32"];
                }
                else
                {
                    return manifest.Icons.Select(i => new KeyValuePair<int, string>(int.Parse(i.Key), i.Value)).OrderByDescending(i => i.Key).FirstOrDefault().Value;
                }
            }

            var addon = new AddonInfomation()
            {
                Name = manifest.Name,
                Author = manifest.Author,
                Description = manifest.Description,
                Identifier = manifest.Identifier,
                Permissions = manifest.Permissions.Select(p => this._permissionsHandler.GetPermission(p)).Where(p => p is not null).ToList().As<List<Permission>>().AsReadOnly(),
                HostPermissions = manifest.HostPermissions.AsReadOnly(),
                AutoUpdate = manifest.AutoUpdatePolicy.AutoUpdate,
                UpdateJsonURL = manifest.AutoUpdatePolicy.UpdateJsonUrl,
                ScriptPath = manifest.Scripts.BackgroundScript,
                Version = vResult && version is not null ? version : new Version(),
                IconPath = GetIconPath(),
                TargetAPIVersion = avResult && apiVerison is not null ? apiVerison : new Version(),
            };

            return addon;
        }

        #endregion
    }
}
