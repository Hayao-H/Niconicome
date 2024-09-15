using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Niconico.Download.Comment.V2.Core.V3.Error
{
    public enum ChildCommentCollectionError
    {
        [ErrorEnum(ErrorLevel.Error, "コメントのThreadまたはForkがコレクションのThreadまたはForkと一致しません。")]
        InvalidAddParameter,
        [ErrorEnum(ErrorLevel.Error, "コメント番号が範囲外です。")]
        CommentIndexOutOfRange,
        [ErrorEnum(ErrorLevel.Error, "コメントが存在しません。(No:{0})")]
        CommentNotFound,
        [ErrorEnum(ErrorLevel.Error, "最初のコメントを取得できませんでした。コメントが空の可能性があります。")]
        CannotGetFirstComment,
    }
}
