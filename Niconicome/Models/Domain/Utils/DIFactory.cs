﻿using System.Net.Http;
using System.Net.Security;
using Microsoft.Extensions.DependencyInjection;
using AddonAPI = Niconicome.Models.Local.Addon.API;
using AddonsCoreV2 = Niconicome.Models.Domain.Local.Addons.Core.V2;
using AddonsDomainAPI = Niconicome.Models.Domain.Local.Addons.API;
using AddonsV2 = Niconicome.Models.Local.Addon.V2;
using AddonVM = Niconicome.ViewModels.Mainpage.Subwindows.AddonManager;
using Auth = Niconicome.Models.Auth;
using Backup = Niconicome.Models.Domain.Local.DataBackup;
using Channel = Niconicome.Models.Domain.Niconico.Remote.Channel;
using CommentConverter = Niconicome.Models.Domain.Niconico.Download.Comment.V2.Core.Converter;
using CommentFetch = Niconicome.Models.Domain.Niconico.Download.Comment.V2.Fetch;
using CommentIntegrate = Niconicome.Models.Domain.Niconico.Download.Comment.V2.Integrate;
using CommentLocal = Niconicome.Models.Domain.Niconico.Download.Comment.V2.Local;
using Cookies = Niconicome.Models.Domain.Local.Cookies;
using DataBase = Niconicome.Models.Domain.Local;
using DB = Niconicome.Models.Infrastructure.Database;
using DLActions = Niconicome.Models.Network.Download.Actions;
using DlComment = Niconicome.Models.Domain.Niconico.Download.Comment;
using DlDescription = Niconicome.Models.Domain.Niconico.Download.Description;
using DlIchiba = Niconicome.Models.Domain.Niconico.Download.Ichiba;
using DlThumb = Niconicome.Models.Domain.Niconico.Download.Thumbnail;
using DlVideo = Niconicome.Models.Domain.Niconico.Download.Video;
using Dmc = Niconicome.Models.Domain.Niconico.Dmc;
using DomainExt = Niconicome.Models.Domain.Local.External;
using DomainNet = Niconicome.Models.Domain.Network;
using DomainPlaylist = Niconicome.Models.Domain.Local.Playlist;
using DomainSettings = Niconicome.Models.Domain.Local.Settings;
using DomainWatch = Niconicome.Models.Domain.Niconico.Watch;
using DomainWatchV2 = Niconicome.Models.Domain.Niconico.Watch.V2;
using DomainXeno = Niconicome.Models.Domain.Local.External.Import.Xeno;
using Download = Niconicome.Models.Network.Download;
using DownloadTask = Niconicome.Models.Network.Download.DLTask;
using Ext = Niconicome.Models.Local.External;
using Fetch = Niconicome.Models.Network.Fetch;
using FF = Niconicome.Models.Domain.Local.External.Software.Mozilla.Firefox;
using Handlers = Niconicome.Models.Domain.Local.Handlers;
using Ichiba = Niconicome.Models.Domain.Niconico.Video.Ichiba;
using Infla = Niconicome.Models.Infrastructure;
using IO = Niconicome.Models.Domain.Local.IO;
using Local = Niconicome.Models.Local;
using LocalFile = Niconicome.Models.Domain.Local.LocalFile;
using Machine = Niconicome.Models.Domain.Local.Machine;
using MyApplication = Niconicome.Models.Local.Application;
using Mylist = Niconicome.Models.Domain.Niconico.Remote.Mylist;
using Net = Niconicome.Models.Network;
using NetworkVideo = Niconicome.Models.Network.Video;
using Niconico = Niconicome.Models.Domain.Niconico;
using OS = Niconicome.Models.Local.OS;
using Playlist = Niconicome.Models.Playlist;
using PlaylistPlaylist = Niconicome.Models.Playlist.Playlist;
using PlaylistV2 = Niconicome.Models.Playlist.V2;
using Register = Niconicome.Models.Network.Register;
using RemoteV2 = Niconicome.Models.Domain.Niconico.Remote.V2;
using Restore = Niconicome.Models.Local.Restore;
using Resume = Niconicome.Models.Domain.Niconico.Download.Video.Resume;
using Search = Niconicome.Models.Domain.Niconico.Remote.Search;
using Series = Niconicome.Models.Domain.Niconico.Remote.Series;
using Server = Niconicome.Models.Domain.Local.Server;
using Settings = Niconicome.Models.Local.Settings;
using SQlite = Niconicome.Models.Domain.Local.SQLite;
using State = Niconicome.Models.Local.State;
using Store = Niconicome.Models.Domain.Local.Store;
using Style = Niconicome.Models.Domain.Local.Style;
using TabsVM = Niconicome.ViewModels.Mainpage.Tabs;
using Timer = Niconicome.Models.Local.Timer;
using Utils = Niconicome.Models.Utils;
using UVideo = Niconicome.Models.Domain.Niconico.Video;
using VList = Niconicome.Models.Playlist.VideoList;
using VM = Niconicome.ViewModels;
using Watch = Niconicome.Models.Network.Watch;
using Software = Niconicome.Models.Domain.Local.External.Software;
using NicoImport = Niconicome.Models.Domain.Local.DataBackup.Import.Niconicome;
using SettingsVM = Niconicome.ViewModels.Setting.V2.Page;
using XenoImport = Niconicome.Models.Domain.Local.DataBackup.Import.Xeno;

namespace Niconicome.Models.Domain.Utils
{
    public class DIFactory
    {
        private static ServiceProvider GetProvider()
        {
            var services = new ServiceCollection();
            services.AddWpfBlazorWebView();
            services.AddBlazorWebViewDeveloperTools();
            services.AddHttpClient<Niconico::INicoHttp, Niconico::NicoHttp>()
                .ConfigureHttpMessageHandlerBuilder(builder =>
                {
                    var shandler = builder.Services.GetRequiredService<Settings::ILocalSettingHandler>();
                    var skip = shandler.GetBoolSetting(Settings::SettingsEnum.SkipSSLVerification);
                    if (builder.PrimaryHandler is HttpClientHandler handler)
                    {
                        handler.CookieContainer = builder.Services.GetRequiredService<Niconico::ICookieManager>().CookieContainer;
                        handler.UseCookies = true;
                        handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                        {
                            if (skip) return true;
                            return sslPolicyErrors == SslPolicyErrors.None;
                        };
                    }
                });
            services.AddSingleton<Niconico::ICookieManager, Niconico::CookieManager>();
            services.AddTransient<Auth::ISession, Auth::Session>();
            services.AddSingleton<Niconico::INiconicoContext, Niconico::NiconicoContext>();
            services.AddTransient<IErrorHandler, ErrorHandler>();
            services.AddTransient<ILogger, Logger>();
            services.AddSingleton<NicoLogger.ILogWriter, Infla::Log.LogStream>();
            services.AddTransient<NicoLogger.INiconicomeLogger, NicoLogger.NiconicomeLogger>();
            services.AddSingleton<DataBase::IDataBase, DataBase::DataBase>();
            services.AddTransient<Store::IPlaylistStoreHandler, Store::PlaylistStoreHandler>();
            services.AddTransient<Store::IVideoStoreHandler, Store::VideoStoreHandler>();
            services.AddTransient<PlaylistPlaylist::IPlaylistHandler, PlaylistPlaylist::PlaylistHandler>();
            services.AddSingleton<PlaylistPlaylist::IPlaylistTreeHandler, PlaylistPlaylist::PlaylistTreeHandler>();
            services.AddSingleton<PlaylistPlaylist::ISortInfoHandler, PlaylistPlaylist::SortInfoHandler>();
            services.AddTransient<DomainWatch::IWatchInfohandler, DomainWatch::WatchInfohandler>();
            services.AddTransient<Watch::IWatch, Watch::Watch>();
            services.AddTransient<Watch::IDomainModelConverter, Watch::DomainModelConverter>();
            services.AddTransient<Net::IVideoThumnailUtility, Net::VideoThumnailUtility>();
            services.AddTransient<Mylist::IMylistHandler, Mylist::MylistHandler>();
            services.AddTransient<Mylist::IWatchLaterHandler, Mylist::WatchLaterHandler>();
            services.AddTransient<UVideo::IUserVideoHandler, UVideo::UserVideoHandler>();
            services.AddTransient<Net::IRemotePlaylistHandler, Net::RemotePlaylistHandler>();
            services.AddSingleton<State::ILocalInfo, State::LocalInfo>();
            services.AddTransient<DomainNet::ICacheHandler, DomainNet::CacheHandler>();
            services.AddTransient<DomainNet::ICacheStraem, DomainNet::CacheStream>();
            services.AddSingleton<Net::INetworkVideoHandler, Net::NetworkVideoHandler>();
            services.AddSingleton<State::IMessageHandler, State::MessageHandler>();
            services.AddTransient<Niconico::IAccountManager, Niconico::AccountManager>();
            services.AddTransient<DomainWatch::IDmcDataHandler, DomainWatch::DmcDataHandler>();
            services.AddTransient<DomainWatch::IWatchSession, DomainWatch::WatchSession>();
            services.AddTransient<DomainWatch::IWatchPlaylisthandler, DomainWatch::WatchPlaylistHandler>();
            services.AddTransient<Dmc::IStreamhandler, Dmc::StreamHandler>();
            services.AddTransient<Dmc::IM3U8Handler, Dmc::M3U8Handler>();
            services.AddTransient<DlVideo::IVideoDownloadHelper, DlVideo::VideoDownloadHelper>();
            services.AddTransient<DlVideo::IVideoDownloader, DlVideo::VideoDownloader>();
            services.AddTransient<DlVideo::ISegmentWriter, DlVideo::SegmentWriter>();
            services.AddTransient<DlVideo::IVideoEncoader, DlVideo::VideoEncoader>();
            services.AddTransient<DlVideo::ITsMerge, DlVideo::TsMerge>();
            services.AddSingleton<Download::IContentDownloadHelper, Download::ContentDownloadHelper>();
            services.AddTransient<DlThumb::IThumbDownloader, DlThumb::ThumbDownloader>();
            services.AddTransient<Playlist::IVideoFilter, Playlist::VideoFilter>();
            services.AddTransient<Search::ISearch, Search::Search>();
            services.AddTransient<Search::ISearchClient, Search::SearchClient>();
            services.AddTransient<Search::ISearchUrlConstructor, Search::SearchUrlConstructor>();
            services.AddSingleton<State::IErrorMessanger, State::ErrorMessenger>();
            services.AddSingleton<MyApplication::IStartUp, MyApplication::StartUp>();
            services.AddTransient<Store::ISettingHandler, Store::SettingHandler>();
            services.AddTransient<Settings::ILocalSettingHandler, Settings::LocalSettingHandler>();
            services.AddTransient<INiconicoUtils, NiconicoUtils>();
            services.AddSingleton<State::ILocalState, State::LocalState>();
            services.AddTransient<LocalFile::IEncodeutility, LocalFile::Encodeutility>();
            services.AddTransient<Store::IVideoFileStorehandler, Store::VideoFileStorehandler>();
            services.AddTransient<Restore::IRestoreManager, Restore::RestoreManager>();
            services.AddTransient<Backup::IBackupManager, Backup::BackupManager>();
            services.AddTransient<DomainPlaylist::IPlaylistFileFactory, DomainPlaylist::PlaylistFileFactory>();
            services.AddTransient<Ext.Playlist.IPlaylistCreator, Ext::Playlist.PlaylistCreator>();
            services.AddSingleton<MyApplication::IShutdown, MyApplication::Shutdown>();
            services.AddTransient<DlComment::ICommentDownloader, DlComment::CommentDownloader>();
            services.AddTransient<DlComment::ICommentConverter, DlComment::CommentConverter>();
            services.AddTransient<DlComment::ICommentClient, DlComment::CommentClient>();
            services.AddTransient<DlComment::ICommentStream, DlComment::CommentStream>();
            services.AddTransient<DlComment::ICommentRequestBuilder, DlComment::CommentRequestBuilder>();
            services.AddTransient<DlComment::IOfficialVideoUtils, DlComment::OfficialVideoUtils>();
            services.AddTransient<Playlist::IVideoHandler, Playlist::VideoHandler>();
            services.AddTransient<Channel::IChannelPageHtmlParser, Channel::ChannelPageHtmlParser>();
            services.AddTransient<Channel::IChannelVideoHandler, Channel::ChannelVideoHandler>();
            services.AddTransient<DomainXeno::IXenoImportManager, DomainXeno::XenoImportManager>();
            services.AddTransient<DomainXeno::IXenoRootParser, DomainXeno::XenoRootParser>();
            services.AddTransient<DomainXeno::IXenoVideoNodeParser, DomainXeno::XenoVideoNodeParser>();
            services.AddTransient<DomainXeno::IXenoPlaylistConverter, DomainXeno::XenoPlaylistConverter>();
            services.AddTransient<Store::IVideoDirectoryStoreHandler, Store::VideoDirectoryStoreHandler>();
            services.AddTransient<Download::ILocalContentHandler, Download::LocalContentHandler>();
            services.AddTransient<DownloadTask::IDownloadTaskPool, DownloadTask::DownloadTaskPool>();
            services.AddSingleton<DownloadTask::IDownloadManager, DownloadTask.DownloadManager>();
            services.AddTransient<DownloadTask::IDownloadTask, DownloadTask::DownloadTask>();
            services.AddSingleton<Download::IDownloadSettingsHandler, Download::DownloadSettingsHandler>();
            services.AddTransient<Handlers::ICoreWebview2Handler, Handlers::CoreWebview2Handler>();
            services.AddTransient<Auth::IAutoLogin, Auth::AutoLogin>();
            services.AddSingleton<State::Toast.IToastHandler, State::Toast.ToastHandler>();
            services.AddTransient<SQlite::ISQliteLoader, SQlite::SQliteLoader>();
            services.AddTransient<SQlite::ISqliteCookieLoader, SQlite::SqliteCookieLoader>();
            services.AddTransient<Cookies::IChromeCookieDecryptor, Cookies::ChromeCookieDecryptor>();
            services.AddTransient<LocalFile::ICookieJsonLoader, LocalFile::CookieJsonLoader>();
            services.AddTransient<Cookies::IWebview2LocalCookieManager, Cookies::Webview2LocalCookieManager>();
            services.AddTransient<Auth::IWebview2SharedLogin, Auth::Webview2SharedLogin>();
            services.AddTransient<Auth::IFirefoxSharedLogin, Auth::FirefoxSharedLogin>();
            services.AddTransient<LocalFile::ILocalDirectoryHandler, LocalFile::LocalDirectoryHandler>();
            services.AddTransient<Net::IVideoIDHandler, Net::VideoIDHandler>();
            services.AddTransient<DlDescription::IDescriptionDownloader, DlDescription::DescriptionDownloader>();
            services.AddTransient<DlDescription::IVideoInfoContentProducer, DlDescription::VideoInfoContentProducer>();
            services.AddTransient<Local::ILocalVideoUtils, Local::LocalVideoUtils>();
            services.AddTransient<IO::INicoDirectoryIO, IO::NicoDirectoryIO>();
            services.AddTransient<IO::INicoFileIO, IO::NicoFileIO>();
            services.AddTransient<IO::IFileWatcher, IO::FileWatcher>();
            services.AddTransient<Resume::ISegmentsDirectoryHandler, Resume::SegmentsDirectoryHandler>();
            services.AddTransient<Resume::IStreamResumer, Resume::StreamResumer>();
            services.AddSingleton<VList::ICurrent, VList::Current>();
            services.AddSingleton<VList::IVideoListContainer, VList::VideoListContainer>();
            services.AddTransient<VList::IVideoListRefresher, VList::VideoListRefresher>();
            services.AddTransient<Ext::IExternalAppUtils, Ext::ExternalAppUtils>();
            services.AddTransient<DomainExt::ICommandExecuter, DomainExt::CommandExecuter>();
            services.AddTransient<DomainNet::INetWorkHelper, DomainNet::NetWorkHelper>();
            services.AddTransient<Ichiba::IIchibaHtmlParser, Ichiba::IchibaHtmlParser>();
            services.AddTransient<Ichiba::INiconicoIchibaHandler, Ichiba::NiconicoIchibaHandler>();
            services.AddTransient<IPathOrganizer, PathOrganizer>();
            services.AddTransient<DlIchiba::IIchibaInfoDownloader, DlIchiba::IchibaInfoDownloader>();
            services.AddTransient<FF::IFirefoxProfileManager, FF::FirefoxProfileManager>();
            services.AddTransient<Cookies::IFirefoxCookieManager, Cookies::FirefoxCookieManager>();
            services.AddTransient<PlaylistPlaylist::IPlaylistSettingsHandler, PlaylistPlaylist::PlaylistSettingsHandler>();
            services.AddSingleton<Utils::IWindowsHelper, Utils::WindowsHelper>();
            services.AddSingleton<MyApplication::IThemehandler, MyApplication::ThemeHandler>();
            services.AddTransient<MyApplication::IApplicationPowerManager, MyApplication::ApplicationPowerManager>();
            services.AddTransient<Style::IUserChromeHandler, Style::UserChromeHandler>();
            services.AddSingleton<State::IStyleHandler, State::StyleHandler>();
            services.AddSingleton<Settings::ILocalSettingsContainer, Settings::LocalSettingsContainer>();
            services.AddTransient<Series::ISeriesHandler, Series::SeriesHandler>();
            services.AddTransient<Series::ISeriesPageHtmlParser, Series::SeriesPageHtmlParser>();
            services.AddSingleton<Playlist::IVideoInfoContainer, Playlist::VideoInfoContainer>();
            services.AddSingleton<Playlist::ILightVideoListinfoHandler, Playlist::LightVideoListinfoHandler>();
            services.AddTransient<AddonAPI.Local.IO.IOutput, AddonAPI.Local.IO.Output>();
            services.AddTransient<AddonAPI.Net.Http.Fetch.IFetch, AddonAPI.Net.Http.Fetch.Fetch>();
            services.AddTransient<AddonAPI.Net.Http.Fetch.IAddonHttp, AddonAPI.Net.Http.Fetch.AddonHttp>();
            services.AddTransient<AddonAPI::IAPIEntryPoint, AddonAPI::APIEntryPoint>();
            services.AddSingleton<AddonsDomainAPI::Hooks.IHooksManager, AddonsDomainAPI::Hooks.HooksManager>();
            services.AddTransient<AddonAPI::Net.Hooks.IHooks, AddonAPI::Net.Hooks.Hooks>();
            services.AddTransient<AddonAPI::Local.IO.ILog, AddonAPI::Local.IO.Log>();
            services.AddSingleton<Event.IEventManager, Event.EventManager>();
            services.AddTransient<Machine::IComPowerManager, Machine::ComPowerManager>();
            services.AddSingleton<DLActions::IPostDownloadActionssManager, DLActions::PostDownloadActionsManager>();
            services.AddSingleton<Timer::IDlTimer, Timer::DlTimer>();
            services.AddTransient<AddonsDomainAPI::Storage.LocalStorage.IStorageHelper, AddonsDomainAPI::Storage.LocalStorage.StorageHelper>();
            services.AddTransient<AddonsDomainAPI::Storage.LocalStorage.IStorageHandler, AddonsDomainAPI::Storage.LocalStorage.StorageHandler>();
            services.AddTransient<AddonAPI::Local.Storage.IStorage, AddonAPI::Local.Storage.Storage>();
            services.AddTransient<AddonAPI::Local.Storage.ILocalStorage, AddonAPI::Local.Storage.LocalStorage>();
            services.AddTransient<AddonsDomainAPI::Resource.IResourceHander, AddonsDomainAPI::Resource.ResourceHander>();
            services.AddTransient<AddonAPI::Local.Resource.IPublicResourceHandler, AddonAPI::Local.Resource.PublicResourceHandler>();
            services.AddTransient<AddonsDomainAPI::Tab.ITabInfomation, AddonsDomainAPI::Tab.TabInfomation>();
            services.AddSingleton<AddonAPI::Local.Tab.ITabsContainer, AddonAPI::Local.Tab.TabsContainer>();
            services.AddTransient<AddonAPI::Local.Tab.ITabItem, AddonAPI::Local.Tab.TabItem>();
            services.AddSingleton<AddonAPI::Local.Tab.ITabsContainer, AddonAPI::Local.Tab.TabsContainer>();
            services.AddTransient<AddonAPI::Local.Tab.ITabsContainerHandler, AddonAPI::Local.Tab.TabsContainerHandler>();
            services.AddTransient<AddonAPI::Local.Tab.ITabsManager, AddonAPI::Local.Tab.TabsManager>();
            services.AddSingleton<Fetch::IOnlineVideoRefreshManager, Fetch::OnlineVideoRefreshManager>();
            services.AddTransient<PlaylistPlaylist::IVideosUnchecker, PlaylistPlaylist::VideosUnchecker>();
            services.AddTransient<Register::IVideoRegistrationHandler, Register::VideoRegistrationHandler>();
            services.AddTransient<Utils::IWindowTabHelper, Utils::WindowTabHelper>();
            services.AddTransient<Playlist::SharedUtils.IVideoPlaylistConverter, Playlist::SharedUtils.VideoPlaylistConverter>();
            services.AddSingleton<PlaylistPlaylist::IPlaylistInfoContainer, PlaylistPlaylist::PlaylistInfoContainer>();
            services.AddSingleton<Utils::InitializeAwaiter.IInitializeAwaiterHandler, Utils::InitializeAwaiter.InitializeAwaiterHandler>();
            services.AddTransient<AddonAPI::Net.Download.Integrate.IDownloadSettings, AddonAPI::Net.Download.Integrate.DownloadSettings>();
            services.AddTransient<CommentFetch::ICommentClient, CommentFetch::CommentClient>();
            services.AddTransient<CommentFetch::ICommentRequestBuilder, CommentFetch::CommentRequestBuilder>();
            services.AddTransient<CommentFetch::IOfficialCommentHandler, CommentFetch::OfficialCommentHandler>();
            services.AddTransient<CommentLocal::ICommentLoader, CommentLocal::CommentLoader>();
            services.AddTransient<CommentLocal::ICommentWriter, CommentLocal::CommentWriter>();
            services.AddTransient<CommentIntegrate::ICommentDownloader, CommentIntegrate::CommentDownloader>();
            services.AddTransient<CommentConverter::ILocalCommentConverter, CommentConverter::LocalCommentConverter>();
            services.AddTransient<CommentConverter::INetCommentConverter, CommentConverter::NetCommentConverter>();
            services.AddTransient<FF::IStoreFirefoxProfileManager, FF::StoreFirefoxProfileManager>();
            services.AddTransient<Auth::IStoreFirefoxSharedLogin, Auth::StoreFirefoxSharedLogin>();
            services.AddTransient<Cookies::IStoreFirefoxCookieManager, Cookies::StoreFirefoxCookieManager>();
            services.AddSingleton<OS::IClipbordManager, OS::ClipbordManager>();
            services.AddTransient<VM::Blazor.TransitionViewModel>();
            services.AddSingleton<State::IBlazorPageManager, State::BlazorPageManager>();
            services.AddTransient<AddonsV2::IAddonInstallManager, AddonsV2::AddonInstallManager>();
            services.AddTransient<AddonsV2::IAddonManager, AddonsV2::AddonManager>();
            services.AddSingleton<AddonsV2::IAddonStatusContainer, AddonsV2::AddonStatusContainer>();
            services.AddTransient<AddonsCoreV2::Engine.IAddonLogger, AddonsCoreV2::Engine.AddonLogger>();
            services.AddTransient<AddonsCoreV2::Engine.Context.IAddonContext, AddonsCoreV2::Engine.Context.AddonContext>();
            services.AddSingleton<AddonsCoreV2::Engine.Context.IAddonContextsContainer, AddonsCoreV2::Engine.Context.AddonContextsContainer>();
            services.AddTransient<AddonsCoreV2::Engine.Infomation.IManifestLoader, AddonsCoreV2::Engine.Infomation.ManifestLoader>();
            services.AddTransient<AddonsCoreV2::Engine.JavaScript.IJavaScriptEngine, AddonsCoreV2::Engine.JavaScript.JavaScriptEngine>();
            services.AddTransient<AddonsCoreV2::Install.IAddonExtractor, AddonsCoreV2::Install.AddonExtractor>();
            services.AddTransient<AddonsCoreV2::Install.IAddonInstaller, AddonsCoreV2::Install.AddonInstaller>();
            services.AddTransient<AddonsCoreV2::Loader.IAddonLoader, AddonsCoreV2::Loader.AddonLoader>();
            services.AddTransient<AddonsCoreV2::Permisson.IPermissionsHandler, AddonsCoreV2::Permisson.PermissionsHandler>();
            services.AddTransient<AddonsCoreV2::Uninstall.IAddonUninstaller, AddonsCoreV2::Uninstall.AddonUninstaller>();
            services.AddTransient<AddonsCoreV2::Update.IAddonUpdateChecker, AddonsCoreV2::Update.AddonUpdateChecker>();
            services.AddTransient<AddonsCoreV2::Update.IAddonUpdator, AddonsCoreV2::Update.AddonUpdator>();
            services.AddTransient<AddonsCoreV2::Utils.IHostPermissionsHandler, AddonsCoreV2::Utils.HostPermissionsHandler>();
            services.AddTransient<AddonsCoreV2::Utils.IAddonSettingsHandler, AddonsCoreV2::Utils.AddonSettingsHandler>();
            services.AddTransient<AddonVM::Pages.IndexViewModel>();
            services.AddTransient<AddonVM::Pages.InstallViewModel>();
            services.AddTransient<AddonVM::Pages.AboutViewModel>();
            services.AddTransient<Utils::IBlazorHelper, Utils::BlazorHelper>();
            services.AddTransient<Store::V2.IApplicationStore, DB::ApplicationDBHandler>();
            services.AddSingleton<NicoLogger.ILogWriter, Infla::Log.LogStream>();
            services.AddTransient<AppEnvironment.IAppInfomationHandler, Infla::AppEnvironment.NiconicomeInfomationHandler>();
            services.AddTransient<AppEnvironment.IOSInfomationHandler, Infla::AppEnvironment.WindowsInfomationHandler>();
            services.AddSingleton<Store::V2.ISettingsStore, DB::Json.SettingJsonHandler>();
            services.AddSingleton<Store::V2.IPlaylistStore, DB::PlaylistDBHandler>();
            services.AddSingleton<Store::V2.IVideoStore, DB::VideoDBHandler>();
            services.AddSingleton<Store::V2.ITagStore, DB::TagDBHandler>();
            services.AddSingleton<DomainSettings::ISettingsContainer, DomainSettings::SettingsConainer>();
            services.AddTransient<DomainSettings::ISettingMigratioin, DomainSettings::SettingMigratioin>();
            services.AddSingleton<DB::LiteDB.ILiteDBHandler, DB::LiteDB.LiteDBHandler>();
            services.AddTransient<Error.IErrorHandler, Error.ErrorHandler>();
            services.AddSingleton<NetworkVideo::IThumbnailUtility, NetworkVideo::ThumbnailUtility>();
            services.AddSingleton<PlaylistV2::IPlaylistVideoContainer, PlaylistV2::PlaylistVideoContainer>();
            services.AddSingleton<PlaylistV2::Manager.Helper.ILocalVideoLoader, PlaylistV2::Manager.Helper.LocalVideoLoader>();
            services.AddTransient<PlaylistV2::Manager.IPlaylistManager, PlaylistV2::Manager.PlaylistManager>();
            services.AddTransient<PlaylistV2::Manager.IVideoListManager, PlaylistV2::Manager.VideoListManager>();
            services.AddTransient<PlaylistV2::Manager.Helper.IVideoListCRDHandler, PlaylistV2::Manager.Helper.VideoListCRDHandler>();
            services.AddTransient<PlaylistV2::Manager.Helper.IVideoListUpdateHandler, PlaylistV2::Manager.Helper.VideoListUpdateHandler>();
            services.AddTransient<PlaylistV2::Migration.IVideoAndPlayListMigration, PlaylistV2::Migration.VideoAndPlayListMigration>();
            services.AddTransient<TabsVM::VideoList.Pages.IndexViewModel>();
            services.AddTransient<TabsVM::VideoList.Pages.MigrationViewModel>();
            services.AddTransient<TabsVM::VideoList.Pages.PlaylistViewModel>();
            services.AddTransient<TabsVM::VideoList.Pages.VideoDetailViewModel>();
            services.AddTransient<TabsVM::VideoList.Pages.SearchViewModel>();
            services.AddTransient<RemoteV2::Mylist.IMylistHandler, RemoteV2::Mylist.MylistHandler>();
            services.AddTransient<RemoteV2::Mylist.IWatchLaterHandler, RemoteV2::Mylist.WatchLaterHandler>();
            services.AddTransient<RemoteV2::Channel.IChannelVideoHandler, RemoteV2::Channel.ChannelVideoHandler>();
            services.AddTransient<RemoteV2::Channel.IChannelPageHtmlParser, RemoteV2::Channel.ChannelPageHtmlParser>();
            services.AddTransient<RemoteV2::Series.ISeriesHandler, RemoteV2::Series.SeriesHandler>();
            services.AddTransient<RemoteV2::UserVideo.IUserVideoHandler, RemoteV2::UserVideo.UserVideoHandler>();
            services.AddTransient<RemoteV2::Search.ISearch, RemoteV2::Search.Search>();
            services.AddTransient<RemoteV2::Search.ISearchUrlConstructor, RemoteV2::Search.SearchUrlConstructor>();
            services.AddTransient<NetworkVideo::IInputTextParser, NetworkVideo::InputTextParser>();
            services.AddSingleton<NetworkVideo::INetVideosInfomationHandler, NetworkVideo::NetVideosInfomationHandler>();
            services.AddTransient<StringHandler.IStringHandler, StringHandler.StringHandler>();
            services.AddTransient<DomainWatchV2::IWatchPageInfomationHandler, DomainWatchV2::WatchPageInfomationHandler>();
            services.AddTransient<VM::Blazor.BlazorBaseViewModel>();
            services.AddTransient<Ext::IExternalProcessUtils, Ext::ExternalProcessUtils>();
            services.AddSingleton<State::MessageV2.IMessageHandler, State::MessageV2.MessageHandler>();
            services.AddTransient<Ext::IExternalAppUtilsV2, Ext::ExternalAppUtilsV2>();
            services.AddTransient<IO::V2.INiconicomeFileIO, Infla::IO.WindowsFileIO>();
            services.AddTransient<IO::V2.INiconicomeDirectoryIO, Infla::IO.WindowsDirectoryIO>();
            services.AddTransient<Store::V2.IVideoFileStore, DB::VideoFileDBHandler>();
            services.AddTransient<PlaylistV2::Utils.IVideoInfoCopyManager, PlaylistV2::Utils.VideoInfoCopyManager>();
            services.AddTransient<PlaylistV2::Utils.IVideoInfoFilterManager, PlaylistV2::Utils.VideoInfoFilterManager>();
            services.AddSingleton<Server::Core.IServer, Server::Core.Server>();
            services.AddSingleton<Server::Core.IUrlHandler, Server::Core.UrlHandler>();
            services.AddSingleton<Server::RequestHandler.Video.IVideoRequestHandler, Server::RequestHandler.Video.VideoRequestHandler>();
            services.AddSingleton<Server::RequestHandler.M3U8.IM3U8RequestHandler, Server::RequestHandler.M3U8.M3U8RequestHandler>();
            services.AddSingleton<Server::RequestHandler.TS.ITSRequestHandler, Server::RequestHandler.TS.TSRequestHandler>();
            services.AddSingleton<Server::RequestHandler.NotFound.INotFoundRequestHandler, Server::RequestHandler.NotFound.NotFoundRequestHandler>();
            services.AddSingleton<Server::RequestHandler.UserChrome.IUserChromeRequestHandler, Server::RequestHandler.UserChrome.UserChromeRequestHandler>();
            services.AddSingleton<Server::HLS.IHLSManager, Server::HLS.HLSManager>();
            services.AddTransient<State::Style.IUserChromeHandler, State::Style.UserChromeHandler>();
            services.AddTransient<Software::NiconicomeProcess.IProcessManager, Software::NiconicomeProcess.ProcessManager>();
            services.AddTransient<Software::FFmpeg.ffprobe.IFFprobeHandler, Software::FFmpeg.ffprobe.FFprobeHandler>();
            services.AddSingleton<State::Style.IVideoListWidthManager, State::Style.VideoListWidthManager>();
            services.AddTransient<PlaylistV2::Manager.ISearchManager, PlaylistV2::Manager.SearchManager>();
            services.AddTransient<NicoImport::IExportHandler, NicoImport::ExportHandler>();
            services.AddTransient<NicoImport::IImportHandler, NicoImport::ImportHandler>();
            services.AddTransient<NicoImport::Converter.IExportConverter, NicoImport::Converter.ExportConverter>();
            services.AddTransient<NicoImport::Converter.IImportConverter, NicoImport::Converter.ImportConverter>();
            services.AddTransient<Restore::Import.Niconicome.IImportExportManager, Restore::Import.Niconicome.ImportExportManager>();
            services.AddTransient<SettingsVM::ImportViewModel>();
            services.AddTransient<XenoImport::Parser.IXenoDataParser, XenoImport::Parser.XenoDataParser>();
            services.AddTransient<XenoImport::IXenoImportHandler, XenoImport::XenoImportHandler>();
            services.AddTransient<Restore::Import.Xeno.IXenoImportManager, Restore::Import.Xeno.XenoImportManager>();

            return services.BuildServiceProvider();
        }


        private static ServiceProvider? provider;

        public static ServiceProvider Provider
        {
            get
            {
                if (DIFactory.provider == null)
                {
                    DIFactory.provider = DIFactory.GetProvider();
                }

                return DIFactory.provider;
            }
        }

        public static T Resolve<T>() where T : notnull
        {
            return Provider.GetRequiredService<T>();
        }
    }
}
