using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xaml.Behaviors;
using Niconicome.Extensions.System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace Niconicome.ViewModels.Setting
{
    /// <summary>
    /// ハイパーリンクを機能させる
    /// </summary>
    class HyperlinkBehavior : Behavior<Hyperlink>
    {

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.RequestNavigate += this.OnRequest;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.RequestNavigate -= this.OnRequest;
        }

        private void OnRequest(object? sender, RequestNavigateEventArgs e)
        {
            e.Handled = true;
            ProcessEx.StartWithShell(e.Uri.AbsoluteUri);
        }
    }
}
