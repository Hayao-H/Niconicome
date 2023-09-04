using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Niconico.Download.Video.V2.Local.HLS.Error
{
    public enum SegmentDirectoryHandlerError
    {
        [ErrorEnum(ErrorLevel.Error,"指定されたセグメントディレクトリが存在しません。(id:{0}, resolution:{1})")]
        NotExists,
        [ErrorEnum(ErrorLevel.Error,"不正なディレクトリ名です。(path:{0})")]
        InvalidDirectoryName,
    }
}
