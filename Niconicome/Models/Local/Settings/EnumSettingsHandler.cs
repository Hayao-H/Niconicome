using System;
using System.Windows.Media;
using Niconicome.Models.Domain.Local;
using Niconicome.Models.Helper.Result;
using Value = Niconicome.Models.Local.Settings.EnumSettingsValue;

namespace Niconicome.Models.Local.Settings
{
    public interface IEnumSettingsHandler
    {
        T GetSetting<T>() where T : Enum;
        IAttemptResult SaveSetting<T>(T data) where T:Enum;
    }

    class EnumSettingsHandler : IEnumSettingsHandler
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
            SettingsEnum setting = this.GetSettingType<T>();

            var value = this.localSettingHandler.GetIntSetting(setting);
            if (value == -1) value = 0;

            return (T)Enum.ToObject(typeof(T), value);
        }

        /// <summary>
        /// 設定値を保存する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public IAttemptResult SaveSetting<T>(T data) where T : Enum
        {
            SettingsEnum setting = this.GetSettingType<T>();

            this.localSettingHandler.SaveSetting(Convert.ToInt32(data), setting);

            return new AttemptResult() { IsSucceeded = true };
        }

        /// <summary>
        /// 設定名を取得
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private SettingsEnum GetSettingType<T>()
        {
            if (typeof(T) == typeof(Value::VideodbClickSettings))
            {
                return SettingsEnum.VListItemdbClick;
            }
            else if (typeof(T) == typeof(Value::VideoInfoTypeSettings))
            {
                return SettingsEnum.VideoInfoType;
            }
            else
            {
                throw new InvalidOperationException($"指定した型に一致する設定は存在しません。({typeof(T).Name})");
            }
        }
    }
}
