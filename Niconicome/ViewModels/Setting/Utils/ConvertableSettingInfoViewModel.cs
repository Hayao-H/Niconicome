using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Helper.Result;

namespace Niconicome.ViewModels.Setting.Utils
{
    public class ConvertableSettingInfoViewModel<TModel, TView> : SettingInfoViewModelBase<TView> where TModel : notnull where TView : notnull
    {
        public ConvertableSettingInfoViewModel(IAttemptResult<ISettingInfo<TModel>> setting, TView defaultVal, Func<TModel, TView> convertback, Func<TView, TModel> convert)
        {
            this.IsEnabled = setting.IsSucceeded;
            this._defaultVal = defaultVal;
            this._setting = setting.Data;
            this._convertback = convertback;
            this._convert = convert;
        }
        public ConvertableSettingInfoViewModel(ISettingInfo<TModel> setting, Func<TModel, TView> convertback, Func<TView, TModel> convert)
        {
            this.IsEnabled = true;
            this._defaultVal = convertback(setting.Value);
            this._setting = setting;
            this._convertback = convertback;
            this._convert = convert;
        }

        #region field

        private readonly ISettingInfo<TModel>? _setting;

        private readonly TView _defaultVal;

        private Func<TModel, TView> _convertback;

        private Func<TView, TModel> _convert;

        #endregion

        #region Props

        /// <summary>
        /// 設定値
        /// </summary>
        public TView Value
        {
            get => this._setting is null ? this._defaultVal : this._convertback(this._setting.Value);
            set
            {
                if (this._setting is null) return;
                this._setting.Value = this._convert(value);
                this.OnChange(value);
                this.OnPropertyChanged();
            }
        }


        #endregion
    }

    public class ConvertableSettingInfoViewModelD<T>
    {
        public ConvertableSettingInfoViewModelD(T value)
        {
            this.Value = value;
        }


        public T Value { get; init; }

        public bool IsEnabled => true;
    }
}
