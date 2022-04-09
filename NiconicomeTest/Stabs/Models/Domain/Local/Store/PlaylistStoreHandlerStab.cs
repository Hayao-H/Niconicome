using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Playlist;
using Niconicome.Models.Playlist.Playlist;
using STypes = Niconicome.Models.Domain.Local.Store.Types;

namespace NiconicomeTest.Stabs.Models.Domain.Local.Store
{
    class PlaylistStoreHandlerStab : IPlaylistStoreHandler
    {
        public IAttemptResult<List<STypes::Playlist>> GetAllPlaylists()
        {
            return AttemptResult<List<STypes::Playlist>>.Succeeded(new List<STypes::Playlist>());
        }

        public IAttemptResult<STypes::Playlist> GetRootPlaylist()
        {
            return AttemptResult<STypes::Playlist>.Succeeded(new STypes::Playlist());
        }

        public IAttemptResult<STypes::Playlist> GetPlaylist(int id)
        {
            return AttemptResult<STypes::Playlist>.Succeeded(new STypes::Playlist());
        }

        public IAttemptResult<STypes::Playlist> GetPlaylist(Expression<Func<STypes::Playlist, bool>> predicate)
        {
            return AttemptResult<STypes::Playlist>.Succeeded(new STypes::Playlist());
        }

        public IAttemptResult<List<STypes::Playlist>> GetChildPlaylists(int id)
        {
            return AttemptResult<List<STypes::Playlist>>.Succeeded(new List<STypes::Playlist>());
        }

        public IAttemptResult<int> AddPlaylist(int parentID, string name)
        {
            return AttemptResult<int>.Succeeded(-1);
        }

        public IAttemptResult Update(STypes::Playlist playlist)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult DeletePlaylist(int id)
        {
            return AttemptResult.Succeeded();
        }

        public bool Exists(int id)
        {
            return true;
        }

        public bool Exists(Expression<Func<STypes::Playlist, bool>> predicate)
        {
            return true;
        }

        public IAttemptResult WireVideo(STypes::Video video, int playlistId)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult UnWireVideo(int id, int playlistId)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult Move(int id, int destId)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult SetAsRemotePlaylist(int id, string remoteId, RemoteType type)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult SetAsLocalPlaylist(int id)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult Initialize()
        {
            return AttemptResult.Succeeded();
        }

    }
}
