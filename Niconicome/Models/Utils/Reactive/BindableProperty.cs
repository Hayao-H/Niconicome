using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Utils.Reactive
{

    public interface IBindableProperty<T> : IBindable
    {
        /// <summary>
        /// 値
        /// </summary>
        T Value { get; set; }

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

        /// <summary>
        /// Bndablesクラスで管理
        /// </summary>
        /// <param name="bindables"></param>
        /// <returns></returns>
        BindableProperty<T> AddTo(Bindables bindables);

    }

    public class BindableProperty<T> : BindablePropertyBase, INotifyPropertyChanged, IBindableProperty<T>
    {
        public BindableProperty(T initialValue)
        {
            this._value = initialValue;
        }

        ~BindableProperty()
        {
            this.Dispose();
        }

        #region field

        private bool _hasDisposed;

        private readonly List<Action<T>> _handlers = new();

        private T _value;

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region Method

        public void RegisterPropertyChangeHandler(Action<T> handler)
        {
            this._handlers.Add(handler);
        }

        public void UnRegisterPropertyChangeHandler(Action<T> handler)
        {
            this._handlers.RemoveAll(x => x == handler);
        }

        public BindableProperty<T> AddTo(Bindables bindables)
        {
            bindables.Add(this);
            return this;
        }


        #endregion

        #region Props

        public T Value
        {
            get => this._value;
            set
            {
                this._value = value;
                this.OnPropertyChanged();
            }
        }

        #endregion

        #region private

        /// <summary>
        /// プロパティの変更をハンドルする
        /// </summary>
        /// <param name="name"></param>
        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            base.OnPropertyChanged();

            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            try
            {
                foreach (var handler in this._handlers)
                {
                    handler(this._value);
                };
            }
            catch { }
        }

        #endregion

        public override void Dispose()
        {
            if (this._hasDisposed) return;
            this._hasDisposed = true;

            this._handlers.Clear();
            base.Dispose();
            GC.SuppressFinalize(this);
        }

    }
}
