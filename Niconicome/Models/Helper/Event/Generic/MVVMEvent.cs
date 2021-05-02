using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Niconicome.Models.Helper.Event.Generic
{
    public interface IViewModelEvent<T>
    {
        T Data { get; }
        Type TargetVMType { get; }
        bool CheckTarget(Type receiversType, object? sender = null);
    }

    class MVVMEvent<T> : IViewModelEvent<T>
    {
        public MVVMEvent(T data, Type targetType, Func<object, bool>? additionalChecker = null)
        {
            this.Data = data;
            this.TargetVMType = targetType;
            this.additionalChecker = additionalChecker;
        }

        public T Data { get; init; }

        public Type TargetVMType { get; init; }

        private readonly Func<object, bool>? additionalChecker;

        /// <summary>
        /// 自分が正しい受け取りてであるかどうかをチェックする
        /// </summary>
        /// <param name="receiversType"></param>
        /// <returns></returns>
        public bool CheckTarget(Type receiversType, object? sender = null)
        {
            if (receiversType != this.TargetVMType)
            {
                return false;
            }
            else if (this.additionalChecker is not null && sender is not null)
            {
                return this.additionalChecker(sender);
            }

            return true;
        }
    }
}
