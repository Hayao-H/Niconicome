using System;
using Niconicome.Models.Helper.Result;
using Value = Niconicome.Models.Local.Settings.EnumSettingsValue;

namespace Niconicome.Models.Local.Settings
{
    public interface IEnumSettingsHandler
    {
        T GetSetting<T>() where T : Enum;
        IAttemptResult SaveSetting<T>(T data) where T:Enum;
    }

    class EnumSettingsHandler:IEnumSettingsHandler
    {
        public EnumSettingsHandler(ILocalSettingHandler localSettingHandler)
        {
            this.localSettingHandler = localSettingHandler;
        }

        #region フィールド

        private readonly ILocalSettingHandler localSettingHandler;

        #endregion

        /// <summary>
        /// 設定を取得する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="settingName"></param>
        /// <returns></returns>
        public T GetSetting<T>() where T : Enum
        {
            if (typeof(T) == typeof(Value::VideodbClickSettings))
            {
                var value = this.localSettingHandler.GetIntSetting(SettingsEnum.VListItemdbClick);

                if (value == -1) value = 0;

                return (T)Enum.ToObject(typeof(T), value);
            }

            throw new InvalidOperationException($"指定した型に一致する設定は存在しません。({typeof(T).Name})");
        }

        /// <summary>
        /// 設定値を保存する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public IAttemptResult SaveSetting<T>(T data) where T : Enum
        {
            if (data is Value::VideodbClickSettings value)
            {
                this.localSettingHandler.SaveSetting(value, SettingsEnum.VListItemdbClick);

                return new AttemptResult() { IsSucceeded = true };
            }

            throw new InvalidOperationException($"指定した型に一致する設定は存在しません。({typeof(T).Name})");
        }
    }
}
