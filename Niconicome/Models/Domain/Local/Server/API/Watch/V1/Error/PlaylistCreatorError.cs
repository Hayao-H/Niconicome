using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Local.Server.API.Watch.V1.Error
{
   public enum PlaylistCreatorError
    {
        [ErrorEnum(ErrorLevel.Error, "有効なストリームの検索に失敗しました。")]
        StreamNotFound,
    }
}
