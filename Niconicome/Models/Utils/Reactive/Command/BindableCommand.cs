using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Niconicome.Models.Utils.Reactive.Command
{

    public interface IBindableCommand : ICommand
    {

    }

    public class BindableCommand : IBindableCommand
    {
        public BindableCommand(Action command, IBindableProperty<bool> source)
        {
            this._source = new List<IBindableProperty<bool>>() { source }.AsReadOnly();
            this._command = command;
            this.Initialize();
        }

        public BindableCommand(Action command, params IBindableProperty<bool>[] source)
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
            try
            {
                this._command();
            }
            catch { }
        }

        #endregion

        #region field

        private readonly IReadOnlyCollection<IBindableProperty<bool>> _source;

        private Action _command;

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
