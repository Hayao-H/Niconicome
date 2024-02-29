using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V3.Error
{
    public enum KeyDownlaoderError
    {
        [ErrorEnum(ErrorLevel.Error,"キーのダウンロードに失敗しました。")]
        FailedToDownloadKey,
    }
}
