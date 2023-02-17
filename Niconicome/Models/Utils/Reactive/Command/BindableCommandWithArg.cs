using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Niconicome.Models.Utils.Reactive.Command
{
    public interface IBindableCommand<T> : ICommand
    {

    }

    public class BindableCommand<T> : IBindableCommand<T>
    {
        public BindableCommand(Action<T> command)
        {
            this._source = new List<IBindableProperty<bool>>().AsReadOnly();
            this._command = command;
        }

        public BindableCommand(Action<T> command, IBindableProperty<bool> source)
        {
            this._source = new List<IBindableProperty<bool>>() { source }.AsReadOnly();
            this._command = command;
            this.Initialize();
        }

        public BindableCommand(Action<T> command, params IBindableProperty<bool>[] source)
        {
            this._source = new List<IBindableProperty<bool>>(source).AsReadOnly();
            this._command = command;
            this.Initialize();
        }

        #region ICommand

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => this._canExecute.Value;

        public void Execute(object? parameter)
        {
            if (parameter is not null and T param)
            {
                try
                {
                    this._command(param);
                }
                catch { }
            }
        }

        #endregion

        #region field

        private readonly IReadOnlyCollection<IBindableProperty<bool>> _source;

        private Action<T> _command;

        private readonly IBindableProperty<bool> _canExecute = new BindableProperty<bool>(true);

        private SynchronizationContext? _ctx;

        #endregion

        #region private

        private void Initialize()
        {
            foreach (var source in this._source)
            {
                source.Subscribe(this.OnChange);
            }

            this._ctx = SynchronizationContext.Current;

            this._canExecute.Subscribe(_ =>
            {
                this._ctx?.Post(_ =>
                {
                    try
                    {
                        this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                    }
                    catch { }
                }, null);
            });
        }

        private bool CheckIfAllTrue()
        {
            foreach (var source in this._source)
            {
                if (!source.Value) return false;
            }

            return true;
        }

        private void OnChange(bool value)
        {
            bool canExec = this.CheckIfAllTrue();
            if (this._canExecute.Value != canExec)
            {
                this._canExecute.Value = canExec;
            }
        }

        #endregion
    }
}
