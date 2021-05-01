using System;
using System.Collections.Generic;
using System.Linq;
using Niconicome.Models.Playlist;
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

        public VideoStoreHandler(IDataBase dataBase)
        {
            this.databaseInstance = dataBase;
        }

        private readonly IDataBase databaseInstance;


        /// <summary>
        /// 全ての動画を取得する
        /// </summary>
        /// <returns></returns>
        public IEnumerable<STypes::Video> GetAllVideos()
        {

            return this.databaseInstance.GetAllRecords<STypes::Video>(STypes::Video.TableName);
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
            if (this.Exists(videoData.NiconicoId))
            {
                var video = this.GetVideo(videoData.NiconicoId)!;
                if (video.PlaylistIds is null)
                {
                    video.PlaylistIds = new List<int>();
                }
                if (!video.PlaylistIds.Contains(playlistId))
                {
                    video.PlaylistIds.Add(playlistId);
                    this.databaseInstance.Update(video, STypes::Video.TableName);
                }
                videoId = video.Id;
            }
            else
            {
                var video = new STypes::Video()
                {
                    PlaylistIds = new List<int>() { playlistId },
                };

                this.SetData(video, videoData);

                videoId = this.databaseInstance.Store(video, STypes::Video.TableName);

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

                this.databaseInstance.DeleteAll<STypes::Video>(STypes::Video.TableName, video => video.Id == videoID);
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
            if (this.Exists(videoData.Id))
            {
                var video = this.GetVideo(videoData.Id)!;
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
            var video = this.databaseInstance.GetRecord<STypes::Video>(STypes::Video.TableName, id);
            return video;
        }

        /// <summary>
        /// 指定したIDの動画を取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public STypes::Video? GetVideo(string niconicoId)
        {
            var video = this.databaseInstance.GetRecord<STypes::Video>(STypes::Video.TableName, video => video.NiconicoId == niconicoId);
            return video;
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
            dbVideo.NiconicoId = videoData.NiconicoId;
            dbVideo.Title = videoData.Title;
            dbVideo.UploadedOn = videoData.UploadedOn;
            dbVideo.ThumbUrl = videoData.ThumbUrl;
            dbVideo.LargeThumbUrl = videoData.LargeThumbUrl;
            dbVideo.ThumbPath = videoData.ThumbPath;
            dbVideo.IsSelected = videoData.IsSelected;
            dbVideo.FileName = videoData.FileName;
            dbVideo.Tags = videoData.Tags.ToList();
            dbVideo.ViewCount = videoData.ViewCount;
            dbVideo.CommentCount = videoData.CommentCount;
            dbVideo.MylistCount = videoData.MylistCount;
            dbVideo.LikeCount = videoData.LikeCount;
            dbVideo.Duration = videoData.Duration;
            dbVideo.OwnerID = videoData.OwnerID;
            dbVideo.OwnerName = videoData.OwnerName;
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
                    var playlist = this.databaseInstance.GetRecord<STypes::Playlist>(STypes::Playlist.TableName, playlistId)!;
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
