﻿using Microsoft.Extensions.DependencyInjection;
using Niconicome.Models.Auth;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Local;
using Niconicome.Models.Local.Addon.API.Local.Tab;
using Niconicome.Models.Local.Addon.V2;
using Niconicome.Models.Local.Application;
using Niconicome.Models.Local.OS;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Local.State;
using Niconicome.Models.Local.Timer;
using Niconicome.Models.Network;
using Niconicome.Models.Network.Download;
using Niconicome.Models.Network.Download.Actions;
using Niconicome.Models.Network.Download.DLTask;
using Niconicome.Models.Network.Fetch;
using Niconicome.Models.Network.Register;
using Niconicome.Models.Network.Watch;
using Niconicome.Models.Playlist;
using Niconicome.Models.Playlist.Playlist;
using Niconicome.Models.Utils;
using Niconicome.Models.Utils.InitializeAwaiter;
using Ext = Niconicome.Models.Local.External;
using VideoList = Niconicome.Models.Playlist.VideoList;
using Playlist = Niconicome.Models.Playlist.V2;

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
        public static IVideoFilter VideoFilter { get; private set; } = DIFactory.Provider.GetRequiredService<IVideoFilter>();
        public static ILocalSettingHandler SettingHandler { get; private set; } = DIFactory.Provider.GetRequiredService<ILocalSettingHandler>();
        public static IPlaylistCreator PlaylistCreator { get; private set; } = DIFactory.Provider.GetRequiredService<IPlaylistCreator>();
        public static ISnackbarHandler SnackbarHandler { get; private set; } = DIFactory.Provider.GetRequiredService<ISnackbarHandler>();
        public static IShutdown Shutdown { get; private set; } = DIFactory.Provider.GetRequiredService<IShutdown>();
        public static IStartUp StartUp { get; private set; } = DIFactory.Provider.GetRequiredService<IStartUp>();
        public static IVideoIDHandler VideoIDHandler { get; private set; } = DIFactory.Provider.GetRequiredService<IVideoIDHandler>();
        public static VideoList::ICurrent CurrentPlaylist { get; private set; } = DIFactory.Provider.GetRequiredService<VideoList::ICurrent>();
        public static VideoList::IVideoListContainer VideoListContainer { get; private set; } = DIFactory.Provider.GetRequiredService<VideoList::IVideoListContainer>();
        public static Ext::IExternalAppUtils ExternalAppUtils { get; private set; } = DIFactory.Provider.GetRequiredService<Ext::IExternalAppUtils>();
        public static IDownloadSettingsHandler DownloadSettingsHandler { get; private set; } = DIFactory.Provider.GetRequiredService<IDownloadSettingsHandler>();
        public static ISortInfoHandler SortInfoHandler { get; private set; } = DIFactory.Provider.GetRequiredService<ISortInfoHandler>();
        public static IPlaylistTreeHandler PlaylistTreeHandler { get; private set; } = DIFactory.Provider.GetRequiredService<IPlaylistTreeHandler>();
        public static IWindowsHelper WindowsHelper { get; private set; } = DIFactory.Provider.GetRequiredService<IWindowsHelper>();
        public static IThemehandler Themehandler { get; private set; } = DIFactory.Provider.GetRequiredService<IThemehandler>();
        public static IApplicationPowerManager ApplicationPower { get; private set; } = DIFactory.Provider.GetRequiredService<IApplicationPowerManager>();
        public static IStyleHandler StyleHandler { get; private set; } = DIFactory.Provider.GetRequiredService<IStyleHandler>();
        public static ILocalSettingsContainer SettingsContainer { get; private set; } = DIFactory.Provider.GetRequiredService<ILocalSettingsContainer>();
        public static IPostDownloadActionssManager PostDownloadTasksManager { get; private set; } = DIFactory.Provider.GetRequiredService<IPostDownloadActionssManager>();
        public static IDlTimer DlTimer { get; private set; } = DIFactory.Provider.GetRequiredService<IDlTimer>();
        public static ITabsContainerHandler TabHandler { get; private set; } = DIFactory.Provider.GetRequiredService<ITabsContainerHandler>();

        /// <summary>
        /// 動画情報の更新とか
        /// </summary>
        public static IOnlineVideoRefreshManager VideoRefreshManager { get; private set; } = DIFactory.Provider.GetRequiredService<IOnlineVideoRefreshManager>();

        /// <summary>
        /// 動画の追加とか
        /// </summary>
        public static IVideoRegistrationHandler VideoRegistrationHandler { get; private set; } = DIFactory.Provider.GetRequiredService<IVideoRegistrationHandler>();

        /// <summary>
        /// ローカル情報
        /// </summary>
        public static ILocalState LocalState { get; private set; } = DIFactory.Provider.GetRequiredService<ILocalState>();

        /// <summary>
        /// ウィンドウ
        /// </summary>
        public static IWindowTabHelper WindowTabHelper { get; private set; } = DIFactory.Provider.GetRequiredService<IWindowTabHelper>();

        /// <summary>
        /// 初期化ヘルパー
        /// </summary>
        public static IInitializeAwaiterHandler InitializeAwaiterHandler { get; private set; } = DIFactory.Provider.GetRequiredService<IInitializeAwaiterHandler>();

        /// <summary>
        /// ダウンロードを一括管理
        /// </summary>
        public static IDownloadManager DownloadManager { get; private set; } = DIFactory.Provider.GetRequiredService<IDownloadManager>();

        /// <summary>
        /// クリップボード管理
        /// </summary>
        public static IClipbordManager ClipbordManager { get; private set; } = DIFactory.Provider.GetRequiredService<IClipbordManager>();

        /// <summary>
        /// ページ遷移管理クラス
        /// </summary>
        public static IBlazorPageManager BlazorPageManager { get; private set; } = DIFactory.Provider.GetRequiredService<IBlazorPageManager>();

        /// <summary>
        /// アドンマネージャー
        /// </summary>
        public static IAddonManager AddonManager { get; private set; } = DIFactory.Provider.GetRequiredService<IAddonManager>();

        /// <summary>
        /// 設定コンテナ
        /// </summary>
        public static ISettingsConainer SettingsConainer { get; private set; } = DIFactory.Resolve<ISettingsConainer>();

        /// <summary>
        /// 動画・プレイリストコンテナ
        /// </summary>
        public static Playlist::IPlaylistVideoContainer PlaylistVideoContainer { get; private set; } = DIFactory.Resolve<Playlist::IPlaylistVideoContainer>();

    }
}
