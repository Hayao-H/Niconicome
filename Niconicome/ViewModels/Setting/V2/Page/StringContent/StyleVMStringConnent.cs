using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.StringHandler;

namespace Niconicome.ViewModels.Setting.V2.Page.StringContent
{
    public enum StyleVMStringContent
    {
        [StringEnum("ライト")]
        Light,
        [StringEnum("ダーク")]
        Dark,
        [StringEnum("システム設定に従う")]
        Inherit,
        [StringEnum("ユーザー定義スタイルシートの書き出しに失敗しました。")]
        FailedToWriteUserChrome,
        [StringEnum("ユーザー定義スタイルシートの書き出しに失敗しました。(詳細:{0})")]
        FailedToWriteUserChromeDetail,
        [StringEnum("ユーザー定義スタイルシートの書き出しが完了しました。")]
        WritingUserChromeHasCompleted,
        [StringEnum("システムテーマを適用するには、再起動する必要があります。")]
        NeedRestart,
        [StringEnum("再起動")]
        Restart,
        [StringEnum("現在のOSバージョンはでは、このオプションを選択できません。")]
        NoCompatibility,
    }
}
