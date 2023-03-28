using System.Collections.Generic;
using System.Linq;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Infrastructure.Database.LiteDB;
using Err = Niconicome.Models.Infrastructure.Database.Error.DBCleanerError;
using Types = Niconicome.Models.Infrastructure.Database.Types;

namespace Niconicome.Models.Infrastructure.Database
{
    public class DBCleaner : IStoreCleaner
    {
        public DBCleaner(ILiteDBHandler database,IErrorHandler errorHandler)
        {
            this._database = database;
            this._errorHandler = errorHandler;
        }

        #region field

        private readonly ILiteDBHandler _database;

        private readonly IErrorHandler _errorHandler;

        #endregion

        #region Method

        public IAttemptResult CleanPlaylists()
        {

            IAttemptResult<IReadOnlyList<Types::Playlist>> dbResult = this._database.GetAllRecords<Types::Playlist>(TableNames.Playlist);
            if (!dbResult.IsSucceeded || dbResult.Data is null)
            {
                return AttemptResult.Fail(dbResult.Message);
            }

            Dictionary<int, Types::Playlist> playlists = dbResult.Data.ToDictionary(p => p.Id);
            Dictionary<int, bool> playlistsUsageData = dbResult.Data.ToDictionary(p => p.Id, _ => false);

            Types::Playlist? root = dbResult.Data.FirstOrDefault(p => p.PlaylistType == Types::DBPlaylistType.Root);
            if (root is null)
            {
                this._errorHandler.HandleError(Err.RootNotFount);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(Err.RootNotFount));
            }

            void CheckChildren(IEnumerable<int> children)
            {
                foreach (var child in children)
                {
                    playlistsUsageData[child] = true;
                    CheckChildren(playlists[child].Children);
                }
            }

            CheckChildren(root.Children);
            playlistsUsageData[root.Id] = true;

            foreach (var target in playlistsUsageData.Where(x => !x.Value).Select(x => x.Key))
            {
                if (!CheckWhetherDeletionAllowed(playlists[target]))
                {
                    continue;
                }
                this._database.Delete(TableNames.Playlist, target);
            }

            return AttemptResult.Succeeded();
        }

        public IAttemptResult CleanVideos()
        {
            IAttemptResult<IReadOnlyList<Types::Playlist>> playlistResult = this._database.GetAllRecords<Types::Playlist>(TableNames.Playlist);
            if (!playlistResult.IsSucceeded || playlistResult.Data is null)
            {
                return AttemptResult.Fail(playlistResult.Message);
            }

            IAttemptResult<IReadOnlyList<Types::Video>> videoResult = this._database.GetAllRecords<Types::Video>(TableNames.Video);
            if (!videoResult.IsSucceeded || videoResult.Data is null)
            {
                return AttemptResult.Fail(videoResult.Message);
            }

            IAttemptResult<IReadOnlyList<Types::SharedVideo>> sharedResult = this._database.GetAllRecords<Types::SharedVideo>(TableNames.SharedVideo);
            if (!sharedResult.IsSucceeded || sharedResult.Data is null)
            {
                return AttemptResult.Fail(sharedResult.Message);
            }

            Dictionary<string, int> sharedIDs = sharedResult.Data.ToDictionary(v => v.NiconicoId, v => v.Id);

            //動画の使用状況
            Dictionary<VideoKey, bool> videoUsageData = videoResult.Data.ToDictionary(v => new VideoKey(v.PlaylistID, v.SharedVideoID), _ => false);

            //動画ID
            Dictionary<VideoKey, int> videoIDs = videoResult.Data.ToDictionary(v => new VideoKey(v.PlaylistID, v.SharedVideoID), v => v.Id);

            foreach (var p in playlistResult.Data)
            {
                foreach (var video in p.Videos)
                {
                    videoUsageData[new VideoKey(p.Id, sharedIDs[video])] = true;
                }
            }

            foreach (var key in videoUsageData.Where(v => !v.Value).Select(x => x.Key))
            {
                this._database.Delete(TableNames.Video, videoIDs[key]);
            }

            return AttemptResult.Succeeded();
        }

        #endregion

        #region private

        private record VideoKey(int PlaylistID, int SharedID);

        private bool CheckWhetherDeletionAllowed(Types::Playlist playlist)
        {
            if (playlist.PlaylistType == Types::DBPlaylistType.Root)
            {
                return false;
            }

            if (playlist.PlaylistType == Types::DBPlaylistType.Temporary)
            {
                return false;
            }

            if (playlist.PlaylistType == Types::DBPlaylistType.DownloadFailedHistory)
            {
                return false;
            }

            if (playlist.PlaylistType == Types::DBPlaylistType.DownloadSucceededHistory)
            {
                return false;
            }

            if (playlist.PlaylistType == Types::DBPlaylistType.PlaybackHistory)
            {
                return false;
            }

            return true;
        }

        #endregion

    }
}
