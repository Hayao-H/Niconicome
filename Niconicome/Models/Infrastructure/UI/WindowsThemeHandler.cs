using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Local.Application;
using Niconicome.Models.Utils;
using Windows.UI.ViewManagement;

namespace Niconicome.Models.Infrastructure.UI
{
    public class WindowsThemeHandler : IOSThemeHandler
    {
        public WindowsThemeHandler()
        {
            this.Initialize();
        }

        private UISettings? _settings;

        public bool IsDarkMode
        {
            get
            {
                if (this._settings is null) return false;
                return this._settings.GetColorValue(UIColorType.Background) == Windows.UI.Color.FromArgb(255,0,0,0);
            }
        }

        public event EventHandler? ThemeChanged;

        public bool FunctionHasCompatibility => Compatibility.IsOsVersionLargerThan(10, 0, 10240);

        /// <summary>
        /// 初期化
        /// </summary>
        private void Initialize()
        {
            if (this.FunctionHasCompatibility)
            {
                this._settings = new UISettings();
                this._settings.ColorValuesChanged += (sender, e) =>
                {
                    this.ThemeChanged?.Invoke(this, new EventArgs());
                };
            }
        }
    }
}
