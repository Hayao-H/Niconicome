using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Infrastructure.IO
{
    public enum WindowsFileIOError
    {
        [ErrorEnum(ErrorLevel.Error, "指定されたファイルが存在しません。(path:{0})")]
        FileDoesNotExist,
        [ErrorEnum(ErrorLevel.Error,"Shell.APplicationの型情報取得に失敗しました。")]
        FailedToGetShellType,
        [ErrorEnum(ErrorLevel.Error,"Shellインスタンスの作成に失敗しました。")]
        FailedToCreateShellInstance,
        [ErrorEnum(ErrorLevel.Error,"Shell.Applicationでの垂直方向解像度の取得に失敗しました。")]
        FailedToGetVerticalResolution,
        [ErrorEnum(ErrorLevel.Error,"垂直方向解像度の解析に失敗しました。")]
        FailedToParseVerticalResoltion,
        [ErrorEnum(ErrorLevel.Error,"ファイルへの書き込みに失敗しました。(path:{0})")]
        FailedToWrite,
        [ErrorEnum(ErrorLevel.Error, "ファイルへの読み込みに失敗しました。(path:{0})")]
        FailedToRead,
        [ErrorEnum(ErrorLevel.Error, "ファイル検索中にエラーが発生しました。(path:{0})")]
        ErrorWhenEnumerateVideoFiles,
        [ErrorEnum(ErrorLevel.Error,"ファイルの削除に失敗しました。(path:{0})")]
        FailedToDeleteFile,
        [ErrorEnum(ErrorLevel.Error, "ファイルのコピーに失敗しました。(source:{0}, target:{1})")]
        FailedToCopyFile,
        [ErrorEnum(ErrorLevel.Error,"最終書き込み日時の変更に失敗しました。(path:{0})")]
        FailedToSetLastWriteTime,
    }
}
