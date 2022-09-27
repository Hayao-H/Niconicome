using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.Infomation;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Const = Niconicome.Models.Const;

namespace Niconicome.Models.Domain.Local.Addons.Core.V2.Install
{
    public interface IAddonInstaller
    {
        /// <summary>
        /// アドオンの情報を読み込む
        /// </summary>
        /// <param name="archiveFilePath"></param>
        /// <returns></returns>
        IAttemptResult<IAddonInfomation> LoadInfomation(string archiveFilePath);

        /// <summary>
        /// アドオンファイルを展開する
        /// </summary>
        /// <param name="achiveFilePath"></param>
        /// <returns></returns>
        IAttemptResult<InstallInfomation> Install(string achiveFilePath);

        /// <summary>
        /// 指定したディレクトリにアドオンファイルを展開する
        /// </summary>
        /// <param name="archiveFilePath"></param>
        /// <param name="targetDirectory"></param>
        /// <returns></returns>
        IAttemptResult<InstallInfomation> InstallToSpecifiedDiectory(string archiveFilePath, string targetDirectory);
    }

    public class AddonInstaller : IAddonInstaller
    {
        public AddonInstaller(IAddonExtractor extractor, IManifestLoader manifestLoader, ILogger logger, INicoFileIO fileIO, INicoDirectoryIO directoryIO)
        {
            this._extractor = extractor;
            this._manifestLoader = manifestLoader;
            this._logger = logger;
            this._fileIO = fileIO;
            this._directoryIO = directoryIO;
        }

        #region field

        private readonly IAddonExtractor _extractor;

        private readonly IManifestLoader _manifestLoader;

        private readonly ILogger _logger;

        private readonly INicoFileIO _fileIO;

        private readonly INicoDirectoryIO _directoryIO;

        #endregion

        #region Method

        public IAttemptResult<IAddonInfomation> LoadInfomation(string archiveFilePath)
        {

            IAttemptResult<IAddonInfomation> loadResult = this._manifestLoader.LoadManifestFromArchive(archiveFilePath, "");

            return loadResult;

        }

        public IAttemptResult<InstallInfomation> Install(string archiveFilePath)
        {
            string targetDir = Path.Combine(AppContext.BaseDirectory, FileFolder.AddonsFolder, Guid.NewGuid().ToString("D"));

            return this.InstallToSpecifiedDiectory(archiveFilePath, targetDir);
        }

        public IAttemptResult<InstallInfomation> InstallToSpecifiedDiectory(string archiveFilePath, string targetDirectory)
        {
            IAttemptResult extractResult = this._extractor.Extract(archiveFilePath, targetDirectory);
            if (!extractResult.IsSucceeded)
            {
                return AttemptResult<InstallInfomation>.Fail(extractResult.Message);
            }

            string manifestPath = Path.Combine(targetDirectory, Const::FileFolder.ManifestFileName);

            //一応マニフェストが存在するか確かめる
            if (!this._fileIO.Exists(manifestPath))
            {
                return AttemptResult<InstallInfomation>.Fail("アドオンファイル内にマニフェストファイルが存在しません。");
            }

            return AttemptResult<InstallInfomation>.Succeeded(new InstallInfomation(manifestPath, targetDirectory));
        }

        #endregion

    }
}
