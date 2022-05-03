using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Playlist.Playlist;

namespace NiconicomeTest.Stabs.Models.Playlist.Playlist
{
    public class PlaylistHandlerStub : IPlaylistHandler
    {

        public IAttemptResult<int> AddPlaylist(int parentId)
        {
            return AttemptResult<int>.Succeeded(-1);
        }

        public IAttemptResult<int> AddPlaylistToRoot()
        {
            return AttemptResult<int>.Succeeded(-1);
        }

        public IAttemptResult DeletePlaylist(int playlistID)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult Update(ITreePlaylistInfo newpaylist)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult<ITreePlaylistInfo> GetPlaylist(int id)
        {
            return AttemptResult<ITreePlaylistInfo>.Succeeded(new BindableTreePlaylistInfo());
        }

        public IAttemptResult<ITreePlaylistInfo> GetSpecialPlaylist(SpecialPlaylistTypes types)
        {
            return AttemptResult<ITreePlaylistInfo>.Succeeded(new BindableTreePlaylistInfo());
        }

        public IAttemptResult<ITreePlaylistInfo> GetRootPlaylist()
        {
            return AttemptResult<ITreePlaylistInfo>.Succeeded(new BindableTreePlaylistInfo());
        }

        public IAttemptResult<IEnumerable<ITreePlaylistInfo>> GetAllPlaylists()
        {
            return AttemptResult<IEnumerable<ITreePlaylistInfo>>.Succeeded(new List<ITreePlaylistInfo>());
        }

        public IAttemptResult WireVideoToPlaylist(int videoID, int playlistID)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult UnWireVideoToPlaylist(int videoID, int playlistID)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult BookMark(int videoID, int playlistID)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult MoveVideoToPrev(int videoIndex, int playlistID)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult MoveVideoToForward(int videoIndex, int playlistID)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult Refresh()
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult Refresh(bool expandAll, bool inheritExpandedState, bool isInitialRefresh)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult Move(int id, int targetId)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult SaveAllPlaylists()
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult SetAsRemotePlaylist(int playlistId, string remoteID, string name, RemoteType type)
        {
            return AttemptResult.Succeeded();
        }

        public IAttemptResult SetAsLocalPlaylist(int playlistId)
        {
            return AttemptResult.Succeeded();
        }
    }
}
