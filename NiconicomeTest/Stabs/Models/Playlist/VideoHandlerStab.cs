using System.Collections.Generic;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Playlist;

namespace NiconicomeTest.Stabs.Models.Playlist
{
    class VideoHandlerStab : IVideoHandler
    {
        public IAttemptResult<int> AddVideo(IListVideoInfo video)
        {
            return AttemptResult<int>.Succeeded(-1);
        }

        public IAttemptResult RemoveVideo(IListVideoInfo video)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult Update(IListVideoInfo video)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult<IListVideoInfo> GetVideo(int id)
        {
            return AttemptResult<IListVideoInfo>.Succeeded(new NonBindableListVideoInfo());
        }

        public IAttemptResult<IEnumerable<IListVideoInfo>> GetAllVideos()
        {
            return AttemptResult<IEnumerable<IListVideoInfo>>.Succeeded(new List<IListVideoInfo>());

        }

        public bool Exist(int id)
        {
            return true;
        }
    }
}
