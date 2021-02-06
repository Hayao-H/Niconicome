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

        protected virtual void OnPropertyChanged([CallerMemberName] String name = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName] String name = null)
        {
            if (field?.Equals(value) ?? false) return false;
            field = value;
            this.OnPropertyChanged(name);
            return true;
        }
    }
}
