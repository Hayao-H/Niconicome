using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Niconicome.Models.Utils.Reactive;

namespace Niconicome.ViewModels.Shared
{
    public class AlertViewModel
    {
        public AlertViewModel()
        {
            this.AlertVisibilityClass = new BindableProperty<string>("Hide").AddTo(this._alertBindables);
            this.AlertButtonVisibilityClass = new BindableProperty<string>("Hide").AddTo(this._alertBindables);
            this.AlertTypeClass = new BindableProperty<string>(string.Empty).AddTo(this._alertBindables);
            this.AlertContent = new BindableProperty<string>(string.Empty).AddTo(this._alertBindables);
            this.AlertButtonContent = new BindableProperty<string>(string.Empty).AddTo(this._alertBindables);
        }

        protected Bindables _alertBindables = new();

        private Action? _action;

        public IBindableProperty<string> AlertVisibilityClass { get; init; }

        public IBindableProperty<string> AlertTypeClass { get; init; }

        public IBindableProperty<string> AlertContent { get; init; }

        public IBindableProperty<string> AlertButtonVisibilityClass { get; init; }

        public IBindableProperty<string> AlertButtonContent { get; init; }

        public void ExecuteAlertCommand()
        {
            if (this._action is null) return;
            this._action();
        }

        protected void ShowAlert(string content, AlertType type, Action? action = null, string buttonContent = "")
        {
            if (action is not null)
            {
                this.AlertButtonVisibilityClass.Value = string.Empty;
                this._action = action;
                this.AlertButtonContent.Value = buttonContent;
            }
            else
            {
                this.AlertButtonVisibilityClass.Value = "Hide";
                this.AlertButtonContent.Value = string.Empty;
            }

            this.AlertVisibilityClass.Value = string.Empty;
            this.AlertContent.Value = content;
            this.AlertTypeClass.Value = type switch
            {
                AlertType.Error => "alert-danger",
                _ => "alert-info"
            };

            var timer = new Timer(3 * 1000);
            timer.AutoReset = false;
            timer.Elapsed += (_, _) =>
            {
                this.AlertVisibilityClass.Value = "Hide";
            };
            timer.Enabled = true;
        }

        protected enum AlertType
        {
            Info,
            Error,
        }
    }
}
