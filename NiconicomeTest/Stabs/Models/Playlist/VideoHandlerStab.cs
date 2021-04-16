using System.Collections.Generic;
using Niconicome.Models.Playlist;

namespace NiconicomeTest.Stabs.Models.Playlist
{
    class VideoHandlerStab : IVideoHandler
    {
        public int Count { get; private set; } = 0;

        public int AddVideo(IListVideoInfo video, int playlidtId)
        {
            this.Count++;
            return 0;
        }

        public void RemoveVideo(int videoID, int playlistID)
        {
            this.Count--;
        }

        public void Update(IListVideoInfo video)
        {
        }

        public bool Exist(int id)
        {
            return true;
        }

        public IEnumerable<IListVideoInfo> GetAllVideos()
        {
            return new List<IListVideoInfo>() { this.GetVideo(0) };
        }

        public IListVideoInfo GetVideo(int id)
        {
            return new NonBindableListVideoInfo();
        }
    }
}
