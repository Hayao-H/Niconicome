using System;
using System.Collections.Generic;
using Niconicome.Models.Domain.Local.Addons.API.Storage.LocalStorage;
using Niconicome.Models.Helper.Result;

namespace NiconicomeTest.Stabs.Models.Domain.Local.Addon.API.Storage
{
    class StorageHelperStab : IStorageHelper
    {
        public StorageHelperStab(Dictionary<string,string> data) {
            this._data = data;
        }


        #region field

        private readonly Dictionary<string, string> _data;

        #endregion

        public void Initialize(string addonName, string packageID)
        {

        }

        public IAttemptResult SetItem(string key, string value)
        {
            if (this._data.ContainsKey(key))
            {
                this._data[key] = value;
            }
            else
            {
                this._data.Add(key, value);
            }

            return new AttemptResult() { IsSucceeded = true };
        }

        public void RemoveItem(string key)
        {
            this._data.Remove(key);
        }

        public void Clear()
        {
            this._data.Clear();
        }

        public string? GetItem(string key)
        {
            if (this._data.ContainsKey(key))
            {
                return this._data[key];
            }
            else
            {
                return null;
            }
        }

        public void Dispose()
        {

        }
    }
}
