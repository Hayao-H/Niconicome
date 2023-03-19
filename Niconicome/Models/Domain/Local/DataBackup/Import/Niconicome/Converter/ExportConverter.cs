using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Playlist;
using Type = Niconicome.Models.Domain.Local.DataBackup.Import.Niconicome.Type;

namespace Niconicome.Models.Domain.Local.DataBackup.Import.Niconicome.Converter
{
    public interface IExportConverter
    {
        /// <summary>
        /// プレイリストデータを変換
        /// </summary>
        /// <param name="playlist"></param>
        /// <returns></returns>
        Type::Playlist ConvertPlaylist(IPlaylistInfo playlist);

        /// <summary>
        /// 動画データを変換
        /// </summary>
        /// <param name="video"></param>
        /// <returns></returns>
        Type::Video ConvertVideo(IVideoInfo video);

        /// <summary>
        /// タグデータを変換
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        Type::Tag ConvertTag(ITagInfo tag);
    }

    public class ExportConverter : IExportConverter
    {
        public Type::Playlist ConvertPlaylist(IPlaylistInfo playlist)
        {
            return new Type::Playlist()
            {
                Id = playlist.ID,
                Name = playlist.Name.Value,
                RemoteParameter = playlist.RemoteParameter,
                PlaylistType = playlist.PlaylistType switch
                {
                    PlaylistType.Local => 0,
                    PlaylistType.Mylist => 1,
                    PlaylistType.Series => 2,
                    PlaylistType.WatchLater => 3,
                    PlaylistType.UserVideos => 4,
                    PlaylistType.Channel => 5,
                    PlaylistType.Root => 6,
                    _ => 0,
                },
                SortType = playlist.SortType switch
                {

                    SortType.NiconicoID => 0,
                    SortType.Title => 1,
                    SortType.UploadedOn => 2,
                    SortType.AddedAt => 3,
                    SortType.ViewCount => 4,
                    SortType.CommentCount => 5,
                    SortType.MylistCount => 6,
                    SortType.LikeCount => 7,
                    SortType.IsDownlaoded => 8,
                    SortType.Custom => 9,
                    _ => 0,
                },
                IsExpanded = playlist.IsExpanded.Value,
                Videos = playlist.Videos.Select(v => v.ID).ToList(),
                Children = playlist.Children.Select(p => p.ID).ToList(),
            };
        }

        public Type::Video ConvertVideo(IVideoInfo video)
        {
            return new Type::Video()
            {
                Id = video.ID,
                PlaylistID = video.PlaylistID,
                NiconicoId = video.NiconicoId,
                Title = video.Title,
                UploadedOn = video.UploadedOn,
                AddedAt = video.AddedAt,
                ViewCount = video.ViewCount,
                CommentCount = video.CommentCount,
                MylistCount = video.MylistCount,
                LikeCount = video.LikeCount,
                OwnerID = video.OwnerID,
                OwnerName = video.OwnerName,
                ThumbUrl = video.ThumbUrl,
                LargeThumbUrl = video.LargeThumbUrl,
                ChannelID = video.ChannelID,
                ChannelName = video.ChannelName,
                Duration = video.Duration,
                Tags = video.Tags.Select(t => t.ID).ToList(),
                IsDeleted = video.IsDeleted,
                IsSelected = video.IsSelected.Value,
            };
        }

        public Type::Tag ConvertTag(ITagInfo tag)
        {
            return new Type::Tag()
            {
                Id = tag.ID,
                Name = tag.Name,
                IsNicodicExist = tag.IsNicodicExist,
            };
        }


    }
}
