﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.API.Storage.LocalStorage;
using Niconicome.Models.Domain.Local.Addons.Core;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.Infomation;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Local.Addon.API.Local.Storage
{
    public interface ILocalStorage : IDisposable
    {
        void clear();
        string? getItem(string key);
        void removeItem(string key);
        void setItem(string key, string value);
        void Initialize(IAddonInfomation info);
    }

    public class LocalStorage : ILocalStorage
    {
        public LocalStorage(IStorageHandler handler)
        {
            this._handler = handler;
        }

        #region field

        private readonly IStorageHandler _handler;

        private bool _isInitialized;

        #endregion

        #region Method

        public void setItem(string key, string value)
        {
            IAttemptResult result = this._handler.SetItem(key, value);
            if (!result.IsSucceeded)
            {
                throw new Exception(result.Message);
            }
        }

        public string? getItem(string key)
        {
            return this._handler.GetItem(key);
        }
        public void removeItem(string key)
        {
            this._handler.RemoveItem(key);
        }

        public void clear()
        {
            this._handler.Clear();
        }

        public void Initialize(IAddonInfomation info)
        {
            if (this._isInitialized) return;
            this._handler.Initialize(info.Name, info.DirectoryName);
            this._isInitialized = true;
        }

        public void Dispose()
        {
            this._handler.Dispose();
        }

        #endregion
    }
}
