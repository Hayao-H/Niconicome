using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Infrastructure.Database.Error
{
    public enum DBCleanerError
    {
        [ErrorEnum(ErrorLevel.Error,"ルートプレイリストが存在しません。")]
        RootNotFount,
    }
}
