using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Domain.Local.Addons.API.Storage.LocalStorage
{

    public interface IStorageHandler : IDisposable
    {
        /// <summary>
        /// 初期化する
        /// </summary>
        /// <param name="addonName"></param>
        /// <param name="directoryName"></param>
        /// <returns></returns>
        void Initialize(string addonName, string directoryName);

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

        private string? _directoryName;

        #endregion

        #region Method

        public void Initialize(string addonName, string directoryName)
        {
            this._helper.Initialize(addonName, directoryName);
            this._directoryName = directoryName;
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
            if (this._directoryName is null) throw new InvalidOperationException("NOT_INITIALIZED");
        }

        #endregion
    }
}
