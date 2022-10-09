using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Local.Addon;
using Niconicome.Models.Local.Application;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Local.State;

namespace Niconicome.Workspaces
{
    public static class AddonPage
    {
        public static IAddonHandler AddonHandler { get; private set; } = DIFactory.Provider.GetRequiredService<IAddonHandler>();

        public static IAddonInstallManager InstallManager { get; private set; } = DIFactory.Provider.GetRequiredService<IAddonInstallManager>();

        public static ILocalSettingsContainer SettingsContainer { get; private set; } = DIFactory.Provider.GetRequiredService<ILocalSettingsContainer>();

        public static IApplicationPowerManager PowerManager { get; private set; } = DIFactory.Provider.GetRequiredService<IApplicationPowerManager>();

        public static ISnackbarHandler Queue { get; private set; } = Mainpage.SnackbarHandler.CreateNewHandler();

        /// <summary>
        /// ローカル情報
        /// </summary>
        public static ILocalState LocalState { get; private set; } = DIFactory.Provider.GetRequiredService<ILocalState>();

        /// <summary>
        /// ページ遷移管理クラス
        /// </summary>
        public static IBlazorPageManager BlazorPageManager { get; private set; }=DIFactory.Provider.GetRequiredService<IBlazorPageManager>();
    }
}
