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
        IAttemptResult<ISettingInfo<T>> GetSetting<T>(string settingName) where T : notnull, IComparable<T>;
    }

    public class SettingsConainer : ISettingsConainer
    {
        public SettingsConainer(ISettingsStore store)
        {
            this._store = store;
        }

        #region field

        private readonly ISettingsStore _store;

        #endregion

        #region Method

        public IAttemptResult<ISettingInfo<T>> GetSetting<T>(string settingName) where T : notnull, IComparable<T>
        {
            return this._store.GetSetting<T>(settingName);
        }


        #endregion
    }

}
