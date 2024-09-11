using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Auth.Error
{
    public enum ChromeSharedLoginError
    {
        [ErrorEnum(ErrorLevel.Error, "Google Chromeからのクッキーの取得に失敗しました。")]
        FailedToGetCookie,
    }
}
