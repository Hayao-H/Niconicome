using System;

namespace Niconicome.Models.Helper.Event.Generic
{

    public enum ChangeType
    {
        Add,
        Remove,
        Clear,
        Overall,
    }

    public class ListChangedEventArgs<T> : EventArgs
    {
        public ListChangedEventArgs(T? data, ChangeType changeType)
        {
            this.Data = data;
            this.ChangeType = changeType;
        }

        public T? Data { get; init; }

        public ChangeType ChangeType { get; init; }
    }
}
