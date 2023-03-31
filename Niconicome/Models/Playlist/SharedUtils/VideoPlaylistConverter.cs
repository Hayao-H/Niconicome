using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Playlist.Playlist;
using STypes = Niconicome.Models.Domain.Local.Store.Types;

namespace Niconicome.Models.Playlist.SharedUtils
{

    public interface IVideoPlaylistConverter
    {
        /// <summary>
        /// Modelの動画情報をストアの動画情報に変換する
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        STypes::Video ConvertLocalVideoToStoreVideo(IListVideoInfo source);

        /// <summary>
        /// ストアの動画情報をModelの動画情報に変換する
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        IListVideoInfo ConvertStoreVideoToLocalVideo(STypes::Video source);

        /// <summary>
        /// Modelのプレイリスト情報をストアのプレイリスト情報に変換する
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        STypes::Playlist ConvertLocalPlaylistToStorePlaylist(ITreePlaylistInfo source);

        /// <summary>
        /// ストアのプレイリスト情報をModelのプレイリスト情報に変換する
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        ITreePlaylistInfo ConvertStorePlaylistToLocalPlaylist(STypes::Playlist source);
    }

    public class VideoPlaylistConverter : IVideoPlaylistConverter
    {

        public VideoPlaylistConverter(IVideoInfoContainer container, IPlaylistStoreHandler playlistStoreHandler, IPlaylistInfoContainer playlistInfoContainer)
        {
            this._container = container;
            this._playlistStoreHandler = playlistStoreHandler;
            this._playlistInfoContainer = playlistInfoContainer;
        }

        #region field

        private readonly IVideoInfoContainer _container;

        private readonly IPlaylistStoreHandler _playlistStoreHandler;

        private readonly IPlaylistInfoContainer _playlistInfoContainer;

        #endregion

        public STypes::Video ConvertLocalVideoToStoreVideo(IListVideoInfo source)
        {
            var converted = new STypes::Video();

            converted.Id = source.Id.Value;
            converted.NiconicoId = source.NiconicoId.Value;
            converted.Title = source.Title.Value;
            converted.UploadedOn = source.UploadedOn.Value;
            converted.ThumbUrl = source.ThumbUrl.Value;
            converted.LargeThumbUrl = source.LargeThumbUrl.Value;
            converted.ThumbPath = source.ThumbPath.Value;
            converted.IsSelected = source.IsSelected.Value;
            converted.FileName = source.FileName.Value;
            converted.Tags = source.Tags.ToList();
            converted.ViewCount = source.ViewCount.Value;
            converted.CommentCount = source.CommentCount.Value;
            converted.MylistCount = source.MylistCount.Value;
            converted.LikeCount = source.LikeCount.Value;
            converted.Duration = source.Duration.Value;
            converted.OwnerID = source.OwnerID.Value;
            converted.OwnerName = source.OwnerName.Value;

            return converted;
        }

        public IListVideoInfo ConvertStoreVideoToLocalVideo(STypes::Video source)
        {
            IListVideoInfo converted = this._container.GetVideo(source.NiconicoId);

            converted.Id.Value = source.Id;
            converted.NiconicoId.Value = source.NiconicoId;
            converted.Title.Value = source.Title;
            converted.IsDeleted.Value = source.IsDeleted;
            converted.OwnerName.Value = source.OwnerName;
            converted.UploadedOn.Value = source.UploadedOn;
            converted.LargeThumbUrl.Value = source.LargeThumbUrl;
            converted.ThumbUrl.Value = source.ThumbUrl;
            converted.ThumbPath.Value = source.ThumbPath;
            converted.FileName.Value = source.FileName;
            converted.Tags = source.Tags ?? new List<string>();
            converted.ViewCount.Value = source.ViewCount;
            converted.CommentCount.Value = source.CommentCount;
            converted.MylistCount.Value = source.MylistCount;
            converted.LikeCount.Value = source.LikeCount;
            converted.OwnerID.Value = source.OwnerID;
            converted.Duration.Value = source.Duration;

            return converted;
        }

        public STypes::Playlist ConvertLocalPlaylistToStorePlaylist(ITreePlaylistInfo source)
        {
            var converted = new STypes::Playlist();

            converted.Id = source.Id;
            converted.IsRoot = source.IsRoot;
            converted.PlaylistName = source.Name.Value;
            converted.FolderPath = source.Folderpath;
            converted.IsExpanded = source.IsExpanded;
            converted.SortType = source.VideoSortType;
            converted.IsVideoDescending = source.IsVideoDescending;
            converted.IsTemporary = source.IsTemporary;
            converted.IsDownloadSucceededHistory = source.IsDownloadSucceededHistory;
            converted.IsDownloadFailedHistory = source.IsDownloadFailedHistory;
            converted.IsRemotePlaylist = source.RemoteType != RemoteType.None;
            converted.RemoteId = source.RemoteId;
            converted.IsWatchLater = source.RemoteType == RemoteType.WatchLater;
            converted.IsMylist = source.RemoteType == RemoteType.Mylist;
            converted.IsSeries = source.RemoteType == RemoteType.Series;
            converted.IsChannel = source.RemoteType == RemoteType.Channel;
            converted.IsUserVideos = source.RemoteType == RemoteType.UserVideos;
            converted.BookMarkedVideoID = source.BookMarkedVideoID;
            converted.Videos.Clear();
            converted.Videos.AddRange(source.Videos.Select(v => this.ConvertLocalVideoToStoreVideo(v)));

            if (this._playlistStoreHandler.Exists(source.ParentId))
            {
                IAttemptResult<STypes::Playlist> pResult = this._playlistStoreHandler.GetPlaylist(source.ParentId);
                if (pResult.IsSucceeded && pResult.Data is not null)
                {
                    converted.ParentPlaylist = pResult.Data;
                }
            }

            return converted;
        }

        public ITreePlaylistInfo ConvertStorePlaylistToLocalPlaylist(STypes::Playlist source)
        {
            var converted = this._playlistInfoContainer.GetPlaylist(source.Id);

            converted.Id = source.Id;
            converted.ParentId = source.ParentPlaylist?.Id ?? -1;
            converted.IsRoot = source.IsRoot;
            converted.Name.Value = source.PlaylistName ?? string.Empty;
            converted.IsRemotePlaylist = source.IsRemotePlaylist;
            converted.RemoteType = source.IsMylist ? RemoteType.Mylist : source.IsUserVideos ? RemoteType.UserVideos : source.IsWatchLater ? RemoteType.WatchLater : source.IsChannel ? RemoteType.Channel : source.IsSeries ? RemoteType.Series : RemoteType.None;
            converted.RemoteId = source.RemoteId ?? string.Empty;
            converted.Folderpath = source.FolderPath ?? string.Empty;
            converted.IsExpanded = converted.IsExpanded || source.IsExpanded;
            converted.VideoSortType = source.SortType;
            converted.IsVideoDescending = source.IsVideoDescending;
            converted.IsDownloadFailedHistory = source.IsDownloadFailedHistory;
            converted.IsDownloadSucceededHistory = source.IsDownloadSucceededHistory;
            converted.IsTemporary = source.IsTemporary;
            converted.BookMarkedVideoID = source.BookMarkedVideoID;
            converted.Videos.Clear();
            converted.Videos.AddRange(source.Videos.Select(v => this.ConvertStoreVideoToLocalVideo(v)));

            return converted;
        }


    }
}
