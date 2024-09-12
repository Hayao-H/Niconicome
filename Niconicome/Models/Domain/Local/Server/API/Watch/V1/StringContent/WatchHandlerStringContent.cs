using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.StringHandler;

namespace Niconicome.Models.Domain.Local.Server.API.Watch.V1.StringContent
{
    public enum WatchHandlerStringContent
    {
        [StringEnum("URLが不正です。")]
        InvalidRequest,
        [StringEnum("動画IDまたはプレイリストIDが不正です。")]
        FailedToExtractVideoID,
        [StringEnum("ストリームの読み込みに失敗しました 。")]
        FailedToReadStream,
        [StringEnum("ストリームの書き込みに失敗しました 。")]
        FailedToWriteStream,
        [StringEnum("指定された動画は存在しません。")]
        NotFound,
        [StringEnum("セッションIDが不正です。")]
        SessionNotExist,
        [StringEnum("ファイルの読み込みに失敗しました。")]
        FailedtoLoadFile,
    }
}
