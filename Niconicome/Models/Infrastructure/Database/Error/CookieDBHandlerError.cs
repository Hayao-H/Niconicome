using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Infrastructure.Database.Error
{
    public enum CookieDBHandlerError
    {
        [ErrorEnum(ErrorLevel.Error,"Cookieが保存されていません。")]
        CookieNotFound,
        [ErrorEnum(ErrorLevel.Error,"Cookieの暗号化に失敗しました。")]
        FailedToEncrypt,
        [ErrorEnum(ErrorLevel.Error,"Cookieの復号に失敗しました。")]
        FailedToDecrypt,
    }
}
