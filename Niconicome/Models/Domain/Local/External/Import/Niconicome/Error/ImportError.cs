using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Local.External.Import.Niconicome.Error
{
    public enum ImportError
    {
        [ErrorEnum(ErrorLevel.Error, "インポート対象のデータが壊れています。")]
        DataIsInvalid,
        [ErrorEnum(ErrorLevel.Error, "インポートデータの解析に失敗しました。")]
        FailedToLoadData,
        [ErrorEnum(ErrorLevel.Warning, "プレイリストのインポートに失敗しました。(name：{0})")]
        FailedToImportPlaylist,
        [ErrorEnum(ErrorLevel.Warning,"動画のインポートに失敗しました。(playlist：{0}, id:{1})")]
        FailedToImportVideo,
        [ErrorEnum(ErrorLevel.Warning, "タグのインポートに失敗しました。(name:{0})")]
        FailedToImportTag,
    }
}
