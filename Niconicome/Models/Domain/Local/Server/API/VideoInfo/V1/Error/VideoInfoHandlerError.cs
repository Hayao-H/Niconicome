using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.Error;

namespace Niconicome.Models.Domain.Local.Server.API.VideoInfo.V1.Error
{
    public enum VideoInfoHandlerError
    {
        [ErrorEnum(ErrorLevel.Error,"指定された動画がプレイリストに存在しません。(playlistID:{0}, niconicoID:{1})")]
        VideoNotFound,
        [ErrorEnum(ErrorLevel.Error,"不正なリクエストです。(url:{0})")]
        InvalidRequest,
    }
}
