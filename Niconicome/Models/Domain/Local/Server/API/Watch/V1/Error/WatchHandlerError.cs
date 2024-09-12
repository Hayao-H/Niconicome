using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Local.Server.API.Watch.V1.Error
{
    public enum WatchHandlerError
    {
        [ErrorEnum(ErrorLevel.Error, "URLからセッションIDを取得できませんでした。(url:{0})")]
        CannotExtractSessionID,
        [ErrorEnum(ErrorLevel.Error, "URLからセッションプレイリスト・動画IDを取得できませんでした。(url:{0})")]
        CannotExtractPlaylistAndVideoID,
        [ErrorEnum(ErrorLevel.Error, "有効なストリームの検索に失敗しました。")]
        StreamNotFound,
        [ErrorEnum(ErrorLevel.Error,"ストリームの書き込みに失敗しました。")]
        FailedToWriteStream,
    }
}
