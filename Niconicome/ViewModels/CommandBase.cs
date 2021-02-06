using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Diagnostics;

namespace Niconicome.ViewModels
{
    class CommandBase<T> : ICommand
    {
        public CommandBase(Func<object?,bool> canExecute,Action<T?> execute)
        {
            this.canExecute = canExecute;
            this.execute = execute;
        }

        private readonly Func<object?,bool> canExecute;

        private readonly Action<T?> execute;

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? param)
        {
                return this.canExecute?.Invoke((object?)param) ?? true;

        }
        public void Execute(object? param)
        {
            this.execute((T)param);
        }

        public void RaiseCanExecutechanged()
        {
            this.CanExecuteChanged?.Invoke(this,EventArgs.Empty);
        }
    }
}
