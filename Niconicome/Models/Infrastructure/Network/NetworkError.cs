using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Infrastructure.Network
{
    public enum NetworkError
    {
        [ErrorEnum(ErrorLevel.Error, "使用中のポート一覧の取得に失敗しました。")]
        FailedToGetPortInfo,
    }
}
