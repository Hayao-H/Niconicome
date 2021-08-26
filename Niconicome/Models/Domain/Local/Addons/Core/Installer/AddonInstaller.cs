using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using Niconicome.Extensions.System;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Local.Store.Types;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Helper.Result.Generic;
using Reactive.Bindings.ObjectExtensions;
using Const = Niconicome.Models.Const;

namespace Niconicome.Models.Domain.Local.Addons.Core.Installer
{
    public interface IAddonInstaller
    {
        /// <summary>
        /// アドオンをインストールする
        /// </summary>
        /// <param name="tmpPath">一時フォルダーのパス</param>
        /// <param name="updateInfomation">アップデートの場合、引数でデータを与える</param>
        /// <returns></returns>
        IAttemptResult<AddonInfomation> Install(string tmpPath, AddonInfomation? updateInfomation = null);

        /// <summary>
        /// アドオンを一時フォルダーに解凍する
        /// </summary>
        /// <param name="path">アドオンパス</param>
        /// <returns></returns>
        IAttemptResult<string> Extract(string path);

        /// <summary>
        /// マニフェストを読み込む
        /// </summary>
        /// <param name="tmpPath">一時フォルダーのパス</param>
        /// <returns></returns>
        IAttemptResult<AddonInfomation> LoadManifest(string tmpPath);

        /// <summary>
        /// インストール時に作成された一時ファイルを書き換える
        /// </summary>
        /// <returns></returns>
        IAttemptResult ReplaceTemporaryFiles();
    }

    public class AddonInstaller : IAddonInstaller
    {
        public AddonInstaller(ILogger logger, IManifestLoader manifestLoader, INicoFileIO fileIO, INicoDirectoryIO directoryIO, IAddonStoreHandler storeHandler, IAddonInfomationsContainer container, IAddonUninstaller uninstaller)
        {
            this.logger = logger;
            this.manifestLoader = manifestLoader;
            this.fileIO = fileIO;
            this.directoryIO = directoryIO;
            this.storeHandler = storeHandler;
            this.container = container;
            this.uninstaller = uninstaller;
        }

        #region field

        private readonly ILogger logger;

        private readonly IManifestLoader manifestLoader;

        private readonly INicoFileIO fileIO;

        private readonly INicoDirectoryIO directoryIO;

        private readonly IAddonStoreHandler storeHandler;

        private readonly IAddonInfomationsContainer container;

        private readonly IAddonUninstaller uninstaller;

        #endregion

        /// <summary>
        /// インストールする
        /// </summary>
        /// <param name="tempPath"></param>
        /// <returns></returns>
        public IAttemptResult<AddonInfomation> Install(string tempPath, AddonInfomation? updateInfomation = null)
        {
            if (!this.directoryIO.Exists(tempPath))
            {
                return new AttemptResult<AddonInfomation>() { Message = "指定した一時フォルダーは存在しません。" };
            }

            string packageID = updateInfomation?.PackageID.Value ?? Path.GetFileName(tempPath);

            //マニフェスト読み込み
            IAttemptResult<AddonInfomation> mResult = this.LoadManifest(tempPath);
            if (!mResult.IsSucceeded || mResult.Data is null)
            {
                return new AttemptResult<AddonInfomation>() { Message = mResult.Message, Exception = mResult.Exception };
            }

            this.UninstallIfAble(mResult.Data);

            //DBに保存
            IAttemptResult<int> sResult;
            mResult.Data.PackageID.Value = packageID;
            AddonInfomation addon;
            if (updateInfomation is not null)
            {
                mResult.Data.ID.Value = updateInfomation.ID.Value;
                mResult.Data.Identifier.Value = updateInfomation.Identifier.Value;
                sResult = this.storeHandler.Update(mResult.Data);
            }
            else
            {
                sResult = this.storeHandler.StoreAddon(mResult.Data);

            }

            if (!sResult.IsSucceeded)
            {
                return new AttemptResult<AddonInfomation>() { Message = sResult.Message, Exception = sResult.Exception };
            }
            else
            {
                mResult.Data.ID.Value = sResult.Data;
                addon = this.container.GetAddon(sResult.Data);
                addon.SetData(mResult.Data);
            }

            string? iconFileName = Path.GetFileName(updateInfomation?.IconPathRelative.Value);

            //移動
            IAttemptResult mvResult = this.MoveAddon(tempPath, Path.Combine(FileFolder.AddonsFolder, packageID), iconFileName);
            if (!mvResult.IsSucceeded)
            {
                return new AttemptResult<AddonInfomation>() { Message = mvResult.Message, Exception = mvResult.Exception };
            }

            //一時フォルダーを削除
            IAttemptResult dResult = this.DeleteTempFolder(tempPath);
            if (!dResult.IsSucceeded)
            {
                return new AttemptResult<AddonInfomation>() { Message = dResult.Message, Exception = dResult.Exception };
            }

            return new AttemptResult<AddonInfomation>() { IsSucceeded = true, Data = addon };

        }

        /// <summary>
        /// 解凍する
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IAttemptResult<string> Extract(string path)
        {

            if (!this.fileIO.Exists(path))
            {
                return new AttemptResult<string>() { Message = "指定したアドオンファイルは存在しません。" };
            }

            string packageID = Guid.NewGuid().ToString("D");
            string targetPath = Path.Combine("tmp", packageID);

            //解凍
            IAttemptResult exResult = this.ExtractAddon(path, targetPath);
            return new AttemptResult<string>() { Data = targetPath, IsSucceeded = exResult.IsSucceeded, Message = exResult.Message, Exception = exResult.Exception };

        }

        /// <summary>
        /// マニフェストを読み込む
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IAttemptResult<AddonInfomation> LoadManifest(string path)
        {
            string manifestPath = Path.Combine(path, "manifest.json");

            if (!this.fileIO.Exists(manifestPath))
            {
                return new AttemptResult<AddonInfomation>() { Message = "指定したアドオンファイルは存在しません。" };
            }

            //マニフェスト読み込み
            return this.manifestLoader.LoadManifest(manifestPath);
        }

        public IAttemptResult ReplaceTemporaryFiles()
        {
            List<string> files;
            try
            {
                files = this.directoryIO.GetFiles(FileFolder.AddonsFolder, "*.tmp", true);
            }
            catch (Exception e)
            {
                this.logger.Error("一時ファイルの一覧を取得できませんでした。", e);
                return new AttemptResult<string>() { Message = "一時ファイルの一覧を取得できませんでした。", Exception = e };
            }

            foreach (string file in files)
            {
                if (!file.EndsWith(".tmp")) continue;

                string newPath = Regex.Replace(file, @"\.tmp$", "");

                try
                {
                    this.fileIO.Move(file, newPath, true);
                }
                catch (Exception e)
                {
                    this.logger.Error($"一時ファイルの移動に失敗しました。(旧:{file}, 新:{newPath})", e);
                    return new AttemptResult<string>() { Message = $"一時ファイルの移動に失敗しました。(旧:{file}, 新:{newPath})", Exception = e };
                }
            }

            return new AttemptResult<string>() { IsSucceeded = true };
        }

        #region private

        /// <summary>
        /// アドオンを解凍する
        /// </summary>
        /// <param name="path"></param>
        /// <param name="targetPath"></param>
        /// <returns></returns>
        private IAttemptResult ExtractAddon(string path, string targetPath)
        {

            if (!this.directoryIO.Exists(targetPath))
            {
                try
                {
                    this.directoryIO.Create(targetPath);
                }
                catch (Exception e)
                {
                    this.logger.Error("アドオン解凍先ディレクトリの作成に失敗しました。", e);
                    return new AttemptResult() { Message = "アドオン解凍先ディレクトリの作成に失敗しました。", Exception = e };
                }
            }

            try
            {
                ZipFile.ExtractToDirectory(path, targetPath);
            }
            catch (Exception e)
            {
                this.logger.Error("アドオンの展開に失敗しました。", e);
                return new AttemptResult() { Message = "アドオンの展開に失敗しました。", Exception = e };
            }

            return new AttemptResult() { IsSucceeded = true };
        }


        /// <summary>
        /// アドオンを移動する
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="targetPath"></param>
        /// <returns></returns>
        private IAttemptResult MoveAddon(string sourcePath, string targetPath, string? iconFileName)
        {
            if (!this.directoryIO.Exists(targetPath))
            {
                try
                {
                    this.directoryIO.Create(targetPath);
                }
                catch (Exception e)
                {
                    this.logger.Error("アドオンの移動先ディレクトリの作成に失敗しました。", e);
                    return new AttemptResult() { Message = "アドオンの移動先ディレクトリの作成に失敗しました。", Exception = e };
                }
            }

            bool isIconUsed = iconFileName is not null;

            try
            {
                this.directoryIO.MoveAllFiles(sourcePath, targetPath, p =>
                {
                    if (!isIconUsed) return p;
                    return Regex.IsMatch(p, $"^.*{iconFileName}$") ? p + ".tmp" : p;
                });
            }
            catch (Exception e)
            {
                this.logger.Error("アドオンの移動に失敗しました。", e);
                return new AttemptResult() { Message = "アドオンの移動に失敗しました。", Exception = e };
            }

            return new AttemptResult() { IsSucceeded = true };
        }

        private IAttemptResult DeleteTempFolder(string tempPath)
        {
            try
            {
                this.directoryIO.Delete(tempPath);
            }
            catch (Exception e)
            {
                this.logger.Error("アドオン一時フォルダーの削除に失敗しました。", e);
                return new AttemptResult() { Exception = e, Message = "アドオン一時フォルダーの削除に失敗しました。" };
            }

            return new AttemptResult() { IsSucceeded = true };
        }

        /// <summary>
        /// インストール済みで、実体のないアドオンをアンインストールする
        /// </summary>
        /// <returns></returns>
        private void UninstallIfAble(AddonInfomation addon)
        {
            var isIdNull = string.IsNullOrEmpty(addon.Identifier.Value);

            if (!this.storeHandler.IsInstallled(db => isIdNull ? db.Name == addon.Name.Value : db.Identifier == addon.Identifier.Value))
            {
                return;
            }

            AddonInfomation? info = this.storeHandler.GetAddon(db
                => isIdNull ? db.Name == addon.Name.Value : db.Identifier == addon.Identifier.Value);
            if (info is null)
            {
                return;
            }
            else
            {
                this.uninstaller.Uninstall(info.ID.Value);
            }
        }

        #endregion
    }
}
