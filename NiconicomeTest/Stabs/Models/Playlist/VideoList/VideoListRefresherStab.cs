using System.Collections.Generic;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Playlist;
using Niconicome.Models.Playlist.VideoList;

namespace NiconicomeTest.Stabs.Models.Playlist.VideoList
{
    class VideoListRefresherStab : IVideoListRefresher
    {
        public IAttemptResult Refresh(List<IListVideoInfo> videos)
        {
            return new AttemptResult() { IsSucceeded = true };
        }
    }
}
