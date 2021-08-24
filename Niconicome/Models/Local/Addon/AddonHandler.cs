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
using Niconicome.Models.Helper.Result.Generic;
using Niconicome.Models.Local.Addon.API;
using Niconicome.Models.Local.Addon.API.Net.Http.Fetch;
using Niconicome.Models.Local.Addon.Result;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Network.Watch;

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

        public AddonHandler(IAddonInfomationsContainer container, INicoDirectoryIO directoryIO, ILogger logger, IAddonEngine engine, ILocalSettingHandler settingHandler, IAddonContexts contexts, IAddonUninstaller uninstaller, IAddonInstaller installer, IAddonStoreHandler storeHandler)
        {
            this.container = container;
            this.directoryIO = directoryIO;
            this.logger = logger;
            this.engine = engine;
            this.contexts = contexts;
            this.settingHandler = settingHandler;
            this.uninstaller = uninstaller;
            this.installer = installer;
            this.storeHandler = storeHandler;
        }

        #region field

        private readonly IAddonInfomationsContainer container;

        private readonly IAddonContexts contexts;

        private readonly IAddonEngine engine;

        private readonly INicoDirectoryIO directoryIO;

        private readonly ILogger logger;

        private readonly ILocalSettingHandler settingHandler;

        private readonly IAddonUninstaller uninstaller;

        private readonly IAddonInstaller installer;

        private readonly IAddonStoreHandler storeHandler;

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

            IAttemptResult dResult = this.uninstaller.DeleteListed();
            if (!dResult.IsSucceeded)
            {
                return new AttemptResult() { Message = "アンインストール済みアドオンフォルダーの削除に失敗しました。", Exception = dResult.Exception };
            }

            IAttemptResult mResult = this.installer.ReplaceTemporaryFiles();
            if (!mResult.IsSucceeded)
            {
                return new AttemptResult() { Message = mResult.Message, Exception = mResult.Exception };
            }

            List<string> packages;
            try
            {
                packages = this.directoryIO.GetDirectorys(FileFolder.AddonsFolder);
            }
            catch (Exception e)
            {
                this.logger.Error("アドオンパッケージ一覧の取得に失敗しました。", e);
                return new AttemptResult() { Message = "アドオンパッケージ一覧の取得に失敗しました。", Exception = e };
            }

            bool isDevMode = this.settingHandler.GetBoolSetting(SettingsEnum.IsDevMode);
            bool isAddonDebuggingEnable = this.settingHandler.GetBoolSetting(SettingsEnum.IsAddonDebugEnable);

            foreach (var packagePath in packages)
            {
                string package = Path.GetFileName(packagePath);
                IAttemptResult<bool> result = await this.engine.InitializeAsync(package, isDevMode);
                if (!result.IsSucceeded)
                {
                    var failedResult = new FailedAddonResult(package, result.Message ?? string.Empty, result.Data);
                    this.LoadFailedAddons.Add(failedResult);
                }
            }

            foreach (KeyValuePair<int, IAddonContext> item in this.contexts.Contexts)
            {
                AddonInfomation info = this.container.GetAddon(item.Key);
                IAttemptResult result = item.Value.Initialize(info, engine =>
                {
                    IAPIEntryPoint entryPoint = DIFactory.Provider.GetRequiredService<IAPIEntryPoint>();
                    entryPoint.Initialize(info, engine);
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

                }, isAddonDebuggingEnable);

                if (!result.IsSucceeded)
                {
                    var failedResult = new FailedAddonResult(info.PackageID.Value, result.Message ?? string.Empty, true);
                    this.LoadFailedAddons.Add(failedResult);
                }
            }

            IAttemptResult<IEnumerable<string>> packageIds = this.storeHandler.GetAllAddonsPackageID();
            if (packageIds.IsSucceeded && packageIds.Data is not null)
            {
                List<string> loaded = this.contexts.Contexts.Select(v => v.Value.AddonInfomation!.PackageID.Value).ToList();
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
        public ObservableCollection<AddonInfomation> Addons => this.container.Addons;

        public ObservableCollection<FailedAddonResult> LoadFailedAddons { get; init; } = new();

        #endregion

    }
}
