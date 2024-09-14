using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Niconicome.Extensions;
using Niconicome.Extensions.System;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.External.Software.FFmpeg.ffprobe;
using Niconicome.Models.Domain.Local.IO;
using Niconicome.Models.Domain.Local.IO.V2;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Network.Video;
using Niconicome.Models.Playlist.V2.Manager.Error;

namespace Niconicome.Models.Playlist.V2.Manager.Helper
{

    public interface ILocalVideoLoader
    {
        /// <summary>
        /// 非同期で動画のサムネパスとファイルパスを設定
        /// </summary>
        /// <param name="videos"></param>
        /// <param name="quick"></param>
        /// <returns></returns>
        Task<IAttemptResult> SetPathAsync(IEnumerable<IVideoInfo> videos, bool quick);
    }

    public class LocalVideoLoader : ILocalVideoLoader
    {
        public LocalVideoLoader(INiconicomeDirectoryIO directoryIO, IThumbnailUtility thumbnailUtility, ISettingsContainer settingsConainer, IPlaylistVideoContainer playlistVideoContainer, IErrorHandler errorHandler, INiconicomeFileIO fileIO)
        {
            this._directoryIO = directoryIO;
            this._fileIO = fileIO;
            this._thumbnailUtility = thumbnailUtility;
            this._settingsContainer = settingsConainer;
            this._playlistVideoContainer = playlistVideoContainer;
            this._errorHandler = errorHandler;
        }

        #region field

        private readonly INiconicomeDirectoryIO _directoryIO;

        private readonly INiconicomeFileIO _fileIO;

        private readonly IThumbnailUtility _thumbnailUtility;

        private readonly ISettingsContainer _settingsContainer;

        private readonly IPlaylistVideoContainer _playlistVideoContainer;

        private readonly IErrorHandler _errorHandler;

        private List<string>? _cachedFiles;

        private List<string>? _cachedFolders;

        #endregion

        #region Method
        public async Task<IAttemptResult> SetPathAsync(IEnumerable<IVideoInfo> videos, bool quick)
        {
            if (this._playlistVideoContainer.CurrentSelectedPlaylist is null)
            {
                this._errorHandler.HandleError(LocalVideoLoaderError.PlaylistIsNotSelected);
                return AttemptResult.Fail(this._errorHandler.GetMessageForResult(LocalVideoLoaderError.PlaylistIsNotSelected));
            }

            this._cachedFiles = null;
            this._cachedFolders = null;

            int playlistID = this._playlistVideoContainer.CurrentSelectedPlaylist.ID;
            string folderPath = this._playlistVideoContainer.CurrentSelectedPlaylist.FolderPath;
            string economy = this._settingsContainer.GetSetting(SettingNames.EnonomyQualitySuffix, "").Data?.Value ?? "";

            if (string.IsNullOrEmpty(folderPath))
            {
                folderPath = this.GetDownlaodDirectory(this._playlistVideoContainer.CurrentSelectedPlaylist);
            }
            this._playlistVideoContainer.CurrentSelectedPlaylist.TemporaryFolderPath = folderPath;

            ///削除動画のサムネを保存
            await this._thumbnailUtility.DownloadDeletedVideoThumbAsync();

            foreach (var video in videos)
            {
                if (playlistID != (this._playlistVideoContainer.CurrentSelectedPlaylist?.ID ?? -1))
                {
                    this._errorHandler.HandleError(LocalVideoLoaderError.PlaylistChanged);
                    return AttemptResult.Fail(this._errorHandler.GetMessageForResult(LocalVideoLoaderError.PlaylistChanged));
                }

                if (this.IsDownloaded(video))
                {
                    video.IsDownloaded.Value = true;
                }
                else if (CheckWhetherSetDownloadPathOrNot(quick, video))
                {
                    IAttemptResult<string> pathResult = this.GetFilePath(video.NiconicoId, folderPath);
                    if (pathResult.IsSucceeded && pathResult.Data is not null)
                    {
                        video.FilePath = pathResult.Data;
                        video.IsDownloaded.Value = true;

                        if (!economy.IsNullOrEmpty())
                        {
                            if (pathResult.Data.Contains(economy))
                            {
                                video.IsEconomy = true;
                            }
                            else
                            {
                                video.IsEconomy = false;
                            }
                        }
                    }
                    else
                    {
                        video.IsDownloaded.Value = false;
                    }
                }
                else
                {
                    video.FilePath = string.Empty;
                    video.IsDownloaded.Value = false;
                }


                //サムネイル
                bool hasCache = this._thumbnailUtility.IsThumbExists(video.NiconicoId);

                if (!hasCache)
                {
                    this._thumbnailUtility.DownloadThumb(video.NiconicoId, video.ThumbUrl, result =>
                    {
                        if (result.IsSucceeded && result.Data is not null)
                        {
                            video.ThumbPath.Value = result.Data;
                        }
                        else
                        {
                            video.ThumbPath.Value = this._thumbnailUtility.GetDeletedVideoThumb();
                        }
                    });
                }
                else
                {
                    IAttemptResult<string> tResult = this._thumbnailUtility.GetThumbPath(video.NiconicoId);
                    if (tResult.IsSucceeded && tResult.Data is not null)
                    {
                        video.ThumbPath.Value = tResult.Data;
                    }
                    else
                    {
                        video.ThumbPath.Value = this._thumbnailUtility.GetDeletedVideoThumb();
                    }
                }

            }

            return AttemptResult.Succeeded();
        }

        #endregion

        #region private

        /// <summary>
        /// 動画ファイルのパスを取得
        /// </summary>
        /// <param name="niconicoID"></param>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        private IAttemptResult<string> GetFilePath(string niconicoID, string folderPath)
        {

            if (!Path.IsPathRooted(folderPath))
            {
                folderPath = Path.Combine(AppContext.BaseDirectory, folderPath);
            }

            if (this._cachedFiles is null || this._cachedFolders is null)
            {
                this._cachedFiles = new List<string>();
                this._cachedFolders = new List<string>();
                if (this._directoryIO.Exists(folderPath))
                {
                    this._cachedFiles.AddRange(this._directoryIO.GetFiles(folderPath, $"*{FileFolder.Mp4FileExt}").Data?.Select(p => Path.Combine(folderPath, p))?.ToList() ?? new List<string>());
                    this._cachedFiles.AddRange(this._directoryIO.GetFiles(folderPath, $"*{FileFolder.TsFileExt}").Data?.Select(p => Path.Combine(folderPath, p)).ToList() ?? new List<string>());
                    this._cachedFolders.AddRange(this._directoryIO.GetDirectories(folderPath).Data?.Select(p => Path.Combine(folderPath, p)).ToList() ?? new List<string>());
                }
            }

            //stream.jsonを確認
            string? firstFolder = this._cachedFolders.FirstOrDefault(p => p.Contains(niconicoID));
            if (firstFolder is not null && this._fileIO.Exists(Path.Combine(firstFolder, "stream.json")))
            {
                return AttemptResult<string>.Succeeded(Path.Combine(firstFolder, "stream.json"));
            }

            string? firstMp4 = this._cachedFiles.FirstOrDefault(p => p.Contains(niconicoID));

            //.mp4ファイルを確認
            if (firstMp4 is not null)
            {
                return AttemptResult<string>.Succeeded(firstMp4);
            }

            string? firstTS = this._cachedFiles.FirstOrDefault(p => p.Contains(niconicoID));
            if (firstTS is not null)
            //.tsファイルを確認
            {
                return AttemptResult<string>.Succeeded(firstTS);
            }


            return AttemptResult<string>.Fail();

        }

        private string GetDownlaodDirectory(IPlaylistInfo playlist)
        {
            string pathOfPlaylist = playlist.FolderPath;

            //そもそもプレイリストにパスが設定されている
            if (!string.IsNullOrEmpty(pathOfPlaylist)) return pathOfPlaylist;

            //デフォルト設定を取得
            IAttemptResult<ISettingInfo<string>> settingResult = this._settingsContainer.GetSetting(SettingNames.DefaultFolder, FileFolder.DefaultDownloadDir);
            if (!settingResult.IsSucceeded || settingResult.Data is null) return Path.Combine(AppContext.BaseDirectory, FileFolder.DefaultDownloadDir);

            if (settingResult.Data.Value.Contains(Format.FolderAutoMapSymbol))
            {
                string path = settingResult.Data.Value.Replace(Format.FolderAutoMapSymbol, string.Join(@"\", new List<string>(playlist.ParentNames) { playlist.Name.Value }));
                return path;
            }
            else
            {
                return settingResult.Data.Value;
            }
        }

        /// <summary>
        /// ファイルパスをセットする必要があるかどうか
        /// </summary>
        /// <param name="quick"></param>
        /// <param name="videoInfo"></param>
        /// <returns></returns>
        private bool CheckWhetherSetDownloadPathOrNot(bool quick, IVideoInfo videoInfo)
        {
            if (quick)
            {
                return false;
            }

            if (string.IsNullOrEmpty(videoInfo.FilePath))
            {
                return true;
            }


            return false;
        }

        /// <summary>
        /// ダウンロード状態をチェック
        /// </summary>
        /// <param name="videoInfo"></param>
        /// <returns></returns>
        private bool IsDownloaded(IVideoInfo videoInfo)
        {
            return this._fileIO.Exists(videoInfo.FilePath);
        }

        #endregion
    }
}
