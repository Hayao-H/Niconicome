using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.StringHandler;

namespace Niconicome.ViewModels.Setting.V2.Page.StringContent
{
    public enum ImportVMSC
    {
        [StringEnum("データのエクスポートに失敗しました。")]
        ExportFailed,
        [StringEnum("データのエクスポートに失敗しました。（詳細：{0}）")]
        ExportFailedDetal,
        [StringEnum("データのエクスポートに成功しました。（path：{0}）")]
        ExportSucceeded,
        [StringEnum("パスが未入力です。")]
        ImportPathIsEmpty,
        [StringEnum("データのインポートに失敗しました。")]
        ImportFailed,
        [StringEnum("データのインポートに失敗しました。（詳細：{0}）")]
        ImportFailedDetail,
        [StringEnum("データのインポートに成功しました。（プレイリスト：{0}件、動画：{1}件）")]
        ImportSucceeded,
    }
}
