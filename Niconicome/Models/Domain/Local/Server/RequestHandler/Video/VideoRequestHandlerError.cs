using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Local.Server.RequestHandler.Video
{
    public enum VideoRequestHandlerError
    {
        [ErrorEnum(ErrorLevel.Error,"リクエストが不正です。(url:{0})")]
        RequestUrlInalid,
        [ErrorEnum(ErrorLevel.Error,"指定された動画はダウンロードされていません。(playlist:{0},id:{1})")]
        VideoIsNotDownloaded,
        [ErrorEnum(ErrorLevel.Error,"動画ファイルが存在しません。(path:{0})")]
        VideoDoesNotExist,
        [ErrorEnum(ErrorLevel.Error,"動画ファイルへのアクセスに失敗しました。")]
        FailedToOpenVodeo,
    }
}
