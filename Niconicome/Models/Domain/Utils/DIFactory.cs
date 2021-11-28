﻿using System.Net.Http;
using System.Net.Security;
using Microsoft.Extensions.DependencyInjection;
using AddonAPI = Niconicome.Models.Local.Addon.API;
using Addons = Niconicome.Models.Local.Addon;
using AddonsCore = Niconicome.Models.Domain.Local.Addons.Core;
using AddonsDomainAPI = Niconicome.Models.Domain.Local.Addons.API;
using Auth = Niconicome.Models.Auth;
using Channel = Niconicome.Models.Domain.Niconico.Remote.Channel;
using Cookies = Niconicome.Models.Domain.Local.Cookies;
using DataBase = Niconicome.Models.Domain.Local;
using DlComment = Niconicome.Models.Domain.Niconico.Download.Comment;
using DlDescription = Niconicome.Models.Domain.Niconico.Download.Description;
using DlIchiba = Niconicome.Models.Domain.Niconico.Download.Ichiba;
using DlThumb = Niconicome.Models.Domain.Niconico.Download.Thumbnail;
using DlVideo = Niconicome.Models.Domain.Niconico.Download.Video;
using Dmc = Niconicome.Models.Domain.Niconico.Dmc;
using DomainDownload = Niconicome.Models.Domain.Niconico.Download;
using DomainExt = Niconicome.Models.Domain.Local.External;
using DomainNet = Niconicome.Models.Domain.Network;
using DomainPlaylist = Niconicome.Models.Domain.Local.Playlist;
using DomainWatch = Niconicome.Models.Domain.Niconico.Watch;
using DomainXeno = Niconicome.Models.Domain.Local.External.Import.Xeno;
using Download = Niconicome.Models.Network.Download;
using Ext = Niconicome.Models.Local.External;
using FF = Niconicome.Models.Domain.Local.External.Software.Mozilla.Firefox;
using Handlers = Niconicome.Models.Domain.Local.Handlers;
using Ichiba = Niconicome.Models.Domain.Niconico.Video.Ichiba;
using Import = Niconicome.Models.Local.External.Import;
using IO = Niconicome.Models.Domain.Local.IO;
using Local = Niconicome.Models.Local;
using LocalFile = Niconicome.Models.Domain.Local.LocalFile;
using Machine = Niconicome.Models.Domain.Local.Machine;
using MyApplication = Niconicome.Models.Local.Application;
using Mylist = Niconicome.Models.Domain.Niconico.Remote.Mylist;
using Net = Niconicome.Models.Network;
using Niconico = Niconicome.Models.Domain.Niconico;
using Permissions = Niconicome.Models.Domain.Local.Addons.Core.Permisson;
using Playlist = Niconicome.Models.Playlist;
using PlaylistPlaylist = Niconicome.Models.Playlist.Playlist;
using Resume = Niconicome.Models.Domain.Niconico.Download.Video.Resume;
using Search = Niconicome.Models.Domain.Niconico.Remote.Search;
using Series = Niconicome.Models.Domain.Niconico.Remote.Series;
using Settings = Niconicome.Models.Local.Settings;
using SQlite = Niconicome.Models.Domain.Local.SQLite;
using State = Niconicome.Models.Local.State;
using Store = Niconicome.Models.Domain.Local.Store;
using Style = Niconicome.Models.Domain.Local.Style;
using Utils = Niconicome.Models.Utils;
using UVideo = Niconicome.Models.Domain.Niconico.Video;
using VList = Niconicome.Models.Playlist.VideoList;
using Watch = Niconicome.Models.Network.Watch;
using DLActions = Niconicome.Models.Network.Download.Actions;
using Timer = Niconicome.Models.Local.Timer;
using Fetch = Niconicome.Models.Network.Fetch;

namespace Niconicome.Models.Domain.Utils
{
    public class DIFactory
    {
        private static ServiceProvider GetProvider()
        {

            var services = new ServiceCollection();
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
            services.AddTransient<Niconico.IDbUrlHandler, Niconico::DbUrlHandler>();
            services.AddTransient<Auth::ISession, Auth::Session>();
            services.AddSingleton<Niconico::INiconicoContext, Niconico::NiconicoContext>();
            services.AddTransient<IErrorHandler, ErrorHandler>();
            services.AddSingleton<ILogStream, LogStream>();
            services.AddTransient<ILogger, Logger>();
            services.AddSingleton<DataBase::IDataBase, DataBase::DataBase>();
            services.AddTransient<Store::IPlaylistStoreHandler, Store::PlaylistStoreHandler>();
            services.AddTransient<Store::IVideoStoreHandler, Store::VideoStoreHandler>();
            services.AddTransient<PlaylistPlaylist::IPlaylistHandler, PlaylistPlaylist::PlaylistHandler>();
            services.AddSingleton<PlaylistPlaylist::IPlaylistTreeHandler, PlaylistPlaylist::PlaylistTreeHandler>();
            services.AddSingleton<PlaylistPlaylist::ISortInfoHandler, PlaylistPlaylist::SortInfoHandler>();
            services.AddTransient<DomainWatch::IWatchPageHtmlParser, DomainWatch::WatchPageHtmlParser>();
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
            services.AddTransient<DomainDownload::IDownloadMessenger, DomainDownload::DownloadMessanger>();
            services.AddTransient<DlVideo::IVideoEncoader, DlVideo::VideoEncoader>();
            services.AddTransient<DlVideo::ITsMerge, DlVideo::TsMerge>();
            services.AddSingleton<Download::IContentDownloader, Download::ContentDownloader>();
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
            services.AddTransient<Settings.IEnumSettingsHandler, Settings::EnumSettingsHandler>();
            services.AddTransient<INiconicoUtils, NiconicoUtils>();
            services.AddSingleton<State::ILocalState, State::LocalState>();
            services.AddTransient<LocalFile::IEncodeutility, LocalFile::Encodeutility>();
            services.AddTransient<Store::IVideoFileStorehandler, Store::VideoFileStorehandler>();
            services.AddTransient<Local::IRestore, Local::Restore>();
            services.AddTransient<Local::IBackuphandler, Local::BackupHandler>();
            services.AddTransient<DomainPlaylist::IPlaylistFileFactory, DomainPlaylist::PlaylistFileFactory>();
            services.AddTransient<Local::IPlaylistCreator, Local::PlaylistCreator>();
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
            services.AddTransient<Import::IXenoImportGeneralManager, Import::XenoImportGeneralManager>();
            services.AddTransient<Store::IVideoDirectoryStoreHandler, Store::VideoDirectoryStoreHandler>();
            services.AddTransient<Download::ILocalContentHandler, Download::LocalContentHandler>();
            services.AddTransient<Download::IDownloadTaskPool, Download::DownloadTaskPool>();
            services.AddSingleton<Download::IDownloadTasksHandler, Download::DownloadTasksHandler>();
            services.AddSingleton<Download::IDownloadSettingsHandler, Download::DownloadSettingsHandler>();
            services.AddTransient<Handlers::ICoreWebview2Handler, Handlers::CoreWebview2Handler>();
            services.AddTransient<Auth::IAutoLogin, Auth::AutoLogin>();
            services.AddSingleton<State::ISnackbarHandler, State::SnackbarHandler>();
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
            services.AddTransient<Permissions::IPermissionsHandler, Permissions::PermissionsHandler>();
            services.AddTransient<AddonsCore::IJavaScriptExecuter, AddonsCore::JavaScriptExecuter>();
            services.AddTransient<AddonsCore::AutoUpdate.Github.IReleaseChecker, AddonsCore::AutoUpdate.Github.ReleaseChecker>();
            services.AddSingleton<AddonsCore::IAddonInfomationsContainer, AddonsCore::AddonInfomationsContainer>();
            services.AddSingleton<AddonsCore::Engine.Context.IAddonContexts, AddonsCore::Engine.Context.AddonContexts>();
            services.AddSingleton<AddonsCore::Engine.Context.IAddonContext, AddonsCore::Engine.Context.AddonContext>();
            services.AddSingleton<AddonsCore::Engine.IAddonEngine, AddonsCore::Engine.AddonEngine>();
            services.AddTransient<AddonsCore::Engine.IAddonLogger, AddonsCore::Engine.AddonLogger>();
            services.AddTransient<AddonsCore::Installer.IAddonStoreHandler, AddonsCore::Installer.AddonStoreHandler>();
            services.AddTransient<AddonsCore::Installer.IAddonInstaller, AddonsCore::Installer.AddonInstaller>();
            services.AddTransient<AddonsCore::Installer.IManifestLoader, AddonsCore::Installer.ManifestLoader>();
            services.AddSingleton<Addons::IAddonHandler, Addons::AddonHandler>();
            services.AddSingleton<Addons::IAddonInstallManager, Addons::AddonInstallManager>();
            services.AddTransient<AddonAPI.Local.IO.IOutput, AddonAPI.Local.IO.Output>();
            services.AddTransient<AddonAPI.Net.Http.Fetch.IFetch, AddonAPI.Net.Http.Fetch.Fetch>();
            services.AddTransient<AddonAPI.Net.Http.Fetch.IAddonHttp, AddonAPI.Net.Http.Fetch.AddonHttp>();
            services.AddTransient<AddonAPI::IAPIEntryPoint, AddonAPI::APIEntryPoint>();
            services.AddTransient<AddonsCore::Installer.IAddonUninstaller, AddonsCore::Installer.AddonUninstaller>();
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
            services.AddTransient<AddonsCore::Utils.IHostPermissionsHandler, AddonsCore::Utils.HostPermissionsHandler>();
            services.AddTransient<AddonsDomainAPI::Tab.ITabInfomation, AddonsDomainAPI::Tab.TabInfomation>();
            services.AddSingleton<AddonAPI::Local.Tab.ITabsContainer, AddonAPI::Local.Tab.TabsContainer>();
            services.AddTransient<AddonAPI::Local.Tab.ITabItem, AddonAPI::Local.Tab.TabItem>();
            services.AddSingleton<AddonAPI::Local.Tab.ITabsContainer, AddonAPI::Local.Tab.TabsContainer>();
            services.AddTransient<AddonAPI::Local.Tab.ITabsContainerHandler, AddonAPI::Local.Tab.TabsContainerHandler>();
            services.AddTransient<AddonAPI::Local.Tab.ITabsManager, AddonAPI::Local.Tab.TabsManager>();
            services.AddSingleton<Fetch::IOnlineVideoRefreshManager, Fetch::OnlineVideoRefreshManager>();
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
    }
}
