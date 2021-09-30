using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.Core.Installer;
using Niconicome.Models.Helper.Result;
using Windows.UI.Composition;

namespace Niconicome.Models.Domain.Local.Addons.API.Storage.LocalStorage
{

    public interface IStorageHandler : IDisposable
    {
        /// <summary>
        /// 初期化する
        /// </summary>
        /// <param name="addonName"></param>
        /// <param name="packageID"></param>
        /// <returns></returns>
        void Initialize(string addonName, string packageID);

        /// <summary>
        /// 値をセットする
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IAttemptResult SetItem(string key, string value);

        /// <summary>
        /// 値を取得する
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string? GetItem(string key);

        /// <summary>
        /// 指定したアイテムを削除する
        /// </summary>
        /// <param name="key"></param>
        void RemoveItem(string key);

        /// <summary>
        /// 全てのアイテムを削除する
        /// </summary>
        void Clear();
    }

    public class StorageHandler : IStorageHandler
    {
        public StorageHandler(IStorageHelper helper)
        {
            this._helper = helper;
        }

        #region field

        private readonly IStorageHelper _helper;

        private string? _packageID;

        #endregion

        #region Method

        public void Initialize(string addonName, string packageID)
        {
            this._helper.Initialize(addonName, packageID);
            this._packageID = packageID;
        }

        public IAttemptResult SetItem(string key, string value)
        {
            this.CheckIfInitialized();
            return this._helper.SetItem(key, value);
        }

        public string? GetItem(string key)
        {
            this.CheckIfInitialized();
            return this._helper.GetItem(key);
        }

        public void Clear()
        {
            this.CheckIfInitialized();
            this._helper.Clear();
        }

        public void RemoveItem(string key)
        {
            this.CheckIfInitialized();
            this._helper.RemoveItem(key);
        }

        public void Dispose()
        {
            this._helper.Dispose();
        }

        #endregion

        #region private

        private void CheckIfInitialized()
        {
            if (this._packageID is null) throw new InvalidOperationException("NOT_INITIALIZED");
        }

        #endregion
    }
}
