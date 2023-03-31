using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Local.External.Software.NiconicomeProcess
{
    public enum ProcessManagerError
    {
        [ErrorEnum(ErrorLevel.Error, "プロセスの起動に失敗しました。")]
        FailedToStartProcess,
    }
}
