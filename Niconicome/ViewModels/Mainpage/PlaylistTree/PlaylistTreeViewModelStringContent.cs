using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.StringHandler;

namespace Niconicome.ViewModels.Mainpage.PlaylistTree
{
    public enum PlaylistTreeViewModelStringContent
    {
        [StringEnum("プレイリストが作成されました。")]
        PlaylistCreated,
        [StringEnum("プレイリストが削除されました。(id:{0})")]
        PlaylistDeleted,
        [StringEnum("本当にプレイリストを削除しますか？")]
        ConfirmOfDeletion,
    }
}
