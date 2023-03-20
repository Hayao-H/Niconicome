using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Text;
using Niconicome.Extensions;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Infrastructure.Database.Error;
using Niconicome.Models.Infrastructure.Database.LiteDB;
using Niconicome.Models.Infrastructure.Database.Types;
using Types = Niconicome.Models.Infrastructure.Database.Types;

namespace Niconicome.Models.Infrastructure.Database
{
    public class PlaylistDBHandler : IPlaylistStore
    {
        public PlaylistDBHandler(ILiteDBHandler database, IVideoStore videoStore, IErrorHandler errorHandler)
        {
            this._database = database;
            this._videoStore = videoStore;
            this._errorHandler = errorHandler;
        }

        #region field

        private readonly ILiteDBHandler _database;

        private readonly IVideoStore _videoStore;

        private readonly IErrorHandler _errorHandler;

        #endregion

        #region Method

        public IAttemptResult<int> Create(string name)
        {
            var data = new Types.Playlist()
            {
                Name = name,
            };
            return this._database.Insert(data);
        }

        public IAttemptResult<IPlaylistInfo> GetPlaylist(int ID)
        {
            IAttemptResult<Types::Playlist> dbResult = this._database.GetRecord<Types::Playlist>(TableNames.Playlist, ID);
            if (!dbResult.IsSucceeded || dbResult.Data is null) return AttemptResult<IPlaylistInfo>.Fail(dbResult.Message);

            IPlaylistInfo info = this.Convert(dbResult.Data);

            return AttemptResult<IPlaylistInfo>.Succeeded(info);
        }

        public IAttemptResult<IPlaylistInfo> GetPlaylistByType(PlaylistType type)
        {
            DBPlaylistType pType = this.Convert(type);
            IAttemptResult<Types::Playlist> dbResult = this._database.GetRecord<Types::Playlist>(TableNames.Playlist, p => p.PlaylistType == pType);
            if (!dbResult.IsSucceeded || dbResult.Data is null) return AttemptResult<IPlaylistInfo>.Fail(dbResult.Message);

            IPlaylistInfo info = this.Convert(dbResult.Data);

            return AttemptResult<IPlaylistInfo>.Succeeded(info);
        }


        public IAttemptResult<IEnumerable<IPlaylistInfo>> GetAllPlaylist()
        {

            IAttemptResult<IReadOnlyList<Types::Playlist>> dbResult = this._database.GetAllRecords<Types::Playlist>(TableNames.Playlist);
            if (!dbResult.IsSucceeded || dbResult.Data is null) return AttemptResult<IEnumerable<IPlaylistInfo>>.Fail(dbResult.Message);

            var infos = new List<IPlaylistInfo>();

            foreach (var id in dbResult.Data.Select(p => p.Id))
            {
                IAttemptResult<IPlaylistInfo> pResult = this.GetPlaylist(id);
                if (!pResult.IsSucceeded || pResult.Data is null) continue;
                infos.Add(pResult.Data);
            }

            return AttemptResult<IEnumerable<IPlaylistInfo>>.Succeeded(infos.AsReadOnly());
        }


        public IAttemptResult Update(IPlaylistInfo playlist)
        {
            Types::Playlist data = this.Convert(playlist);

            bool dupeVideoExist = data.Videos.GroupBy(v => v).Where(v => v.Count() > 1).Any();
            if (dupeVideoExist)
            {
                this._errorHandler.HandleError(PlaylistDBHandlerError.VideoDuplicated, data.Id);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(PlaylistDBHandlerError.VideoDuplicated, data.Id));
            }

            return this._database.Update(data);
        }

        public IAttemptResult Delete(int ID)
        {
            return this._database.Delete(TableNames.Playlist, ID);
        }

        public IAttemptResult Clear()
        {
            return this._database.Clear(TableNames.Playlist);
        }

        public bool Exist(PlaylistType playlistType)
        {
            DBPlaylistType dBPlaylistType = this.Convert(playlistType);
            return this._database.Exists<Types::Playlist>(TableNames.Playlist, p => p.PlaylistType == dBPlaylistType);
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
                SortType = data.SortType switch
                {
                    SortType.Title => DBSortType.Title,
                    SortType.UploadedOn => DBSortType.UploadedOn,
                    SortType.AddedAt => DBSortType.AddedAt,
                    SortType.ViewCount => DBSortType.ViewCount,
                    SortType.CommentCount => DBSortType.CommentCount,
                    SortType.MylistCount => DBSortType.MylistCount,
                    SortType.LikeCount => DBSortType.LikeCount,
                    SortType.IsDownlaoded => DBSortType.IsDownlaoded,
                    SortType.Custom => DBSortType.Custom,
                    _ => DBSortType.NiconicoID,
                },
                IsAscendant = data.IsAscendant,
                IsExpanded = data.IsExpanded.Value,
                Videos = data.Videos.Select(v => v.NiconicoId).ToList(),
                Children = this.CanUpdateChildren(data) ? data.Children.Where(p => p.PlaylistType != PlaylistType.Temporary && p.PlaylistType != PlaylistType.DownloadSucceededHistory && p.PlaylistType != PlaylistType.DownloadFailedHistory).Select(v => v.ID).ToList() : data.ChildrenID.ToList(),
            };
        }

        /// <summary>
        /// プレイリストの種類についてローカル情報をデータベースの型に変換
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private DBPlaylistType Convert(PlaylistType type)
        {
            return type switch
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
            };
        }

        /// <summary>
        /// データベースの型をローカル情報に変換
        /// </summary>
        /// <param name="playlist"></param>
        /// <returns></returns>
        private IPlaylistInfo Convert(Types::Playlist playlist)
        {
            var videos = new List<IVideoInfo>();
            foreach (var videoID in playlist.Videos)
            {
                IAttemptResult<IVideoInfo> vResult = this._videoStore.GetVideo(videoID, playlist.Id);
                if (!vResult.IsSucceeded || vResult.Data is null) continue;
                videos.Add(vResult.Data);
            }

            var info = new PlaylistInfo(playlist.Name, videos, this, playlist.Children)
            {
                ID = playlist.Id,
            };

            info.IsAutoUpdateEnabled = false;

            info.FolderPath = playlist.FolderPath;
            info.PlaylistType = playlist.PlaylistType switch
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
            };
            info.SortType = playlist.SortType switch
            {
                DBSortType.Title => SortType.Title,
                DBSortType.UploadedOn => SortType.UploadedOn,
                DBSortType.AddedAt => SortType.AddedAt,
                DBSortType.ViewCount => SortType.ViewCount,
                DBSortType.CommentCount => SortType.CommentCount,
                DBSortType.MylistCount => SortType.MylistCount,
                DBSortType.LikeCount => SortType.LikeCount,
                DBSortType.IsDownlaoded => SortType.IsDownlaoded,
                DBSortType.Custom => SortType.Custom,
                _ => SortType.NiconicoID,
            };
            info.IsAscendant = playlist.IsAscendant;
            info.RemoteParameter = playlist.RemoteParameter;
            info.IsExpanded.Value = playlist.IsExpanded;

            info.IsAutoUpdateEnabled = true;
            return info;
        }

        /// <summary>
        /// Childrenプロパティーを参照可能かどうか
        /// </summary>
        /// <param name="playlistInfo"></param>
        /// <returns></returns>
        private bool CanUpdateChildren(IPlaylistInfo playlistInfo)
        {
            if (playlistInfo.Children.Count > 0)
            {
                return true;
            }

            return playlistInfo.ChildrenID.Count == 0;
        }

        #endregion
    }
}
