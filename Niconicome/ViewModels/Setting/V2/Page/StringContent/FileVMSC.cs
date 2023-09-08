using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.StringHandler;

namespace Niconicome.ViewModels.Setting.V2.Page.StringContent
{
    public enum FileVMSC
    {
        [StringEnum("ルールの追加に失敗しました。")]
        FailedToAddRule,
        [StringEnum("ルールの削除に失敗しました。")]
        FailedToRemovedRule,
        [StringEnum("詳細：{0}")]
        Detail,

    }
}
