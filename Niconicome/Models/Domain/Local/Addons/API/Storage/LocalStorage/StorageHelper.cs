using System;
using System.IO;
using Niconicome.Models.Domain.Local.Addons.Core.Engine;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Helper.Result;
using Const = Niconicome.Models.Const;

namespace Niconicome.Models.Domain.Local.Addons.API.Storage.LocalStorage
{
    public interface IStorageHelper
    {
        /// <summary>
        /// 初期化する
        /// </summary>
        /// <param name="addonName">アドオン名</param>
        void Initialize(string addonName);

        /// <summary>
        /// データをセットする
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="store"></param>
        void SetItem(string key, string value, StorageData store);

        /// <summary>
        /// 指定したデータを削除する
        /// </summary>
        /// <param name="key"></param>
        /// <param name="store"></param>
        void RemoveItem(string key, StorageData store);

        /// <summary>
        /// データをクリアする
        /// </summary>
        /// <param name=""></param>
        void Clear(StorageData store);

        /// <summary>
        /// データを取得する
        /// </summary>
        /// <param name="key"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        string? GetItem(string key, StorageData store);

        /// <summary>
        /// ローカルのファイルを読み込む
        /// </summary>
        /// <param name="packageID"></param>
        /// <returns></returns>
        IAttemptResult<StorageData> LoadFile(string packageID);

        /// <summary>
        /// ローカルに保存する
        /// </summary>
        /// <param name="packageID"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        IAttemptResult SaveFile(string packageID, StorageData store);

    }

    public class StorageHelper : IStorageHelper
    {
        public StorageHelper(INicoFileIO fileIO, IAddonLogger logger)
        {
            this._fileIO = fileIO;
            this._logger = logger;
        }

        #region field

        private readonly INicoFileIO _fileIO;

        private readonly IAddonLogger _logger;

        private string? _addonName;

        #endregion

        #region Method

        public IAttemptResult<StorageData> LoadFile(string packageID)
        {
            this.CheckIfInitialized();

            string path = Path.Combine(AppContext.BaseDirectory, Const::FileFolder.AddonsFolder, packageID, Const::Adddon.LocalStorageFileName);
            if (!File.Exists(path))
            {
                return new AttemptResult<StorageData>() { IsSucceeded = true, Data = new StorageData() { PackageID = packageID } };
            }

            string raw;
            try
            {
                raw = this._fileIO.OpenRead(path);
            }
            catch (Exception e)
            {
                this._logger.Error("ローカルストレージファイルの展開に失敗しました。", this._addonName!, e);
                return new AttemptResult<StorageData>();
            }

            StorageData data;
            try
            {
                data = JsonParser.DeSerialize<StorageData>(raw);
            }
            catch (Exception e)
            {
                this._logger.Error("ローカルストレージファイルの解析に失敗しました。", this._addonName!, e);
                return new AttemptResult<StorageData>();
            }

            return new AttemptResult<StorageData>() { IsSucceeded = true, Data = data };
        }

        public IAttemptResult SaveFile(string packageID, StorageData store)
        {
            this.CheckIfInitialized();

            string path = Path.Combine(AppContext.BaseDirectory, Const::FileFolder.AddonsFolder, packageID, Const::Adddon.LocalStorageFileName);
            bool canWrite;
            StorageToken token;

            try
            {
                token = new StorageToken(store);
                canWrite = token.CanWrite;
            } catch(Exception e)
            {
                this._logger.Error("ストレージオブジェクトの解析に失敗しました。", this._addonName!, e);
                return new AttemptResult() { Message = "INTERNAL_ERROR (SERIALIZE_ERR)" };
            }

            if (!canWrite)
            {
                this._logger.Error("ストレージオブジェクトを書き込むことができません(サイズオーバー等)。", this._addonName!);
                return new AttemptResult() { Message = "QUOTA_EXCEEDED_ERR" };
            }

            try
            {
                this._fileIO.Write(path, token.Data);
            }
            catch (Exception e)
            {

                this._logger.Error("ストレージファイルへの書き込みに失敗しました。", this._addonName!, e);
                return new AttemptResult() { Message= "INTERNAL_ERROR (WRITING_ERR)" };
            }

            return new AttemptResult() { IsSucceeded = true };
        }

        public void SetItem(string key, string value, StorageData store)
        {
            if (store.Data.ContainsKey(key))
            {
                store.Data[key] = value;
            }
            else
            {
                store.Data.Add(key, value);
            }
        }

        public string? GetItem(string key, StorageData store)
        {
            store.Data.TryGetValue(key, out string? value);
            return value;
        }

        public void RemoveItem(string key, StorageData store)
        {
            if (store.Data.ContainsKey(key))
            {
                store.Data.Remove(key);
            }
        }


        public void Clear(StorageData store)
        {
            store.Data.Clear();
        }


        public void Initialize(string addonName)
        {
            this._addonName = addonName;
        }

        #endregion

        #region private

        private void CheckIfInitialized()
        {
            if (this._addonName is null) throw new InvalidOperationException("NOT_INITIALIZED");
        }

        #endregion
    }
}
