using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Playlist;
using Reactive.Bindings;

namespace Niconicome.Models.Domain.Local.Settings
{
    public interface ISettingInfo<T> where T : notnull, IComparable<T>
    {
        /// <summary>
        /// 設定名
        /// </summary>
        string SettingName { get; }

        /// <summary>
        /// 設定値
        /// </summary>
        T Value { get; set; }

        /// <summary>
        /// 設定値(RP)
        /// </summary>
        ReactiveProperty<T> ReactiveValue { get; }
    }

    public class SettingInfo<T> : ISettingInfo<T> where T : notnull, IComparable<T>
    {
        public SettingInfo(string settingName, T initialValue, ISettingsStore store)
        {
            this.SettingName = settingName;
            this._value = initialValue;
            this._store = store;

            this.ReactiveValue = new ReactiveProperty<T>(initialValue);
            this.ReactiveValue.Subscribe(value =>
            {
                if (EqualityComparer<T>.Default.Equals(value, this._value)) return;

                this._value = value;
                this._store.SetSetting(this);
            });

        }

        #region field

        private T _value;

        private readonly ISettingsStore _store;

        #endregion

        #region Props

        public string SettingName { get; init; }


        public T Value
        {
            get => this._value;
            set
            {
                this._value = value;
                this.ReactiveValue.Value = value;
                this._store.SetSetting(this);
            }
        }

        public ReactiveProperty<T> ReactiveValue { get; init; }


        #endregion
    }
}
