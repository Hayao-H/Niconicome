using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Domain.Utils.StringHandler;

namespace Niconicome.Models.Playlist.V2.Manager.StringContent
{
    public enum VideoListManagerString
    {

        [StringEnum("リモートプレイリストとの同期が完了しました。(追加：{0}, 削除{1}, 更新：{2})")]
        SyncWithRemotePlaylistHasCompleted,
    }
}
