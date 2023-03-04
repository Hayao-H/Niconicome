using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Local.DataBackup
{
    public enum BackupManagerError
    {
        [ErrorEnum(ErrorLevel.Error, "インデックスファイルの読み込みに失敗しました。")]
        FailedToLoadIndex,
        [ErrorEnum(ErrorLevel.Error, "インデックスファイルの書き込みに失敗しました。")]
        FailedToWriteIndex,
        [ErrorEnum(ErrorLevel.Error, "バックアップフォルダーの作成に失敗しました。")]
        FailedToCreateDirectory,
        [ErrorEnum(ErrorLevel.Error,"DBファイルが存在しません。")]
        DBFileNotExists,
        [ErrorEnum(ErrorLevel.Error,"指定されたIDのバックアップは存在しません。(id:{0})")]
        SpecifiedBackupDoesNotExists,
        [ErrorEnum(ErrorLevel.Error,"バックアップファイルが存在しません(id:{0})")]
        BackupFileDoesNotExist,
    }
}
