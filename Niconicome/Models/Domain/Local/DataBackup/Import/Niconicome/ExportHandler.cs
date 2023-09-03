using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.DataBackup.Import.Niconicome.Converter;
using Niconicome.Models.Domain.Local.DataBackup.Import.Niconicome.Error;
using Niconicome.Models.Domain.Local.DataBackup.Import.Niconicome.StringContent;
using Niconicome.Models.Domain.Local.IO.V2;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Domain.Utils.StringHandler;
using Niconicome.Models.Helper.Result;
using Windows.Devices.Printers;
using Type = Niconicome.Models.Domain.Local.DataBackup.Import.Niconicome.Type;

namespace Niconicome.Models.Domain.Local.DataBackup.Import.Niconicome
{
    public interface IExportHandler
    {
        /// <summary>
        /// データをエクスポート
        /// </summary>
        /// <returns></returns>
        IAttemptResult<string> ExportData();
    }

    public class ExportHandler : IExportHandler
    {
        public ExportHandler(IPlaylistStore playlistStore, IVideoStore videoStore, ITagStore tagStore, INiconicomeFileIO fileIO, IExportConverter converter, IErrorHandler errorHandler, IStringHandler stringHandler, INiconicomeDirectoryIO directoryIO)
        {
            this._playlistStore = playlistStore;
            this._videoStore = videoStore;
            this._tagStore = tagStore;
            this._fileIO = fileIO;
            this._converter = converter;
            this._errorHandler = errorHandler;
            this._stringHandler = stringHandler;
            this._directoryIO = directoryIO;
        }

        #region field

        private readonly IPlaylistStore _playlistStore;

        private readonly IVideoStore _videoStore;

        private readonly ITagStore _tagStore;

        private readonly INiconicomeFileIO _fileIO;

        private readonly IExportConverter _converter;

        private readonly IErrorHandler _errorHandler;

        private readonly IStringHandler _stringHandler;

        private readonly INiconicomeDirectoryIO _directoryIO;

        #endregion

        #region Method

        public IAttemptResult<string> ExportData()
        {
            IAttemptResult<Type::Data> dResult = this.GetData();
            if (!dResult.IsSucceeded || dResult.Data is null)
            {
                return AttemptResult<string>.Fail(dResult.Message);
            }

            string content;

            try
            {
                content = JsonParser.Serialize(dResult.Data);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(ExportError.FailedToSerialize, ex);
                return AttemptResult<string>.Fail(this._errorHandler.GetMessageForResult(ExportError.FailedToSerialize, ex));
            }

            var fileName = this._stringHandler.GetContent(ExportSC.FileName, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));
            var folderPath = Path.Join(AppContext.BaseDirectory, FileFolder.ExportFolderPath);
            var path = Path.Join(folderPath, fileName);

            if (!this._directoryIO.Exists(folderPath))
            {
                IAttemptResult dirResult = this._directoryIO.CreateDirectory(folderPath);
                if (!dirResult.IsSucceeded)
                {
                    return AttemptResult<string>.Fail(dirResult.Message);
                }
            }

            IAttemptResult wResult = this._fileIO.Write(path, content);
            if (!wResult.IsSucceeded)
            {
                return AttemptResult<string>.Fail(wResult.Message);
            }

            return AttemptResult<string>.Succeeded(path);
        }

        #endregion

        #region private

        /// <summary>
        /// データを取得
        /// </summary>
        /// <returns></returns>
        private IAttemptResult<Type::Data> GetData()
        {

            IAttemptResult<IEnumerable<IPlaylistInfo>> pResult = this._playlistStore.GetAllPlaylist();
            if (!pResult.IsSucceeded || pResult.Data is null)
            {
                return AttemptResult<Type::Data>.Fail(pResult.Message);
            }

            IAttemptResult<IEnumerable<IVideoInfo>> vResult = this._videoStore.GetAllVideos();
            if (!vResult.IsSucceeded || vResult.Data is null)
            {
                return AttemptResult<Type::Data>.Fail(vResult.Message);
            }

            IAttemptResult<IEnumerable<ITagInfo>> tResult = this._tagStore.GetAllTag();
            if (!tResult.IsSucceeded || tResult.Data is null)
            {
                return AttemptResult<Type::Data>.Fail(tResult.Message);
            }

            var playlists = new List<Type::Playlist>();
            IReadOnlyList<int> exclude = this.GetExcludePlaylistIDs(pResult.Data);

            foreach (var p in pResult.Data)
            {
                if (!this.CheckWhetherExportablePlaylist(p))
                {
                    continue;
                }

                playlists.Add(this._converter.ConvertPlaylist(p, exclude));
            }

            var videos = new List<Type::Video>();
            foreach (var v in vResult.Data)
            {
                if (v.PlaylistID == 0)
                {
                    continue;
                }
                videos.Add(this._converter.ConvertVideo(v));
            }

            var tags = new List<Type::Tag>();
            foreach (var t in tResult.Data)
            {
                tags.Add(this._converter.ConvertTag(t));
            }

            var data = new Type::Data()
            {
                Videos = videos,
                Playlists = playlists,
                Tags = tags
            };

            return AttemptResult<Type::Data>.Succeeded(data);
        }

        /// <summary>
        /// 特殊プレイリストはroot以外エクスポートしない
        /// </summary>
        /// <param name="playlist"></param>
        /// <returns></returns>
        private bool CheckWhetherExportablePlaylist(IPlaylistInfo playlist)
        {
            if (playlist.PlaylistType == PlaylistType.Temporary)
            {
                return false;
            }

            if (playlist.PlaylistType == PlaylistType.DownloadFailedHistory)
            {
                return false;
            }

            if (playlist.PlaylistType == PlaylistType.DownloadSucceededHistory)
            {
                return false;
            }

            if (playlist.PlaylistType == PlaylistType.PlaybackHistory)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// エクスポートしないプレイリストのIDを取得
        /// </summary>
        /// <param name="playlists"></param>
        /// <returns></returns>
        private IReadOnlyList<int> GetExcludePlaylistIDs(IEnumerable<IPlaylistInfo> playlists)
        {
            var exclude = new List<int>();
            foreach (var playlist in playlists)
            {
                if (!this.CheckWhetherExportablePlaylist(playlist))
                {
                    exclude.Add(playlist.ID);
                }
            }

            return exclude.AsReadOnly();
        }

        #endregion
    }
}
