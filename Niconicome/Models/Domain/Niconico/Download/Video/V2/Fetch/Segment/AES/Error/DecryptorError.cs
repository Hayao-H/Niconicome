using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V2.Fetch.Segment.AES.Error
{
    public enum DecryptorError
    {
        [ErrorEnum(ErrorLevel.Error,"復号に失敗しました。")]
        FailedToDecrypt,
    }
}
