using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Local.Server.API.Resource.V1.Error
{
    public enum ResourceHandlerError
    {
        [ErrorEnum(ErrorLevel.Error, "不正なリクエストです。(url:{0})")]
        InvalidUrl,
    }
}
