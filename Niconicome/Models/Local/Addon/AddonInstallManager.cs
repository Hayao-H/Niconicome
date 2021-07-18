using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.Addons.Core;
using Niconicome.Models.Domain.Local.Addons.Core.Engine;
using Niconicome.Models.Domain.Local.Addons.Core.Installer;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Helper.Result.Generic;
using Reactive.Bindings;
using Reactive.Bindings.ObjectExtensions;

namespace Niconicome.Models.Local.Addon
{
    public interface IAddonInstallManager
    {

        /// <summary>
        /// アドオンを読み込む
        /// </summary>
        /// <param name="path"></param>
        /// <returns>アドオン情報,解凍先パス</returns>
        IAttemptResult LoadAddon(string path);

        /// <summary>
        /// アドオンをインストールする
        /// </summary>
        /// <returns></returns>
        IAttemptResult InstallAddon();

        /// <summary>
        /// インストールフラグ
        /// </summary>
        ReactiveProperty<bool> IsInstalling { get; }

        /// <summary>
        /// 解凍済みフラグ
        /// </summary>
        ReactiveProperty<bool> IsLoaded { get; }

        /// <summary>
        /// 読み込んだアドオン情報
        /// </summary>
        ReactiveProperty<AddonInfomation> Infomation { get; }
    }

    public class AddonInstallManager : IAddonInstallManager
    {
        public AddonInstallManager(IAddonInfomationsContainer container, INicoDirectoryIO directoryIO, ILogger logger, IAddonEngine engine, IAddonInstaller installer)
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

        private string? tempPath;

        #endregion

        #region Methods


        public IAttemptResult LoadAddon(string path)
        {
            this.IsInstalling.Value = true;

            //解凍
            IAttemptResult<string> extractResult = this.installer.Extract(path);
            if (!extractResult.IsSucceeded || extractResult.Data is null)
            {
                this.Cancel();
                return new AttemptResult() { Message = extractResult.Message, Exception = extractResult.Exception };
            }

            this.tempPath = extractResult.Data;

            IAttemptResult<AddonInfomation> mResult = this.installer.LoadManifest(extractResult.Data);
            if (!mResult.IsSucceeded || mResult.Data is null)
            {
                this.Cancel();
                return new AttemptResult() { Message = mResult.Message, Exception = mResult.Exception };
            }

            this.IsLoaded.Value = true;
            this.Infomation.Value = mResult.Data;
            return new AttemptResult() { IsSucceeded = true };

        }

        public IAttemptResult InstallAddon()
        {
            if (!this.IsLoaded.Value)
            {
                return new AttemptResult() { Message = "アドオンの読み込みが完了していません。" };
            }

            IAttemptResult<AddonInfomation> result = this.installer.Install(this.tempPath!);
            if (!result.IsSucceeded)
            {
                return new AttemptResult() { Message = result.Message, Exception = result.Exception };
            }

            return new AttemptResult() { IsSucceeded = true };

        }



        #endregion

        #region Props

        public ReactiveProperty<bool> IsInstalling { get; init; } = new();

        public ReactiveProperty<bool> IsLoaded { get; init; } = new();

        public ReactiveProperty<AddonInfomation> Infomation { get; init; } = new();


        #endregion

        #region private

        private void Cancel()
        {
            this.IsInstalling.Value = false;
            this.IsLoaded.Value = false;
            this.tempPath = null;
        }

        #endregion
    }
}
