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
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Local.Addon
{
    public interface IAddonHandler
    {
        ObservableCollection<AddonInfomation> Addons { get; }
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

        #endregion

        #region Props

        /// <summary>
        /// アドオン
        /// </summary>
        public ObservableCollection<AddonInfomation> Addons => this.container.Addons;

        #endregion
    }
}
