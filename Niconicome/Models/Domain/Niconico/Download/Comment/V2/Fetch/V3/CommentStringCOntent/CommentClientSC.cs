using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.StringHandler;

namespace Niconicome.Models.Domain.Niconico.Download.Comment.V2.Fetch.V3.CommentStringContent
{
    public enum CommentClientSC
    {
        [StringEnum("過去ログを取得中（{0}件目・{1}コメ取得済み）")]
        FetchingKakoLog,
        [StringEnum("コメントを再取得中（{0}件目・{1}コメ取得済み）")]
        FetchingUnfethed,
        [StringEnum("過去ログの取得を開始します。")]
        StartFetchKakoLog,
        [StringEnum("取得できなかったコメントを再取得します。")]
        StartFetchUnfetched,
    }
}
