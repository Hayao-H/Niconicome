using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Helper.Result;

namespace Niconicome.ViewModels.Setting.Utils
{
    public class SettingInfoViewModel<T> : SettingInfoViewModelBase<T> where T : notnull
    {
        public SettingInfoViewModel(IAttemptResult<ISettingInfo<T>> setting, T defaultVal) {
            this.IsEnabled = setting.IsSucceeded;
            this._setting = setting.Data;
            this._defaultVal = defaultVal; 
        }

        public SettingInfoViewModel(ISettingInfo<T> setting)
        {
            this.IsEnabled = true;
            this._setting = setting;
            this._defaultVal = setting.Value;
        }

        #region field

        protected readonly T _defaultVal;

        protected readonly ISettingInfo<T>? _setting;

        #endregion


        #region Props

        /// <summary>
        /// 設定値
        /// </summary>
        public virtual T Value
        {
            get => this._setting is null ? this._defaultVal : this._setting.Value;
            set
            {
                if (this._setting is null) return;
                this._setting.Value = value;
                this.OnPropertyChanged();
            }
        }

        #endregion
    }

    public class SettingInfoViewModelD<T>
    {
        public SettingInfoViewModelD(T value)
        {
            this.Value = value;
        }

        public T Value { get; init; }

        public bool IsEnabled => true;
    }

}
