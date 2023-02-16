using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Local.External.Error
{
    public enum ExternalAppUtilsV2Error
    {
        [ErrorEnum(ErrorLevel.Error,"指定された動画はダウンロードされていません。(id:{0})")]
        VideoIsNotDownloaded,
        [ErrorEnum(ErrorLevel.Error,"エクスプローラーの起動に失敗しました。。(path:{0})")]
        FailedToOpenExplorer,
    }
}
