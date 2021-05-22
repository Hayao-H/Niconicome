using System.Net.Http;
using System.Net.Security;
using Microsoft.Extensions.DependencyInjection;
using Auth = Niconicome.Models.Auth;
using Channel = Niconicome.Models.Domain.Niconico.Video.Channel;
using Cookies = Niconicome.Models.Domain.Local.Cookies;
using DataBase = Niconicome.Models.Domain.Local;
using DlComment = Niconicome.Models.Domain.Niconico.Download.Comment;
using DlDescription = Niconicome.Models.Domain.Niconico.Download.Description;
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
using Handlers = Niconicome.Models.Domain.Local.Handlers;
using Ichiba = Niconicome.Models.Domain.Niconico.Video.Ichiba;
using Import = Niconicome.Models.Local.External.Import;
using IO = Niconicome.Models.Domain.Local.IO;
using Local = Niconicome.Models.Local;
using LocalFile = Niconicome.Models.Domain.Local.LocalFile;
using MyApplication = Niconicome.Models.Local.Application;
using Mylist = Niconicome.Models.Domain.Niconico.Mylist;
using Net = Niconicome.Models.Network;
using Niconico = Niconicome.Models.Domain.Niconico;
using Playlist = Niconicome.Models.Playlist;
using Resume = Niconicome.Models.Domain.Niconico.Download.Video.Resume;
using Search = Niconicome.Models.Domain.Niconico.Search;
using Settings = Niconicome.Models.Local.Settings;
using SQlite = Niconicome.Models.Domain.Local.SQLite;
using State = Niconicome.Models.Local.State;
using Store = Niconicome.Models.Domain.Local.Store;
using Utils = Niconicome.Models.Domain.Utils;
using UVideo = Niconicome.Models.Domain.Niconico.Video;
using VList = Niconicome.Models.Playlist.VideoList;
using Watch = Niconicome.Models.Network.Watch;
using DlIchiba = Niconicome.Models.Domain.Niconico.Download.Ichiba;

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
            services.AddTransient<Utils::IErrorHandler, Utils::ErrorHandler>();
            services.AddSingleton<Utils::ILogStream, Utils::LogStream>();
            services.AddTransient<Utils::ILogger, Utils::Logger>();
            services.AddSingleton<DataBase::IDataBase, DataBase::DataBase>();
            services.AddTransient<Store::IPlaylistStoreHandler, Store::PlaylistStoreHandler>();
            services.AddTransient<Store::IVideoStoreHandler, Store::VideoStoreHandler>();
            services.AddSingleton<Playlist::IPlaylistHandler, Playlist::PlaylistHandler>();
            services.AddTransient<Playlist::IPlaylistTreeConstructor, Playlist::PlaylistTreeConstructor>();
            services.AddTransient<DomainWatch::IWatchPageHtmlParser, DomainWatch::WatchPageHtmlParser>();
            services.AddTransient<DomainWatch::IWatchInfohandler, DomainWatch::WatchInfohandler>();
            services.AddTransient<Watch::IWatch, Watch::Watch>();
            services.AddTransient<Watch::IDomainModelConverter, Watch::DomainModelConverter>();
            services.AddTransient<Net::IVideoThumnailUtility, Net::VideoThumnailUtility>();
            services.AddTransient<Mylist::IMylistHandler, Mylist::MylistHandler>();
            services.AddTransient<Mylist::IWatchLaterHandler, Mylist::WatchLaterHandler>();
            services.AddTransient<UVideo::IUserVideoHandler, UVideo::UserVideoHandler>();
            services.AddTransient<Net::IRemotePlaylistHandler, Net::RemotePlaylistHandler>();
            services.AddTransient<State::ILocalInfo, State::LocalInfo>();
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
            services.AddTransient<MyApplication::IShutdown, MyApplication::Shutdown>();
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
            services.AddTransient<LocalFile::ILocalDirectoryHandler, LocalFile::LocalDirectoryHandler>();
            services.AddTransient<Net::IVideoIDHandler, Net::VideoIDHandler>();
            services.AddTransient<DlDescription::IDescriptionDownloader, DlDescription::DescriptionDownloader>();
            services.AddTransient<DlDescription::IVideoInfoContentProducer, DlDescription::VideoInfoContentProducer>();
            services.AddTransient<Local::ILocalVideoUtils, Local::LocalVideoUtils>();
            services.AddTransient<IO::INicoDirectoryIO, IO::NicoDirectoryIO>();
            services.AddTransient<IO::INicoFileIO, IO::NicoFileIO>();
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
            services.AddTransient<Utils::IPathOrganizer, Utils::PathOrganizer>();
            services.AddTransient<DlIchiba::IIchibaInfoDownloader, DlIchiba::IchibaInfoDownloader>();

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
