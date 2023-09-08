using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Niconico.Download.General
{
    public enum ReplaceHandlerError
    {
        [ErrorEnum(ErrorLevel.Error,"指定されたデータが存在しません。(from:{0}, to:{1})")]
        DataNotExist,
    }
}
