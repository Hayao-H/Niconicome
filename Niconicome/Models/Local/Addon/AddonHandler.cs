using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.Addons.Core;
using Niconicome.Models.Domain.Local.Addons.Core.Engine;
using Niconicome.Models.Domain.Local.Addons.Core.Installer;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Helper.Result.Generic;
using Reactive.Bindings;

namespace Niconicome.Models.Local.Addon
{
    public interface IAddonHandler
    {
        /// <summary>
        /// /アドオンの一覧
        /// </summary>
        ObservableCollection<AddonInfomation> Addons { get; }

        /// <summary>
        /// 初期化する
        /// </summary>
        /// <returns></returns>
        Task<IAttemptResult> InitializeAsync();

        /// <summary>
        /// アドオンを読み込む
        /// </summary>
        /// <param name="path"></param>
        /// <returns>アドオン情報,解凍先パス</returns>
        IAttemptResult<(AddonInfomation, string)> LoadAddon(string path);

        ReactiveProperty<bool> IsInstalling { get; }

    }

    public class AddonHandler : IAddonHandler
    {

        public AddonHandler(IAddonInfomationsContainer container, INicoDirectoryIO directoryIO, ILogger logger, IAddonEngine engine, IAddonInstaller installer)
        {
            this.container = container;
            this.directoryIO = directoryIO;
            this.logger = logger;
            this.engine = engine;
            this.installer = installer;
        }

        #region field

        private readonly IAddonInfomationsContainer container;

        private readonly IAddonEngine engine;

        private readonly INicoDirectoryIO directoryIO;

        private readonly ILogger logger;

        private readonly IAddonInstaller installer;

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

            List<string> packages;
            try
            {
                packages = this.directoryIO.GetFiles(FileFolder.AddonsFolder);
            }
            catch (Exception e)
            {
                this.logger.Error("アドオンパッケージ一覧の取得に失敗しました。", e);
                return new AttemptResult() { Message = "アドオンパッケージ一覧の取得に失敗しました。", Exception = e };
            }

            foreach (var package in packages)
            {
                IAttemptResult result = await this.engine.InitializeAsync(package);
                if (!result.IsSucceeded)
                {
                    return result;
                }
            }

            this.isInitialized = true;
            return new AttemptResult() { IsSucceeded = true };
        }

        public IAttemptResult<(AddonInfomation, string)> LoadAddon(string path)
        {
            //解凍
            IAttemptResult<string> extractResult = this.installer.Extract(path);
            if (!extractResult.IsSucceeded || extractResult.Data is null)
            {
                return new AttemptResult<(AddonInfomation, string)>() { Message = extractResult.Message, Exception = extractResult.Exception };
            }

            IAttemptResult<AddonInfomation> mResult = this.installer.LoadManifest(extractResult.Data);
            if (!mResult.IsSucceeded || mResult.Data is null)
            {
                return new AttemptResult<(AddonInfomation, string)>() { Message = mResult.Message, Exception = mResult.Exception };
            }

            return new AttemptResult<(AddonInfomation, string)>() { Data = (mResult.Data, extractResult.Data), IsSucceeded = true };

        }


        #endregion

        #region Props

        /// <summary>
        /// アドオン
        /// </summary>
        public ObservableCollection<AddonInfomation> Addons => this.container.Addons;

        /// <summary>
        /// インストールフラグ
        /// </summary>
        public ReactiveProperty<bool> IsInstalling { get; init; } = new();


        #endregion
    }
}
