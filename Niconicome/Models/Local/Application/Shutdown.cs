using System;
using Niconicome.Models.Domain.Local;
using Niconicome.Models.Domain.Local.Addons.Core.Engine.Context;
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
        public Shutdown(IDataBase dataBase, IPlaylistHandler playlistHandler,IAddonContexts contexts)
        {
            this.dataBase = dataBase;
            this.playlistHandler = playlistHandler;
            this._contexts = contexts;
        }

        #region field

        private readonly IDataBase dataBase;

        private readonly IPlaylistHandler playlistHandler;

        private readonly IAddonContexts _contexts;

        #endregion

        public bool IsShutdowned { get; private set; }

        /// <summary>
        /// 最終処理
        /// </summary>
        public void ShutdownApp()
        {
            if (this.IsShutdowned) throw new InvalidOperationException("終了処理は一度のみ可能です。");
            this.playlistHandler.SaveAllPlaylists();
            this._contexts.KillAll();
            this.dataBase.Dispose();
            this.IsShutdowned = true;
        }
    }
}
