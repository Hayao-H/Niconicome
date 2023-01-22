using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Network.Video.Error
{
    public enum NetVideosInfomationHandlerError
    {
        [ErrorEnum(ErrorLevel.Error,"リモートプレイリストではない形式です。({0})")]
        NotRemotePlaylist,
    }
}
