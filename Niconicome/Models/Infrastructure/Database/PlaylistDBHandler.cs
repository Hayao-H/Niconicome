using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Infrastructure.Database.LiteDB;
using Niconicome.Models.Infrastructure.Database.Types;
using Types = Niconicome.Models.Infrastructure.Database.Types;

namespace Niconicome.Models.Infrastructure.Database
{
    public class PlaylistDBHandler : IPlaylistStore
    {
        public PlaylistDBHandler(ILiteDBHandler database, IVideoStore videoStore)
        {
            this._database = database;
            this._videoStore = videoStore;
        }

        #region field

        private readonly ILiteDBHandler _database;

        private readonly IVideoStore _videoStore;

        #endregion

        #region Method

        public IAttemptResult<int> Create(IPlaylistInfo playlist)
        {
            Types::Playlist data = this.Convert(playlist);
            return this._database.Insert(data);
        }

        public IAttemptResult<IPlaylistInfo> GetPlaylist(int ID)
        {
            IAttemptResult<Types::Playlist> dbResult = this._database.GetRecord<Types::Playlist>(TableNames.Playlist, ID);
            if (!dbResult.IsSucceeded || dbResult.Data is null) return AttemptResult<IPlaylistInfo>.Fail(dbResult.Message);

            Types::Playlist playlist = dbResult.Data;

            var videos = new List<IVideoInfo>();
            foreach (var videoID in playlist.Videos)
            {
                IAttemptResult<IVideoInfo> vResult = this._videoStore.GetVideo(videoID);
                if (!vResult.IsSucceeded || vResult.Data is null) continue;
                videos.Add(vResult.Data);
            }

            var info = new PlaylistInfo(playlist.Name, videos, this)
            {
                ID = playlist.Id,
                ParentPlaylistID = playlist.Id,
                FolderPath = playlist.FolderPath,
                PlaylistType = playlist.PlaylistType switch
                {
                    DBPlaylistType.Mylist => PlaylistType.Mylist,
                    DBPlaylistType.Series => PlaylistType.Series,
                    DBPlaylistType.WatchLater => PlaylistType.WatchLater,
                    DBPlaylistType.UserVideos => PlaylistType.UserVideos,
                    DBPlaylistType.Channel => PlaylistType.Channel,
                    DBPlaylistType.Root => PlaylistType.Root,
                    DBPlaylistType.Temporary => PlaylistType.Temporary,
                    DBPlaylistType.DownloadSucceededHistory => PlaylistType.DownloadSucceededHistory,
                    DBPlaylistType.DownloadFailedHistory => PlaylistType.DownloadFailedHistory,
                    DBPlaylistType.PlaybackHistory => PlaylistType.PlaybackHistory,
                    _ => PlaylistType.Local,
                },
                RemoteParameter = playlist.RemoteParameter,
            };

            return AttemptResult<IPlaylistInfo>.Succeeded(info);
        }

        public IAttemptResult Update(IPlaylistInfo playlist)
        {
            Types::Playlist data = this.Convert(playlist);
            return this._database.Update(data);
        }

        public IAttemptResult Delete(int ID)
        {
            return this._database.Delete(TableNames.Playlist, ID);
        }

        #endregion

        #region prvate

        /// <summary>
        /// ローカル情報をデータベースの型に変換
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private Types::Playlist Convert(IPlaylistInfo data)
        {
            return new Types.Playlist()
            {
                Id = data.ID,
                Name = data.Name.Value,
                FolderPath = data.FolderPath,
                RemoteParameter = data.RemoteParameter,
                PlaylistType = data.PlaylistType switch
                {
                    PlaylistType.Mylist => DBPlaylistType.Mylist,
                    PlaylistType.Series => DBPlaylistType.Series,
                    PlaylistType.WatchLater => DBPlaylistType.WatchLater,
                    PlaylistType.UserVideos => DBPlaylistType.UserVideos,
                    PlaylistType.Channel => DBPlaylistType.Channel,
                    PlaylistType.Root => DBPlaylistType.Root,
                    PlaylistType.Temporary => DBPlaylistType.Temporary,
                    PlaylistType.DownloadSucceededHistory => DBPlaylistType.DownloadSucceededHistory,
                    PlaylistType.DownloadFailedHistory => DBPlaylistType.DownloadFailedHistory,
                    PlaylistType.PlaybackHistory => DBPlaylistType.PlaybackHistory,
                    _ => DBPlaylistType.Local
                },
                Videos = data.Videos.Select(v => v.ID).ToList(),
                Children = data.Children.Select(v => v.ID).ToList(),
            };
        }

        #endregion
    }
}
