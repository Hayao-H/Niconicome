using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Infrastructure.Database.LiteDB
{
    public enum LiteDBError
    {
        [ErrorEnum(ErrorLevel.Error,"[{0}]テーブルへのアクセスに失敗しました。")]
        AccessFailed,
        [ErrorEnum(ErrorLevel.Log,"データベースが初期化されました。")]
        Initialized,
        [ErrorEnum(ErrorLevel.Log,"[{0}]テーブルが作成されました。")]
        TableCreated,
        [ErrorEnum(ErrorLevel.Log,"[{0}]テーブルを取得しました。")]
        TableRetrieved,
        [ErrorEnum(ErrorLevel.Error, "[{0}]テーブルの取得に失敗しました。")]
        TableRetrieveFailed,
        [ErrorEnum(ErrorLevel.Log,"[{0}]テーブルからID{1}のレコードを取得しました。")]
        RecordRetrieved,
        [ErrorEnum(ErrorLevel.Log, "[{0}]テーブルから全てのレコードを取得しました。")]
        AllRecordsRetrieved,
        [ErrorEnum(ErrorLevel.Log, "[{0}]テーブルにID{1}のレコードを挿入しました。")]
        RecordInserted,
        [ErrorEnum(ErrorLevel.Log, "[{0}]テーブルからID{1}のレコードを削除しました。")]
        RecordDeleted,
        [ErrorEnum(ErrorLevel.Log, "[{0}]テーブルから{1}件のレコードを削除しました。")]
        RecordsDeleted,
        [ErrorEnum(ErrorLevel.Warning, "[{0}]テーブルからのレコード削除に失敗しました。")]
        RecordsDeleteFailed,
        [ErrorEnum(ErrorLevel.Warning, "[{0}]テーブルからID{1}のレコード削除に失敗しました。")]
        RecordDeleteFailed,
        [ErrorEnum(ErrorLevel.Warning, "[{0}]テーブルには更新対象となるID{1}のレコードが存在しません。")]
        RecordUpdated,
        [ErrorEnum(ErrorLevel.Log, "[{0}]テーブルでID{1}のレコードを更新しました。")]
        UpdateTargetRecordNotExists,
        [ErrorEnum(ErrorLevel.Warning,"指定されたレコードが存在しません。")]
        RecordNotFound,
    }
}
