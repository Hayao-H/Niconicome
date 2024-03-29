﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Niconicome.Extensions;
using Reactive.Bindings.Extensions;

namespace Niconicome.Models.Utils.Reactive
{
    public class BindableCollection<TItem, TOrigin> : ObservableCollection<TItem>, IBindable, IDisposable
    {
        public BindableCollection(ObservableCollection<TOrigin> baseCollection, Func<TOrigin, TItem> converter)
        {
            this._ctx = SynchronizationContext.Current;
            this._baseCollection = baseCollection;
            this._converter = converter;
            this.AddRange(baseCollection.Select(x => converter(x)));

            baseCollection.CollectionChanged += this.OnBaseCollecitonChanged;
        }

        public BindableCollection(ReadOnlyObservableCollection<TOrigin> baseCollection, Func<TOrigin, TItem> converter)
        {
            this._ctx = SynchronizationContext.Current;
            this._converter = converter;
            this._baseCollection = baseCollection;
            this.AddRange(baseCollection.Select(x => converter(x)));

            baseCollection.As<INotifyCollectionChanged>().CollectionChanged += this.OnBaseCollecitonChanged;
        }

        public void RegisterPropertyChangeHandler(Action handler)
        {
            this._handler += handler;
        }

        public void UnRegisterPropertyChangeHandler(Action handler)
        {
            this._handler -= handler;
        }

        #region field

        private readonly object _baseCollection;

        private readonly Func<TOrigin, TItem> _converter;

        private Action? _handler;

        private readonly object _lockObj = new object();

        private readonly SynchronizationContext? _ctx;

        #endregion

        #region Method

        /// <summary>
        /// 読み取り専用のコレクションを取得する
        /// </summary>
        /// <returns></returns>
        public ReadOnlyObservableCollection<TItem> AsReadOnly()
        {
            return new ReadOnlyObservableCollection<TItem>(this);
        }

        #endregion

        #region private

        private void OnBaseCollecitonChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            this._ctx?.Post(_ =>
            {
                lock (this._lockObj)
                {
                    if (e.Action == NotifyCollectionChangedAction.Add)
                    {
                        if (e.NewItems?[0] is TOrigin item)
                        {
                            TItem converted = this._converter(item);
                            if (converted is null)
                            {
                                return;
                            }
                            else
                            {
                                this.Add(converted);
                            }
                        }
                    }
                    else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldStartingIndex >= 0)
                    {
                        this.RemoveAt(e.OldStartingIndex);
                    }
                    else if (e.Action == NotifyCollectionChangedAction.Replace)
                    {
                        if (e.NewItems?[0] is TOrigin item) this[e.OldStartingIndex] = this._converter(item);
                    }
                    else if (e.Action == NotifyCollectionChangedAction.Move)
                    {
                        this.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.Action == NotifyCollectionChangedAction.Reset)
                    {
                        this.Clear();
                    }
                    else
                    {
                        return;
                    }

                    try
                    {
                        this._handler?.Invoke();
                    }
                    catch { }
                }
            }, null);
        }

        #endregion

        public void Dispose()
        {
            this._handler = null;
            this._baseCollection.As<INotifyCollectionChanged>().CollectionChanged -= this.OnBaseCollecitonChanged;
        }
    }
}
