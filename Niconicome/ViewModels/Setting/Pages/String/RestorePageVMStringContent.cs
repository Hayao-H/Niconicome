using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Domain.Utils.StringHandler;

namespace Niconicome.ViewModels.Setting.Pages.String
{
    public enum RestorePageVMStringContent
    {
        [StringEnum("バックアップ「{0}」を作成しました。")]
        BackupCreated,
        [StringEnum("バックアップを削除しました。")]
        BackupDeleted,
        [StringEnum("バックアップの削除に失敗しました。")]
        FailedToDeleteBackup,
        [StringEnum("バックアップの作成に失敗しました。")]
        FailedToCreateBackup,
        [StringEnum("本当にこのバックアップを適用しますか？現在の設定は全て削除され、操作は元に戻すことができません。")]
        ConfirmMessageOfApplyingBackup,
        [StringEnum("バックアップを適用しました。")]
        BackupApplyed,
        [StringEnum("再起動")]
        Restart,
        [StringEnum("バックアップの適用に失敗しました。")]
        FailedToApplyBackup,
        [StringEnum("本当に設定を削除しますか？この操作は元に戻すことができません。")]
        ConfirmMessageOfResetSetting,
        [StringEnum("設定をリセットしました。")]
        ResetOfSettingHasCompleted,
        [StringEnum("本当に全ての動画・プレイリストを削除しますか？この操作は元に戻すことができません。")]
        ConfirmMessageOfDeletingAllData,
        [StringEnum("データをリセットしました。")]
        DeletingAllDataHasCompleted,
    }
}
