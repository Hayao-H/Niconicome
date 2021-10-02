using System;
using System.IO;
using System.IO.Packaging;
using LiteDB;
using Niconicome.Models.Domain.Local.Addons.Core.Engine;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Helper.Result;
using Reactive.Bindings.ObjectExtensions;
using Windows.ApplicationModel;
using Const = Niconicome.Models.Const;

namespace Niconicome.Models.Domain.Local.Addons.API.Storage.LocalStorage
{
    public interface IStorageHelper : IDisposable
    {
        /// <summary>
        /// 初期化する
        /// </summary>
        /// <param name="addonName">アドオン名</param>
        void Initialize(string addonName, string packageID);

        /// <summary>
        /// データをセットする
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="store"></param>
        IAttemptResult SetItem(string key, string value);

        /// <summary>
        /// 指定したデータを削除する
        /// </summary>
        /// <param name="key"></param>
        /// <param name="store"></param>
        void RemoveItem(string key);

        /// <summary>
        /// データをクリアする
        /// </summary>
        /// <param name=""></param>
        void Clear();

        /// <summary>
        /// データを取得する
        /// </summary>
        /// <param name="key"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        string? GetItem(string key);

    }

    public class StorageHelper : IStorageHelper
    {
        public StorageHelper(IAddonLogger logger, ILogger globalLogger)
        {
            this._logger = logger;
            this._globalLogger = globalLogger;
        }

        #region field

        private readonly IAddonLogger _logger;

        private readonly ILogger _globalLogger;

        private IDataBase? _dataBase;

        private string? _addonName;

        private bool _isDisposed;

        #endregion

        #region Method

        public IAttemptResult SetItem(string key, string value)
        {
            this.CheckIfInitialized();
            var data = new StorageDataType() { Name = key, Value = value };

            if (this._dataBase!.Exists<StorageDataType>(StorageDataType.TableName, d => d.Name == key))
            {
                int Id = this._dataBase.GetRecord<StorageDataType>(StorageDataType.TableName, d => d.Name == key)!.Data!.Id;
                data.Id = Id;
                return this._dataBase.Update(data, StorageDataType.TableName);
            }
            else
            {
                return this._dataBase.Store(data, StorageDataType.TableName);
            }
        }

        public string? GetItem(string key)
        {
            this.CheckIfInitialized();

            IAttemptResult<StorageDataType> data = this._dataBase!.GetRecord<StorageDataType>(StorageDataType.TableName, d => d.Name == key);

            if (!data.IsSucceeded || data.Data is null)
            {
                return null;
            }
            else
            {
                return data.Data.Value;
            }
        }

        public void RemoveItem(string key)
        {
            this.CheckIfInitialized();
            this._dataBase!.DeleteAll<StorageDataType>(StorageDataType.TableName, d => d.Name == key);
        }


        public void Clear()
        {
            this.CheckIfInitialized();
            this._dataBase!.Clear(StorageDataType.TableName);
        }


        public void Initialize(string addonName, string packageID)
        {
            this._addonName = addonName;
            this.OpenDataBase(this._globalLogger, packageID);
        }

        public void Dispose()
        {
            if (this._isDisposed)
            {
                return;
            }

            this._dataBase?.Dispose();
            this._isDisposed = true;
        }

        #endregion

        #region private

        private void CheckIfInitialized()
        {
            if (this._addonName is null || this._dataBase is null) throw new InvalidOperationException("NOT_INITIALIZED");
        }

        private void OpenDataBase(ILogger logger, string packageID)
        {
            ILiteDatabase db;
            string path = Path.Combine(AppContext.BaseDirectory, Const::FileFolder.AddonsFolder, packageID, Const::AdddonConstant.LocalStorageFileName);

            try
            {
                db = new LiteDatabase($"Filename={path};");
            }
            catch (Exception e)
            {
                this._logger.Error("データベースの展開に失敗しました。", this._addonName!, e);
                return;
            }

            this._dataBase = new DataBase(db, logger);
        }

        #endregion
    }
}
