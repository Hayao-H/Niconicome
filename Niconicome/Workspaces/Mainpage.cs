using Microsoft.Extensions.DependencyInjection;
using Niconicome.Models.Auth;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Utils;
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
using Ext = Niconicome.Models.Local.External;
using VideoList = Niconicome.Models.Playlist.VideoList;
using Playlist = Niconicome.Models.Playlist.V2;
using Niconicome.Models.Local.State.Toast;
using Niconicome.Models.Domain.Utils.StringHandler;
using MessageV2 = Niconicome.Models.Local.State.MessageV2;
using Niconicome.Models.Local.External.Playlist;
using Server = Niconicome.Models.Domain.Local.Server;
using Style = Niconicome.Models.Local.State.Style;
using Tab = Niconicome.Models.Local.State.Tab.V1;
using PostDL = Niconicome.Models.Network.Download.Actions.V2;
using Cookie = Niconicome.Models.Auth.Cookie;
using Niconico = Niconicome.Models.Domain.Niconico;

namespace Niconicome.Workspaces
{
    static class Mainpage
    {
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
        public static IToastHandler SnackbarHandler { get; private set; } = DIFactory.Provider.GetRequiredService<IToastHandler>();
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
        public static ISettingsContainer SettingsConainer { get; private set; } = DIFactory.Resolve<ISettingsContainer>();

        /// <summary>
        /// 動画・プレイリストコンテナ
        /// </summary>
        public static Playlist::IPlaylistVideoContainer PlaylistVideoContainer { get; private set; } = DIFactory.Resolve<Playlist::IPlaylistVideoContainer>();

        /// <summary>
        /// 動画リスト
        /// </summary>
        public static Playlist::Manager.IVideoListManager VideoListManager { get; private set; } = DIFactory.Resolve<Playlist::Manager.IVideoListManager>();

        /// <summary>
        /// プレリストの管理
        /// </summary>
        public static Playlist::Manager.IPlaylistManager PlaylistManager { get; private set; } = DIFactory.Resolve<Playlist::Manager.IPlaylistManager>();

        /// <summary>
        /// プレイリストと動画の移行
        /// </summary>
        public static Playlist::Migration.IVideoAndPlayListMigration VideoAndPlayListMigration { get; private set; } = DIFactory.Resolve<Playlist::Migration.IVideoAndPlayListMigration>();

        /// <summary>
        /// プロセス
        /// </summary>
        public static Ext::IExternalProcessUtils ExternalProcessUtils { get; private set; } = DIFactory.Resolve<Ext::IExternalProcessUtils>();

        /// <summary>
        /// 文字列生成
        /// </summary>
        public static IStringHandler StringHandler { get; private set; } = DIFactory.Resolve<IStringHandler>();

        /// <summary>
        /// メッセージハンドラ
        /// </summary>
        public static MessageV2::IMessageHandler MessageHandler { get; private set; } = DIFactory.Resolve<MessageV2::IMessageHandler>();

        /// <summary>
        /// 外部アプリハンドラ
        /// </summary>
        public static Ext::IExternalAppUtilsV2 ExternalAppUtilsV2 { get; private set; } = DIFactory.Resolve<Ext::IExternalAppUtilsV2>();

        /// <summary>
        /// 動画情報のコピー
        /// </summary>
        public static Playlist::Utils.IVideoInfoCopyManager VideoInfoCopyManager { get; private set; } = DIFactory.Resolve<Playlist::Utils.IVideoInfoCopyManager>();

        /// <summary>
        /// 動画フィルター
        /// </summary>
        public static Playlist::Utils.IVideoInfoFilterManager VideoInfoFilterManager { get; private set; } = DIFactory.Resolve<Playlist::Utils.IVideoInfoFilterManager>();

        /// <summary>
        /// HLS
        /// </summary>
        public static Server::HLS.IHLSManager HLSManager { get; private set; } = DIFactory.Resolve<Server::HLS.IHLSManager>();

        /// <summary>
        /// カラム幅
        /// </summary>
        public static Style::IVideoListWidthManager VideoListWidthManager { get; private set; } = DIFactory.Resolve<Style::IVideoListWidthManager>();

        /// <summary>
        /// 検索
        /// </summary>
        public static Playlist::Manager.ISearchManager SearchManager { get; private set; } = DIFactory.Resolve<Playlist::Manager.ISearchManager>();

        /// <summary>
        /// ウィンドウの位置など
        /// </summary>
        public static Style::IWindowStyleManager WindowStyleManager { get; private set; } = DIFactory.Resolve<Style::IWindowStyleManager>();

        /// <summary>
        /// マウスイベント
        /// </summary>
        public static Playlist::Manager.IPlaylistEventManager PlaylistEventManager { get; private set; } = DIFactory.Resolve<Playlist::Manager.IPlaylistEventManager>();

        /// <summary>
        /// 動画情報
        /// </summary>
        public static Server::API.VideoInfo.V1.IVideoInfoHandler VideoInfoHandler { get; set; } = DIFactory.Resolve<Server::API.VideoInfo.V1.IVideoInfoHandler>();

        /// <summary>
        /// タブ
        /// </summary>
        public static Tab::ITabControler TabControler { get; private set; } = DIFactory.Resolve<Tab::ITabControler>();

        /// <summary>
        /// DL後の処理
        /// </summary>
        public static PostDL::IPostDownloadActionssManager PostDownloadActionsManager { get; private set; } = DIFactory.Resolve<PostDL::IPostDownloadActionssManager>();

        /// <summary>
        /// Cookie管理
        /// </summary>
        public static Cookie::INiconicoCookieManager NiconicoCookieManager { get; private set; } = DIFactory.Resolve<Cookie::INiconicoCookieManager>();

        /// <summary>
        /// セッション管理
        /// </summary>
        public static Niconico::INiconicoContext NiconicoContext { get; private set; } = DIFactory.Resolve<Niconico::INiconicoContext>();

        /// <summary>
        /// アプリケーションのテーマ管理
        /// </summary>
        public static IThemehandler Themehandler { get; private set; } = DIFactory.Provider.GetRequiredService<IThemehandler>();

    }
}
