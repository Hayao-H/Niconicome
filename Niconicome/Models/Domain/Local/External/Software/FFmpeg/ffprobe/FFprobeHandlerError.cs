using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Local.External.Software.FFmpeg.ffprobe
{
    public enum FFprobeHandlerError
    {
        [ErrorEnum(ErrorLevel.Error, "ffprobeの実行に失敗しました。(詳細:{0})")]
        FailedToRunFFprobe,
        [ErrorEnum(ErrorLevel.Error, "出力の読み込みに失敗しました。")]
        FailedToReadResponse,
        [ErrorEnum(ErrorLevel.Error, "例外の読み込みに失敗しました。")]
        FailedToReadError,
        [ErrorEnum(ErrorLevel.Error, "ffprobe出力データの解析に失敗しました。")]
        FailedToDeserializeData,
        [ErrorEnum(ErrorLevel.Error, "ffprobeの出力にvideoが含まれません。")]
        NoCodecTypeVideo,
    }
}
