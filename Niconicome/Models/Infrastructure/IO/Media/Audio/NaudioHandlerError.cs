using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Infrastructure.IO.Media.Audio
{
    public enum NaudioHandlerError
    {
        [ErrorEnum(ErrorLevel.Error,"指定されたファイルが存在しません。(path：{0})")]
        FileNotExist,
        [ErrorEnum(ErrorLevel.Error,"音声の再生中にエラーが発生しました。")]
        FailedToPlay,
    }
}
