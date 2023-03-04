using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Local.Server.RequestHandler.UserChrome
{
    public enum UserChromeRequestHandlerError
    {
        [ErrorEnum(ErrorLevel.Error, "ユーザー定義スタイルシートへのアクセスに失敗しました。")]
        FailedToAccessCSS,
    }
}
