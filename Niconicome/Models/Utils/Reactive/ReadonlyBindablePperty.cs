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

    }


    public class ReadonlyBindablePperty<T> : BindablePropertyBase<T>, IReadonlyBindablePperty<T>, IDisposable
    {
        public ReadonlyBindablePperty(IBindableProperty<T> baseProperty) : base(baseProperty.Value)
        {
            this._base = baseProperty;
            this._changeHandler = () => this.OnPropertyChanged(nameof(this.Value));
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
