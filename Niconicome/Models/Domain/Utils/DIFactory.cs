﻿using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Auth = Niconicome.Models.Auth;
using Channel = Niconicome.Models.Domain.Niconico.Video.Channel;
using DataBase = Niconicome.Models.Domain.Local;
using DlComment = Niconicome.Models.Domain.Niconico.Download.Comment;
using DlThumb = Niconicome.Models.Domain.Niconico.Download.Thumbnail;
using DlVideo = Niconicome.Models.Domain.Niconico.Download.Video;
using Dmc = Niconicome.Models.Domain.Niconico.Dmc;
using DomainNet = Niconicome.Models.Domain.Network;
using DomainPlaylist = Niconicome.Models.Domain.Local.Playlist;
using DomainWatch = Niconicome.Models.Domain.Niconico.Watch;
using Download = Niconicome.Models.Domain.Niconico.Download;
using LocalFile = Niconicome.Models.Domain.Local.LocalFile;
using Local = Niconicome.Models.Local;
using MyApplication = Niconicome.Models.Local.Application;
using Mylist = Niconicome.Models.Domain.Niconico.Mylist;
using Net = Niconicome.Models.Network;
using Niconico = Niconicome.Models.Domain.Niconico;
using Playlist = Niconicome.Models.Playlist;
using Search = Niconicome.Models.Domain.Niconico.Search;
using State = Niconicome.Models.Local.State;
using Store = Niconicome.Models.Domain.Local.Store;
using Utils = Niconicome.Models.Domain.Utils;
using UVideo = Niconicome.Models.Domain.Niconico.Video;
using DomainXeno = Niconicome.Models.Domain.Local.External.Import.Xeno;
using Import = Niconicome.Models.Local.External.Import;
using Handlers = Niconicome.Models.Domain.Local.Handlers;

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
                    if (builder.PrimaryHandler is HttpClientHandler handler)
                    {
                        handler.CookieContainer = builder.Services.GetRequiredService<Niconico::ICookieManager>().CookieContainer;
                        handler.UseCookies = true;
                    }
                });
            services.AddSingleton<Niconico::ICookieManager, Niconico::CookieManager>();
            services.AddTransient<Niconico.IDbUrlHandler, Niconico::DbUrlHandler>();
            services.AddTransient<Auth::ISession, Auth::Session>();
            services.AddTransient<Niconico::INiconicoContext, Niconico::NiconicoContext>();
            services.AddTransient<Utils::IErrorHandler, Utils::ErrorHandler>();
            services.AddSingleton<Utils::ILogStream, Utils::LogStream>();
            services.AddTransient<Utils::ILogger, Utils::Logger>();
            services.AddSingleton<DataBase::IDataBase, DataBase::DataBase>();
            services.AddTransient<Store::IPlaylistStoreHandler, Store::PlaylistStoreHandler>();
            services.AddTransient<Store::IVideoStoreHandler, Store::VideoStoreHandler>();
            services.AddSingleton<Playlist::IPlaylistVideoHandler, Playlist::PlaylistVideoHandler>();
            services.AddTransient<Playlist::ITreePlaylistInfoHandler, Playlist::TreePlaylistInfoHandler>();
            services.AddTransient<DomainWatch::IWatchPageHtmlParser, DomainWatch::WatchPageHtmlParser>();
            services.AddTransient<DomainWatch::IWatchInfohandler, DomainWatch::WatchInfohandler>();
            services.AddTransient<Net::IWatch, Net::Watch>();
            services.AddTransient<Net::IVideoThumnailUtility, Net::VideoThumnailUtility>();
            services.AddSingleton<Playlist::ICurrent, Playlist::Current>();
            services.AddTransient<Mylist::IMylistHandler, Mylist::MylistHandler>();
            services.AddTransient<Mylist::IWatchLaterHandler, Mylist::WatchLaterHandler>();
            services.AddTransient<UVideo::IUserVideoHandler, UVideo::UserVideoHandler>();
            services.AddTransient<Net::IRemotePlaylistHandler, Net::RemotePlaylistHandler>();
            services.AddTransient<State::ILocalInfo, State::LocalInfo>();
            services.AddTransient<DomainNet::ICacheHandler, DomainNet::CacheHandler>();
            services.AddTransient<DomainNet::ICacheStraem, DomainNet::CacheStream>();
            services.AddTransient<Net::INetworkVideoHandler, Net::NetworkVideoHandler>();
            services.AddSingleton<State::IMessageHandler, State::MessageHandler>();
            services.AddTransient<Auth::IAccountManager, Auth::AccountManager>();
            services.AddTransient<DomainWatch::IDmcDataHandler, DomainWatch::DmcDataHandler>();
            services.AddTransient<DomainWatch::IWatchSession, DomainWatch::WatchSession>();
            services.AddTransient<DomainWatch::IWatchPlaylisthandler, DomainWatch::WatchPlaylistHandler>();
            services.AddTransient<Dmc::IStreamhandler, Dmc::StreamHandler>();
            services.AddTransient<Dmc::IM3U8Handler, Dmc::M3U8Handler>();
            services.AddTransient<DlVideo::IDownloadTaskHandler, DlVideo::DownloadTaskHandler>();
            services.AddTransient<DlVideo::IVideoDownloadHelper, DlVideo::VideoDownloadHelper>();
            services.AddTransient<DlVideo::IVideoDownloader, DlVideo::VideoDownloader>();
            services.AddTransient<DlVideo::ISegmentWriter, DlVideo::SegmentWriter>();
            services.AddTransient<Download::IDownloadMessenger, Download::DownloadMessanger>();
            services.AddTransient<DlVideo::IVideoEncoader, DlVideo::VideoEncoader>();
            services.AddTransient<DlVideo::ITsMerge, DlVideo::TsMerge>();
            services.AddTransient<Net::IContentDownloader, Net::ContentDownloader>();
            services.AddTransient<DlThumb::IThumbDownloader, DlThumb::ThumbDownloader>();
            services.AddTransient<Playlist::IVideoFilter, Playlist::VideoFilter>();
            services.AddTransient<Search::ISearch, Search::Search>();
            services.AddTransient<Search::ISearchClient, Search::SearchClient>();
            services.AddSingleton<State::IErrorMessanger, State::ErrorMessenger>();
            services.AddTransient<MyApplication::IStartUp, MyApplication::StartUp>();
            services.AddTransient<Store::ISettingHandler, Store::SettingHandler>();
            services.AddTransient<Local::ILocalSettingHandler, Local::LocalSettingHandler>();
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
            services.AddTransient<Net::ILocalContentHandler, Net::LocalContentHandler>();
            services.AddTransient<Handlers::ICoreWebview2Handler, Handlers::CoreWebview2Handler>();

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
