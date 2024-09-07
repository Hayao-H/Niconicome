using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Utils.Reactive;
using Reactive.Bindings;

namespace Niconicome.Models.Domain.Local.Settings
{
    public interface ISettingInfo<T> where T :  notnull
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

        /// <summary>
        /// 設定値(BP)
        /// </summary>
        IBindableProperty<T> Bindablevalue { get; }

        /// <summary>
        /// 変更監視
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        ISettingInfo<T> WithSubscribe(Action<T> handler);

        /// <summary>
        ///変更監視を停止 
        /// </summary>
        /// <param name="handler"></param>
        void UnSubscribe(Action<T> handler);

        /// <summary>
        /// フィルター関数を登録
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="function"></param>
        void RegisterWhereFunc(Func<T, bool> predicate);

        /// <summary>
        /// 変換関数を登録
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="function"></param>
        void RegisterSelectFunc(Func<T, T> converter);
    }

    public class SettingInfo<T> : ISettingInfo<T> where T : notnull
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
                this.OnChange(value);
                this._store.SetSetting(this);
            });

            this.Bindablevalue = new BindableProperty<T>(initialValue);
            this.Bindablevalue.RegisterPropertyChangeHandler(value =>
            {
                if (EqualityComparer<T>.Default.Equals(value, this._value)) return;

                this._value = value;
                this.OnChange(value);
                this._store.SetSetting(this);
            });

        }

        #region field

        private T _value;

        private readonly ISettingsStore _store;

        private Func<T, bool>? _predicate;

        private Func<T, T>? _converter;

        private Action<T>? _handlers;

        private Action _nonValueHandlers;

        #endregion

        #region Props

        public string SettingName { get; init; }


        public T Value
        {
            get => this._value;
            set
            {
                if (this._predicate is not null && !this._predicate(value)) return;
                if (this._converter is not null)
                {
                    value = this._converter(value);
                }

                this._value = value;
                this.OnChange(value);
                this.ReactiveValue.Value = value;
                this.Bindablevalue.Value = value;
                this._store.SetSetting(this);
            }
        }

        public ReactiveProperty<T> ReactiveValue { get; init; }

        public IBindableProperty<T> Bindablevalue { get; init; }


        #endregion

        #region Method

        public void RegisterWhereFunc(Func<T, bool> predicate)
        {
            this._predicate = predicate;
        }

        public void RegisterSelectFunc(Func<T, T> converter)
        {
            this._converter = converter;
        }

        public ISettingInfo<T> WithSubscribe(Action<T> handler)
        {
            this._handlers += handler;
            return this;
        }

        public void UnSubscribe(Action<T> handler)
        {
            this._handlers -= handler;
        }

        #endregion

        #region private

        private void OnChange(T value)
        {
            try
            {
                this._handlers?.Invoke(value);
            }
            catch { }
        }

        #endregion
    }
}
