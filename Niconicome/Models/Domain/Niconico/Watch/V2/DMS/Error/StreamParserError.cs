using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Niconico.Watch.V2.DMS.Error
{
    public enum StreamParserError
    {
        [ErrorEnum(ErrorLevel.Error, "ストリーム情報の取得に失敗しました。(status:{0}, url:{1})")]
        FailedToGetPlaylistWithHttpError,
        [ErrorEnum(ErrorLevel.Error, "ストリーム情報の取得に失敗しました。(詳細：{0})")]
        FailedToGetPlaylist,
    }
}
