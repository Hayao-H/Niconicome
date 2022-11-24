using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Playlist.V2.Manager.Error
{
    public enum PlaylistManagerError
    {
        [ErrorEnum(ErrorLevel.Error,"指定したプレイリスト(ID:{0})がキャッシュ内に存在しません。")]
        PlaylistNotFount,
        [ErrorEnum(ErrorLevel.Error,"指定した親プレイリスト(ID:{0})がキャッシュ内に存在しません。")]
        ParentPlaylistNotFount,
    }
}
