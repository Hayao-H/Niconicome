using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local;
using Niconicome.Models.Network;

namespace Niconicome.Models.Playlist.VideoList
{

    public interface IVideoListRefresher
    {
        IAttemptResult Refresh(List<IListVideoInfo> videos);
    }

    public class VideoListRefresher : IVideoListRefresher
    {
        public VideoListRefresher(IPlaylistStoreHandler playlistStoreHandler,IVideoHandler videoHandler,ILocalSettingHandler localSettingHandler,ILocalVideoUtils localVideoUtils,IVideoThumnailUtility videoThumnailUtility,ICurrent current)
        {
            this.playlistStoreHandler = playlistStoreHandler;
            this.videoHandler = videoHandler;
            this.current = current;
            this.settingHandler = localSettingHandler;
            this.videoThumnailUtility = videoThumnailUtility;
            this.localVideoUtils = localVideoUtils;
        }

        #region DIされるクラス

        private readonly IPlaylistStoreHandler playlistStoreHandler;

        private readonly IVideoHandler videoHandler;

        private readonly ILocalSettingHandler settingHandler;

        private readonly ILocalVideoUtils localVideoUtils;

        private readonly IVideoThumnailUtility videoThumnailUtility;

        private readonly ICurrent current;

        #endregion

        /// <summary>
        /// 動画リストの更新のみを担当する
        /// </summary>
        /// <param name="videos"></param>
        /// <returns></returns>
        public IAttemptResult Refresh(List<IListVideoInfo> videos)
        {
            var playlistID = this.current.SelectedPlaylistID;
            var playlist = this.playlistStoreHandler.GetPlaylist(playlistID);
            var originVideos = playlist?.Videos;

            if (playlist is null)
            {
                return new AttemptResult()
                {
                    Message = $"データベースからのプレイリストの取得に失敗しました。(id:{playlistID})",
                };
            }

            if (originVideos is null)
            {
                return new AttemptResult()
                {
                    Message = $"データベースからのプレイリストの取得に失敗しました。(id:{playlistID}, detail: VIDEO_PROPERTY_IS_NULL)",
                };
            }

            var format = this.settingHandler.GetStringSetting(Settings.FileNameFormat) ?? "[<id>]<title>";
            var replaceStricted = this.settingHandler.GetBoolSetting(Settings.ReplaceSBToMB);
            var defaultDir = this.settingHandler.GetStringSetting(Settings.DefaultFolder) ?? FileFolder.DefaultDownloadDir;
            var folderPath = playlist.FolderPath ?? defaultDir;

            foreach (var originVideo in originVideos)
            {
                if (playlistID != this.current.SelectedPlaylistID)
                {
                    return new AttemptResult()
                    {
                        Message = $"動画リスト更新中にプレイリストが変更されました。",
                    };
                }

                var video = this.videoHandler.GetVideo(originVideo.Id);

                //保持されている動画情報があれば引き継ぐ
                var lightVideo = LightVideoListinfoHandler.GetLightVideoListInfo(originVideo.Id, playlistID);

                if (lightVideo is not null)
                {
                    video.MessageGuid = lightVideo.MessageGuid;
                    video.IsSelected = lightVideo.IsSelected;
                    video.Message = VideoMessanger.GetMessage(lightVideo.MessageGuid);
                    video.FileName = lightVideo.FileName;
                }
                else
                {
                    video.FileName = string.Empty;
                }

                if (video.FileName.IsNullOrEmpty())
                {
                    video.FileName = this.localVideoUtils.GetFilePath(video, folderPath, format, replaceStricted);
                }
                video.IsDownloaded = !video.FileName.IsNullOrEmpty();

                //サムネイル
                bool hasCache = this.videoThumnailUtility.HasThumbnailCache(video);
                bool isValid = this.videoThumnailUtility.IsValidThumbnail(video);

                if (isValid && !hasCache)
                {
                    this.videoThumnailUtility.GetThumbAsync(video);
                    video.ThumbPath = this.videoThumnailUtility.GetThumbFilePath(video.NiconicoId);
                    this.videoHandler.Update(video);
                    videos.Add(video);
                }
                else if (!hasCache)
                {
                    video.ThumbPath = this.videoThumnailUtility.GetThumbFilePath("0");
                    videos.Add(video);
                }
                else
                {
                    videos.Add(video);
                }
            }

            return new AttemptResult()
            {
                IsSucceeded = true,
            };
        }
    }
}
