using System.Collections.Generic;
using Niconicome.Models.Domain.Local.DataBackup.Import.Niconicome.Error;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Type = Niconicome.Models.Domain.Local.DataBackup.Import.Niconicome.Type;

namespace Niconicome.Models.Domain.Local.DataBackup.Import.Niconicome.Converter
{
    public interface IImportConverter
    {
        /// <summary>
        /// プレイリスト情報に変換(親子関係の構築は行わない)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        IAttemptResult<ConvertResult> ConvertToPlaylistInfo(Type::Playlist data, ImportSource source);
    }

    public class ImportConverter : IImportConverter
    {
        public ImportConverter(IVideoStore videoStore, IPlaylistStore playlistStore, ITagStore tagStore, IErrorHandler errorHandler)
        {
            this._videoStore = videoStore;
            this._playlistStore = playlistStore;
            this._tagStore = tagStore;
            this._errorHandler = errorHandler;
        }

        #region field

        private readonly IVideoStore _videoStore;

        private readonly IPlaylistStore _playlistStore;

        private readonly ITagStore _tagStore;

        private readonly IErrorHandler _errorHandler;

        #endregion

        #region Method

        public IAttemptResult<ConvertResult> ConvertToPlaylistInfo(Type::Playlist data, ImportSource source)
        {
            IPlaylistInfo playlist;

            //Rootの場合は特殊処理
            if (data.PlaylistType == 6)
            {
                IAttemptResult<IPlaylistInfo> pResult = this._playlistStore.GetPlaylistByType(PlaylistType.Root);
                if (!pResult.IsSucceeded || pResult.Data is null)
                {
                    return AttemptResult<ConvertResult>.Fail(pResult.Message);
                }

                playlist = pResult.Data;
            }
            else
            {
                IAttemptResult<int> cResult = this._playlistStore.Create(data.Name);
                if (!cResult.IsSucceeded)
                {
                    return AttemptResult<ConvertResult>.Fail(cResult.Message);
                }

                IAttemptResult<IPlaylistInfo> pResult = this._playlistStore.GetPlaylist(cResult.Data);
                if (!pResult.IsSucceeded || pResult.Data is null)
                {
                    return AttemptResult<ConvertResult>.Fail(pResult.Message);
                }

                playlist = pResult.Data;
            }

            playlist.IsAutoUpdateEnabled = false;
            playlist.RemoteParameter = data.RemoteParameter;
            playlist.PlaylistType = data.PlaylistType switch
            {
                0 => PlaylistType.Local,
                1 => PlaylistType.Mylist,
                2 => PlaylistType.Series,
                3 => PlaylistType.WatchLater,
                4 => PlaylistType.UserVideos,
                5 => PlaylistType.Channel,
                6 => PlaylistType.Root,
                _ => PlaylistType.Local
            };
            playlist.SortType = data.SortType switch
            {
                0 => SortType.NiconicoID,
                1 => SortType.Title,
                2 => SortType.UploadedOn,
                3 => SortType.AddedAt,
                4 => SortType.ViewCount,
                5 => SortType.CommentCount,
                6 => SortType.MylistCount,
                7 => SortType.LikeCount,
                8 => SortType.IsDownlaoded,
                9 => SortType.Custom,
                _ => SortType.NiconicoID
            };
            playlist.IsAscendant = data.IsAscendant;
            playlist.IsExpanded.Value = data.IsExpanded;

            var succeededV = 0;

            //登録されている動画
            foreach (var video in data.Videos)
            {
                IAttemptResult<IVideoInfo> vResult = this.ConvertToVideoInfo(source.Videos[video], playlist.ID, source);
                if (vResult.IsSucceeded && vResult.Data is not null)
                {
                    playlist.AddVideo(vResult.Data);
                    succeededV++;
                }
                else
                {
                    this._errorHandler.HandleError(ImportError.FailedToImportVideo, data.Name, source.Videos[video].NiconicoId);
                }
            }

            playlist.IsAutoUpdateEnabled = true;

            playlist.FolderPath = data.FolderPath;

            return AttemptResult<ConvertResult>.Succeeded(new ConvertResult(playlist, succeededV));
        }

        #endregion

        #region private

        /// <summary>
        /// 動画情報に変換
        /// </summary>
        /// <param name="source"></param>
        /// <param name="playlistID"></param>
        /// <returns></returns>
        private IAttemptResult<IVideoInfo> ConvertToVideoInfo(Type::Video source, int playlistID, ImportSource importSource)
        {
            IAttemptResult cResult = this._videoStore.Create(source.NiconicoId, playlistID);
            if (!cResult.IsSucceeded)
            {
                return AttemptResult<IVideoInfo>.Fail(cResult.Message);
            }

            IAttemptResult<IVideoInfo> videoResult = this._videoStore.GetVideo(source.NiconicoId, playlistID);
            if (!videoResult.IsSucceeded || videoResult.Data is null)
            {
                return videoResult;
            }

            IVideoInfo video = videoResult.Data;
            video.IsAutoUpdateEnabled = false;

            video.UploadedOn = source.UploadedOn;
            video.AddedAt = source.AddedAt;
            video.ViewCount = source.ViewCount;
            video.CommentCount = source.CommentCount;
            video.MylistCount = source.MylistCount;
            video.LikeCount = source.LikeCount;
            video.OwnerID = source.OwnerID;
            video.OwnerName = source.OwnerName;
            video.ThumbUrl = source.ThumbUrl;
            video.LargeThumbUrl = source.LargeThumbUrl;
            video.ChannelID = source.ChannelID;
            video.ChannelName = source.ChannelName;
            video.Duration = source.Duration;
            video.IsDeleted = source.IsDeleted;
            video.IsSelected.Value = source.IsSelected;

            foreach (var tag in source.Tags)
            {
                IAttemptResult<ITagInfo> tagResult = this.ConvertToTagInfo(importSource.Tags[tag]);
                if (tagResult.IsSucceeded && tagResult.Data is not null)
                {
                    video.AddTag(tagResult.Data);
                }
                else
                {
                    this._errorHandler.HandleError(ImportError.FailedToImportTag, importSource.Tags[tag].Name);
                }
            }

            video.IsAutoUpdateEnabled = true;

            video.Title = source.Title;

            return AttemptResult<IVideoInfo>.Succeeded(video);
        }

        /// <summary>
        /// タグ情報に変換
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private IAttemptResult<ITagInfo> ConvertToTagInfo(Type::Tag source)
        {
            if (!this._tagStore.Exist(source.Name))
            {
                IAttemptResult cResult = this._tagStore.Create(source.Name);
                if (!cResult.IsSucceeded)
                {
                    return AttemptResult<ITagInfo>.Fail(cResult.Message);
                }
            }

            IAttemptResult<ITagInfo> tagResult = this._tagStore.GetTag(source.Name);
            if (!tagResult.IsSucceeded || tagResult.Data is null)
            {
                return tagResult;
            }

            tagResult.Data.IsNicodicExist = source.IsNicodicExist;

            return AttemptResult<ITagInfo>.Succeeded(tagResult.Data);
        }

        #endregion

    }

    public record ImportSource(IReadOnlyDictionary<int, Type::Playlist> Playlists, IReadOnlyDictionary<int, Type::Video> Videos, IReadOnlyDictionary<int, Type::Tag> Tags);

    public record ConvertResult(IPlaylistInfo PlaylistInfo, int ImportedVideos);

}
