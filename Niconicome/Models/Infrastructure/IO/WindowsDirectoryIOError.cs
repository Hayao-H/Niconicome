using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Infrastructure.IO
{
    public enum WindowsDirectoryIOError
    {
        [ErrorEnum(ErrorLevel.Error, "デイレクトリの作成に失敗しました。(path:{0})")]
        FailedToCreateDirectory,
        [ErrorEnum(ErrorLevel.Error,"ディレクトリの削除に失敗しました。(path:{0})")]
        FailedToDeleteDirectory,
        [ErrorEnum(ErrorLevel.Error, "デイレクトリ一覧の取得に失敗しました。(path:{0})")]
        FailedToGetDirectories,
        [ErrorEnum(ErrorLevel.Error, "ファイル一覧の取得に失敗しました。(path:{0})")]
        FailedToGetFiles,
        [ErrorEnum(ErrorLevel.Error, "ディレクトリの移動に失敗しました。(source:{0},destination:{1})")]
        FailedToMoveDirectory,
    }
}
