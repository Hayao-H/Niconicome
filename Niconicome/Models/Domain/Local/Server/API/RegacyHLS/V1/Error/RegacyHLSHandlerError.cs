using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Local.Server.API.RegacyHLS.V1.Error
{
    public enum RegacyHLSHandlerError
    {
        [ErrorEnum(ErrorLevel.Error,"指定されたHLS配信ファイルは生成されていません。(niconicoID:{0}, playlistID:{1})")]
        FileNotExists,
        [ErrorEnum(ErrorLevel.Error,"不正なリクエストです。(url:{0})")]
        InvalidRequest,
    }
}
