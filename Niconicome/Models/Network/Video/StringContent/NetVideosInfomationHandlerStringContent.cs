using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.StringHandler;

namespace Niconicome.Models.Network.Video.StringContent
{
    public enum NetVideosInfomationHandlerStringContent
    {
        [StringEnum("動画の取得を開始しました。;{0}(param:{1})")]
        RetrievingRemotePlaylistHasStarted,
        [StringEnum("動画の取得が完了しました。;{0}(動画数:{1}, param:{2})")]
        RetrievingRemotePlaylistHasCompleted,
        [StringEnum("動画の取得を開始しました。(id:{0})")]
        RetrievingVideoHasStarted,
        [StringEnum("動画の取得が完了しました。(id:{0}, title:{1})")]
        RetrievingVideoHasCompleted,
    }
}
