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
        public int PlayilistCount { get; private set; }

        public int VideoCount { get; private set; }

        public STypes::Playlist GetRootPlaylist()
        {
            return new STypes::Playlist();
        }
        public STypes::Playlist? GetPlaylist(int id)
        {
            return new STypes::Playlist();
        }

        public int AddPlaylist(int parentID, string name)
        {
            this.PlayilistCount++;
            return 0;
        }

        public int AddVideo(IListVideoInfo video, int playlistId)
        {
            this.VideoCount++;
            return 0;
        }

        public void RemoveVideo(int id, int playlistId)
        {
            this.VideoCount--;
        }

        public void Update(ITreePlaylistInfo newplaylist)
        {
        }
        public List<STypes::Playlist> GetChildPlaylists(STypes::Playlist self)
        {
            return new List<STypes::Playlist>() { this.GetRootPlaylist() };
        }

        public List<STypes::Playlist> GetChildPlaylists(int id)
        {
            return new List<STypes::Playlist>() { this.GetRootPlaylist() };

        }

        public List<STypes::Playlist> GetAllPlaylists()
        {
            return new List<STypes::Playlist>() { this.GetRootPlaylist() };
        }


        public IAttemptResult MoveVideoToPrev(int playlistID, int videoIndex)
        {
            return new AttemptResult() { IsSucceeded = true };
        }

        public IAttemptResult MoveVideoToForward(int playlistID, int videoIndex)
        {
            return new AttemptResult() { IsSucceeded = true };

        }

        public void RemoveChildPlaylist(int selfId)
        {
        }

        public void DeletePlaylist(int id)
        {
            this.PlayilistCount--;
        }

        public bool Exists(int id)
        {
            return true;
        }

        public bool Exists(Expression<Func<STypes::Playlist, bool>> predicate)
        {
            return true;
        }


        public bool JustifyPlaylists(int Id)
        {
            return true;
        }
        public bool JustifyPlaylists(IEnumerable<int> Id)
        {
            return true;
        }

        public bool ContainsVideo(string niconicoId, int playlistId)
        {
            return true;
        }

        public void Move(int id, int destId)
        {

        }

        public void Copy(int id, int destId)
        {

        }

        public void SetAsRemotePlaylist(int id, string remoteId, RemoteType type)
        {

        }

        public void SetAsLocalPlaylist(int id)
        {

        }

        public int GetPlaylistsCount()
        {
            return 0;
        }

        public void Refresh()
        {
        }

        public IAttemptResult Initialize()
        {
            return new AttemptResult() { IsSucceeded = true };
        }

    }
}
