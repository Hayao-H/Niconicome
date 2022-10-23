using System;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Local.Addon.V2;
using Niconicome.Models.Local.Application;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Local.State;
using Niconicome.Models.Utils;

namespace Niconicome.Workspaces
{
    public static class AddonPage
    {
        /// <summary>
        /// ローカル情報
        /// </summary>
        public static ILocalState LocalState { get; private set; } = DIFactory.Provider.GetRequiredService<ILocalState>();

        /// <summary>
        /// ページ遷移管理クラス
        /// </summary>
        public static IBlazorPageManager BlazorPageManager { get; private set; }=DIFactory.Provider.GetRequiredService<IBlazorPageManager>();

        /// <summary>
        /// アドオン情報
        /// </summary>
        public static IAddonStatusContainer AddonStatusContainer { get; private set; } = DIFactory.Provider.GetRequiredService<IAddonStatusContainer>();

        /// <summary>
        /// インストーラー
        /// </summary>
        public static IAddonInstallManager AddonInstallManager { get; private set; } = DIFactory.Provider.GetRequiredService<IAddonInstallManager>();

        /// <summary>
        /// Blazorヘルパークラス
        /// </summary>
        public static IBlazorHelper BlazorHelper { get; private set; } = DIFactory.Provider.GetRequiredService<IBlazorHelper>();

        /// <summary>
        /// マネージャー
        /// </summary>
        public static IAddonManager AddonManager { get; private set; } = DIFactory.Provider.GetRequiredService<IAddonManager>();
    }
}
