using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local.Settings;

namespace Niconicome.Models.Domain.Local.Settings
{
    public interface ISettingsConainer
    {
        /// <summary>
        /// 指定した設定を取得する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="settingName"></param>
        /// <returns></returns>
        IAttemptResult<ISettingInfo<T>> GetSetting<T>(string settingName, T defaultValue) where T : notnull;
    }

    public class SettingsConainer : ISettingsConainer
    {
        public SettingsConainer(ISettingsStore store, ISettingMigratioin settingMigratioin)
        {
            this._store = store;
            this._settingMigratioin= settingMigratioin;
        }

        #region field

        private readonly ISettingMigratioin _settingMigratioin;

        private readonly ISettingsStore _store;

        #endregion

        #region Method

        public IAttemptResult<ISettingInfo<T>> GetSetting<T>(string settingName, T defaultValue) where T : notnull
        {
            if (this._settingMigratioin.IsMigrationNeeded)
            {
                IAttemptResult result = this._settingMigratioin.Migrate();
                if (!result.IsSucceeded) return AttemptResult<ISettingInfo<T>>.Fail(result.Message);
            }

            if (!this._store.Exists(settingName))
            {
                this._store.SetSetting(settingName, defaultValue);
            }

            return this._store.GetSetting<T>(settingName);
        }


        #endregion
    }

}
