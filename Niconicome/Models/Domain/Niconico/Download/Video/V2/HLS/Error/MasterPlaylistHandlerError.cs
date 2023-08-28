using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V2.HLS.Error
{
    public enum MasterPlaylistHandlerError
    {
        [ErrorEnum(ErrorLevel.Error,"master.m3u8の取得に失敗しました。(url：{0}, status：{1})")]
        FailedToFetchMasterPlaylist,
        [ErrorEnum(ErrorLevel.Error,"master.m3u8の読み込みに失敗しました。(url：{0})")]
        FailedToLoadMasterPlaylist,
        [ErrorEnum(ErrorLevel.Error,"playlist.m3u8の取得に失敗しました。(url：{0}, status：{1})")]
        FailedToFetchPlaylist,
        [ErrorEnum(ErrorLevel.Error, "playlist.m3u8の読み込みに失敗しました。(url：{0})")]
        FailedToLoadPlaylist,
        [ErrorEnum(ErrorLevel.Error,"Streamの取得に失敗しました。(url: {0})")]
        FailedToGetStreams,
    }
}
