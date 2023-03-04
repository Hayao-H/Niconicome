using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Local.LocalFile.Error
{
    public enum LocalDirectoryHandlerError
    {
        [ErrorEnum(ErrorLevel.Error, "ローカルファイルの探索に失敗しました。(path:{0})")]
        FailedToSearchLocalDirectory,
    }
}
