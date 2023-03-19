using System;
using System.Collections.Generic;
using System.Linq;
using Niconicome.Models.Const;
using Niconicome.Models.Domain.Local.DataBackup.Import.Niconicome.Converter;
using Niconicome.Models.Domain.Local.DataBackup.Import.Niconicome.Error;
using Niconicome.Models.Domain.Local.IO.V2;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Niconico.Net.Json;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Type = Niconicome.Models.Domain.Local.DataBackup.Import.Niconicome.Type;

namespace Niconicome.Models.Domain.Local.DataBackup.Import.Niconicome
{
    public interface IImportHandler
    {
        /// <summary>
        /// データをインポート
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        IAttemptResult<ImportResult> ImportData(string filePath);
    }

    public class ImportHandler : IImportHandler
    {
        public ImportHandler(INiconicomeFileIO fileIO, IErrorHandler errorHandler, IImportConverter converter, IVideoStore videoStore, IPlaylistStore playlistStore, ITagStore tagStore)
        {
            this._fileIO = fileIO;
            this._errorHandler = errorHandler;
            this._converter = converter;
            this._videoStore = videoStore;
            this._playlistStore = playlistStore;
            this._tagStore = tagStore;
        }

        #region field

        private readonly INiconicomeFileIO _fileIO;

        private readonly IErrorHandler _errorHandler;

        private readonly IImportConverter _converter;

        private readonly IVideoStore _videoStore;

        private readonly IPlaylistStore _playlistStore;

        private readonly ITagStore _tagStore;

        #endregion

        #region Method

        public IAttemptResult<ImportResult> ImportData(string filePath)
        {
            IAttemptResult<Type::Data> loadResult = this.GetData(filePath);
            if (!loadResult.IsSucceeded || loadResult.Data is null)
            {
                return AttemptResult<ImportResult>.Fail(loadResult.Message);
            }

            if (!this.ValidateData(loadResult.Data))
            {
                this._errorHandler.HandleError(ImportError.DataIsInvalid);
                return AttemptResult<ImportResult>.Fail(this._errorHandler.GetMessageForResult(ImportError.DataIsInvalid));
            }

            IAttemptResult preResult = this.PreImport();
            if (!preResult.IsSucceeded)
            {
                return AttemptResult<ImportResult>.Fail(preResult.Message);
            }

            Dictionary<int, Type::Playlist> playlists = loadResult.Data.Playlists.ToDictionary(p => p.Id);
            Dictionary<int, Type::Video> videos = loadResult.Data.Videos.ToDictionary(v => v.Id);
            Dictionary<int, Type::Tag> tags = loadResult.Data.Tags.ToDictionary(t => t.Id);

            var source = new ImportSource(playlists, videos, tags);
            return this.ImportInternal(source);
        }

        #endregion

        #region private

        /// <summary>
        /// インポートの前段階
        /// </summary>
        /// <returns></returns>
        private IAttemptResult PreImport()
        {
            IAttemptResult vResult = this._videoStore.Clear();
            if (!vResult.IsSucceeded)
            {
                return vResult;
            }

            IAttemptResult pResult = this._playlistStore.Clear();
            if (!pResult.IsSucceeded)
            {
                return pResult;
            }

            IAttemptResult tResult = this._tagStore.Clear();
            if (!tResult.IsSucceeded)
            {
                return tResult;
            }

            //ルートを作成
            IAttemptResult<int> rootCreationResult = this._playlistStore.Create(LocalConstant.RootPlaylistName);
            if (!rootCreationResult.IsSucceeded)
            {
                return AttemptResult.Fail(rootCreationResult.Message);
            }

            IAttemptResult<IPlaylistInfo> rootResult = this._playlistStore.GetPlaylist(rootCreationResult.Data);
            if (!rootResult.IsSucceeded || rootResult.Data is null)
            {
                return AttemptResult.Fail(rootResult.Message);
            }

            rootResult.Data.PlaylistType = PlaylistType.Root;

            return AttemptResult.Succeeded();
        }

        /// <summary>
        /// インポート用のデータを取得
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private IAttemptResult<Type::Data> GetData(string filePath)
        {
            IAttemptResult<string> readResult = this._fileIO.Read(filePath);

            if (!readResult.IsSucceeded || readResult.Data is null)
            {
                return AttemptResult<Type::Data>.Fail(readResult.Message);
            }

            Type::Data data;

            try
            {
                data = JsonParser.DeSerialize<Type::Data>(readResult.Data);
            }
            catch (Exception ex)
            {
                this._errorHandler.HandleError(ImportError.FailedToLoadData, ex);
                return AttemptResult<Type::Data>.Fail(this._errorHandler.GetMessageForResult(ImportError.FailedToLoadData, ex));
            }

            return AttemptResult<Type::Data>.Succeeded(data);
        }

        /// <summary>
        /// インポートデータを検証
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool ValidateData(Type::Data data)
        {
            //rootがない
            if (!data.Playlists.Any(p => p.PlaylistType == 6))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// インポートの実処理
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private IAttemptResult<ImportResult> ImportInternal(ImportSource source)
        {
            var convertedP = new Dictionary<int, IPlaylistInfo>();
            var succeededV = 0;
            var succeededP = 0;

            foreach (var kv in source.Playlists)
            {
                IAttemptResult<ConvertResult> cResult = this._converter.ConvertToPlaylistInfo(kv.Value, source);
                if (cResult.IsSucceeded && cResult.Data is not null)
                {
                    convertedP.Add(kv.Key, cResult.Data.PlaylistInfo);
                    succeededP++;
                    succeededV += cResult.Data.ImportedVideos;
                }
                else
                {
                    this._errorHandler.HandleError(ImportError.FailedToImportPlaylist, kv.Value.Name);
                }
            }

            //親子関係の構築
            foreach (var kv in convertedP)
            {
                Type::Playlist data = source.Playlists[kv.Key];

                foreach (var child in data.Children)
                {
                    if (!convertedP.ContainsKey(child))
                    {
                        continue;
                    }

                    kv.Value.AddChild(convertedP[child]);
                }
            }

            return AttemptResult<ImportResult>.Succeeded(new ImportResult(succeededP, succeededV));
        }

        #endregion
    }
}
