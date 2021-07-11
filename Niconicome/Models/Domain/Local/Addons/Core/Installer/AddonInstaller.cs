using System;
using System.IO;
using System.IO.Compression;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Helper.Result.Generic;

namespace Niconicome.Models.Domain.Local.Addons.Core.Installer
{
    public interface IAddonInstaller
    {
        IAttemptResult<AddonInfomation> Install(string path);
    }

    public class AddonInstaller : IAddonInstaller
    {
        public AddonInstaller(ILogger logger, IManifestLoader manifestLoader, INicoFileIO fileIO, INicoDirectoryIO directoryIO, IAddonStoreHandler storeHandler)
        {
            this.logger = logger;
            this.manifestLoader = manifestLoader;
            this.fileIO = fileIO;
            this.directoryIO = directoryIO;
            this.storeHandler = storeHandler;
        }

        #region field

        private readonly ILogger logger;

        private readonly IManifestLoader manifestLoader;

        private readonly INicoFileIO fileIO;

        private readonly INicoDirectoryIO directoryIO;

        private readonly IAddonStoreHandler storeHandler;

        #endregion

        /// <summary>
        /// インストールする
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IAttemptResult<AddonInfomation> Install(string path)
        {
            if (!this.fileIO.Exists(path))
            {
                return new AttemptResult<AddonInfomation>() { Message = "指定したアドオンファイルは存在しません。" };
            }

            string packageID = Guid.NewGuid().ToString("D");
            string targetPath = Path.Combine("tmp", packageID);
            string manifestPath = Path.Combine(targetPath, "manifest.json");


            //解凍
            IAttemptResult exResult = this.ExtractAddon(path, targetPath);
            if (!exResult.IsSucceeded)
            {
                return new AttemptResult<AddonInfomation>() { Message = exResult.Message, Exception = exResult.Exception };
            }

            //マニフェスト読み込み
            IAttemptResult<AddonInfomation> mResult = this.manifestLoader.LoadManifest(manifestPath);
            if (!mResult.IsSucceeded || mResult.Data is null)
            {
                return new AttemptResult<AddonInfomation>() { Message = mResult.Message, Exception = mResult.Exception };
            }

            //DBに保存
            IAttemptResult<int> sResult = this.storeHandler.StoreAddon(mResult.Data);
            if (!sResult.IsSucceeded)
            {
                return new AttemptResult<AddonInfomation>() { Message = sResult.Message, Exception = sResult.Exception };
            }

            //移動
            IAttemptResult mvResult = this.MoveAddon(targetPath, FileFolder.AddonsFolder);
            if (!mvResult.IsSucceeded)
            {
                return new AttemptResult<AddonInfomation>() { Message = mvResult.Message, Exception = mvResult.Exception };
            }

            //取得
            AddonInfomation? addon = this.storeHandler.GetAddon(d => d.Id == sResult.Data);
            if (addon is null)
            {
                return new AttemptResult<AddonInfomation>() { Message = "DBへの登録に失敗しました。" };
            }

            return new AttemptResult<AddonInfomation>() { IsSucceeded = true, Data = addon };

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
                    this.directoryIO.Create(path);
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
        private IAttemptResult MoveAddon(string sourcePath, string targetPath)
        {
            try
            {
                this.directoryIO.Move(sourcePath, targetPath);
            }
            catch (Exception e)
            {
                this.logger.Error("アドオンの移動に失敗しました。", e);
                return new AttemptResult() { Message = "アドオンの移動に失敗しました。", Exception = e };
            }

            return new AttemptResult() { IsSucceeded = true };
        }
        #endregion
    }
}
