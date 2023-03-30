using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Helper.Result;
using NiconicomeTest.Stabs.Models.Domain.Playlist;

namespace NiconicomeTest.Stabs.Models.Domain.Local.Store.V2
{
    internal class VideoStoreStub : IVideoStore
    {
        public IAttemptResult<IVideoInfo> GetVideo(string niconicoID, int playlistID)
        {
            return AttemptResult<IVideoInfo>.Succeeded(new VideoInfoStub());
        }

        public IAttemptResult<IVideoInfo> GetOnlySharedVideoVideo(int ID)
        {
            return AttemptResult<IVideoInfo>.Succeeded(new VideoInfoStub());
        }

        public IAttemptResult<IEnumerable<IVideoInfo>> GetAllVideos()
        {
            return AttemptResult<IEnumerable<IVideoInfo>>.Succeeded(new List<IVideoInfo>());
        }

        public IAttemptResult<int> Create(string niconicoID)
        {
            return AttemptResult<int>.Succeeded(0);
        }

        public IAttemptResult Create(string niconicoID, int playlistID)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult Delete(int ID, int playlistID)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult Clear()
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult Update(IVideoInfo video)
        {
            return AttemptResult.Succeeded();
        }

        public void Flush()
        {

        }

        public bool Exist(string niconicoID, int playlistID)
        {
            return false;
        }
    }
}
