using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Utils.Reactive
{
    public interface IBindable : IDisposable
    {
        /// <summary>
        /// 値の変更を監視する
        /// </summary>
        /// <param name="handler"></param>
        void RegisterPropertyChangeHandler(Action handler);

        /// <summary>
        /// 値の変更の監視を停止する
        /// </summary>
        /// <param name="handler"></param>
        void UnRegisterPropertyChangeHandler(Action handler);

    }

    public interface IBindablePropertyBase<T> : IBindable
    {
        /// <summary>
        /// 値の変更を監視する
        /// </summary>
        /// <param name="handler"></param>
        void RegisterPropertyChangeHandler(Action<T> handler);

        /// <summary>
        /// 値の変更の監視を停止する
        /// </summary>
        /// <param name="handler"></param>
        void UnRegisterPropertyChangeHandler(Action<T> handler);
    }

    public class BindablePropertyBase<T> : IBindablePropertyBase<T>, INotifyPropertyChanged
    {
        public BindablePropertyBase(T initialValue)
        {
            this._value = initialValue;
        }

        #region field

        private readonly List<Action> _nonParamHandlers = new();

        private readonly List<Action<T>> _handlers = new();

        protected T _value;

        #endregion

        #region Method

        public void RegisterPropertyChangeHandler(Action handler)
        {
            this._nonParamHandlers.Add(handler);
        }

        public void UnRegisterPropertyChangeHandler(Action handler)
        {
            this._nonParamHandlers.Remove(handler);
        }

        public void RegisterPropertyChangeHandler(Action<T> handler)
        {
            this._handlers.Add(handler);
        }

        public void UnRegisterPropertyChangeHandler(Action<T> handler)
        {
            this._handlers.RemoveAll(x => x == handler);
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region private

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

            try
            {
                foreach (var handler in this._handlers)
                {
                    handler(this._value);
                };
            }
            catch { }

            try
            {
                foreach (var handler in this._nonParamHandlers)
                {
                    handler();
                }
            }
            catch { }
        }
        #endregion

        public virtual void Dispose()
        {
            this._nonParamHandlers.Clear();
            this._handlers.Clear();
        }

    }
}
