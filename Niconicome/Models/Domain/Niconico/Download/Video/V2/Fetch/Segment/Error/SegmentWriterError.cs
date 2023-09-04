using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V2.Fetch.Segment.Error
{
    public enum SegmentWriterError
    {
        [ErrorEnum(ErrorLevel.Error,"セグメントファイルの保存先ディレクトリパスの取得に失敗しました。(path:{0})")]
        FailedToGetDirPath,
    }
}
