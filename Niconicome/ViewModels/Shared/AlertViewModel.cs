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
            this.AlertTypeClass = new BindableProperty<string>(string.Empty).AddTo(this._alertBindables);
            this.AlertContent = new BindableProperty<string>(string.Empty).AddTo(this._alertBindables);
        }

        protected Bindables _alertBindables = new();

        public IBindableProperty<string> AlertVisibilityClass { get; init; }

        public IBindableProperty<string> AlertTypeClass { get; init; }

        public IBindableProperty<string> AlertContent { get; init; }

        protected void ShowAlert(string content, AlertType type)
        {
            this.AlertVisibilityClass.Value = string.Empty;
            this.AlertContent.Value = content;
            this.AlertTypeClass.Value = type switch
            {
                AlertType.Error => "alert-danger",
                _ => "alert-info"
            };

            var timer = new Timer(5 * 1000);
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
