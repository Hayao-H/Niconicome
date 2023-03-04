using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Utils.StringHandler;

namespace Niconicome.Models.Domain.Niconico.Remote.V2.UserVideo
{
    public enum UserVideoStringContent
    {
        [StringEnum("{0}さんの投稿動画")]
        UserVideoTitle,
    }
}
