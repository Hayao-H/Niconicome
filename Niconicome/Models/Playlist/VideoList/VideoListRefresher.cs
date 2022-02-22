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
using STypes = Niconicome.Models.Domain.Local.Store.Types;

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
        public VideoListRefresher(IPlaylistStoreHandler playlistStoreHandler, IVideoHandler videoHandler, ILocalVideoUtils localVideoUtils, IVideoThumnailUtility videoThumnailUtility, ICurrent current, ILightVideoListinfoHandler lightVideoListinfoHandler, ILocalSettingsContainer container)
        {
            this._playlistStoreHandler = playlistStoreHandler;
            this._videoHandler = videoHandler;
            this._current = current;
            this._videoThumnailUtility = videoThumnailUtility;
            this._localVideoUtils = localVideoUtils;
            this._lightVideoListinfoHandler = lightVideoListinfoHandler;
            this._settingsContainer = container;
        }

        #region field

        private readonly IPlaylistStoreHandler _playlistStoreHandler;

        private readonly IVideoHandler _videoHandler;

        private readonly ILocalSettingsContainer _settingsContainer;

        private readonly ILocalVideoUtils _localVideoUtils;

        private readonly IVideoThumnailUtility _videoThumnailUtility;

        private readonly ICurrent _current;

        private ILightVideoListinfoHandler _lightVideoListinfoHandler;
        #endregion

        /// <summary>
        /// 動画リストの更新のみを担当する
        /// </summary>
        /// <param name="videos"></param>
        /// <returns></returns>
        public IAttemptResult Refresh(IEnumerable<IListVideoInfo> originalVideos, Action<IListVideoInfo> addFunc, bool disableDBRetrieving = false)
        {
            var playlistID = this._current.SelectedPlaylist.Value?.Id ?? -1;

            if (playlistID == -1)
            {
                return AttemptResult.Fail($"プレイストが選択されていません。");
            }


            string format = this._settingsContainer.GetReactiveStringSetting(SettingsEnum.FileNameFormat, Format.DefaultFileNameFormat).Value;
            bool replaceStricted = this._settingsContainer.GetReactiveBoolSetting(SettingsEnum.ReplaceSBToMB).Value;
            string folderPath = this._current.PlaylistFolderPath;
            string? economySuffix = this._settingsContainer.GetReactiveStringSetting(SettingsEnum.EconomySuffix).Value;
            bool searchExact = this._settingsContainer.GetReactiveBoolSetting(SettingsEnum.SearchExact).Value;

            this._videoThumnailUtility.GetFundamentalThumbsIfNotExist();
            this._localVideoUtils.ClearCache();
            this._lightVideoListinfoHandler.AddPlaylist(playlistID);

            foreach (var originalVideo in originalVideos)
            {
                if (playlistID != (this._current.SelectedPlaylist.Value?.Id ?? -1))
                {
                    return AttemptResult.Fail($"動画リスト更新中にプレイリストが変更されました。");
                }

                IListVideoInfo video;
                if (disableDBRetrieving)
                {
                    video = originalVideo;
                }
                else
                {
                    IAttemptResult<IListVideoInfo> vResult = this._videoHandler.GetVideo(originalVideo.Id.Value);
                    if (!vResult.IsSucceeded||vResult.Data is null)
                    {
                        continue;
                    }
                    video = vResult.Data;
                }

                ILightVideoListInfo light = this._lightVideoListinfoHandler.GetLightVideoListInfo(video.NiconicoId.Value, playlistID);
                video.Message = light.Message;
                video.IsSelected = light.IsSelected;


                var filename = this._localVideoUtils.GetFilePath(video, folderPath, format, replaceStricted, searchExact);
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
                bool hasCache = this._videoThumnailUtility.HasThumbnailCache(video);
                bool IsValidUrl = this._videoThumnailUtility.IsValidThumbnailUrl(video);
                bool IsValidPath = this._videoThumnailUtility.IsValidThumbnailPath(video);

                if (IsValidUrl && !hasCache)
                {
                    this._videoThumnailUtility.GetThumbAsync(video, () =>
                    {
                        video.IsThumbDownloading.Value = false;
                        if (this._videoThumnailUtility.HasThumbnailCache(video))
                        {
                            video.ThumbPath.Value = this._videoThumnailUtility.GetThumbFilePath(video.NiconicoId.Value);
                        }
                    });
                    video.IsThumbDownloading.Value = true;
                    video.ThumbPath.Value = this._videoThumnailUtility.GetThumbFilePath("0");
                    addFunc(video);
                }
                else if (!IsValidPath && hasCache)
                {
                    video.ThumbPath.Value = this._videoThumnailUtility.GetThumbFilePath(video.NiconicoId.Value);
                    this._videoHandler.Update(video);
                    addFunc(video);
                }
                else if (!hasCache)
                {
                    video.ThumbPath.Value = this._videoThumnailUtility.GetThumbFilePath("0");
                    addFunc(video);
                }
                else
                {
                    video.ThumbPath.Value = this._videoThumnailUtility.GetThumbFilePath(video.NiconicoId.Value);
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
