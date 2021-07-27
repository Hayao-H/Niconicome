using System;
using System.IO;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.Addons.Core.Engine.Context;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.Addons.Core.Installer
{
    public interface IAddonUninstaller
    {
        /// <summary>
        /// アドオンをアンインストールする
        /// </summary>
        /// <param name="id">アドオンのID</param>
        /// <returns></returns>
        IAttemptResult Uninstall(int id);
    }

    public class AddonUninstaller : IAddonUninstaller
    {
        public AddonUninstaller(IAddonStoreHandler storeHandler, INicoDirectoryIO directoryIO, IAddonInfomationsContainer container, ILogger logger,IAddonContexts contexts)
        {
            this.storeHandler = storeHandler;
            this.directoryIO = directoryIO;
            this.container = container;
            this.logger = logger;
            this.contexts = contexts;
        }

        #region field

        private readonly IAddonStoreHandler storeHandler;

        private readonly INicoDirectoryIO directoryIO;

        private readonly IAddonInfomationsContainer container;

        private readonly ILogger logger;

        private readonly IAddonContexts contexts;

        #endregion

        public IAttemptResult Uninstall(int id)
        {
            if (!this.storeHandler.IsInstallled(addon => addon.Id == id))
            {
                return new AttemptResult() { Message = $"アドオン(id:{id})はインストールされていません。" };
            }

            AddonInfomation addon = this.container.GetAddon(id);
            string addonDir = Path.Combine(FileFolder.AddonsFolder, addon.PackageID.Value);

            try
            {
                this.contexts.Kill(id);
            } catch (Exception e)
            {
                this.logger.Error($"アドオンコンテクストの破棄に失敗しました。(name:{addon.Name.Value})", e);
                return new AttemptResult() { Message = "アドオンコンテクストの破棄に失敗しました。", Exception = e };
            }

            IAttemptResult storeResult = this.storeHandler.Delete(addon => addon.Id == id);

            if (!storeResult.IsSucceeded)
            {
                return storeResult;
            }

            try
            {
                this.directoryIO.Delete(addonDir);
            }
            catch (Exception ex)
            {
                this.logger.Error("アドオンディレクトリの削除に失敗しました。", ex);
                return new AttemptResult() { Message = "アドオンフォルダーの削除に失敗しました。", Exception = ex };
            }

            this.container.Remove(id);

            return new AttemptResult() { IsSucceeded = true };
        }
    }
}
