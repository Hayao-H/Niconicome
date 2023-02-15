using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Playlist.V2.Manager.Error
{
    public enum VideoListManagerError
    {
        [ErrorEnum(ErrorLevel.Warning, "プレイリストが選択されていない状態で動画リストの更新を試行しました。")]
        PlaylistIsNotSelected,
        [ErrorEnum(ErrorLevel.Log, "動画を読み込み中に選択されているプレイリストが変更されました。")]
        PlaylistChanged,
        [ErrorEnum(ErrorLevel.Error,"選択されているプレイリストはリモートプレイリストではありません。(PlaylistType:{0})")]
        NotARemotePlaylist,
        [ErrorEnum(ErrorLevel.Log,"リモートプレイリストとの同期が完了しました。(Type:{0}, 追加：{1}, 削除{2}, 更新：{3})")]
        SyncWithRemotePlaylistHasCompleted,
        [ErrorEnum(ErrorLevel.Error,"指定された動画は現在のプレイリストに存在しません。(playlist:{0}, id:{1})")]
        VideoDoesNotExistInCurrentPlaylist,
    }
}
