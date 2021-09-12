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

    public interface IStorageHandler
    {
        /// <summary>
        /// 初期化する
        /// </summary>
        /// <param name="addonName"></param>
        /// <param name="packageID"></param>
        /// <returns></returns>
        IAttemptResult Initialize(string addonName, string packageID);

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

        private StorageData? _data;

        private string? _packageID;

        #endregion

        #region Method

        public IAttemptResult Initialize(string addonName, string packageID)
        {
            this._helper.Initialize(addonName);
            this._packageID = packageID;
            IAttemptResult<StorageData> result = this._helper.LoadFile(packageID);
            if (result.IsSucceeded)
            {
                this._data = result.Data;
            }
            return result;
        }

        public IAttemptResult SetItem(string key, string value)
        {
            this.CheckIfInitialized();
            this._helper.SetItem(key, value, this._data!);
            return this._helper.SaveFile(this._packageID!, this._data!);
        }

        public string? GetItem(string key)
        {
            this.CheckIfInitialized();
            return this._helper.GetItem(key, this._data!);
        }

        public void Clear()
        {
            this.CheckIfInitialized();
            this._helper.Clear(this._data!);
            this._helper.SaveFile(this._packageID!, this._data!);
        }

        public void RemoveItem(string key)
        {
            this.CheckIfInitialized();
            this._helper.RemoveItem(key, this._data!);
            this._helper.SaveFile(this._packageID!, this._data!);
        }


        #endregion

        #region Meta

        public void Initialize(StorageData initialData)
        {
            this._data = initialData;
            this._packageID = string.Empty;
            this._helper.Initialize(string.Empty);
        }

        #endregion

        #region private

        private void CheckIfInitialized()
        {
            if (this._data is null || this._packageID is null) throw new InvalidOperationException("NOT_INITIALIZED");
        }

        #endregion
    }
}
