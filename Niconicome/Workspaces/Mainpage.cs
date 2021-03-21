using Niconicome.Models.Auth;
using Niconicome.Models.Playlist;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Network;
using Microsoft.Extensions.DependencyInjection;
using Niconicome.Models.Local;
using MaterialDesign = MaterialDesignThemes.Wpf;
using Niconicome.Models.Local.Application;
using Niconicome.Models.Local.State;
using Niconicome.Models.Network.Download;

namespace Niconicome.Workspaces
{
    static class Mainpage
    {
        public static ISession Session { get; private set; } = DIFactory.Provider.GetRequiredService<ISession>();
        public static IPlaylistVideoHandler PlaylistTree { get; private set; } = DIFactory.Provider.GetRequiredService<IPlaylistVideoHandler>();
        public static IVideoHandler VideoHandler { get; private set; } = DIFactory.Provider.GetRequiredService<IVideoHandler>();
        public static ICurrent CurrentPlaylist { get; private set; } = DIFactory.Provider.GetRequiredService<ICurrent>();
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
    }
}
