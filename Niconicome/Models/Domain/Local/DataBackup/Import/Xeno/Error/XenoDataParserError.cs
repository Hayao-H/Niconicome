using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Local.DataBackup.Import.Xeno.Error
{
    public enum XenoDataParserError
    {
        [ErrorEnum(ErrorLevel.Error,"指定されたファイルが存在しません。(path:{0})")]
        FileDoesNotExist,
        [ErrorEnum(ErrorLevel.Error,"行の解析に失敗しました。(inpuit:{0})")]
        FailedToParseLineOfRoot,
        [ErrorEnum(ErrorLevel.Error,"レイヤーの取得に失敗しました。(input:{0})")]
        FailedToParseayer,
        [ErrorEnum(ErrorLevel.Error,"レイヤー構造に問題が存在するためノードの変換に失敗しました。(prevLayer:{0}, layer:{1})")]
        FailedToConvertRootNodeToPlaylist,
        [ErrorEnum(ErrorLevel.Error,"行の解析に失敗しました。(inpuit:{0})")]
        FailedToParseLineOfPlaylist,
        [ErrorEnum(ErrorLevel.Error,"動画情報の解析に失敗しました。")]
        FailedToParseVideo,
    }
}
