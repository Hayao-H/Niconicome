using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Local.Server.RequestHandler.NotFound
{
    public enum NotFoundRequestHandlerError
    {
        [ErrorEnum(ErrorLevel.Error,"メッセージの書き込みに失敗しました。")]
        FailedToWriteMessage,
    }
}
