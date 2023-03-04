using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Niconicome.Models.Utils.Reactive
{
    public class Bindables : IDisposable, IBindable
    {
        #region field

        private readonly List<IBindable> _bindables = new();

        private readonly List<Action> _handlers = new();

        private Timer? _timer;

        private object _lock = new object();

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
            lock (this._lock)
            {
                if (this._timer is not null)
                {
                    this._timer.Stop();
                }

                this._timer = new Timer(1000);
                this._timer.AutoReset = false;
                this._timer.Elapsed += (_, _) =>
                {
                    try
                    {
                        foreach (var handler in this._handlers)
                        {
                            handler();
                        }

                        this._timer = null;
                    }
                    catch { }
                };

                this._timer.Enabled = true;
            }
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
