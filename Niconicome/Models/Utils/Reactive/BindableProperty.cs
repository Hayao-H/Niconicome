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

        public　IReadonlyBindablePperty<T> AsReadOnly()
        {
            return new ReadonlyBindablePperty<T>(this);
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

        public override void Dispose()
        {
            if (this._hasDisposed) return;
            this._hasDisposed = true;

            base.Dispose();
            GC.SuppressFinalize(this);
        }

    }
}
