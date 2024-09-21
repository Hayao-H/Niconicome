using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Local.Application.Error
{
    public enum ThemeHandlerError
    {
        [ErrorEnum(ErrorLevel.Error, "IThemeの取得に失敗しました。")]
        FailedToGetITheme,
        [ErrorEnum(ErrorLevel.Error, "IThemeの設定に失敗しました。")]
        FailedToSetITheme,
        [ErrorEnum(ErrorLevel.Log, "テーマが変更されました。({0})")]
        ThemeChanged,
    }
}
