using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Utils.Reactive
{
    public class Bindables : IDisposable
    {
        #region field

        private readonly List<IBindable> _bindables = new();

        private readonly List<Action> _handlers = new();

        #endregion

        #region Method

        /// <summary>
        /// IBindableを追加
        /// </summary>
        /// <param name="bindable"></param>
        public void Add(IBindable bindable)
        {
            this._bindables.Add(bindable);
            bindable.RegisterPropertyChangeHandler(this.OnProprtyChange);
        }

        /// <summary>
        /// 値の変更を一括で監視する
        /// </summary>
        /// <param name="handler"></param>
        public void RegisterPropertyChangeHandler(Action handler)
        {
            this._handlers.Add(handler);
        }

        #endregion

        #region private

        private void OnProprtyChange()
        {
            try
            {
                foreach (var handler in this._handlers)
                {
                    handler();
                }
            }
            catch { }
        }

        #endregion
        public void Dispose()
        {
            foreach (var bindable in this._bindables) {
                bindable.Dispose();
            }

            this._bindables.Clear();
        }
    }
}
