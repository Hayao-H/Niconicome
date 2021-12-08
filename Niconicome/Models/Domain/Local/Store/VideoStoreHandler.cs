using System;
using System.Collections.Generic;
using System.Linq;
using Niconicome.Extensions.System;
using Niconicome.Models.Domain.Utils;
using Niconicome.Models.Network.Watch;
using Niconicome.Models.Playlist;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;
using STypes = Niconicome.Models.Domain.Local.Store.Types;

namespace Niconicome.Models.Domain.Local.Store
{
    public interface IVideoStoreHandler
    {
        int AddVideo(IListVideoInfo video, int playlistId);
        void RemoveVideo(int videoID, int playlistID);
        void Update(IListVideoInfo video);
        IEnumerable<STypes::Video> GetAllVideos();
        STypes::Video? GetVideo(int Id);
        STypes::Video? GetVideo(string niconicoId);
        bool Exists(int id);
        bool Exists(string niconicoId);
        void JustifyVideos();
    }

    public class VideoStoreHandler : IVideoStoreHandler
    {

        public VideoStoreHandler(IDataBase dataBase, ILogger logger)
        {
            this.databaseInstance = dataBase;
            this.logger = logger;
        }

        #region field

        private readonly IDataBase databaseInstance;

        private readonly ILogger logger;
        #endregion


        /// <summary>
        /// 全ての動画を取得する
        /// </summary>
        /// <returns></returns>
        public IEnumerable<STypes::Video> GetAllVideos()
        {

            var result = this.databaseInstance.GetAllRecords<STypes::Video>(STypes::Video.TableName);

            if (!result.IsSucceeded || result.Data is null)
            {
                if (result.Exception is not null)
                {
                    this.logger.Error("全動画の取得に失敗しました。", result.Exception);
                }
                else
                {
                    this.logger.Error("全動画の取得に失敗しました。");
                }

                return Enumerable.Empty<STypes::Video>();
            }

            return result.Data;
        }


        /// <summary>
        /// 動画を追加する
        /// </summary>
        /// <param name="video"></param>
        /// <param name="playlistId"></param>
        public int AddVideo(IListVideoInfo videoData, int playlistId)
        {
            int videoId;

            //既にデータベースに存在する場合は再利用
            if (this.Exists(videoData.NiconicoId.Value))
            {
                var video = this.GetVideo(videoData.NiconicoId.Value)!;
                if (video.PlaylistIds is null)
                {
                    video.PlaylistIds = new List<int>();
                }
                if (!video.PlaylistIds.Contains(playlistId))
                {
                    video.PlaylistIds.Add(playlistId);
                }

                //動画情報が存在する場合は更新
                if (!videoData.Title.Value.IsNullOrEmpty())
                {
                    this.SetData(video, videoData);
                }

                this.databaseInstance.Update(video, STypes::Video.TableName);
                videoId = video.Id;
            }
            else
            {
                var video = new STypes::Video()
                {
                    PlaylistIds = new List<int>() { playlistId },
                };

                this.SetData(video, videoData);

                var result = this.databaseInstance.Store(video, STypes::Video.TableName);

                if (!result.IsSucceeded)
                {
                    if (result.Exception is not null)
                    {
                        this.logger.Error($"動画({videoData.NiconicoId})の追加に失敗しました。", result.Exception);
                    }
                    else
                    {
                        this.logger.Error($"動画({videoData.NiconicoId})の追加に失敗しました。");
                    }

                    return -1;
                }

                videoId = result.Data;
            }

            return videoId;
        }


        /// <summary>
        /// 動画を削除する
        /// </summary>
        /// <param name="videoID"></param>
        /// <param name="playlistID"></param>
        public void RemoveVideo(int videoID, int playlistID)
        {
            if (!this.Exists(videoID))
            {
                throw new InvalidOperationException("指定された動画が存在しません。");
            }
            else
            {
                var video = this.GetVideo(videoID)!;

                //動画側の参照を切る
                if (video.PlaylistIds is not null)
                {
                    video.PlaylistIds.RemoveAll(pid => pid == playlistID);

                    if (video.PlaylistIds.Count > 0)
                    {
                        this.databaseInstance.Update(video, STypes::Video.TableName);
                        return;
                    }
                }

                var result = this.databaseInstance.DeleteAll<STypes::Video>(STypes::Video.TableName, video => video.Id == videoID);

                if (!result.IsSucceeded)
                {
                    if (result.Exception is not null)
                    {
                        this.logger.Error($"動画({videoID})の{playlistID}からの削除に失敗しました。", result.Exception);
                    }
                    else
                    {
                        this.logger.Error($"動画({videoID})の{playlistID}からの削除に失敗しました。");
                    }

                }

                this.logger.Log($"動画({videoID})を{playlistID}から削除しました。");

            }
        }

        /// <summary>
        /// 動画を削除する
        /// </summary>
        /// <param name="videoId"></param>
        private void RemoveVideo(int videoId)
        {
            this.databaseInstance.DeleteAll<STypes::Video>(STypes::Video.TableName, video => video.Id == videoId);
        }

        /// <summary>
        /// 情報を更新する
        /// </summary>
        /// <param name="videoData"></param>
        public void Update(IListVideoInfo videoData)
        {
            if (this.Exists(videoData.Id.Value))
            {
                var video = this.GetVideo(videoData.Id.Value)!;
                this.SetData(video, videoData);
                this.Update(video);
            }
        }

        /// <summary>
        /// 情報を更新する
        /// </summary>
        /// <param name="video"></param>
        private void Update(STypes::Video video)
        {
            this.databaseInstance.Update(video, STypes::Video.TableName);
        }

        /// <summary>
        /// 指定したIDの動画を取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public STypes::Video? GetVideo(int id)
        {
            var result = this.databaseInstance.GetRecord<STypes::Video>(STypes::Video.TableName, id);


            if (!result.IsSucceeded || result.Data is null)
            {
                if (result.Exception is not null)
                {
                    this.logger.Error($"動画({id})の取得に失敗しました。", result.Exception);
                }
                else
                {
                    this.logger.Error($"動画({id})の取得に失敗しました。");
                }

                return null;
            }

            return result.Data;
        }

        /// <summary>
        /// 指定したIDの動画を取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public STypes::Video? GetVideo(string niconicoId)
        {
            var result = this.databaseInstance.GetRecord<STypes::Video>(STypes::Video.TableName, video => video.NiconicoId == niconicoId);

            if (!result.IsSucceeded || result.Data is null)
            {
                if (result.Exception is not null)
                {
                    this.logger.Error($"動画({niconicoId})の取得に失敗しました。", result.Exception);
                }
                else
                {
                    this.logger.Error($"動画({niconicoId})の取得に失敗しました。");
                }

                return null;
            }
            return result.Data;
        }

        /// <summary>
        /// 動画の存在をチェックする
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Exists(int id)
        {
            return this.databaseInstance.Exists<STypes::Video>(STypes::Video.TableName, id);
        }

        /// <summary>
        /// 動画の存在をチェックする
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Exists(string niconicoId)
        {
            return this.databaseInstance.Exists<STypes::Video>(STypes::Video.TableName, video => video.NiconicoId == niconicoId);
        }

        /// <summary>
        /// データをセットする
        /// </summary>
        /// <param name="dbVideo"></param>
        /// <param name="videoData"></param>
        private void SetData(STypes::Video dbVideo, IListVideoInfo videoData)
        {
            dbVideo.NiconicoId = videoData.NiconicoId.Value;
            dbVideo.Title = videoData.Title.Value;
            dbVideo.UploadedOn = videoData.UploadedOn.Value;
            dbVideo.ThumbUrl = videoData.ThumbUrl.Value;
            dbVideo.LargeThumbUrl = videoData.LargeThumbUrl.Value;
            dbVideo.ThumbPath = videoData.ThumbPath.Value;
            dbVideo.IsSelected = videoData.IsSelected.Value;
            dbVideo.FileName = videoData.FileName.Value;
            dbVideo.Tags = videoData.Tags.ToList();
            dbVideo.ViewCount = videoData.ViewCount.Value;
            dbVideo.CommentCount = videoData.CommentCount.Value;
            dbVideo.MylistCount = videoData.MylistCount.Value;
            dbVideo.LikeCount = videoData.LikeCount.Value;
            dbVideo.Duration = videoData.Duration.Value;
            dbVideo.OwnerID = videoData.OwnerID.Value;
            dbVideo.OwnerName = videoData.OwnerName.Value;
        }

        private void JustifyVideo(STypes::Video video)
        {
            if (video.PlaylistIds is null || video.PlaylistIds.Count == 0)
            {
                this.RemoveVideo(video.Id);
            }
            else
            {
                int original = video.PlaylistIds.Count;
                video.PlaylistIds = video.PlaylistIds.Where(playlistId =>
                {
                    bool exists = this.databaseInstance.Exists<STypes::Playlist>(STypes::Playlist.TableName, playlistId);
                    if (!exists) return false;
                    var playlist = this.databaseInstance.GetRecord<STypes::Playlist>(STypes::Playlist.TableName, playlistId).Data!;
                    bool contains = playlist.Videos.Select(v => v.Id).Contains(video.Id);
                    return contains;
                }).Distinct().ToList();

                if (video.PlaylistIds is null)
                {
                    this.RemoveVideo(video.Id);
                }
                else
                {
                    this.Update(video);
                }
            }
        }

        /// <summary>
        /// 動画の整合性をとる
        /// </summary>
        public void JustifyVideos()
        {
            var videos = this.GetAllVideos();
            foreach (var video in videos)
            {
                this.JustifyVideo(video);
            }
        }



    }
}
