using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V2.Fetch.Segment.AES.Error
{
    public enum AESInfomationHandlerError
    {
        [ErrorEnum(ErrorLevel.Error, "Keyの取得に失敗しました。(status:{0})")]
        FailedToGetKey,
        [ErrorEnum(ErrorLevel.Error, "Keyの解析に失敗しました。")]
        FailedToParse,
        [ErrorEnum(ErrorLevel.Error,"不正なIVです。(iv:{0})")]
        InvalidIV,
    }
}
