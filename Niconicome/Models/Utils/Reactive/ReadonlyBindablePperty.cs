using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Utils.Reactive
{
    public interface IReadonlyBindablePperty<T> : IBindablePropertyBase<T>
    {
        /// <summary>
        /// 値
        /// </summary>
        T Value { get; }

        /// <summary>
        /// Bndablesクラスで管理
        /// </summary>
        /// <param name="bindables"></param>
        /// <returns></returns>
        IReadonlyBindablePperty<T> AddTo(Bindables bindables);


        /// <summary>
        /// Disposableクラスで管理
        /// </summary>
        /// <param name="bindables"></param>
        /// <returns></returns>
        IReadonlyBindablePperty<T> AddTo(Disposable disposable);

        /// <summary>
        /// 値の変更を監視する
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        IReadonlyBindablePperty<T> Subscribe(Action<T> handler);

        /// <summary>
        /// 購読解除
        /// </summary>
        /// <param name="handler"></param>
        void UnSubscribe(Action<T> handler);

        /// <summary>
        /// プロパティーを変換
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        IReadonlyBindablePperty<U> Select<U>(Func<T, U> selector);
    }


    public class ReadonlyBindablePperty<T> : BindablePropertyBase<T>, IReadonlyBindablePperty<T>, IDisposable
    {
        public ReadonlyBindablePperty(IBindableProperty<T> baseProperty) : base(baseProperty.Value)
        {
            this._base = baseProperty;
            this._changeHandler = () => this.OnPropertyChanged(this.Value, nameof(this.Value));
            baseProperty.RegisterPropertyChangeHandler(this._changeHandler);
        }

        ~ReadonlyBindablePperty()
        {
            this.Dispose();
        }

        #region field

        private readonly IBindableProperty<T> _base;

        private bool _hasDisposed;

        private Action _changeHandler;

        #endregion

        #region Props

        public T Value => this._base.Value;

        #endregion

        #region Method

        public IReadonlyBindablePperty<T> AddTo(Bindables bindables)
        {
            bindables.Add(this);
            return this;
        }

        public IReadonlyBindablePperty<T> AddTo(Disposable disposable)
        {
            disposable.Add(this);
            return this;
        }

        public IReadonlyBindablePperty<T> Subscribe(Action<T> handler)
        {
            this.RegisterPropertyChangeHandler(handler);
            return this;
        }

        public IReadonlyBindablePperty<U> Select<U>(Func<T, U> selector)
        {
            var p = new BindableProperty<U>(selector(this.Value));

            Action<T> subscriber = x => p.Value = selector(x);

            this.Subscribe(subscriber);
            p.PropertyDisposed += (_, _) => this.UnRegisterPropertyChangeHandler(subscriber);

            return p.AsReadOnly();
        }

        public void UnSubscribe(Action<T> handler)
        {
            this.UnRegisterPropertyChangeHandler(handler);
        }


        #endregion

        public override void Dispose()
        {
            if (this._hasDisposed) return;
            this._hasDisposed = true;

            base.Dispose();
            this._base.UnRegisterPropertyChangeHandler(this._changeHandler);
            GC.SuppressFinalize(this);
        }
    }
}
