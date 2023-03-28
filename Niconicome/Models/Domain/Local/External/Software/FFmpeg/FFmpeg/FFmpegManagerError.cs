using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Local.External.Software.FFmpeg.FFmpeg
{
    public enum FFmpegManagerError
    {
        [ErrorEnum(ErrorLevel.Error, "ffmpegの実行に失敗しました。(詳細:{0})")]
        FailedToRunFFmpeg,
    }
}
