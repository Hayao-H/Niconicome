using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Playlist.V2.Manager.Error
{
    public enum SearchManagerError
    {
        [ErrorEnum(ErrorLevel.Warning, "プレイリストが選択されていない状態で動画の登録を試行しました。")]
        PlaylistIsNotSelected,
    }
}
