using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;

namespace Niconicome.Models.Helper.Event.Generic
{
    public interface IViewModelEvent<T>
    {
        T Data { get; }
        Type TargetVMType { get; }
        bool CheckTarget(Type receiversType, object? sender = null);
        EventType EventType { get; }
    }

    class MVVMEvent<T> : IViewModelEvent<T>
    {
        public MVVMEvent(T data, Type targetType, EventType eventType = EventType.UnSpecified, Func<object, bool>? additionalChecker = null)
        {
            this.Data = data;
            this.TargetVMType = targetType;
            this.additionalChecker = additionalChecker;
            this.EventType = eventType;
        }

        /// <summary>
        /// データ
        /// </summary>
        public T Data { get; init; }

        /// <summary>
        /// 宛先のVM
        /// </summary>
        public Type TargetVMType { get; init; }

        public EventType EventType { get; init; }

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

    public enum EventType
    {
        UnSpecified,
        Download,
    }
}
