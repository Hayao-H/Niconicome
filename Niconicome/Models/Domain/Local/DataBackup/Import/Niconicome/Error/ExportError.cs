using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Local.DataBackup.Import.Niconicome.Error
{
    public enum ExportError
    {
        [ErrorEnum(ErrorLevel.Error, "データのシリアライズに失敗しました。")]
        FailedToSerialize,
    }
}
