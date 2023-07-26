using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Helper.Result;

namespace Niconicome.Models.Utils.Reactive
{
    public interface IBindableSettingInfo<T> : IBindableProperty<T>
    {
        new IBindableSettingInfo<T> AddTo(Bindables bindables);
    }

    public class BindableSettingInfo<T> : BindableProperty<T>, IBindableSettingInfo<T> where T : notnull
    {

        public BindableSettingInfo(IAttemptResult<ISettingInfo<T>> result, T defaultValue) : this(result, defaultValue, x => x, _ => true)
        {
        }

        public BindableSettingInfo(IAttemptResult<ISettingInfo<T>> result, T defaultValue, Func<T, T> selecter, Func<object, bool> predicate) : base(defaultValue)
        {
            if (!result.IsSucceeded || result.Data is null)
            {
                this.IsEnabled = false;
            }
            else
            {
                this._settingInfo = result.Data;
                this._predicate = predicate;
                this.Value = this._settingInfo.Value;
                this.IsEnabled = true;
                this.Subscribe(v => this._settingInfo.Value = selecter(v));
            }
        }

        #region Method

        public new IBindableSettingInfo<T> AddTo(Bindables bindables)
        {
            bindables.Add(this);
            return this;
        }


        #endregion

        #region field

        private readonly ISettingInfo<T>? _settingInfo;

        private readonly Func<object, bool>? _predicate;

        #endregion

        #region Props

        /// <summary>
        /// 設定が有効であるかどうか
        /// </summary>
        public bool IsEnabled { get; init; }

        /// <summary>
        /// 設定値
        /// </summary>
        public override T Value
        {
            get => this._value;
            set
            {
                if (this._predicate is not null && !this._predicate(value))
                {
                    return;
                }

                this._value = value;
                this.OnPropertyChanged(value);
            }
        }

        #endregion
    }
}
