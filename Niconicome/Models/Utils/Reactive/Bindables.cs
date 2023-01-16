using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Utils.Reactive
{
    public class Bindables : IDisposable, IBindable
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

        public void RegisterPropertyChangeHandler(Action handler)
        {
            this._handlers.Add(handler);
        }

        public void UnRegisterPropertyChangeHandler(Action handler)
        {
            this._handlers.Remove(handler);
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
            foreach (var bindable in this._bindables)
            {
                bindable.UnRegisterPropertyChangeHandler(this.OnProprtyChange);
            }

            this._bindables.Clear();
        }
    }
}
