using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Niconicome.ViewModels.Setting.Utils
{
    public class SettingInfoViewModelBase<T> : BindableBase where T : notnull
    {
        #region field

        private Action<T>? _handler;

        #endregion

        #region Props

        /// <summary>
        /// 設定が有効であるかどうか
        /// </summary>
        public bool IsEnabled { get; init; }

        #endregion

        #region Method

        /// <summary>
        /// 値の変更を監視する
        /// </summary>
        /// <param name="handler"></param>
        public void RegisterPropChangeHandler(Action<T> handler)
        {
            this._handler += handler;
        }

        #endregion

        #region private

        protected void OnChange(T value)
        {
            this._handler?.Invoke(value);
        }

        #endregion
    }
}
