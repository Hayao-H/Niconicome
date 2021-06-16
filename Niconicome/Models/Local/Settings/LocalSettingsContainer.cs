using System;
using System.Collections.Generic;
using Niconicome.ViewModels;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Niconicome.Models.Local.Settings
{
    interface ILocalSettingsContainer
    {
        ReactiveProperty<bool> GetReactiveBoolSetting(SettingsEnum settingType);
        ReactiveProperty<int> GetReactiveIntSetting(SettingsEnum settingType, Func<int, bool>? whereFunc = null, Func<int, int>? selectFunc = null);
        ReactiveProperty<string> GetReactiveStringSetting(SettingsEnum settingType, string defaultValue = "");
    }

    class LocalSettingsContainer : BindableBase, ILocalSettingsContainer
    {
        public LocalSettingsContainer(ILocalSettingHandler settingHandler)
        {
            this.settingHandler = settingHandler;
        }

        #region field
        private readonly Dictionary<SettingsEnum, ReactiveProperty<bool>> boolSettingsContainer = new();

        private readonly Dictionary<SettingsEnum, ReactiveProperty<string>> stringSettingsContainer = new();

        private readonly Dictionary<SettingsEnum, ReactiveProperty<int>> intSettingsContainer = new();

        private readonly ILocalSettingHandler settingHandler;
        #endregion

        /// <summary>
        /// 真偽値設定を取得
        /// </summary>
        /// <param name="settingType"></param>
        /// <returns></returns>
        public ReactiveProperty<bool> GetReactiveBoolSetting(SettingsEnum settingType)
        {
            this.boolSettingsContainer.TryGetValue(settingType, out ReactiveProperty<bool>? setting);

            if (setting is null)
            {
                var newSetting = new ReactiveProperty<bool>(this.settingHandler.GetBoolSetting(settingType));
                this.boolSettingsContainer.Add(settingType, newSetting);
                newSetting.Subscribe(value => this.settingHandler.SaveSetting(value, settingType)).AddTo(this.disposables);
                return newSetting;
            }
            else
            {
                return setting;
            }
        }

        /// <summary>
        /// 整数値設定を取得
        /// </summary>
        /// <param name="settingType"></param>
        /// <param name="whereFunc"></param>
        /// <param name="selectFunc"></param>
        /// <returns></returns>
        public ReactiveProperty<int> GetReactiveIntSetting(SettingsEnum settingType, Func<int, bool>? whereFunc = null, Func<int, int>? selectFunc = null)
        {
            if (whereFunc is null)
            {
                whereFunc = _ => true;
            }

            if (selectFunc is null)
            {
                selectFunc = value => value;
            }

            this.intSettingsContainer.TryGetValue(settingType, out ReactiveProperty<int>? setting);

            if (setting is null)
            {
                var newSetting = new ReactiveProperty<int>(this.settingHandler.GetIntSetting(settingType));
                this.intSettingsContainer.Add(settingType, newSetting);
                newSetting.Subscribe(value =>
                {
                    if (!whereFunc(value)) return;
                    value = selectFunc(value);
                    this.settingHandler.SaveSetting(value, settingType);
                }).AddTo(this.disposables);
                return newSetting;
            }
            else
            {
                return setting;
            }

        }

        /// <summary>
        /// 文字列設定を取得
        /// </summary>
        /// <param name="settingType"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public ReactiveProperty<string> GetReactiveStringSetting(SettingsEnum settingType, string defaultValue = "")
        {
            this.stringSettingsContainer.TryGetValue(settingType, out ReactiveProperty<string>? setting);

            if (setting is null)
            {
                var newSetting = new ReactiveProperty<string>(this.settingHandler.GetStringSetting(settingType) ?? defaultValue);
                this.stringSettingsContainer.Add(settingType, newSetting);
                newSetting.Subscribe(value => this.settingHandler.SaveSetting(value, settingType)).AddTo(this.disposables);
                return newSetting;
            }
            else
            {
                return setting;
            }

        }
    }
}
