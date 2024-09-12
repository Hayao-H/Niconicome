using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Local.Server.API.Comment.V1.Error
{
    public enum CommentRequestHandlerError
    {
        [ErrorEnum(ErrorLevel.Error, "URLからセッションプレイリスト・動画IDを取得できませんでした。(url:{0})")]
        CannotExtractPlaylistAndVideoID,
        [ErrorEnum(ErrorLevel.Error, "指定されたコンテンツは存在しません。")]
        NotFound,
    }
}
