using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.Addons.Core.Engine;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.Addons.API.Resource
{
    public interface IResourceHander
    {
        /// <summary>
        /// リソースを取得する
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        IAttemptResult<string> GetResource(string relativePath);

        /// <summary>
        /// 初期化処理
        /// </summary>
        /// <param name="packageID"></param>
        /// <param name="addonName"></param>
        void Initialize(string packageID, string addonName);
    }

    public class ResourceHander : IResourceHander
    {
        public ResourceHander(INicoFileIO fileIO,IAddonLogger logger)
        {
            this._fileIO = fileIO;
            this._logger = logger;
        }

        #region field

        private INicoFileIO _fileIO;

        private string? _packageID;

        private string? _addonName;

        private bool _isInitialized;

        private IAddonLogger _logger;

        #endregion

        public IAttemptResult<string> GetResource(string relativePath)
        {
            this.CheckIfInitialized();

            string path = Path.Combine(FileFolder.AddonsFolder, this._packageID!, relativePath);

            string data;
            try
            {
                data = this._fileIO.OpenRead(path);
            }
            catch (Exception ex)
            {
                this._logger.Error($"ファイルの読み込みに失敗しました。 (path: {relativePath})", this._addonName!, ex);
                return new AttemptResult<string>() { IsSucceeded = false };
            }

            return new AttemptResult<string>() { IsSucceeded = true, Data = data };
        }

        public void Initialize(string packageID, string addonName)
        {
            if (this._isInitialized) return;

            this._addonName = addonName;
            this._packageID = packageID;
            this._isInitialized = true;
        }


        #region private

        private void CheckIfInitialized()
        {
            if (!this._isInitialized) throw new InvalidOperationException("Not Initialized");
        }

        #endregion

    }
}
