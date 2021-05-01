using System;
using Niconicome.Models.Domain.Local;
using Niconicome.Models.Playlist;

namespace Niconicome.Models.Local.Application
{
    public interface IShutdown
    {
        void ShutdownApp();
    }

    public class Shutdown : IShutdown
    {
        public Shutdown(IDataBase dataBase,IPlaylistHandler playlistHandler)
        {
            this.dataBase = dataBase;
            this.playlistHandler = playlistHandler;
        }

        private readonly IDataBase dataBase;

        private readonly IPlaylistHandler playlistHandler;

        private bool isShutdowned;

        /// <summary>
        /// 最終処理
        /// </summary>
        public void ShutdownApp()
        {
            if (this.isShutdowned) throw new InvalidOperationException("終了処理は一度のみ可能です。");
            this.playlistHandler.SaveAllPlaylists();
            this.dataBase.Dispose();
            this.isShutdowned = true;
        }
    }
}
