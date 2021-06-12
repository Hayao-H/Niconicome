using Microsoft.Extensions.DependencyInjection;
using Niconicome.Models.Auth;
using Niconicome.Models.Playlist.Playlist;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Local;
using Niconicome.Models.Local.Application;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Local.State;
using Niconicome.Models.Network;
using Niconicome.Models.Network.Download;
using Niconicome.Models.Network.Watch;
using Niconicome.Models.Playlist;
using Ext = Niconicome.Models.Local.External;
using VideoList = Niconicome.Models.Playlist.VideoList;
using Niconicome.Models.Utils;

namespace Niconicome.Workspaces
{
    static class Mainpage
    {
        public static ISession Session { get; private set; } = DIFactory.Provider.GetRequiredService<ISession>();
        public static IPlaylistHandler PlaylistHandler { get; private set; } = DIFactory.Provider.GetRequiredService<IPlaylistHandler>();
        public static IVideoHandler VideoHandler { get; private set; } = DIFactory.Provider.GetRequiredService<IVideoHandler>();
        public static IWatch Watch { get; private set; } = DIFactory.Provider.GetRequiredService<IWatch>();
        public static IRemotePlaylistHandler RemotePlaylistHandler { get; private set; } = DIFactory.Provider.GetRequiredService<IRemotePlaylistHandler>();
        public static ILocalInfo LocalInfo { get; private set; } = DIFactory.Provider.GetRequiredService<ILocalInfo>();
        public static INetworkVideoHandler NetworkVideoHandler { get; private set; } = DIFactory.Provider.GetRequiredService<INetworkVideoHandler>();
        public static IMessageHandler Messagehandler { get; private set; } = DIFactory.Provider.GetRequiredService<IMessageHandler>();
        public static IContentDownloader Videodownloader { get; private set; } = DIFactory.Provider.GetRequiredService<IContentDownloader>();
        public static IVideoFilter VideoFilter { get; private set; } = DIFactory.Provider.GetRequiredService<IVideoFilter>();
        public static ILocalSettingHandler SettingHandler { get; private set; } = DIFactory.Provider.GetRequiredService<ILocalSettingHandler>();
        public static IPlaylistCreator PlaylistCreator { get; private set; } = DIFactory.Provider.GetRequiredService<IPlaylistCreator>();
        public static ISnackbarHandler SnaclbarHandler { get; private set; } = DIFactory.Provider.GetRequiredService<ISnackbarHandler>();
        public static IShutdown Shutdown { get; private set; } = DIFactory.Provider.GetRequiredService<IShutdown>();
        public static IStartUp StartUp { get; private set; } = DIFactory.Provider.GetRequiredService<IStartUp>();
        public static IVideoIDHandler VideoIDHandler { get; private set; } = DIFactory.Provider.GetRequiredService<IVideoIDHandler>();
        public static IDownloadTasksHandler DownloadTasksHandler { get; private set; } = DIFactory.Provider.GetRequiredService<IDownloadTasksHandler>();
        public static VideoList::ICurrent CurrentPlaylist { get; private set; } = DIFactory.Provider.GetRequiredService<VideoList::ICurrent>();
        public static VideoList::IVideoListContainer VideoListContainer { get; private set; } = DIFactory.Provider.GetRequiredService<VideoList::IVideoListContainer>();
        public static Ext::IExternalAppUtils ExternalAppUtils { get; private set; } = DIFactory.Provider.GetRequiredService<Ext::IExternalAppUtils>();
        public static IEnumSettingsHandler EnumSettingsHandler { get; private set; } = DIFactory.Provider.GetRequiredService<IEnumSettingsHandler>();
        public static IDownloadSettingsHandler DownloadSettingsHandler { get; private set; } = DIFactory.Provider.GetRequiredService<IDownloadSettingsHandler>();
        public static ISortInfoHandler SortInfoHandler { get; private set; } = DIFactory.Provider.GetRequiredService<ISortInfoHandler>();
        public static IPlaylistTreeHandler PlaylistTreeHandler { get; private set; } = DIFactory.Provider.GetRequiredService<IPlaylistTreeHandler>();
        public static IWindowsHelper WindowsHelper { get; private set; } = DIFactory.Provider.GetRequiredService<IWindowsHelper>();
        public static IThemehandler Themehandler { get; private set; } = DIFactory.Provider.GetRequiredService<IThemehandler>();
        public static IApplicationPowerManager ApplicationPower { get; private set; } = DIFactory.Provider.GetRequiredService<IApplicationPowerManager>();
    }
}
