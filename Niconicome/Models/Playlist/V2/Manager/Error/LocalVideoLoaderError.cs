using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Playlist.V2.Manager.Error
{
    public enum LocalVideoLoaderError
    {
        [ErrorEnum(ErrorLevel.Error, "プレイリストが選択されていない状態で動画リストの更新を試行しました。")]
        PlaylistIsNotSelected,
        [ErrorEnum(ErrorLevel.Log,"動画を読み込み中に選択されているプレイリストが変更されました。")]
        PlaylistChanged,
    }
}
