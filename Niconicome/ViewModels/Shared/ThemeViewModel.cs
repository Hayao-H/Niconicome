using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Utils.Reactive;
using WS = Niconicome.Workspaces.Mainpage;

namespace Niconicome.ViewModels.Shared
{
    public class ThemeViewModel : IDisposable
    {
        public ThemeViewModel()
        {
            this.DarkModeClass = WS.Themehandler.IsDarkMode.Select((isDark) => isDark ? "dark" : "light").AddTo(this.Bindables);
        }

        public Bindables Bindables { get; init; } = new();

        public IBindableProperty<string> DarkModeClass { get; init; }

        public virtual void Dispose()
        {
            this.DarkModeClass.Dispose();
            this.Bindables.Dispose();
        }
    }
}
