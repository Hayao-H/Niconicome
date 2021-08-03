using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Niconicome.Extensions.System;
using Niconicome.Extensions.System.List;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Helper.Result.Generic;
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
    }

    public class AddonInstaller : IAddonInstaller
    {
        public AddonInstaller(ILogger logger, IManifestLoader manifestLoader, INicoFileIO fileIO, INicoDirectoryIO directoryIO, IAddonStoreHandler storeHandler, IAddonInfomationsContainer container)
        {
            this.logger = logger;
            this.manifestLoader = manifestLoader;
            this.fileIO = fileIO;
            this.directoryIO = directoryIO;
            this.storeHandler = storeHandler;
            this.container = container;
        }

        #region field

        private readonly ILogger logger;

        private readonly IManifestLoader manifestLoader;

        private readonly INicoFileIO fileIO;

        private readonly INicoDirectoryIO directoryIO;

        private readonly IAddonStoreHandler storeHandler;

        private readonly IAddonInfomationsContainer container;

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

            string iconFileName = Path.GetFileName(updateInfomation?.IconPathRelative.Value) ?? string.Empty;
            string excludePattern = !iconFileName.IsNullOrEmpty() ? $".*{iconFileName}" : "";

            //移動
            IAttemptResult mvResult = this.MoveAddon(tempPath, Path.Combine(FileFolder.AddonsFolder, packageID), excludePattern);
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
        private IAttemptResult MoveAddon(string sourcePath, string targetPath, string excludePattern)
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

            try
            {
                this.directoryIO.MoveAllFiles(sourcePath, targetPath, excludePattern);
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

        #endregion
    }
}
