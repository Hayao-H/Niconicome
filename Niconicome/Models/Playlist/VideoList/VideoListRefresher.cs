using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Extensions.System;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.Store;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Local;
using Niconicome.Models.Local.Settings;
using Niconicome.Models.Network;

namespace Niconicome.Models.Playlist.VideoList
{

    public interface IVideoListRefresher
    {
        IAttemptResult Refresh(ObservableCollection<IListVideoInfo> videos);
    }

    public class VideoListRefresher : IVideoListRefresher
    {
        public VideoListRefresher(IPlaylistStoreHandler playlistStoreHandler, IVideoHandler videoHandler, ILocalSettingHandler localSettingHandler, ILocalVideoUtils localVideoUtils, IVideoThumnailUtility videoThumnailUtility, ICurrent current)
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
        public IAttemptResult Refresh(ObservableCollection<IListVideoInfo> videos)
        {
            var playlistID = this.current.SelectedPlaylist.Value?.Id ?? -1;

            if (playlistID == -1)
            {
                return new AttemptResult()
                {
                    Message = $"プレイストが選択されていません。",
                };
            }
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

            var format = this.settingHandler.GetStringSetting(SettingsEnum.FileNameFormat) ?? "[<id>]<title>";
            var replaceStricted = this.settingHandler.GetBoolSetting(SettingsEnum.ReplaceSBToMB);
            var defaultDir = this.settingHandler.GetStringSetting(SettingsEnum.DefaultFolder) ?? FileFolder.DefaultDownloadDir;
            var folderPath = playlist.FolderPath ?? defaultDir;

            this.videoThumnailUtility.GetFundamentalThumbsIfNotExist();

            foreach (var originVideo in originVideos)
            {
                if (playlistID != (this.current.SelectedPlaylist.Value?.Id ?? -1))
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
                    video.IsSelected.Value = lightVideo.IsSelected;
                    video.Message.Value = VideoMessenger.GetMessage(lightVideo.MessageGuid);
                    video.FileName.Value = lightVideo.FileName;
                }
                else
                {
                    video.FileName.Value = string.Empty;
                }

                if (video.FileName.Value.IsNullOrEmpty())
                {
                    video.FileName.Value = this.localVideoUtils.GetFilePath(video, folderPath, format, replaceStricted);
                }
                video.IsDownloaded.Value = !video.FileName.Value.IsNullOrEmpty();

                //サムネイル
                bool hasCache = this.videoThumnailUtility.HasThumbnailCache(video);
                bool IsValidUrl = this.videoThumnailUtility.IsValidThumbnailUrl(video);
                bool IsValidPath = this.videoThumnailUtility.IsValidThumbnailPath(video);

                if (IsValidUrl && !hasCache)
                {
                    this.videoThumnailUtility.GetThumbAsync(video, () =>
                    {
                        video.IsThumbDownloading.Value = false;
                        if (this.videoThumnailUtility.HasThumbnailCache(video))
                        {
                            video.ThumbPath.Value = this.videoThumnailUtility.GetThumbFilePath(video.NiconicoId.Value);
                        }
                    });
                    video.IsThumbDownloading.Value = true;
                    video.ThumbPath.Value = this.videoThumnailUtility.GetThumbFilePath("0");
                    videos.Add(video);
                }
                else if (!IsValidPath && hasCache)
                {
                    video.ThumbPath.Value = this.videoThumnailUtility.GetThumbFilePath(video.NiconicoId.Value);
                    this.videoHandler.Update(video);
                    videos.Add(video);
                }
                else if (!hasCache)
                {
                    video.ThumbPath.Value = this.videoThumnailUtility.GetThumbFilePath("0");
                    videos.Add(video);
                }
                else
                {
                    video.ThumbPath.Value = this.videoThumnailUtility.GetThumbFilePath(video.NiconicoId.Value);
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
