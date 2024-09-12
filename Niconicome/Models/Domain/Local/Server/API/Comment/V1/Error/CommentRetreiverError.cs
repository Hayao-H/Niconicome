using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Local.Server.API.Comment.V1.Error
{
    public enum CommentRetreiverError
    {
        [ErrorEnum(ErrorLevel.Error, "コメントファイルが見つかりませんでした。")]
        CommentFileNotFound,
        [ErrorEnum(ErrorLevel.Error, "コメントのデシリアライズに失敗しました。")]
        FailedToDeserializeComment,
    }
}
