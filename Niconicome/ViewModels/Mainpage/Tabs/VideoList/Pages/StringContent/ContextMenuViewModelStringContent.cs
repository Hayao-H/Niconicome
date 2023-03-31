using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.StringHandler;

namespace Niconicome.ViewModels.Mainpage.Tabs.VideoList.Pages.StringContent
{
   public enum ContextMenuViewModelStringContent
    {
        [StringEnum("保存フォルダーを開きました。")]
        SucceededToOpenExplorer,
        [StringEnum("エクスプローラーの起動に失敗しました。。")]
        FailedToOpenExplorer,
        [StringEnum("プレイリストの作成に成功しました。(失敗:{0}件)")]
        PlaylistCreated,
    }
}
