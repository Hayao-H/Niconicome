using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
        /// 読み込みに失敗したアドオン
        /// </summary>
        ObservableCollection<IAttemptResult<string>> LoadFailedAddons { get; }

        /// <summary>
        /// 初期化する
        /// </summary>
        /// <returns></returns>
        Task<IAttemptResult> InitializeAsync();

    }

    public class AddonHandler : IAddonHandler
    {

        public AddonHandler(IAddonInfomationsContainer container, INicoDirectoryIO directoryIO, ILogger logger, IAddonEngine engine)
        {
            this.container = container;
            this.directoryIO = directoryIO;
            this.logger = logger;
            this.engine = engine;
        }

        #region field

        private readonly IAddonInfomationsContainer container;

        private readonly IAddonEngine engine;

        private readonly INicoDirectoryIO directoryIO;

        private readonly ILogger logger;

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
                packages = this.directoryIO.GetDirectorys(FileFolder.AddonsFolder);
            }
            catch (Exception e)
            {
                this.logger.Error("アドオンパッケージ一覧の取得に失敗しました。", e);
                return new AttemptResult() { Message = "アドオンパッケージ一覧の取得に失敗しました。", Exception = e };
            }

            foreach (var packagePath in packages)
            {
                string package = Path.GetFileName(packagePath);
                IAttemptResult result = await this.engine.InitializeAsync(package);
                if (!result.IsSucceeded)
                {
                    var failedResult = new AttemptResult<string>() { Message = result.Message, Data = package};
                    this.LoadFailedAddons.Add(failedResult);
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

        public ObservableCollection<IAttemptResult<string>> LoadFailedAddons { get; init; } = new();

        #endregion

    }
}
