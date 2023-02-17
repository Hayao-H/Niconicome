using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Infrastructure.Database.Error
{
    public enum PlaylistDBHandlerError
    {
        [ErrorEnum(ErrorLevel.Error, "重複した動画が存在します。(id:{0})")]
        VideoDuplicated
    }
}
