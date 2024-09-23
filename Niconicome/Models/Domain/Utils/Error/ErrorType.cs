using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Addons.API.Hooks;
using Niconicome.Models.Domain.Local.DataBackup;
using Niconicome.Models.Domain.Local.DataBackup.Import.Niconicome.Error;
using Niconicome.Models.Domain.Local.DataBackup.Import.Xeno.Error;
using Niconicome.Models.Domain.Local.External.Software.FFmpeg.FFmpeg;
using Niconicome.Models.Domain.Local.External.Software.FFmpeg.ffprobe;
using Niconicome.Models.Domain.Local.External.Software.NiconicomeProcess;
using Niconicome.Models.Domain.Local.LocalFile.Error;
using Niconicome.Models.Domain.Local.Server.Core;
using Niconicome.Models.Domain.Local.Server.HLS;
using Niconicome.Models.Domain.Local.Server.RequestHandler.M3U8;
using Niconicome.Models.Domain.Local.Server.RequestHandler.NotFound;
using Niconicome.Models.Domain.Local.Server.RequestHandler.TS;
using Niconicome.Models.Domain.Local.Server.RequestHandler.UserChrome;
using Niconicome.Models.Domain.Local.Server.RequestHandler.Video;
using Niconicome.Models.Domain.Niconico.Download.Comment.V2.Fetch.V3.Error;
using Niconicome.Models.Domain.Niconico.Download.Comment.V2.Fetch.V3.Threadkey;
using Niconicome.Models.Domain.Niconico.Download.General;
using Niconicome.Models.Domain.Niconico.Download.Video.V2.Error;
using Niconicome.Models.Domain.Niconico.Download.Video.V2.Fetch.Segment.AES.Error;
using Niconicome.Models.Domain.Niconico.Download.Video.V2.Fetch.Segment.Error;
using Niconicome.Models.Domain.Niconico.Download.Video.V2.HLS.Error;
using Niconicome.Models.Domain.Niconico.Download.Video.V2.Local.Encode;
using Niconicome.Models.Domain.Niconico.Download.Video.V2.Local.HLS.Error;
using Niconicome.Models.Domain.Niconico.Remote.V2.Error;
using Niconicome.Models.Domain.Niconico.Watch.V2.Error;
using Niconicome.Models.Infrastructure.Database.Error;
using Niconicome.Models.Infrastructure.Database.Json;
using Niconicome.Models.Infrastructure.Database.LiteDB;
using Niconicome.Models.Infrastructure.IO;
using Niconicome.Models.Infrastructure.IO.Media.Audio;
using Niconicome.Models.Infrastructure.Network;
using Niconicome.Models.Local.External.Error;
using Niconicome.Models.Local.External.Playlist;
using Niconicome.Models.Local.Restore;
using Niconicome.Models.Network.Download.DLTask.Error;
using Niconicome.Models.Network.Video.Error;
using Niconicome.Models.Playlist.V2.Manager.Error;
using VDL3 = Niconicome.Models.Domain.Niconico.Download.Video.V3.Error;
using WatchAPIV1 = Niconicome.Models.Domain.Local.Server.API.Watch.V1.Error;
using CommentAPIV1 = Niconicome.Models.Domain.Local.Server.API.Comment.V1.Error;
using HLSAPIV1 = Niconicome.Models.Domain.Local.Server.API.RegacyHLS.V1.Error;
using ResourceAPIV1 = Niconicome.Models.Domain.Local.Server.API.Resource.V1.Error;
using VideoInfoAPIV1 = Niconicome.Models.Domain.Local.Server.API.VideoInfo.V1.Error;
using PostDL = Niconicome.Models.Network.Download.Actions.V2;
using Auth = Niconicome.Models.Auth.Error;
using Niconico = Niconicome.Models.Domain.Niconico;
using CommentCollection = Niconicome.Models.Domain.Niconico.Download.Comment.V2.Core.V3.Error;
using Application = Niconicome.Models.Local.Application.Error;

namespace Niconicome.Models.Domain.Utils.Error
{
    public static class ErrorTypes
    {
        /// <summary>
        /// エラー列挙型の型一覧
        /// </summary>
        public static Dictionary<int, Type> ErrorEnums = new()
        {
            { 1, typeof(LiteDBError) },
            { 2, typeof(PlaylistManagerError) },
            { 3, typeof(SettingJSONError) },
            { 4, typeof(ThumbnailUtilityError) },
            { 5, typeof(LocalVideoLoaderError) },
            { 6, typeof(VideoListManagerError) },
            { 7, typeof(ApplicationDBHandlerError) },
            { 8, typeof(MylistError) },
            { 9, typeof(WatchLaterError) },
            { 10, typeof(SeriesError) },
            { 11, typeof(ChannelError) },
            { 12, typeof(SearchError) },
            { 13, typeof(UserVideoError) },
            { 14, typeof(NetVideosInfomationHandlerError) },
            { 15, typeof(WatchPageInfomationHandlerError) },
            { 16, typeof(ExternalProcessUtilsError) },
            { 17, typeof(WindowsClipboardManagerError) },
            { 18, typeof(ExternalAppUtilsV2Error) },
            { 19, typeof(WindowsFileIOError) },
            { 20, typeof(WindowsDirectoryIOError) },
            { 21, typeof(PlaylistCreatorError) },
            { 22, typeof(PlaylistDBHandlerError) },
            { 23, typeof(VideoFileDBHandlerError) },
            { 24, typeof(RestoreManagerError) },
            { 25, typeof(BackupManagerError) },
            { 26, typeof(VideoRequestHandlerError) },
            { 27, typeof(NotFoundRequestHandlerError) },
            { 28, typeof(ServerError) },
            { 29, typeof(HLSManagerError) },
            { 30, typeof(M3U8RequestHandlerError) },
            { 31, typeof(TSRequestHandlerError) },
            { 32, typeof(UserChromeRequestHandlerError) },
            { 33, typeof(ProcessManagerError) },
            { 34, typeof(FFprobeHandlerError) },
            { 35, typeof(SearchManagerError) },
            { 36, typeof(ImportError) },
            { 37, typeof(XenoDataParserError) },
            { 38, typeof(XenoImportHandlerError) },
            { 39, typeof(DBCleanerError) },
            { 40, typeof(FFmpegManagerError) },
            { 41, typeof(NetworkError) },
            { 42, typeof(ThreadKeyError) },
            { 43, typeof(CommentClientError) },
            { 44, typeof(HooksManagerError) },
            { 45, typeof(AppError) },
            { 46, typeof(MasterPlaylistHandlerError) },
            { 47, typeof(SegmentDirectoryHandlerError) },
            { 48, typeof(VideoEncoderError) },
            { 49, typeof(WatchSessionError) },
            { 50, typeof(SegmentDownloaderError) },
            { 51, typeof(SegmentWriterError) },
            { 52, typeof(AESInfomationHandlerError) },
            { 53, typeof(DecryptorError) },
            { 54, typeof(ReplaceHandlerError) },
            { 55, typeof(DownloadManagerError) },
            { 56, typeof(NaudioHandlerError) },
            { 57, typeof(LocalFileRemoverError) },
            { 58, typeof(VDL3.StreamParserError) },
            { 59, typeof(VDL3.SegmentDownloaderError) },
            { 60, typeof(VDL3.SegmentDirectoryHandlerError) },
            { 61, typeof(VDL3.SegmentWriterError) },
            { 62, typeof(VDL3.WatchSessionError) },
            { 63, typeof(VDL3.KeyDownlaoderError) },
            { 64, typeof(VDL3.StreamJsonHandlerError) },
            { 65, typeof(WatchAPIV1.WatchHandlerError) },
            { 66, typeof(WatchAPIV1.DecryptorError) },
            { 67, typeof(CommentAPIV1::CommentRequestHandlerError) },
            { 68, typeof(CommentAPIV1::CommentRetreiverError) },
            { 69, typeof(HLSAPIV1::HLSManagerError) },
            { 70, typeof(HLSAPIV1::RegacyHLSHandlerError) },
            { 71, typeof(ResourceAPIV1::ResourceHandlerError) },
            { 72, typeof(VideoInfoAPIV1::VideoInfoHandlerError) },
            { 73, typeof(PostDL::PostDownloadActionsManagerError) },
            { 74, typeof(Auth::ChromeSharedLoginError) },
            { 75, typeof(CookieDBHandlerError) },
            { 76, typeof(Niconico::NiconicoContextError) },
            { 77, typeof(CommentCollection::ChildCommentCollectionError) },
            { 78, typeof(Application::ThemeHandlerError) },
        };
    }
}
