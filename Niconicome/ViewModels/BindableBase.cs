using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Niconicome.ViewModels
{
    public class BindableBase : INotifyPropertyChanged
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
