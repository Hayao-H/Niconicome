using System;
using Niconicome.Models.Domain.Local;
using Niconicome.Models.Domain.Local.Addons.Core.V2.Engine.Context;
using Niconicome.Models.Domain.Local.Server.Core;
using Niconicome.Models.Local.State.Style;
using Niconicome.Models.Playlist;
using Niconicome.Models.Playlist.Playlist;

namespace Niconicome.Models.Local.Application
{
    public interface IShutdown
    {
        void ShutdownApp();
        bool IsShutdowned { get; }
    }

    public class Shutdown : IShutdown
    {
        public Shutdown(IDataBase dataBase, IPlaylistHandler playlistHandler, IAddonContextsContainer contexts, IServer server, IVideoListWidthManager widthManager)
        {
            this.dataBase = dataBase;
            this.playlistHandler = playlistHandler;
            this._contexts = contexts;
            this._server = server;
            this._widthManager = widthManager;
        }

        #region field

        private readonly IDataBase dataBase;

        private readonly IPlaylistHandler playlistHandler;

        private readonly IAddonContextsContainer _contexts;

        private readonly IServer _server;

        private readonly IVideoListWidthManager _widthManager;

        #endregion

        public bool IsShutdowned { get; private set; }

        /// <summary>
        /// 最終処理
        /// </summary>
        public void ShutdownApp()
        {
            if (this.IsShutdowned) throw new InvalidOperationException("終了処理は一度のみ可能です。");
            this.playlistHandler.SaveAllPlaylists();
            this._contexts.ShutDownAll();
            this.dataBase.Dispose();
            this._server.ShutDown();
            this._widthManager.SaveWidth();
            this.IsShutdowned = true;
        }
    }
}
