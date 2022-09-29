using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.Context;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Const = Niconicome.Models.Const;

namespace Niconicome.Models.Domain.Local.Addons.Core.V2.Uninstall
{
    public interface IAddonUninstaller
    {
        /// <summary>
        /// 指定したアドオンをアンインストールする
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        IAttemptResult Uninstall(string ID);
    }

    public class AddonUninstaller : IAddonUninstaller
    {
        public AddonUninstaller(IAddonContextsContainer container, ILogger logger, INicoDirectoryIO directoryIO)
        {
            this._container = container;
            this._logger = logger;
            this._directoryIO = directoryIO;
        }

        #region field

        private readonly IAddonContextsContainer _container;

        private readonly ILogger _logger;

        private readonly INicoDirectoryIO _directoryIO;

        #endregion

        #region Method

        public IAttemptResult Uninstall(string ID)
        {
            IAttemptResult<IAddonContext> getResult = this._container.Get(ID);
            if (!getResult.IsSucceeded || getResult.Data is null)
            {
                return getResult;
            }

            IAddonContext context = getResult.Data;
            this._container.ShutDown(ID);

            string directory = Path.Combine(AppContext.BaseDirectory, Const::FileFolder.AddonsFolder, context.AddonInfomation!.DirectoryName);
            try
            {
                this._directoryIO.Delete(directory);
            }
            catch (Exception ex)
            {
                this._logger.Error("アドオンの削除に失敗しました。", ex);
                return AttemptResult.Fail($"アドオンの削除に失敗しました。(詳細：{ex.Message})");
            }

            return AttemptResult.Succeeded();
        }


        #endregion
    }
}
