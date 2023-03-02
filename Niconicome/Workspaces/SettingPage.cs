using Microsoft.Extensions.DependencyInjection;
using Niconicome.Models.Auth;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Domain.Utils.StringHandler;
using Niconicome.Models.Local.Application;
using Niconicome.Models.Local.Restore;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Local.State;
using Niconicome.Models.Playlist.Playlist;
using Niconicome.Models.Playlist.VideoList;
using MaterialDesign = MaterialDesignThemes.Wpf;

namespace Niconicome.Workspaces
{
    static class SettingPage
    {
        public static ILocalState State { get; private set; } = DIFactory.Provider.GetRequiredService<ILocalState>();
        public static ILocalInfo LocalInfo { get; private set; } = DIFactory.Provider.GetRequiredService<ILocalInfo>();
        public static IRestoreManager Restore { get; private set; } = DIFactory.Provider.GetRequiredService<IRestoreManager>();
        public static MaterialDesign::ISnackbarMessageQueue SnackbarMessageQueue { get; private set; } = new MaterialDesign::SnackbarMessageQueue();
        public static IPlaylistHandler PlaylistTreeHandler { get; private set; } = DIFactory.Provider.GetRequiredService<IPlaylistHandler>();
        public static IMessageHandler MessageHandler { get; private set; } = DIFactory.Provider.GetRequiredService<IMessageHandler>();
        public static IVideoListContainer VideoListContainer { get; private set; } = DIFactory.Provider.GetRequiredService<IVideoListContainer>();
        public static IAutoLogin AutoLogin { get; private set; } = DIFactory.Provider.GetRequiredService<IAutoLogin>();
        public static IThemehandler Themehandler { get; private set; } = DIFactory.Provider.GetRequiredService<IThemehandler>();
        public static IStyleHandler StyleHandler { get; private set; } = DIFactory.Provider.GetRequiredService<IStyleHandler>();
        public static IApplicationPowerManager PowerManager { get; private set; } = DIFactory.Provider.GetRequiredService<IApplicationPowerManager>();

        /// <summary>
        /// 設定情報
        /// </summary>
        public static ISettingsContainer SettingsConainer { get; private set; } = DIFactory.Resolve<ISettingsContainer>();

        /// <summary>
        /// 文字列生成
        /// </summary>
        public static IStringHandler StringHandler { get; private set; } = DIFactory.Resolve<IStringHandler>();
    }
}
