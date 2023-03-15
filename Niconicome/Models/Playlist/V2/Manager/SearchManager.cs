using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Local.Settings;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Domain.Utils.Error;
using Niconicome.Models.Helper.Result;
using Niconicome.Models.Playlist.V2.Manager.Error;
using Remote = Niconicome.Models.Domain.Niconico.Remote.V2;

namespace Niconicome.Models.Playlist.V2.Manager
{
    public interface ISearchManager
    {
        /// <summary>
        /// 動画を検索
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<IAttemptResult<Remote::RemotePlaylistInfo>> SearchVideosAsync(Remote::Search.SearchQuery query);

        /// <summary>
        /// 検索結果を登録
        /// </summary>
        /// <param name="name"></param>
        /// <param name="searchResult"></param>
        /// <returns></returns>
        Task<IAttemptResult> RegisterVideosAsync(string name, IEnumerable<Remote::VideoInfo> videos);
    }

    public class SearchManager : ISearchManager
    {
        public SearchManager(Remote::Search.ISearch search, IVideoListManager video, IPlaylistVideoContainer container, IErrorHandler errorHandler, ISettingsContainer settings)
        {
            this._search = search;
            this._video = video;
            this._container = container;
            this.errorHandler = errorHandler;
            this._settings = settings;
        }

        #region field

        private readonly Remote::Search.ISearch _search;

        private readonly IVideoListManager _video;

        private readonly IPlaylistVideoContainer _container;

        private readonly IErrorHandler errorHandler;

        private readonly ISettingsContainer _settings;

        #endregion

        #region Method

        public async Task<IAttemptResult<Remote::RemotePlaylistInfo>> SearchVideosAsync(Remote::Search.SearchQuery query)
        {
            return await this._search.SearchAsync(query);
        }

        public async Task<IAttemptResult> RegisterVideosAsync(string name, IEnumerable<Remote::VideoInfo> videos)
        {
            if (this._container.CurrentSelectedPlaylist is null)
            {
                this.errorHandler.HandleError(SearchManagerError.PlaylistIsNotSelected);
                return AttemptResult.Fail(this.errorHandler.GetMessageForResult(SearchManagerError.PlaylistIsNotSelected));
            }

            IPlaylistInfo playlist = this._container.CurrentSelectedPlaylist;

            IAttemptResult result = await this._video.RegisterVideosAsync(videos);
            if (!result.IsSucceeded)
            {
                return result;
            }

            IAttemptResult<ISettingInfo<bool>> settingResult = this._settings.GetSetting(SettingNames.AutoRenamingAfterSetNetworkPlaylist, false);
            if (!settingResult.IsSucceeded || settingResult.Data is null)
            {
                return AttemptResult.Fail(settingResult.Message);
            }

            if (settingResult.Data.Value)
            {
                playlist.Name.Value = name;
            }

            return AttemptResult.Succeeded();
        }


        #endregion
    }
}
