using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Infrastructure.Database.Error
{
    public enum ApplicationDBHandlerError
    {
        [ErrorEnum(ErrorLevel.Error,"DBバージョンの解析に失敗しました。(入力値：{0})")]
        FailedToParseDBVersion,
    }
}
