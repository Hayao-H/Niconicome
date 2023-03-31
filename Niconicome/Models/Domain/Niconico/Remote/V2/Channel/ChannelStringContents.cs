using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.StringHandler;

namespace Niconicome.Models.Domain.Niconico.Remote.V2.Channel
{
    public enum ChannelStringContents
    {
        [StringEnum("待機中...({0}s)")]
        Waiting,
        [StringEnum("{0}ページ目を取得します。")]
        ChannnelPageRetrievingHasStarted,
        [StringEnum("{0}ページ目の動画を取得しました。(取得数:{1}, 取得済:{2})")]
        ChannnelPageRetrievingHasCompleted,
    }
}
