using System;
using System.Collections.Generic;
using System.Linq;
using Niconicome.Models.Domain.Local.Addons.Core.Permisson;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Helper.Result.Generic;
using V1 = Niconicome.Models.Domain.Local.Addons.Manifest.V1;

namespace Niconicome.Models.Domain.Local.Addons.Core.Installer
{
    public interface IManifestLoader
    {
        IAttemptResult<AddonInfomation> LoadManifest(string path);
    }

    public class ManifestLoader : IManifestLoader
    {
        public ManifestLoader(INicoFileIO fileIO, ILogger logger, IPermissionsHandler permissionsHandler)
        {
            this.fileIO = fileIO;
            this.logger = logger;
            this.permissionsHandler = permissionsHandler;
        }

        #region field

        private readonly INicoFileIO fileIO;

        private readonly ILogger logger;

        private readonly IPermissionsHandler permissionsHandler;

        #endregion

        /// <summary>
        /// マニフェストを読み込む
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IAttemptResult<AddonInfomation> LoadManifest(string path)
        {
            if (!this.fileIO.Exists(path))
            {
                return new AttemptResult<AddonInfomation>() { Message = $"指定されたマニフェストファイル({path})が存在しません。" };
            }

            string content;

            try
            {
                content = this.fileIO.OpenRead(path);
            }
            catch (Exception e)
            {
                this.logger.Error($"マニフェストファイルの読み込みに失敗しました。({path})", e);
                return new AttemptResult<AddonInfomation>() { Message = "マニフェストファイルの読み込みに失敗しました。", Exception = e };
            }

            V1::Manifest info;
            try
            {
                info = JsonParser.DeSerialize<V1::Manifest>(content);
            }
            catch (Exception e)
            {
                this.logger.Error($"マニフェストファイルの解析に失敗しました。({path})", e);
                return new AttemptResult<AddonInfomation>() { Message = "マニフェストファイルの解析に失敗しました。", Exception = e };
            }

            IAttemptResult manifestResult = this.CheckManifest(info);
            if (!manifestResult.IsSucceeded)
            {
                this.logger.Error($"不正なマニフェストファイルです。({path},{manifestResult.Message})");
                return new AttemptResult<AddonInfomation>() { Message = manifestResult.Message };
            }

            AddonInfomation addon = this.ConvertToAddonInfo(info);

            return new AttemptResult<AddonInfomation>() { IsSucceeded = true, Data = addon };
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
                if (!this.permissionsHandler.IsKnownPermission(permission))
                {
                    return new AttemptResult() { Message = $"不明な権限です。({permission})" };
                }
            }

            foreach (string host in manifest.HostPermissions)
            {
                if (!this.permissionsHandler.IsValidUrlPattern(host))
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
            var addon = new AddonInfomation();
            addon.Name.Value = manifest.Name;
            addon.Author.Value = manifest.Author;
            addon.Description.Value = manifest.Description;
            addon.Version.Value = Version.Parse(manifest.Version);
            addon.Identifier.Value = manifest.Identifier;
            addon.Permissions.AddRange(manifest.Permissions);
            addon.HostPermissions.AddRange(manifest.HostPermissions);
            addon.AutoUpdatePolicy = manifest.AutoUpdatePolicy;
            addon.Scripts = manifest.Scripts;

            if (manifest.Icons.ContainsKey("32"))
            {
                addon.IconPathRelative.Value=manifest.Icons["32"];
            } else
            {
                addon.IconPathRelative.Value = manifest.Icons.Select(i => new KeyValuePair<int, string>(int.Parse(i.Key), i.Value)).OrderByDescending(i => i.Key).FirstOrDefault().Value;
            }

            addon.TargetAPIVersion.Value = Version.Parse(manifest.TargetAPIVersion);

            return addon;
        }

        #endregion
    }
}
