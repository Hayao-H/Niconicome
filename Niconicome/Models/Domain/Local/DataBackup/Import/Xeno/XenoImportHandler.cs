using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.DataBackup.Import.Xeno.Parser;
using Niconicome.Models.Domain.Local.DataBackup.Import.Xeno.Type;
using Niconicome.Models.Domain.Local.Store.V2;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Domain.Utils.StringHandler;
using Niconicome.Models.Helper.Result;
using SC = Niconicome.Models.Domain.Local.DataBackup.Import.Xeno.StringContent.XenoImportHandlerStringContent;
using Err = Niconicome.Models.Domain.Local.DataBackup.Import.Xeno.Error.XenoImportHandlerError;
namespace Niconicome.Models.Domain.Local.DataBackup.Import.Xeno
{
    public interface IXenoImportHandler
    {
        /// <summary>
        /// データをインポート
        /// </summary>
        /// <param name="path"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        IAttemptResult<IXenoImportResult> ImportData(string path, Action<string> onMessage);
    }

    public class XenoImportHandler : IXenoImportHandler
    {
        public XenoImportHandler(IXenoDataParser parser,IVideoStore videoStore,IPlaylistStore playlistStore,IStringHandler stringHandler,IErrorHandler errorHandler)
        {
            this._parser = parser;
            this._videoStore = videoStore;
            this._playlistStore = playlistStore;
            this._stringHandler = stringHandler;
            this._errorHandler = errorHandler;
        }

        #region field

        private readonly IXenoDataParser _parser;

        private readonly IVideoStore _videoStore;

        private readonly IPlaylistStore _playlistStore;

        private readonly IStringHandler _stringHandler;

        private readonly IErrorHandler _errorHandler;

        #endregion

        #region Method

        public IAttemptResult<IXenoImportResult> ImportData(string path, Action<string> onMessage)
        {
            onMessage(this._stringHandler.GetContent(SC.ParsingData));

            IAttemptResult<IEnumerable<IXenoPlaylist>> parseResult = this._parser.ParseData(path);
            if (!parseResult.IsSucceeded || parseResult.Data is null)
            {
                onMessage(this._stringHandler.GetContent(SC.ImportFailed));
                return AttemptResult<IXenoImportResult>.Fail(parseResult.Message);
            }

            IAttemptResult<IPlaylistInfo> rootResult = this.CreateRoot();
            if (!rootResult.IsSucceeded || rootResult.Data is null)
            {
                onMessage(this._stringHandler.GetContent(SC.ImportFailed));
                return AttemptResult<IXenoImportResult>.Fail(rootResult.Message);
            }

            return this.SaveDataToStore(rootResult.Data, parseResult.Data, onMessage);
        }


        #endregion

        #region private

        /// <summary>
        /// ルート作成
        /// </summary>
        /// <returns></returns>
        private IAttemptResult<IPlaylistInfo> CreateRoot()
        {
            IAttemptResult<IPlaylistInfo> rootResult = this._playlistStore.GetPlaylistByType(PlaylistType.Root);
            if (!rootResult.IsSucceeded || rootResult.Data is null)
            {
                return AttemptResult<IPlaylistInfo>.Fail(rootResult.Message);
            }

            IAttemptResult<int> xenoRootCResult = this._playlistStore.Create(this._stringHandler.GetContent(SC.XenoPlaylistName));
            if (!xenoRootCResult.IsSucceeded)
            {
                return AttemptResult<IPlaylistInfo>.Fail(xenoRootCResult.Message);
            }



            IAttemptResult<IPlaylistInfo> xenoRootResult = this._playlistStore.GetPlaylist(xenoRootCResult.Data);
            if (xenoRootResult.IsSucceeded && xenoRootResult.Data is not null)
            {
                rootResult.Data.AddChild(xenoRootResult.Data);
            }

            return xenoRootResult;
        }

        /// <summary>
        /// インポートの実処理
        /// </summary>
        /// <param name="root"></param>
        /// <param name="playlists"></param>
        /// <param name="onMessage"></param>
        /// <returns></returns>
        private IAttemptResult<IXenoImportResult> SaveDataToStore(IPlaylistInfo root, IEnumerable<IXenoPlaylist> playlists, Action<string> onMessage)
        {
            var converted = new Dictionary<string, IPlaylistInfo>();
            var source = playlists.ToDictionary(p => p.ID);

            int importedVideos = 0;

            //まず保存
            foreach (var playlist in playlists)
            {
                onMessage(this._stringHandler.GetContent(SC.ProcessingPlaylist, playlist.Name));

                IAttemptResult<int> cResult = this._playlistStore.Create(playlist.Name);
                if (!cResult.IsSucceeded)
                {
                    this._errorHandler.HandleError(Err.FailedToImportPlaylist, playlist.Name);
                    continue;
                }

                IAttemptResult<IPlaylistInfo> pResult = this._playlistStore.GetPlaylist(cResult.Data);
                if (!pResult.IsSucceeded || pResult.Data is null)
                {
                    this._errorHandler.HandleError(Err.FailedToImportPlaylist, playlist.Name);
                    continue;
                }

                IPlaylistInfo data = pResult.Data;

                //動画
                foreach (var video in playlist.Videos)
                {
                    IAttemptResult vcResult = this._videoStore.Create(video.NiconicoID, data.ID);
                    if (!vcResult.IsSucceeded)
                    {
                        this._errorHandler.HandleError(Err.FailedToImportVideo, playlist.Name, video.NiconicoID);
                        continue;
                    }

                    IAttemptResult<IVideoInfo> vResult = this._videoStore.GetVideo(video.NiconicoID, data.ID);
                    if (!vResult.IsSucceeded || vResult.Data is null)
                    {
                        this._errorHandler.HandleError(Err.FailedToImportVideo, playlist.Name, video.NiconicoID);
                        continue;
                    }

                    data.AddVideo(vResult.Data);
                    importedVideos++;
                }

                if (playlist.IsRoot)
                {
                    root.AddChild(data);
                }

                converted.Add(playlist.ID, pResult.Data);
            }

            onMessage(this._stringHandler.GetContent(SC.ResolveChildren));
            //親子関係
            foreach (var key in converted.Keys)
            {
                foreach (var child in source[key].Children)
                {
                    if (!converted.ContainsKey(child))
                    {
                        continue;
                    }

                    converted[key].AddChild(converted[child]);
                }
            }

            onMessage(this._stringHandler.GetContent(SC.ImportCompleted));

            return AttemptResult<IXenoImportResult>.Succeeded(new XenoImportResult(importedVideos, converted.Count));
        }

        #endregion
    }
}
