using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Network.Video.Error
{
    public enum ThumbnailUtilityError
    {
        [ErrorEnum(ErrorLevel.Log,"指定された動画(id:{0})のサムネイルがキャッシュとして存在しません。")]
        ThumbNotExist,
        [ErrorEnum(ErrorLevel.Error,"サムネイルのURL({0})は不正です。")]
        ThumbUrlIsInvalid,
        [ErrorEnum(ErrorLevel.Error,"サムネイルの取得に失敗しました。(url:{0} status_code:{1})")]
        ThumbFetchFaild,
        [ErrorEnum(ErrorLevel.Error,"サムネイルの書き込みに失敗しました。")]
        ThumbWritingFailed,
    }
}
