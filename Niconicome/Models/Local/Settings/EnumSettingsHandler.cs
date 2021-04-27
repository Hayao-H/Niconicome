using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Local.Settings
{
    public interface IEnumSettingsHandler
    {
        T GetSetting<T>();
        IAttemptResult SaveSetting<T>(T data) where T:Enum;
    }

    class EnumSettingsHandler:IEnumSettingsHandler
    {
        /// <summary>
        /// 設定を取得する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public T GetSetting<T>()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 設定値を保存する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public IAttemptResult SaveSetting<T>(T data) where T : Enum
        {
            throw new NotImplementedException();
        }
    }
}
