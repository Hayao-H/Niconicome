using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Local.Playlist
{
    public enum PlaylistFileFactoryError
    {
        [ErrorEnum(ErrorLevel.Error, "空のプレイリストを作成することは出来ません。")]
        CannotCreateEmptyPlaylist,
        [ErrorEnum(ErrorLevel.Error, "不明なプレイリストです。(type:{0})")]
        PlaylistIsNotSupported,
    }
}
