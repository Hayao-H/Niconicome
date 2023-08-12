using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.StringHandler;

namespace Niconicome.ViewModels.Setting.V2.Page.StringContent
{
    public enum DebugVMSC
    {
        [StringEnum("有効")]
        Enable,
        [StringEnum("無効")]
        Disable,
        [StringEnum("デバッグモード：{0}")]
        Debug,
        [StringEnum("クリップボードにコピーしました。")]
        LogFilePathCopied,
        [StringEnum("クリップボードへのコピーに失敗しました。")]
        LogFilePathCopyFailed,
        [StringEnum("クリップボードへのコピーに失敗しました。(詳細：{0})")]
        LogFilePathCopyFailedDetail,
    }
}
