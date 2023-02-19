using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Domain.Utils.StringHandler;

namespace Niconicome.ViewModels.Mainpage.Tabs.VideoList.Pages.StringContent
{
    public enum IndexViewModelStringContent
    {
        [StringEnum($"ブラウザでの視聴に失敗しました。")]
        FailedToOpenVideoInBrowser,
        [StringEnum("ブラウザで視聴を開始しました。")]
        VideoOpenedInBrowser,
        [StringEnum("動画の追加に成功しました。({0}件)")]
        VideoAdded,
        [StringEnum("この動画のチャンネルは「{0}」です")]
        NotifyChannelID,
        [StringEnum("IDをコピー")]
        CopyChannelIDAction,
        [StringEnum("「{0}」")]
        DeletionConfitmMessageSingle,
        [StringEnum("「{0}」ほか{1}件")]
        DeletionConfitmMessageMulti,
        [StringEnum("{0}件の動画を削除しました。")]
        VideoDeleted,
        [StringEnum("クリップボードに情報をコピーしました。")]
        InfomationCopied,
    }
}
