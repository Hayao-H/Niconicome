using System;

namespace Niconicome.Models.Utils.Reactive
{

    public interface IBindableProperty<T> : IBindablePropertyBase<T>, IDisposable
    {
        /// <summary>
        /// 値
        /// </summary>
        T Value { get; set; }

        /// <summary>
        /// 値の変更を監視する
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        IBindableProperty<T> Subscribe(Action<T> handler);

        /// <summary>
        /// Bndablesクラスで管理
        /// </summary>
        /// <param name="bindables"></param>
        /// <returns></returns>
        IBindableProperty<T> AddTo(Bindables bindables);


        /// <summary>
        /// Read-Only化
        /// </summary>
        /// <returns></returns>
        IReadonlyBindablePperty<T> AsReadOnly();

        /// <summary>
        /// プロパティーを変換
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        IBindableProperty<U> Select<U>(Func<T, U> selector);

        /// <summary>
        /// 変更を同期するプロパティーを追加
        /// </summary>
        /// <returns></returns>
        IBindableProperty<T> CreateSyncedProperty();

        /// <summary>
        /// 購読解除
        /// </summary>
        /// <param name="handler"></param>
        void UnSubscribe(Action<T> handler);

    }



    public class BindableProperty<T> : BindablePropertyBase<T>, IBindableProperty<T>
    {
        public BindableProperty(T initialValue) : base(initialValue)
        {
        }

        ~BindableProperty()
        {
            this.Dispose();
        }

        #region field

        private bool _hasDisposed;

        #endregion

        #region Method

        public IBindableProperty<T> AddTo(Bindables bindables)
        {
            bindables.Add(this);
            return this;
        }

        public IBindableProperty<T> Subscribe(Action<T> handler)
        {
            this.RegisterPropertyChangeHandler(handler);
            return this;
        }

        public IReadonlyBindablePperty<T> AsReadOnly()
        {
            return new ReadonlyBindablePperty<T>(this);
        }

        public IBindableProperty<U> Select<U>(Func<T, U> selector)
        {
            var p = new BindableProperty<U>(selector(this.Value));

            Action<T> subscriber = x => p.Value = selector(x);

            this.Subscribe(subscriber);
            p.PropertyDisposed += (_, _) => this.UnSubscribe(subscriber);

            return p;
        }

        public IBindableProperty<T> CreateSyncedProperty()
        {
            var p = new BindableProperty<T>(this.Value);

            Action<T> subscriber = x =>
            {
                if (x?.Equals(p.Value) ?? false) return;
                p.Value = x;
            };

            this.Subscribe(subscriber);
            p.PropertyDisposed += (_, _) => this.UnSubscribe(subscriber);

            p.Subscribe(x => this.Value = x);

            return p;
        }


        public void UnSubscribe(Action<T> handler)
        {
            this.UnRegisterPropertyChangeHandler(handler);
        }


        #endregion

        #region Props

        public event EventHandler? PropertyDisposed;


        public virtual T Value
        {
            get => this._value;
            set
            {
                this._value = value;
                this.OnPropertyChanged(value);
            }
        }

        #endregion

        public override void Dispose()
        {
            if (this._hasDisposed) return;
            this._hasDisposed = true;

            this.PropertyDisposed?.Invoke(this, EventArgs.Empty);

            base.Dispose();
            GC.SuppressFinalize(this);
        }

    }
}
