using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Local.External.Playlist
{
    public enum PlaylistCreatorError
    {
        [ErrorEnum(ErrorLevel.Error,"ダウンロード済の動画が存在しません。")]
        DownloadedVideoDoesNotExist,
        [ErrorEnum(ErrorLevel.Error, "プレイリストの作成に失敗しました。(type:{0})")]
        FailedToCreatePlaylis,
    }
}
