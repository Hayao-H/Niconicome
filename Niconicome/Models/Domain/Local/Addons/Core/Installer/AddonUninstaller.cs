using System;
using System.IO;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.Addons.Core.Engine.Context;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.ViewModels.Mainpage;
using Const = Niconicome.Models.Const;

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

        /// <summary>
        /// 削除予定のアドオンを削除する
        /// </summary>
        /// <returns></returns>
        IAttemptResult DeleteListed();
    }

    public class AddonUninstaller : IAddonUninstaller
    {
        public AddonUninstaller(IAddonStoreHandler storeHandler, INicoDirectoryIO directoryIO, IAddonInfomationsContainer container, ILogger logger, IAddonContexts contexts, INicoFileIO fileIO)
        {
            this.storeHandler = storeHandler;
            this.directoryIO = directoryIO;
            this.container = container;
            this.logger = logger;
            this.contexts = contexts;
            this.fileIO = fileIO;
        }

        #region field

        private readonly IAddonStoreHandler storeHandler;

        private readonly INicoDirectoryIO directoryIO;

        private readonly IAddonInfomationsContainer container;

        private readonly ILogger logger;

        private readonly IAddonContexts contexts;

        private readonly INicoFileIO fileIO;

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
            }
            catch (Exception e)
            {
                this.logger.Error($"アドオンコンテクストの破棄に失敗しました。(name:{addon.Name.Value})", e);
                return new AttemptResult() { Message = "アドオンコンテクストの破棄に失敗しました。", Exception = e };
            }

            IAttemptResult storeResult = this.storeHandler.Delete(addon => addon.Id == id);

            if (!storeResult.IsSucceeded)
            {
                return storeResult;
            }

            string newName = Path.Combine(FileFolder.AddonsFolder, $"{addon.PackageID}-deleted");

            try
            {
                this.fileIO.AppendText(Path.Combine(FileFolder.AddonsFolder, Const::Adddon.UninstalledAddonsFile), addon.PackageID.Value);
            }
            catch (Exception ex)
            {
                this.logger.Error("アドオンディレクトリの削除予定処理に失敗しました。", ex);
                return new AttemptResult() { Message = "アドオンフォルダーの削除予定処理に失敗しました。", Exception = ex };
            }

            this.container.Remove(id);

            return new AttemptResult() { IsSucceeded = true };
        }

        public IAttemptResult DeleteListed()
        {
            string delAddonsList = Path.Combine(FileFolder.AddonsFolder, Const::Adddon.UninstalledAddonsFile);
            if (!this.fileIO.Exists(delAddonsList))
            {
                return new AttemptResult() { IsSucceeded = true };
            }

            string content;
            try
            {
                content = this.fileIO.OpenRead(delAddonsList);
            }
            catch (Exception e)
            {
                this.logger.Error("削除予定アドオンリストの読み込みに失敗しました。", e);
                return new AttemptResult() { Message = "削除予定アドオンリストの読み込みに失敗しました。", Exception = e };
            }

            foreach (string item in content.Split(Environment.NewLine))
            {
                string targetPath = Path.Combine(FileFolder.AddonsFolder, item);
                if (this.directoryIO.Exists(targetPath))
                {

                    try
                    {
                        this.directoryIO.Delete(targetPath);
                    }
                    catch (Exception e)
                    {
                        this.logger.Error("アドオンの削除に失敗しました。", e);
                        return new AttemptResult() { Message = "アドオンの削除に失敗しました。", Exception = e };
                    }
                }
            }

            try
            {
                this.fileIO.Delete(delAddonsList);
            }
            catch (Exception e)
            {
                this.logger.Error("削除予定アドオンリストの削除に失敗しました。", e);
                return new AttemptResult() { Message = "削除予定アドオンリストの削除に失敗しました。", Exception = e };
            }

            return new AttemptResult() { IsSucceeded = true };
        }

    }
}
