using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Playlist.V2.Manager.Error;
using Niconicome.Models.Playlist.V2.Migration;

namespace Niconicome.Models.Playlist.V2.Manager
{
    public interface IVideoListManager
    {
        /// <summary>
        /// 現在のプレイリスト内の動画を読み込む
        /// </summary>
        Task LoadVideosAsync();
    }

    public class VideoListManager : IVideoListManager
    {
        public VideoListManager(IPlaylistVideoContainer container, ILocalVideoLoader loader, IErrorHandler errorHandler, IVideoAndPlayListMigration migration)
        {
            this._container = container;
            this._loader = loader;
            this._errorHandler = errorHandler;
            this._migration = migration;
        }

        #region field

        private readonly IPlaylistVideoContainer _container;

        private readonly ILocalVideoLoader _loader;

        private readonly IErrorHandler _errorHandler;

        private readonly IVideoAndPlayListMigration _migration;

        #endregion

        #region Method

        public async Task LoadVideosAsync()
        {
            //移行が必要な場合は処理を中止
            if (this._migration.IsMigrationNeeded) return;

            //プレイリストが選択されていない場合はエラー
            if (this._container.CurrentSelectedPlaylist is null)
            {
                this._errorHandler.HandleError(VideoListManagerError.PlaylistIsNotSelected);
                return;
            }

            int playlistID = this._container.CurrentSelectedPlaylist.ID;

            await this._loader.SetPathAsync(this._container.CurrentSelectedPlaylist.Videos);

            if (playlistID != (this._container.CurrentSelectedPlaylist?.ID ?? -1))
            {
                this._errorHandler.HandleError(VideoListManagerError.PlaylistChanged);
                return;
            }

            this._container.Videos.Clear();
            this._container.Videos.AddRange(this._container.CurrentSelectedPlaylist!.Videos);

        }

        #endregion
    }
}
