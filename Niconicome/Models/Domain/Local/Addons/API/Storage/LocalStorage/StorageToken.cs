using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Net.Json;

namespace Niconicome.Models.Domain.Local.Addons.API.Storage.LocalStorage
{
    public interface IStorageToken
    {
        /// <summary>
        /// 書込み可能なサイズであるかどうかを判断
        /// </summary>
        bool CanWrite { get; }

        /// <summary>
        /// 書き込むデータ
        /// </summary>
        string Data { get; }
    }

    public class StorageToken : IStorageToken
    {

        public StorageToken(StorageData store)
        {
            this._store = store;
        }

        #region field

        private readonly StorageData _store;

        private string? _data;

        private int _cachedByte = -1;

        #endregion

        #region Props

        public bool CanWrite
        {
            get
            {
                if (this._cachedByte < 0)
                {
                    this.GetByte();
                }

                return this._cachedByte < 10 * 1024 * 1024;
            }
        }

        public string Data
        {
            get => this._data ?? this.GetData();
        }

        #endregion

        #region private

        private string GetData()
        {
            this._data = JsonParser.Serialize(this._store);
            return this._data;
        }

        private void GetByte()
        {
            if (this._cachedByte > 0) return;
            if (this._data is null)
            {
                this.GetData();
            }

            this._cachedByte = Encoding.UTF8.GetByteCount(this._data!);

        }

        #endregion
    }
}
