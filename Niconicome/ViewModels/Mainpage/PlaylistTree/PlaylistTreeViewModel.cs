using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Niconicome.Models.Domain.Playlist;
using Niconicome.Models.Utils.Reactive;
using WS = Niconicome.Workspaces;

namespace Niconicome.ViewModels.Mainpage.PlaylistTree
{
    public class PlaylistTreeViewModel
    {
        public PlaylistTreeViewModel()
        {
            this.Playlists = new BindableCollection<PlaylistInfoViewModel, IPlaylistInfo>(WS::Mainpage.PlaylistVideoContainer.Playlist, info => new PlaylistInfoViewModel(info)).AsReadOnly();

            WS::Mainpage.PlaylistManager.Initialize();
        }

        #region Props

        public ReadOnlyObservableCollection<PlaylistInfoViewModel> Playlists { get; init; }

        #endregion

        #region Method
        #endregion

    }
}
