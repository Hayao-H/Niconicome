using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Niconico.Remote.V2.Error;
using Niconicome.Models.Domain.Niconico.Watch.V2.Error;
using Niconicome.Models.Infrastructure.Database.Error;
using Niconicome.Models.Infrastructure.Database.Json;
using Niconicome.Models.Infrastructure.Database.LiteDB;
using Niconicome.Models.Infrastructure.IO;
using Niconicome.Models.Local.External.Error;
using Niconicome.Models.Local.External.Playlist;
using Niconicome.Models.Local.OS;
using Niconicome.Models.Network.Video.Error;
using Niconicome.Models.Playlist.V2.Manager.Error;

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
            { 17, typeof(ClipboardManagerError) },
            { 18, typeof(ExternalAppUtilsV2Error) },
            { 19, typeof(WindowsFileIOError) },
            { 20, typeof(WindowsDirectoryIOError) },
            { 21, typeof(PlaylistCreatorError) },
            { 22, typeof(PlaylistDBHandlerError) },
        };
    }
}
