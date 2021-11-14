using System;
using System.IO;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.Core.Engine.Context;
using Niconicome.Models.Domain.Local.Addons.Core.Installer;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Const = Niconicome.Models.Const;

namespace Niconicome.Models.Domain.Local.Addons.Core.Engine
{
    public interface IAddonEngine
    {
        /// <summary>
        /// 非同期に初期化する
        /// </summary>
        /// <param name="packageID"></param>
        /// <param name="isDevMode"></param>
        /// <returns>Dataプロパティーにはインストール状態</returns>
        Task<IAttemptResult<bool>> InitializeAsync(string packageID, bool isDevMode);
    }

    public class AddonEngine : IAddonEngine
    {
        public AddonEngine(IManifestLoader manifestLoader, IAddonStoreHandler storeHandler, ILogger logger, IAddonInfomationsContainer container, IAddonContexts contexts)
        {
            this.manifestLoader = manifestLoader;
            this.storeHandler = storeHandler;
            this.logger = logger;
            this.contexts = contexts;
            this.container = container;
        }

        #region field

        private readonly IManifestLoader manifestLoader;

        private readonly IAddonStoreHandler storeHandler;

        private readonly ILogger logger;

        private readonly IAddonInfomationsContainer container;

        private readonly IAddonContexts contexts;

        #endregion

        /// <summary>
        /// アドオンを初期化して実行する
        /// </summary>
        /// <param name="packageID"></param>
        /// <returns></returns>
        public async Task<IAttemptResult<bool>> InitializeAsync(string packageID, bool isDevMode)
        {
            if (!this.storeHandler.IsInstallled(addon => addon.PackageID == packageID))
            {
                return new AttemptResult<bool>() { Data = false, Message = "インストールされていないアドオンです。アドオンのサイドロードは出来ません。" };
            }

            string manifestPath = Path.Combine(Const::FileFolder.AddonsFolder, packageID, "manifest.json");

            IAttemptResult<AddonInfomation> mResult = this.manifestLoader.LoadManifest(manifestPath);
            if (!mResult.IsSucceeded || mResult.Data is null)
            {
                return new AttemptResult<bool>() { Data = true, Message = "マニフェストファイルの読み込みに失敗しました。", Exception = mResult.Exception };
            }


            AddonInfomation dbData = this.storeHandler.GetAddon(data => data.PackageID == packageID)!;

            if (!isDevMode)
            {
                //アドオンを検証
                IAttemptResult checkResult = this.CheckSafety(dbData, mResult.Data);
                if (!checkResult.IsSucceeded)
                {
                    this.container.Remove(dbData.ID.Value);
                    return new AttemptResult<bool>() { Data = true, Message = checkResult.Message, Exception = checkResult.Exception };
                }
            }

            AddonInfomation addon = this.container.GetAddon(dbData.ID.Value);

            //DBのIDを対応させる
            mResult.Data.PackageID.Value = packageID;
            mResult.Data.ID.Value = dbData.ID.Value;
            addon.SetData(mResult.Data);

            //コンテクストを登録
            IAddonContext context = AddonContext.CreateInstance();
            this.contexts.Contexts.Add(addon.ID.Value, context);

            await Task.Delay(1);

            return new AttemptResult<bool>() { Data = true, IsSucceeded = true };

        }

        #region private

        /// <summary>
        /// アドオンの安全性を検証する
        /// </summary>
        /// <param name="dbData"></param>
        /// <param name="manifestData"></param>
        /// <returns></returns>
        private IAttemptResult CheckSafety(AddonInfomation dbData, AddonInfomation manifestData)
        {
            ///権限確認
            if (manifestData.Permissions.Count != dbData.Permissions.Count)
            {
                this.logger.Error($"権限の不正な書き換えを検知しました。({dbData.Name.Value},{dbData.Permissions.Count}=>{manifestData.Permissions.Count})");
                return new AttemptResult() { Message = "権限が不正に書き換えられました。" };
            }
            else
            {
                foreach (var permission in manifestData.Permissions)
                {
                    if (!dbData.Permissions.Contains(permission))
                    {
                        this.logger.Error($"権限の不正な書き換えを検知しました。({dbData.Name.Value},{permission})");
                        return new AttemptResult() { Message = $"権限が不正に書き換えられました。{permission}" };
                    }
                }
            }

            if (manifestData.HostPermissions.Count != dbData.HostPermissions.Count)
            {
                this.logger.Error($"ホスト権限の不正な書き換えを検知しました。({dbData.Name.Value},{dbData.HostPermissions.Count}=>{manifestData.HostPermissions.Count})");
                return new AttemptResult() { Message = "ホスト権限が不正に書き換えられました。" };
            }
            else
            {

                foreach (var permission in manifestData.HostPermissions)
                {
                    if (!dbData.HostPermissions.Contains(permission))
                    {
                        this.logger.Error($"ホスト権限の不正な書き換えを検知しました。({dbData.Name.Value},{permission})");
                        return new AttemptResult() { Message = $"ホスト権限が不正に書き換えられました。{permission}" };
                    }
                }
            }

            //バージョン確認
            if (manifestData.Version.Value.CompareTo(dbData.Version.Value) != 0)
            {
                this.logger.Error($"バージョンが不正に書き換えられました。({dbData.Version.Value}=>{manifestData.Version.Value})");
                return new AttemptResult() { Message = $"バージョンが不正に書き換えられました。({dbData.Version.Value}=>{manifestData.Version.Value})" };
            }

            //APIバージョン確認
            Version apiVersioin = Const::AddonConstant.APIVersion;
            if (manifestData.TargetAPIVersion.Value.CompareTo(apiVersioin) > 0)
            {
                this.logger.Error($"現在の実行環境はターゲットAPIバージョンを満たしていません。(current:{apiVersioin} target:{manifestData.TargetAPIVersion.Value})");
                return new AttemptResult() { Message = $"現在の実行環境はターゲットAPIバージョンを満たしていません。(current:{apiVersioin} target:{manifestData.TargetAPIVersion.Value})" };
            }

            return new AttemptResult() { IsSucceeded = true };
        }

        #endregion
    }
}
