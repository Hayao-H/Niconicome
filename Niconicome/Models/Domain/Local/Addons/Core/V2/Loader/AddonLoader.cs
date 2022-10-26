using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Text;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.Context;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.Infomation;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Utils;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Const = Niconicome.Models.Const;

namespace Niconicome.Models.Domain.Local.Addons.Core.V2.Loader
{
    public interface IAddonLoader
    {
        /// <summary>
        /// アドオンをすべてロードして実行
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        IAttemptResult<LoadResult> LoadAddons(Func<IAddonInfomation, APIObjectConatainer> factory);

        /// <summary>
        /// アドオンを単体でロードして実行
        /// </summary>
        /// <param name="manifestPath"></param>
        /// <param name="directoryName"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        IAttemptResult<LoadResult> LoadAddon(string manifestPath, string directoryName, Func<IAddonInfomation, APIObjectConatainer> factory);

    }

    public class AddonLoader : IAddonLoader
    {
        public AddonLoader(INicoDirectoryIO directoryIO, INicoFileIO fileIO, ILogger logger, IManifestLoader manifestLoader, IAddonContextsContainer addonContextsContainer)
        {
            this._directoryIO = directoryIO;
            this._fileIO = fileIO;
            this._logger = logger;
            this._manifestLoader = manifestLoader;
            this._addonContextsContainer = addonContextsContainer;
        }

        #region field

        private readonly INicoDirectoryIO _directoryIO;

        private readonly INicoFileIO _fileIO;

        private readonly ILogger _logger;

        private readonly IManifestLoader _manifestLoader;

        private readonly IAddonContextsContainer _addonContextsContainer;

        #endregion

        #region Method

        public IAttemptResult<LoadResult> LoadAddons(Func<IAddonInfomation, APIObjectConatainer> factory)
        {
            IAttemptResult<List<ManifestInfo>> manifestResult = this.GetManifests();
            if (!manifestResult.IsSucceeded || manifestResult.Data is null)
            {
                return AttemptResult<LoadResult>.Fail(manifestResult.Message);
            }

            var succeeded = new List<IAddonInfomation>();
            var failed = new List<FailedResult>();

            foreach (var manifestPath in manifestResult.Data)
            {
                IAttemptResult<IAddonContext> loadResult = this.Load(manifestPath.ManifestPath, manifestPath.DirectorName, factory);
                if (!loadResult.IsSucceeded || loadResult.Data is null)
                {
                    var result = new FailedResult(manifestPath.DirectorName, loadResult.Message ?? "不明な理由によりアドオンの読み込みに失敗しました。");
                    failed.Add(result);
                }
                else
                {
                    this._addonContextsContainer.Add(loadResult.Data);
                    succeeded.Add(loadResult.Data.AddonInfomation!);
                }
            }

            return AttemptResult<LoadResult>.Succeeded(new LoadResult(succeeded, failed));

        }

        public IAttemptResult<LoadResult> LoadAddon(string manifestPath, string directoryName, Func<IAddonInfomation, APIObjectConatainer> factory)
        {
            IAttemptResult<IAddonContext> result = this.Load(manifestPath, directoryName, factory);

            if (!result.IsSucceeded || result.Data is null)
            {
                var failedResult = new FailedResult(directoryName, result.Message ?? "不明な理由によりアドオンの読み込みに失敗しました。");
                return AttemptResult<LoadResult>.Succeeded(new LoadResult(new List<IAddonInfomation>(), new List<FailedResult>() { failedResult }));
            }
            else
            {
                this._addonContextsContainer.Add(result.Data);
                return AttemptResult<LoadResult>.Succeeded(new LoadResult(new List<IAddonInfomation>() { result.Data.AddonInfomation! }, new List<FailedResult>()));
            }
        }



        #endregion
        #region private

        /// <summary>
        /// マニフェストの一覧を取得する
        /// </summary>
        /// <returns></returns>
        private IAttemptResult<List<ManifestInfo>> GetManifests()
        {
            IEnumerable<string> addonDirs;
            string baseDir = AppContext.BaseDirectory;
            var manifests = new List<ManifestInfo>();

            try
            {
                addonDirs = this._directoryIO.GetDirectorys(Const::FileFolder.AddonsFolder).Select(p => Path.GetFileName(p));
            }
            catch (Exception ex)
            {
                this._logger.Error("アドオンディレクトリの探索に失敗しました。", ex);
                return AttemptResult<List<ManifestInfo>>.Fail($"アドオンディレクトリの探索に失敗しました。(詳細：{ex.Message})");
            }


            foreach (var directory in addonDirs)
            {
                string manifestPath = Path.Combine(baseDir, Const::FileFolder.AddonsFolder, directory, Const::FileFolder.ManifestFileName);
                if (this._fileIO.Exists(manifestPath))
                {
                    manifests.Add(new ManifestInfo(manifestPath, directory));
                }
            }

            return AttemptResult<List<ManifestInfo>>.Succeeded(manifests);
        }

        /// <summary>
        /// アドオン情報を取得して実行する
        /// </summary>
        /// <param name="manifestPath"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public IAttemptResult<IAddonContext> Load(string manifestPath, string directoryName, Func<IAddonInfomation, APIObjectConatainer> factory)
        {
            IAttemptResult<IAddonInfomation> iResult = this._manifestLoader.LoadManifest(manifestPath, directoryName);
            if (!iResult.IsSucceeded || iResult.Data is null)
            {
                return AttemptResult<IAddonContext>.Fail($"マニフェストファイルの解析に失敗しました。（詳細：{iResult.Message}）");
            }

            //コンテクスト生成
            IAttemptResult<IAddonContext> cResult = this._addonContextsContainer.Create();
            if (!cResult.IsSucceeded || cResult.Data is null)
            {
                return AttemptResult<IAddonContext>.Fail("コンテクストの生成に失敗しました。");
            }

            IAddonContext context = cResult.Data;
            IAddonInfomation infomation = iResult.Data;

            APIObjectConatainer container = factory(infomation);

            IAttemptResult result = context.ExecuteAddon(infomation, container);

            if (!result.IsSucceeded)
            {
                return AttemptResult<IAddonContext>.Fail(result.Message);
            }
            else
            {
                return AttemptResult<IAddonContext>.Succeeded(context);
            }
        }

        #endregion
    }

    public record LoadResult(List<IAddonInfomation> Succeeded, List<FailedResult> Failed);

    public record FailedResult(string DirectoryName, string Message);

    public record ManifestInfo(string ManifestPath, string DirectorName);
}
