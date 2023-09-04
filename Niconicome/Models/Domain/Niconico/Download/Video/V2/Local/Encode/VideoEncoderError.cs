using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V2.Local.Encode
{
    public enum VideoEncoderError
    {
        [ErrorEnum(ErrorLevel.Error,"セグメントファイルの結合に失敗しました。(target path:{0})")]
        FailedToConcatTS,
        [ErrorEnum(ErrorLevel.Error,"保存先フォルダーのパス取得に失敗しました。(path:{0})")]
        FailedToGetTargetFolderPath,
        [ErrorEnum(ErrorLevel.Error,"エンコード処理がキャンセルされました。")]
        Canceled,
    }
}
