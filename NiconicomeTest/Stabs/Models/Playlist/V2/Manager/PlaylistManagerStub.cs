using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Playlist.V2.Manager;
using NiconicomeTest.Stabs.Models.Domain.Playlist;

namespace NiconicomeTest.Stabs.Models.Playlist.V2.Manager
{
    public class PlaylistManagerStub : IPlaylistManager
    {
        public void Initialize()
        {

        }

        public IAttemptResult Delete(int ID)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult Create(int parentID)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult<IPlaylistInfo> GetSpecialPlaylistByType(SpecialPlaylists playlist)
        {
            return AttemptResult<IPlaylistInfo>.Succeeded(new PlaylistInfoStub());
        }

        public IAttemptResult<IPlaylistInfo> GetPlaylist(int ID)
        {
            return AttemptResult<IPlaylistInfo>.Succeeded(new PlaylistInfoStub());
        }
    }
}
