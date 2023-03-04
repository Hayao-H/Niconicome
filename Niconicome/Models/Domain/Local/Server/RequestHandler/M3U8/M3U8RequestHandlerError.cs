using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Local.Server.RequestHandler.M3U8
{
    public enum M3U8RequestHandlerError
    {
        [ErrorEnum(ErrorLevel.Error, "プレイリストが存在しません。")]
        PlaylistDoesNotExist,
        [ErrorEnum(ErrorLevel.Error, "プレイリストファイルへのアクセスに失敗しました。")]
        FailedToAccessPlaylist,
    }
}
