using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.Addons.Core;
using Niconicome.Models.Domain.Local.Addons.Core.Engine;
using Niconicome.Models.Domain.Local.Addons.Core.Engine.Context;
using Niconicome.Models.Domain.Local.Addons.Core.Installer;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.Addon.API;
using Niconicome.Models.Local.Addon.API.Net.Http.Fetch;
using Niconicome.Models.Local.Addon.Result;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Network.Watch;
using Niconicome.Models.Utils.InitializeAwaiter;
using VM = Niconicome.ViewModels.Mainpage;

namespace Niconicome.Models.Local.Addon
{
    public interface IAddonHandler
    {
        /// <summary>
        /// /アドオンの一覧
        /// </summary>
        ObservableCollection<AddonInfomation> Addons { get; }

        /// <summary>
        /// 読み込みに失敗したアドオン
        /// </summary>
        ObservableCollection<FailedAddonResult> LoadFailedAddons { get; }

        /// <summary>
        /// 初期化する
        /// </summary>
        /// <returns></returns>
        Task<IAttemptResult> InitializeAsync();

    }

    public class AddonHandler : IAddonHandler
    {

        public AddonHandler(IAddonInfomationsContainer container, INicoDirectoryIO directoryIO, ILogger logger, IAddonEngine engine, ILocalSettingHandler settingHandler, IAddonContexts contexts, IAddonUninstaller uninstaller, IAddonInstaller installer, IAddonStoreHandler storeHandler,IInitializeAwaiterHandler initializeAwaiterHandler)
        {
            this._container = container;
            this._directoryIO = directoryIO;
            this._logger = logger;
            this._engine = engine;
            this._contexts = contexts;
            this._settingHandler = settingHandler;
            this._uninstaller = uninstaller;
            this._installer = installer;
            this._storeHandler = storeHandler;
            this._initializeAwaiterHandler = initializeAwaiterHandler;
        }

        #region field

        private readonly IAddonInfomationsContainer _container;

        private readonly IAddonContexts _contexts;

        private readonly IAddonEngine _engine;

        private readonly INicoDirectoryIO _directoryIO;

        private readonly ILogger _logger;

        private readonly ILocalSettingHandler _settingHandler;

        private readonly IAddonUninstaller _uninstaller;

        private readonly IAddonInstaller _installer;

        private readonly IAddonStoreHandler _storeHandler;

        private readonly IInitializeAwaiterHandler _initializeAwaiterHandler;

        private bool isInitialized;

        #endregion

        #region Methods

        /// <summary>
        /// アドオンを初期化する
        /// </summary>
        /// <returns></returns>
        public async Task<IAttemptResult> InitializeAsync()
        {
            if (this.isInitialized)
            {
                return new AttemptResult() { Message = "既に初期化されています。" };
            }

            this._initializeAwaiterHandler.RegisterStep(AwaiterNames.Addon, typeof(VM::MainWindowViewModel));
            await this._initializeAwaiterHandler.GetAwaiter(AwaiterNames.Addon);

            IAttemptResult dResult = this._uninstaller.DeleteListed();
            if (!dResult.IsSucceeded)
            {
                return new AttemptResult() { Message = "アンインストール済みアドオンフォルダーの削除に失敗しました。", Exception = dResult.Exception };
            }

            IAttemptResult mResult = this._installer.ReplaceTemporaryFiles();
            if (!mResult.IsSucceeded)
            {
                return new AttemptResult() { Message = mResult.Message, Exception = mResult.Exception };
            }

            List<string> packages;
            try
            {
                packages = this._directoryIO.GetDirectorys(FileFolder.AddonsFolder);
            }
            catch (Exception e)
            {
                this._logger.Error("アドオンパッケージ一覧の取得に失敗しました。", e);
                return new AttemptResult() { Message = "アドオンパッケージ一覧の取得に失敗しました。", Exception = e };
            }

            bool isDevMode = this._settingHandler.GetBoolSetting(SettingsEnum.IsDevMode);
            bool isAddonDebuggingEnable = this._settingHandler.GetBoolSetting(SettingsEnum.IsAddonDebugEnable);

            foreach (var packagePath in packages)
            {
                string package = Path.GetFileName(packagePath);
                IAttemptResult<bool> result = await this._engine.InitializeAsync(package, isDevMode);
                if (!result.IsSucceeded)
                {
                    var failedResult = new FailedAddonResult(package, result.Message ?? string.Empty, result.Data);
                    this.LoadFailedAddons.Add(failedResult);
                }
            }

            foreach (KeyValuePair<int, IAddonContext> item in this._contexts.Contexts)
            {
                AddonInfomation info = this._container.GetAddon(item.Key);
                IAPIEntryPoint entryPoint = DIFactory.Provider.GetRequiredService<IAPIEntryPoint>();

                IAttemptResult result = item.Value.Initialize(info, engine =>
                {

                    //entryPoint.Initialize(info, engine);
                    engine.AddHostObject("application", entryPoint);

                    IFetch fetch = DIFactory.Provider.GetRequiredService<IFetch>();
                    fetch.Initialize(info);
                    Func<string, dynamic?, Task<Response>> fetchFunc = (url, optionObj) =>
                    {
                        var option = new FetchOption()
                        {
                            method = optionObj?.method,
                            body = optionObj?.body,
                            credentials = optionObj?.credentials,
                        };

                        return fetch.FetchAsync(url, option);
                    };
                    engine.AddHostObject("fetch", fetchFunc);

                }, entryPoint, isAddonDebuggingEnable);

                if (!result.IsSucceeded)
                {
                    var failedResult = new FailedAddonResult(info.PackageID.Value, result.Message ?? string.Empty, true);
                    this.LoadFailedAddons.Add(failedResult);
                }
            }

            IAttemptResult<IEnumerable<string>> packageIds = this._storeHandler.GetAllAddonsPackageID();
            if (packageIds.IsSucceeded && packageIds.Data is not null)
            {
                List<string> loaded = this._contexts.Contexts.Select(v => v.Value.AddonInfomation!.PackageID.Value).ToList();
                foreach (var package in packageIds.Data)
                {
                    if (!loaded.Contains(package))
                    {
                        var failedResult = new FailedAddonResult(package, "インストールされていますが、ファイルが見つかりませんでした。", true);
                        this.LoadFailedAddons.Add(failedResult);
                    }
                }
            }

            this.isInitialized = true;
            return new AttemptResult() { IsSucceeded = true };
        }


        #endregion

        #region Props

        /// <summary>
        /// アドオン
        /// </summary>
        public ObservableCollection<AddonInfomation> Addons => this._container.Addons;

        public ObservableCollection<FailedAddonResult> LoadFailedAddons { get; init; } = new();

        #endregion

    }
}
