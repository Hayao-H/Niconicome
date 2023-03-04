using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Local.External.Error
{
    public enum ExternalProcessUtilsError
    {
        [ErrorEnum(ErrorLevel.Error,"プロセスの起動に失敗しました。(arg:{0})")]
        FailedToStartProcess,
    }
}
