using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Reactive.Disposables;

namespace Niconicome.ViewModels
{
    public class BindableBase : INotifyPropertyChanged, IDisposable
    {
        ~BindableBase()
        {
            this.Dispose();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? name = null)
        {
            if (field?.Equals(value) ?? false) return false;
            field = value;
            this.OnPropertyChanged(name);
            return true;
        }

        protected CompositeDisposable disposables = new();

        protected bool hasDisposed;

        public virtual void Dispose()
        {
            if (this.hasDisposed) return;
            this.disposables.Dispose();
            this.hasDisposed = true;
            GC.SuppressFinalize(this);
        }
    }

    public record BindableRecordBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? name = null)
        {
            if (field?.Equals(value) ?? false) return false;
            field = value;
            this.OnPropertyChanged(name);
            return true;
        }
    }
}
