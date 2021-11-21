using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
        /// <summary>
        /// 引数で与えた動画リスト内の動画情報を更新する
        /// </summary>
        /// <param name="videos"></param>
        /// <param name="addFunc"></param>
        /// <param name="disableDBRetrieving"></param>
        /// <returns></returns>
        IAttemptResult Refresh(IEnumerable<IListVideoInfo> videos, Action<IListVideoInfo> addFunc, bool disableDBRetrieving = false);
    }

    public class VideoListRefresher : IVideoListRefresher
    {
        public VideoListRefresher(IPlaylistStoreHandler playlistStoreHandler, IVideoHandler videoHandler, ILocalSettingHandler localSettingHandler, ILocalVideoUtils localVideoUtils, IVideoThumnailUtility videoThumnailUtility, ICurrent current, ILightVideoListinfoHandler lightVideoListinfoHandler)
        {
            this.playlistStoreHandler = playlistStoreHandler;
            this.videoHandler = videoHandler;
            this.current = current;
            this.settingHandler = localSettingHandler;
            this.videoThumnailUtility = videoThumnailUtility;
            this.localVideoUtils = localVideoUtils;
            this.lightVideoListinfoHandler = lightVideoListinfoHandler;
        }

        #region DIされるクラス

        private readonly IPlaylistStoreHandler playlistStoreHandler;

        private readonly IVideoHandler videoHandler;

        private readonly ILocalSettingHandler settingHandler;

        private readonly ILocalVideoUtils localVideoUtils;

        private readonly IVideoThumnailUtility videoThumnailUtility;

        private readonly ICurrent current;

        private ILightVideoListinfoHandler lightVideoListinfoHandler;
        #endregion

        /// <summary>
        /// 動画リストの更新のみを担当する
        /// </summary>
        /// <param name="videos"></param>
        /// <returns></returns>
        public IAttemptResult Refresh(IEnumerable<IListVideoInfo> videos, Action<IListVideoInfo> addFunc, bool disableDBRetrieving = false)
        {
            var playlistID = this.current.SelectedPlaylist.Value?.Id ?? -1;

            if (playlistID == -1)
            {
                return new AttemptResult()
                {
                    Message = $"プレイストが選択されていません。",
                };
            }

            IEnumerable<IListVideoInfo> originalVideos;
            if (disableDBRetrieving)
            {
                originalVideos = videos;
            }
            else
            {
                var playlist = this.playlistStoreHandler.GetPlaylist(playlistID);

                if (playlist is null)
                {
                    return new AttemptResult()
                    {
                        Message = $"データベースからのプレイリストの取得に失敗しました。(id:{playlistID})",
                    };
                }
                originalVideos = playlist.Videos.Select(v =>
                {
                    IListVideoInfo video = VideoInfoContainer.New();
                    video.Id.Value = v.Id;
                    return video;
                });
            }

            if (originalVideos is null)
            {
                return new AttemptResult()
                {
                    Message = $"データベースからのプレイリストの取得に失敗しました。(id:{playlistID}, detail: VIDEO_PROPERTY_IS_NULL)",
                };
            }

            var format = this.settingHandler.GetStringSetting(SettingsEnum.FileNameFormat) ?? "[<id>]<title>";
            var replaceStricted = this.settingHandler.GetBoolSetting(SettingsEnum.ReplaceSBToMB);
            var folderPath = this.current.PlaylistFolderPath;
            string? economySuffix = this.settingHandler.GetStringSetting(SettingsEnum.EconomySuffix);
            bool searchByID = this.settingHandler.GetBoolSetting(SettingsEnum.SearchFileByID);

            this.videoThumnailUtility.GetFundamentalThumbsIfNotExist();
            this.localVideoUtils.ClearCache();
            this.lightVideoListinfoHandler.AddPlaylist(playlistID);

            foreach (var originalVideo in originalVideos)
            {
                if (playlistID != (this.current.SelectedPlaylist.Value?.Id ?? -1))
                {
                    return new AttemptResult()
                    {
                        Message = $"動画リスト更新中にプレイリストが変更されました。",
                    };
                }

                IListVideoInfo video;
                if (disableDBRetrieving)
                {
                    video = originalVideo;
                }
                else
                {
                    video = this.videoHandler.GetVideo(originalVideo.Id.Value);
                }

                ILightVideoListInfo light = this.lightVideoListinfoHandler.GetLightVideoListInfo(video.NiconicoId.Value, playlistID);
                video.Message = light.Message;
                video.IsSelected = light.IsSelected;


                var filename = this.localVideoUtils.GetFilePath(video, folderPath, format, replaceStricted, searchByID);
                if (filename.EndsWith(FileFolder.Mp4FileExt) || filename.EndsWith(FileFolder.TsFileExt))
                {
                    video.FileName.Value = filename;
                    video.FolderPath.Value = Path.GetDirectoryName(filename) ?? folderPath;
                    video.IsDownloaded.Value = !video.FileName.Value.IsNullOrEmpty();

                    if (economySuffix is not null)
                    {
                        if (filename.Contains(economySuffix))
                        {
                            video.IsEconomy.Value = true;
                        }
                        else
                        {
                            video.IsEconomy.Value = false;
                        }
                    }

                }
                else
                {
                    video.FolderPath.Value = filename;
                }

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
                    addFunc(video);
                }
                else if (!IsValidPath && hasCache)
                {
                    video.ThumbPath.Value = this.videoThumnailUtility.GetThumbFilePath(video.NiconicoId.Value);
                    this.videoHandler.Update(video);
                    addFunc(video);
                }
                else if (!hasCache)
                {
                    video.ThumbPath.Value = this.videoThumnailUtility.GetThumbFilePath("0");
                    addFunc(video);
                }
                else
                {
                    video.ThumbPath.Value = this.videoThumnailUtility.GetThumbFilePath(video.NiconicoId.Value);
                    addFunc(video);
                }
            }

            return new AttemptResult()
            {
                IsSucceeded = true,
            };
        }
    }
}
