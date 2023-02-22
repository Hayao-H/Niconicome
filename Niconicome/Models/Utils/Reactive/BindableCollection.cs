using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions;
using Reactive.Bindings.Extensions;

namespace Niconicome.Models.Utils.Reactive
{
    public class BindableCollection<TItem, TOrigin> : ObservableCollection<TItem>, IBindable
    {
        public BindableCollection(ObservableCollection<TOrigin> baseCollection, Func<TOrigin, TItem> converter)
        {
            this._baseCollection = baseCollection;
            this._converter = converter;
            this.AddRange(baseCollection.Select(x => converter(x)));

            baseCollection.CollectionChanged += this.OnBaseCollecitonChanged;
        }

        public BindableCollection(ReadOnlyObservableCollection<TOrigin> baseCollection, Func<TOrigin, TItem> converter)
        {
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
            lock(this._lockObj)
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
                    foreach (var item in this._baseCollection.As<IEnumerable<TOrigin>>()) this.Add(this._converter(item));
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
        }

        #endregion

        public void Dispose()
        {
            this._handler = null;
        }
    }
}
