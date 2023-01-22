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
        };
    }
}
